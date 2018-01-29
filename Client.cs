// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

using Demo;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
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
        var userMap = UserMapPrxHelper.checkedCast(communicator().propertyToProxy("User.Proxy"));
        if (userMap == null)
        {
            Console.Error.WriteLine("invalid proxy");
            return 1;
        }
        Console.WriteLine("輸入名字：");
        string user_name = Console.ReadLine();

        if (userMap.Register(user_name) == false)
        {
            System.Console.WriteLine("已被註冊!");
            Thread.Sleep(2000);
            return 1;
        }

        //註冊回傳
        Ice.ObjectAdapter adapter = communicator().createObjectAdapter("UserCallback.Client");
        adapter.add(new UserCallBackI(), Ice.Util.stringToIdentity("callbackReceiver"));
        adapter.activate();


        UserCallBackPrx callbackPrx = UserCallBackPrxHelper.uncheckedCast(adapter.createProxy(Ice.Util.stringToIdentity("callbackReceiver")));
        Dictionary<string, string> ctx = new Dictionary<string, string>();
        ctx.Add("user_name", user_name);
        userMap.SetupCallback(callbackPrx, ctx);

        menu();
        MyNode[] allnodes = new MyNode[10000];
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
                    userMap.SendGreeting(msg, ctx);
                    Console.Out.Flush();
                }
                if (line.Equals("l"))
                {
                    allnodes = nodeMap.GetAllNodes(ctx);
                    foreach (var node in allnodes)
                    {
                        Console.WriteLine($"Id:{node.NodeId}   NodeText:{node.NodeText}   ParetnId:{node.ParentId}");
                    }
                    Console.Out.Flush();
                }
                if (line.Equals("c"))
                {
                    Console.WriteLine("Node Id：");
                    string id = Console.ReadLine();
                    Console.WriteLine("節點描述：");
                    string text = Console.ReadLine();
                    MyNode node = new MyNode();
                    node.NodeId = id;
                    node.NodeText = text;
                    node.ParentId = "root";
                    if (nodeMap.GetAllNodes(ctx).Length > 0)
                    {
                        Console.WriteLine("父節點 Id：");
                        node.ParentId = Console.ReadLine();
                    }
                    nodeMap.CreateNode(node, ctx);
                    Console.Out.Flush();
                }
                if (line.Equals("e"))
                {
                    Console.WriteLine("Node Id：");
                    string id = Console.ReadLine();
                    MyNode editNode = nodeMap.GetAllNodes(ctx).FirstOrDefault(p => p.NodeId == id);
                    if (editNode == null)
                    {
                        Console.WriteLine("此Id不存在");
                    }
                    else
                    {
                        Console.WriteLine("節點描述：");
                        editNode.NodeText = Console.ReadLine();
                        nodeMap.EditNode(editNode, ctx);
                    }

                    Console.Out.Flush();
                }
                if (line.Equals("m"))
                {
                    Console.WriteLine("Node Id：");
                    string id = Console.ReadLine();
                    MyNode editNode = nodeMap.GetAllNodes(ctx).FirstOrDefault(p => p.NodeId == id);
                    if (editNode == null)
                    {
                        Console.WriteLine("此Id不存在");
                    }
                    else
                    {
                        Console.WriteLine("父節點 Id：");
                        editNode.ParentId = Console.ReadLine();
                        nodeMap.MoveNode(editNode, ctx);
                    }

                    Console.Out.Flush();
                }
                if (line.Equals("r"))
                {
                    Console.WriteLine("Node Id：");
                    string id = Console.ReadLine();
                    nodeMap.DeleteNode(id, ctx);
                    Console.Out.Flush();
                }
                else if(line.Equals("s"))
                {
                    userMap.shutdown();
                }
                else if(line.Equals("x"))
                {
                    userMap.Unregister(ctx);
                    // Nothing to do
                }
                else if(line.Equals("?"))
                {
                    menu();
                }
                else
                {
                    //Console.WriteLine("unknown command `" + line + "'");
                    //menu();
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
            "l: 列出節點\n" +
            "c: 新增節點\n" +
            "e: 編輯節點\n" +
            "r: 刪除節點\n" +
            "m: 搬遷節點\n" +
            "s: shutdown server\n" +
            "x: exit\n" +
            "?: help\n");
    }
}
