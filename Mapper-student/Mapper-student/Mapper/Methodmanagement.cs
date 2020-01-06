using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mapper_student
{
    public static class Methodmanagement
    {
        public static void Savemethodstofile(string filename)
        {
            string path = Environment.CurrentDirectory + "/logfiles/" + filename;
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
            foreach (var item in Callgraph.listmethods)
            {
                tw.WriteLine(item.print());
                //tw.WriteLine("\n");
            }
            tw.Close();
            tw.Dispose();
        }
        public static List<Method>  Getunusedmethods(List<Loggedmethods> _listlog)
        {
            List<Method> unvisited = new List<Method>();
            foreach (var item in Callgraph.listmethods)
            {
                string classname = item.method.DeclaringType.Name;
                string methodname = item.method.Name;
                //check parameters can be done...
                int cnt = _listlog.Where(p => p.classname == classname && p.methodname == methodname).ToList().Count();
                if (cnt > 0)
                {
                    item.isvisit = true;
                }
                else if (cnt == 0)
                {
                    unvisited.Add(item);
                }
            }
            return unvisited;
        }
    }
}