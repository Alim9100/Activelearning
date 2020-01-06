using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mapper_student
{
    //read log file and save to list for further uses
    public class Logmanagement
    {
        private static int counter = 0;
        private static List<Loggedmethods> _listmethods = null;

        public Logmanagement()
        {
        }
        public List<Loggedmethods> Getusedmethods(string logfile)
        {
            _listmethods = new List<Loggedmethods>();
            Methodextraction(logfile);
            return _listmethods;
        }
        //create a copy and remove the file or save line number

        private void Methodextraction(string logfile)
        {
            string path = Environment.CurrentDirectory + "/logfiles/iLearninlog-phase" + Learnerhandler.learningphase.ToString() + ".txt";
            if (!Directory.Exists(Environment.CurrentDirectory + "/logfiles"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "/logfiles");
            }
            if (!File.Exists(path))
            {
                FileStream f = File.Create(path);
                f.Close();
            }
           
            System.IO.File.Copy(logfile, "tempfile.txt", true);
            StreamWriter tw = new StreamWriter(path);
            //should set counter dynamically
            string line;
            System.IO.StreamReader file =
                new System.IO.StreamReader("tempfile.txt");
            file.ReadLine().Skip(counter);
            while ((line = file.ReadLine()) != null)
            {
                tw.WriteLine(line);
                Loggedmethods _method = Getmethod(line);
                if (_method != null)
                {
                    _listmethods.Add(_method);
                }
                counter++;
            }

            file.Close();
            file.Dispose();
            tw.Dispose();
            tw.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
        }

        private static Loggedmethods Getmethod(string line)
        {
            string classname = "";
            Loggedmethods _logmethod = null;
            if (line.Substring(0, 11).Equals("***TRACE***"))
            {
                line = line.Substring(line.IndexOf("DEBUG") + 5).Trim();
                if (line.Split('|').Length >= 2)
                {
                    if (line.Split('|')[1].Trim().Equals("Starting."))
                    {
                        string des = line.Split('|')[0].Trim();
                        classname = des.Split('.')[0];
                        string methodname = des.Substring(des.IndexOf('.') + 1, des.IndexOf('(') - 1 - des.IndexOf('.'));
                        string _s = des.Substring(des.IndexOf('(') + 1);
                        _s = _s.Substring(0, _s.Length - 1);
                        methodname = methodname == ".ctor" ? classname : methodname;
                        string parameter = _s;
                        _logmethod = new Loggedmethods();
                        _logmethod.methodname = methodname;
                        _logmethod.classname = classname;
                        _logmethod.parameters = parameter;
                    }
                    else if ((line.Split('|')[1].Trim().Contains("Succeeded: returnValue")))
                    {
                        string des = line.Split('|')[0].Trim();
                        classname = des.Split('.')[0];
                        string methodname = des.Substring(des.IndexOf('.') + 1, des.IndexOf('(') - 1 - des.IndexOf('.'));
                        methodname = methodname == ".ctor" ? classname : methodname;
                        int v = _listmethods.Count();
                        for (int i = v - 1; i >= 0; i--)
                        {
                            if (_listmethods[i].methodname == methodname && _listmethods[i].classname == classname)
                            {
                                string str = line.Split('|')[1].Trim();
                                str = str.Substring(str.IndexOf(':')).Trim();
                                str = str.Substring(str.IndexOf('=') + 1).Trim();
                                _listmethods[i].returnstatment = str;
                            }
                        }
                    }
                }
            }
            return _logmethod;
        }

    }
    public class Loggedmethods
    {
        public Loggedmethods()
        {

        }
        public string classname { get; set; }
        public string methodname { get; set; }
        public string parameters { get; set; }
        public string returnstatment { get; set; }
    }
}

