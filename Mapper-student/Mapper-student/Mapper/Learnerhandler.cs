
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Suttest;

namespace Mapper_student
{
    public class Learnerhandler
    {
        public static int learningphase = 1;
        public static Socket ServerSocket { get; private set; }//mapper
        public static Socket ClientSocket { get; set; }//learner
        public Logmanagement _logmanagement = null;
        public Learnerhandler()
        {
            Init();
            _logmanagement = new Logmanagement();
        }
        private static void Init()
        {
            Callgraph.Loadgraph(Environment.CurrentDirectory + "/Callgraph.txt");
            //add initial table
            Initialabstractiontable();
        }

        private static void Initialabstractiontable()
        {
            // object[] x;
            //should change
            Abstractiontable.Resettable();
            object[] x = { "1", "", "", "", "1", "", "", "" };
            Additem(x, "Registerstudent", "REGISTERSTUDENT");

            //Registercity
            object[] x1 = { "" };
            Additem(x1, "Registercity", "REGISTERCITY");

            //Registermajor
            object[] x2 = { "" };
            Additem(x2, "Registermajor", "REGISTERMAJOR");

            //login
            object[] x3 = { "",0 };
            Additem(x3, "Loginuser", "LOGINUSER");

            //login
            object[] x4 = { };
            Additem(x4, "Logout", "LOGOUT");
        }

        private static void Additem(object[] y, string methodname, string symbol)
        {
            Method _m1 = Callgraph.listmethods.Where(p => p.method.Name.Equals(methodname)).Single();
            ParameterInfo[] param1 = _m1.method.GetParameters();
            List<Parameter> p11 = new List<Parameter>();
            for (int i = 0; i < param1.Length; i++)
            {
                Parameter p2 = new Parameter(param1[i], y[i]);
                p11.Add(p2);
            }

            Abstractmethod abs1 = new Abstractmethod(symbol, _m1, p11, "TRUE");
            Abstractiontable.Additem(abs1);
        }

        public void Startlistentolearner()
        {
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 5059);
            ServerSocket.Bind(ip);
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine("Waiting for learner...");
                    ServerSocket.Listen(1);

                    ClientSocket = ServerSocket.Accept();
                    Console.WriteLine("Learner joined");

