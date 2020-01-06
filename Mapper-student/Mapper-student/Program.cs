using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Log4Net;

[assembly: Log(AttributePriority = 1, AttributeTargetMemberAttributes = MulticastAttributes.Protected | MulticastAttributes.Internal | MulticastAttributes.Public | MulticastAttributes.Private)]
[assembly: Log(AttributePriority = 2, AttributeExclude = true, AttributeTargetMembers = "get_*")]
[assembly: Log(AttributePriority = 2, AttributeExclude = true, AttributeTargetMembers = "set_*")]
namespace Mapper_student
{
    public class Program
    {
        [Log(AttributeExclude = true)]
        public static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            LoggingServices.DefaultBackend = new Log4NetLoggingBackend();
            LoggingServices.DefaultBackend.DefaultVerbosity.SetMinimalLevelForNamespace(LogLevel.Error, "Mapper_student");

            Learnerhandler _learner = new Learnerhandler();
            //_learner.Startlistentolearner();
            _learner.Startlistentolearner();
            while (true) { }
        }
    }
}
