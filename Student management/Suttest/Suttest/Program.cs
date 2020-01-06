using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using log4net.Config;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Log4Net;
using System.IO;

[assembly: Log(AttributePriority = 1, AttributeTargetMemberAttributes = MulticastAttributes.Protected | MulticastAttributes.Internal | MulticastAttributes.Public | MulticastAttributes.Private)]
[assembly: Log(AttributePriority = 2, AttributeExclude = true, AttributeTargetMembers = "get_*")]
namespace Suttest
{
    public class Program
    {
        public static List<string> cities;
        public static List<string> majors;
        public static List<string> subjects;
        public static Student _student = null;

        [Log(AttributeExclude = true)]
        public static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            LoggingServices.DefaultBackend = new Log4NetLoggingBackend();
            majors = new List<string>();
            cities = new List<string>();
            subjects = new List<string>();
            Start();
            while (true) { }
        }

        public static void Start()
        {
            while (true)
            {
                int number = Showmenu();
                if (number == 1)
                {
                    Console.WriteLine("insert student ID"); string ID = Console.ReadLine();
                    Console.WriteLine("insert student Firstname"); string Firstname = Console.ReadLine();
                    Console.WriteLine("insert student lastname"); string Lastname = Console.ReadLine();
                    Console.WriteLine("insert student username"); string _username = Console.ReadLine();
                    Console.WriteLine("insert student password"); string _password = Console.ReadLine();
                    Console.WriteLine("insert student major"); string major = Console.ReadLine();
                    Console.WriteLine("insert student city"); string city = Console.ReadLine();
                    Console.WriteLine("insert student mail"); string mail = Console.ReadLine();
                    Registerstudent(ID, Firstname, Lastname, _username, _password, major, city, mail);
                }
                else if (number == 2)
                {
                    Console.WriteLine("insert City name"); string cityname = Console.ReadLine();
                    Registercity(cityname);
                }
                else if (number == 3)
                {
                    Console.WriteLine("insert Major name"); string majorname = Console.ReadLine();
                    Registermajor(majorname);
                }
                else if (number == 4)
                {
                    Console.WriteLine("insert usernamename"); string username = Console.ReadLine();
                    Console.WriteLine("insert password"); string password = Console.ReadLine();
                    Loginuser(username, Convert.ToInt32(password));
                }
                else if (number == 5)
                {
                    Console.WriteLine("LOGOUT");
                    Logout();
                }
                else
                {
                    break;
                }
            }

        }

        //Methods for register new student
        public static bool Registerstudent(string iD, string firstname, string lastname, string username, string password, string major, string city, string mail)
        {
            try
            {
                int ID = Convert.ToInt32(iD);
                int _password = Convert.ToInt32(password);
                if (_student == null)
                {
                    _student = new Student();
                    return Registernewstudent(ID, firstname, lastname, username, _password, major, city, mail);
                }
                else
                {
                    Printerrormessage("a user is registered.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static bool Registernewstudent(int iD, string firstname, string lastname, string username, int password, string major, string city, string mail)
        {
            try
            {
                if (major.Where(p => p.Equals(major)).ToList().Count == 0)
                {
                    if (!Registernewmajor(major))
                        return false;
                }
                if (cities.Where(p => p.Equals(city)).ToList().Count == 0)
                {
                    if (!Registernewcity(city))
                        return false;
                }
                return Registernewstudent2(iD, firstname, lastname, username, password, major, city, mail);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool Registernewstudent2(int iD, string firstname, string lastname, string username, int password, string major, string city, string mail)
        {
            try
            {
                if (!firstname.Equals("") && !lastname.Equals("") && firstname != null && lastname != null)
                {
                    return Registernewstudent3(iD, firstname, lastname, username, password, major, city, mail);
                }
                else
                {
                    Printerrormessage("Firstname or lastname was not inserted in a correct format");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool Registernewstudent3(int iD, string firstname, string lastname, string username, int password, string major, string city, string mail)
        {
            try
            {
                if (username != null && username != "" && password / 1000 >= 1)
                {
                    return Registernewstudent4(iD, firstname, lastname, username, password, major, city, mail);
                }
                else
                {
                    Printerrormessage("username or password was not inserted in a correct format");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool Registernewstudent4(int iD, string firstname, string lastname, string username, int password, string major, string city, string mail)
        {
            try
            {
                if (username.Length >= 4)
                {
                    _student = new Student();
                    _student.firstname = firstname;
                    _student.lastname = lastname;
                    _student.ID = iD;
                    _student.mail = mail;
                    _student.major = major;
                    _student.city = city;
                    _student.username = username;
                    _student.password = password;
                    return true;
                }
                else
                {
                    Printerrormessage("username was not inserted in a correct format");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //end of register methods

        //Methods for register city
        public static bool Registercity(string _city)
        {
            try
            {
                if (cities.Where(p => p.Equals(_city)).ToList().Count == 0)
                {
                    return Registernewcity(_city);
                }
                else
                {
                    Printerrormessage(_city + " is already inserted.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool Registernewcity(string city)
        {
            try
            {
                if (city != null && city != "")
                {
                    cities.Add(city);
                    return true;
                }
                else
                {
                    Printerrormessage("City in not inserted in correct format");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //end of City methods

        //Methods for register city
        public static bool Registermajor(string major)
        {
            try { 
            if (majors.Where(p => p.Equals(major)).ToList().Count == 0)
            {
                return Registernewmajor(major);
            }
            else
            {
                Printerrormessage(major + " is already inserted.");
                return false;
            }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool Registernewmajor(string major)
        {
            try { 
            if (major != null && major != "")
            {
                cities.Add(major);
                return true;
            }
            else
            {
                Printerrormessage("major in not inserted in correct format");
                return false;
            }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //end of Major methods

        //login and loguout methods
        public static bool Logout()
        {
            try { 
            if (_student == null)
            {
                Printerrormessage("no user is defined");
                return false;
            }
            if (_student.Logedin)
            {
                _student.Logedin = false;
                return true;
            }
            else
            {
                Printerrormessage("user in not logedin");
                return false;
            }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool Loginuser(string username, int password)
        {
            try { 
            if (username != "" && username != null && password / 1000 >= 1)
            {
                return checklogin(username, password);
            }
            else
            {
                return false;
            }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool checklogin(string username, int password)
        {
            try { 
            if (_student != null && _student.username == username && _student.password == password)
            {
                _student.Logedin = true;
                return true;
            }
            else
            {
                Printerrormessage("invalid login info");
                return false;
            }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //end of login methods

        public static void Printerrormessage(string v)
        {
            if (!v.Equals(""))
            {
                Console.WriteLine(v);
            }

        }
        public static bool reset()
        {
            _student = null;
            subjects = new List<string>();
            majors = new List<string>();
            cities = new List<string>();
            return true;
        }

        public static int Showmenu()
        {
            Console.WriteLine("Welcome");
            Console.WriteLine("Menu");
            Console.WriteLine("1-Register student");
            Console.WriteLine("2-Register City");
            Console.WriteLine("3-purchasing major");
            Console.WriteLine("4-Login student");
            Console.WriteLine("5-Logout student");
            return Convert.ToInt32(Console.ReadKey()); ;
        }

    }
}
