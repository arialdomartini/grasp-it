using System;
using NLog;

namespace GraspIt
{

    class TwitterClient
    {
        static TwitterClient _instance;

        static string api = null;
        static string version = "2";
        static string method = "sendtweet";

        static NLog.Logger Log = LogManager.GetCurrentClassLogger();

        public static TwitterClient GetInstance()
        {
            return new TwitterClient();
        }
        
        public void Connect()
        {
            var apiUrl = string.Format("{0}/{1}/{2}", api, version, method);
            try
            {
                Connector.Instance.ConnectAndSend(apiUrl);
            }
            catch
            {
                Log.Error("Problems with twitter");
                return;
            }
        }

        class Connector
        {
            public static Connector Instance {
                get { return new Connector(null);}
            }

            bool Connected;
            bool Authenticated;
            bool UserName;
            bool Password;

            public Connector(string apiUrl)
            {
                Connected = false;
            }

            public Connector(bool connected)
            {
                Connected = connected;
            }

            public TwitterClient ConnectAndSend(string apiUrl)
            {

                if (_instance == null)
                    _instance = new TwitterClient();
                try
                {
                    ConnectAndSend(null, 3);
                    return _instance;
                }
                catch(Exception e)
                {
                    Log.Error(e, "Can't connect");
                    return null;
                }
            }

            static Connector ConnectAndSend(string apiUrl, int retry)
            {
                try
                {
                    throw new NotImplementedException();
                    return new Connector(true);
                }
                catch
                {
                    return null;
                }
            }
            
        }
    }
    
}