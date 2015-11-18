using System;

namespace GraspIt
{

    class Logger
    {
        public void Error(string message, Exception e)
        {
            Console.WriteLine(string.Format("{0}, {1} {2}", message, e.Message, e.StackTrace.ToString()));
        }
    }
    
}