                    ProcessQueries();
                }
            });
            t.Name = "Listen";
            t.IsBackground = true;
            t.Start();
        }

        private void ProcessQueries()
        {
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    byte[] Recievedinput = new byte[1024];
                    int Count = ClientSocket.Receive(Recievedinput);
                    if (Count > 0)
                    {
                        string _input = Encoding.ASCII.GetString(Recievedinput, 0, Count);
                        //send to perform an action
                        string _output = Performinput(_input);
                        byte[] output = new byte[1024];
                        output = UnicodeEncoding.ASCII.GetBytes(_output);
                        ClientSocket.Send(output);
                    }
                }
            });
            t.Name = "Processqueries";
            t.IsBackground = true;
            t.Start();
        }
        public string Performinput(string input)
        {
            string output = "";
            if (input.Equals("LEARNINGCOMPLETE"))
            {
                output = Checklearning();
                //output = "FINISHLEARNING";
            }
            else if (input.Equals("reset")) {
                bool res = Suttest.Program.reset();
                 output=res==true? "resetok" : "restnok";
                return output;
            }
            else
            {
                output = Callsutmethods(input);
            }
            return output;
        }
        private string Callsutmethods(string input)
        {
            string output = "";
            Object ob = null; ;
            Abstractmethod _method = Abstractiontable.Getmethod(input);
            if (_method != null)
            {
                if (_method.concretemethod.method.IsPublic)
                {
                    object[] _inputs = new object[_method.parameters.Count];
                    int cnt = 0;
                    foreach (var item in _method.parameters)
                    {
                        _inputs[cnt] = Convert.ChangeType(item.value, item.param.ParameterType);
                        cnt++;
                    }
                    ///
                    ob = _method.concretemethod.method.Invoke(null, _inputs);
                    ///
                }
                else if (_method.concretemethod.method.IsPrivate)
                {
                    object[] _inputs = new object[_method.parameters.Count];
                    int cnt = 0;
                    foreach (var item in _method.parameters)
                    {
                        _inputs[cnt] = Convert.ChangeType(item.value, item.param.ParameterType);
                        cnt++;
                    }
                    MethodInfo dynMethod = _method.concretemethod.method.DeclaringType.GetMethod(_method.concretemethod.method.Name,
                    BindingFlags.NonPublic | BindingFlags.Instance);
                    ob = dynMethod.Invoke(null, _inputs);
                }
            }
            if (ob != null)
            {
                if (Convert.ToBoolean(ob) == true)
                    output = _method.abstractvalue + "-OK";
                else
                    output = _method.abstractvalue + "-NOK";
            }
            return output;
        }
        public string Checklearning()
        {
            List<Loggedmethods> _listlog = _logmanagement.Getusedmethods(Environment.CurrentDirectory + "/log4net.log");
            List<Method> _listunused = Methodmanagement.Getunusedmethods(_listlog);
            Writemethodstatustofile("Methodstatus-phase" + learningphase.ToString() + ".txt", _listunused);
            Writeabstractiontable("Abstraction-table" + learningphase.ToString());
            List<Abstractmethod> newinputs = Discovernewalphabet(_listunused,_listlog);
            if (newinputs.Count() == 0)
            {
                return "FINISHLEARNING";
            }
            else
            {
                string str = "";
                foreach (var item in newinputs)
                {
                    str += item.abstractvalue + ";";
                }
                learningphase++;
                return str;
            }

        }
        private List<Abstractmethod> Discovernewalphabet(List<Method> _listunused, List<Loggedmethods> _listlog)
        {
            Callgraph.listmethods.ForEach(delegate (Method m){
                m.score = 0;
                m.calledmethods = new List<Method>();
            });
            foreach (var item in _listunused)
            {
                int functionnumber = Callgraph.listmethods.Where(p => p.ID == item.ID).Select(p => p.ID).Single();
                Callgraph.listmethods.Where(p => p.ID == functionnumber).Single().score++;
                Method _m = Callgraph.listmethods.Where(p => p.ID == functionnumber).Single();
                Callgraph.listmethods.Where(p => p.ID == functionnumber).Single().calledmethods.Add(_m);
                List<Method> _called = new List<Method>();
                _called.Add(_m);
                Updatecallinformations(_m,_m);
            }
            List<Method> newfunctions = Findbestfunctions(_listunused);
            List<Abstractmethod>newrows= Abstractiontable.Updateabstracttable(newfunctions);
            return newrows;
        }
        private void Updatecallinformations(Method called,Method parent)
        {
            for (int i = 0; i < Callgraph.listmethods.Count + 1; i++)
            {
                if (Callgraph.callgraph[i, parent.ID] == 1)
                {
                    Callgraph.listmethods.Where(p => p.ID == i).Single().score++;
                    Method _m = Callgraph.listmethods.Where(p => p.ID == i).Single();
                    Callgraph.listmethods.Where(p => p.ID == i).Single().calledmethods.Add(called);
                    Updatecallinformations(called,Callgraph.listmethods.Where(p=>p.ID==i).Single());
                }
            }
        }
        private List<Method> Findbestfunctions(List<Method> _listunused)
        {
            //set cover
            List<Method> newmwthods = new List<Method>();
            List<Method> covered = new List<Method>();
            while (!_listunused.All(covered.Contains))
            {
                int id = 0; int count = 0;
                Callgraph.listmethods.ForEach(delegate (Method m) {
                    if (m.calledmethods.Except(covered).ToList().Count > count)
                    {
                        count = m.calledmethods.Except(covered).ToList().Count;
                        id = m.ID;
                    }
                });
                newmwthods.Add(Callgraph.listmethods.Find(p => p.ID == id));
                foreach (var item in Callgraph.listmethods.Find(p => p.ID == id).calledmethods)
                {
                    covered.Add(item);
                }
            }
            return newmwthods;
        }


        private void Writeabstractiontable(string v)
        {
            string path = Environment.CurrentDirectory + "/logfiles/" + v;
            if (!Directory.Exists(Environment.CurrentDirectory + "/logfiles"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "/logfiles");
            }
            if (!File.Exists(path))
            {
                FileStream f = File.Create(path);
                f.Close();

            }
            StreamWriter tw = new StreamWriter(path);
            tw.WriteLine("Abstractiontable step"+learningphase.ToString());
            foreach (var item in Abstractiontable.abstractmethods)
            {
                string str = "";
                str += item.abstractvalue+"--";
                str += item.concretemethod.method.Name + "--";
                str += item.pathcondition + "--";
                foreach (var parametrs in item.parameters)
                {
                    str += "(" + parametrs.param.Name + " -> " + parametrs.value + ") ";
                }
                tw.WriteLine(str);
            }
            tw.Close();
        }
        private void Writemethodstatustofile(string v, List<Method> listunused)
        {
            string path = Environment.CurrentDirectory + "/logfiles/" + v;
            if (!Directory.Exists(Environment.CurrentDirectory + "/logfiles"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "/logfiles");
            }
            if (!File.Exists(path))
            {
                FileStream f = File.Create(path);
                f.Close();

            }
            StreamWriter tw = new StreamWriter(path);
            tw.WriteLine("***************Used methods*****************");
            foreach (var item in Callgraph.listmethods)
            {
                if (item.isvisit)
                {
                    tw.WriteLine(item.print());
                }
            }
            tw.WriteLine("  ");
            tw.WriteLine("***************Used methods*****************");
            foreach (var item in listunused)
            {
                tw.WriteLine(item.print());
            }
            tw.Close();
            tw.Dispose();
        }
    }
}


