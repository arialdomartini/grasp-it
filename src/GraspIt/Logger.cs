using System;

namespace GraspIt
{

    class Logger
    {
        public void Error(string message, Exception e)
        {
            var logMessage = string.Format("{0}, {1} {2}", message, e.Message, e.StackTrace.ToString());
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "log.txt)";
            Console.WriteLine(path);
        }
    }
    
}