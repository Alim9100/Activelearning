using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Suttest;

namespace Mapper_student
{
    public class Method
    {
        public System.Reflection.MethodInfo method { get; set; }
        public bool isvisit { get; set; }
        public int ID { get; set; }
        public int score { get; set; }
        public List<Method> calledmethods { get; set; }
        public Method(string _methoddesc)
        {
            int index = _methoddesc.LastIndexOf('.');
            string classname = _methoddesc.Substring(0, index );
            string methodname = _methoddesc.Substring(index + 1);
            var assembly= Assembly.GetAssembly(typeof(Suttest.Program));
            method =assembly.GetType(classname).GetMethod(methodname);
            isvisit = false;
            calledmethods = null;
        }
        public string print()
        {
            string value = "";
            var parameterDescriptions = string.Join
                    (", ", method.GetParameters()
                                 .Select(x => x.ParameterType + " " + x.Name)
                                 .ToArray());

            value = String.Format("{0} {1} ({2})",
                              method.ReturnType,
                              method.Name,
                              parameterDescriptions);
            return value;
        }
    }
}
