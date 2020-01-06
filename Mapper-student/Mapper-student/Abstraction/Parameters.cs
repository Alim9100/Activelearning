using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mapper_student
{
    public class Parameter
    {
        public ParameterInfo param { get; set; }
        public object value { get; set; }
        public Parameter(ParameterInfo param,object value)
        {
            this.param = param;
            this.value = value;
        }
    }
}
