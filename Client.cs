// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

using Demo;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Threading;

public class Client : Ice.Application
{
    static void Main(string[] args)
    {
        Client app = new Client();
        int status = app.main(args, "config.client");
        if (status != 0)
        {
            System.Environment.Exit(status);
        }
    }

    public override int run(string[] args)
    {
        var nodeMap = NodeMapPrxHelper.checkedCast(communicator().propertyToProxy("Node.Proxy"));
        if(nodeMap == null)
        {
            Console.Error.WriteLine("invalid proxy");
            return 1;
        }
        Console.WriteLine("輸入名字：");
        string user_name = Console.ReadLine();

        if (nodeMap.Register(user_name) == false)
        {
            System.Console.WriteLine("已被註冊!");
            Thread.Sleep(2000);
            return 1;
        }

        //註冊回傳
        Ice.ObjectAdapter adapter = communicator().createObjectAdapter("Callback.Client");
        adapter.add(new CallBackI(), Ice.Util.stringToIdentity("callbackReceiver"));
        adapter.activate();


        CallBackPrx callbackPrx = CallBackPrxHelper.uncheckedCast(adapter.createProxy(Ice.Util.stringToIdentity("callbackReceiver")));
        Dictionary<string, string> ctx = new Dictionary<string, string>();
        ctx.Add("user_name", user_name);
        nodeMap.SetupCallback(callbackPrx, ctx);

        menu();

        string line = null;
        do
        {
            try
            {
                Console.Out.Write("==> ");
                Console.Out.Flush();
                line = Console.In.ReadLine();
                if(line == null)
                {
                    break;
                }
                if(line.Equals("g"))
                {
                    Console.WriteLine("輸入訊息：");
                    string msg = Console.ReadLine();
                    nodeMap.SendGreeting(msg, ctx);
                }
                else if(line.Equals("s"))
                {
                    nodeMap.shutdown();
                }
                else if(line.Equals("x"))
                {
                    nodeMap.Unregister(ctx);
                    // Nothing to do
                }
                else if(line.Equals("?"))
                {
                    menu();
                }
                else
                {
                    Console.WriteLine("unknown command `" + line + "'");
                    menu();
                }
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
        while (!line.Equals("x"));

        return 0;
    }

    private static void menu()
    {
        Console.Write(
            "g: 傳訊息\n" +
            "s: shutdown server\n" +
            "x: exit\n" +
            "?: help\n");
    }
}
