using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suttest
{
    public class Student
    {
        public int ID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string city { get; set; }
        public string major { get; set; }
        public string mail { get; set; }
        public string username { get; set; }
        public int password { get; set; }
        public bool Logedin { get; set; }
        public Student()
        {
            Logedin = false;
        }
        public string print()
        {
            string str = "*****\n";
            str += firstname + " " + lastname;
            return str;
        }
    }
}
