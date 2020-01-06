using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper_student
{
    public static class Callgraph
    {
        public static List<Method> listmethods = new List<Method>();
        public static int[,] callgraph;
        public static void Loadgraph(string filename)
        {
            string line;
            StreamReader file =
                new StreamReader(filename);
            line = file.ReadLine();
            string methodnames = line.Split('=')[1];
            string[] methodarr = methodnames.Split(',');
            foreach (var item in methodarr)
            {
                string str = item.Substring(1); str = str.Substring(0, str.Length - 1);
                string methodname = str.Split('-')[0];
                Method _method = new Method(methodname);
                _method.ID = Convert.ToInt32(str.Split('-')[1]);
                listmethods.Add(_method);
            }
            callgraph = new int[listmethods.Count + 1,listmethods.Count + 1];
            for (int i = 0; i < listmethods.Count+1; i++)
            {
                for (int j = 0; j < listmethods.Count+1; j++)
                {
                    callgraph[i, j] = 0;
                }
            }
            line = file.ReadLine();
            string[] edges = line.Split('=')[1].Split(',');
            foreach (var item in edges)
            {
                int from = Convert.ToInt32(item.Split('-')[0]);
                int to=Convert.ToInt32(item.Split('-')[1]);
                callgraph[from, to] = 1;
            }
            file.Close();
        }
        public static List<Method> Getmethodlist()
        {
            return listmethods;
        }
    }

}
