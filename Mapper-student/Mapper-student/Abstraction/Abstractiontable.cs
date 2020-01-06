using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mapper_student
{
    public static class Abstractiontable
    {
        public static List<Abstractmethod> abstractmethods;
        public static void Resettable()
        {
            abstractmethods = new List<Abstractmethod>();
        }
        public static void Additem(Abstractmethod abs)
        {
            abstractmethods.Add(abs);
        }
        public static Abstractmethod Getmethod(string abstractval)
        {
            return abstractmethods.Where(p => p.abstractvalue.Equals(abstractval)).Single();
        }

        internal static List<Abstractmethod> Updateabstracttable(List<Method> newfunctions)
        {//
            List<Abstractmethod> _list = new List<Abstractmethod>();
            object[] x = { "0", "00", "00", "0000", "1004", "00", "00", "" };
            _list.Add(Additemtotable(x, "Registerstudent", "REGISTERSTUDENT1"));
            object[] y = { "0", 1017 };
            _list.Add(Additemtotable(y, "Loginuser", "LOGINUSER1"));
            object[] y2 = { "0000", 1004 };
            _list.Add(Additemtotable(y2, "Loginuser", "LOGINUSER2"));
            return _list;


        }
        private static Abstractmethod Additemtotable(object[] y, string methodname, string symbol)
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
            return abs1;
        }
    }
}
