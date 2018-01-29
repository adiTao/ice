using System;
using System.Configuration;
using Neo4jClient;

namespace Demo
{
    public static class Neo4jConfig
    {
        public static void Register()
        {
            //Use an IoC container and register as a Singleton
            var url = ConfigurationManager.AppSettings["GraphDBUrl"];
            var user = ConfigurationManager.AppSettings["GraphDBUser"];
            var password = ConfigurationManager.AppSettings["GraphDBPassword"];
            var client = new GraphClient(new Uri(url), user, password);
            client.Connect();

            GraphClient = client;
        }

        public static IGraphClient GraphClient { get; private set; }
    }

}
