// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

using System;
using System.Reflection;
using Demo;

public class Server
{
    class App : Ice.Application
    {
        public override int run(string[] args)
        {
            if(args.Length > 0)
            {
                Console.Error.WriteLine(appName() + ": too many arguments");
                return 1;
            }
            //±Ò¥Înoe4j
            Neo4jConfig.Register();

            var nodeAdapter = communicator().createObjectAdapter("Node");
            nodeAdapter.add(new NodeMapI(), Ice.Util.stringToIdentity("node"));
            nodeAdapter.activate();

            var userAdapter = communicator().createObjectAdapter("User");
            userAdapter.add(new UserMapI(), Ice.Util.stringToIdentity("user"));
            userAdapter.activate();
            communicator().waitForShutdown();
            return 0;
        }
    }

    public static int Main(string[] args)
    {
        var app = new App();
        return app.main(args, "config.server");
    }
}
