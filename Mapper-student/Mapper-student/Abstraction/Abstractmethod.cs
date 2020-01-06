using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper_student
{
    public class Abstractmethod
    {
        public string abstractvalue { get; set; }
        public Method concretemethod { get; set; }
        public List<Parameter> parameters { get; set; }
        public string pathcondition { get; set; }
        public Abstractmethod(string absvalue,Method m,List<Parameter> p,string path) 
        {
            abstractvalue = absvalue;
            concretemethod = m;
            parameters = p;
            pathcondition = path;
        }
    }
}
