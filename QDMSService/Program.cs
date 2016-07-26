using NLog;
using NLog.Targets;
using System;
using System.Linq;

namespace QDMSService
{
    public sealed class QdmsService
    {
        private DataServer _server;

        static void Main(string[] args)
        {
            ColoredConsoleTarget target = new ColoredConsoleTarget();
            target.Layout = "${date:format=HH\\:mm\\:ss}   ${message}";

            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Trace);

            var service = new QdmsService();
            service.OnStart(args);
            Console.ReadLine();
            service.OnStop();
        }

        protected void OnStart(string[] args)
        {
            string appPath = System.IO.Path.GetDirectoryName(AppContext.BaseDirectory);
            string configFile = @"file://"+System.IO.Path.Combine(appPath, "DataServiceConfig.xml");

            Uri uri = new Uri(configFile);

            Config.DataServiceConfig config;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(Config.DataServiceConfig));
            using (var file = System.IO.File.OpenRead(uri.LocalPath))
            {
                config = (Config.DataServiceConfig)x.Deserialize(file);
            }

            _server = new DataServer(config);
            _server.Initialisize();
        }

        protected void OnStop()
        {
            _server.Stop();
        }
    }
}
