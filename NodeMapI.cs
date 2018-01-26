// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

using Demo;
using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;

public class NodeMapI : NodeMapDisp_
{
    public override void SendGreeting(string msg, Ice.Current current)
    {
        string user_name = current.ctx["user_name"];
        Console.Out.WriteLine("{0}:{1}", user_name, msg);
        // 向其他人廣播
        Broadcast(msg, current);
    }

    public override bool CreateMap(string mapName, Ice.Current current)
    {
        return true;
    }

    public override bool CreateNode(MyNode node, Ice.Current current)
    {
        return true;
    }

    public override bool EditNode(MyNode node, Ice.Current current)
    {
        return true;
    }

    public override bool DeleteNode(string nodeId, Ice.Current current)
    {
        return true;
    }

    public override bool MoveNode(MyNode node, Ice.Current current)
    {
        return true;
    }

    public override void shutdown(Ice.Current current)
    {
        Console.Out.WriteLine("Shutting down...");
        current.adapter.getCommunicator().shutdown();
    }

    public override void SetupCallback(CallBackPrx cp, Ice.Current current)
    {
        if (current != null && current.ctx["user_name"] != null)
        {
            string user_name = current.ctx["user_name"];
            Console.Out.WriteLine("使用者 {0} 已註冊", user_name);
            _users.Add(new User(user_name, cp));
        }
        else
        {
            throw new Exception("註冊失敗!!");
        }
    }

    public override bool Register(string name, Ice.Current current)
    {
        Console.Out.WriteLine("驗證 {0} 是否登入!", name);
        if (_users.Count == 0)
            return true;
        return _users.All(user => user.Name != name);
    }

    public override void Unregister(Ice.Current current)
    {
        string user_name = current.ctx["user_name"];
        if (user_name != null)
        {
            for (int i = _users.Count - 1; i >= 0; i--)
            {
                if (_users[i].Name == user_name)
                    _users.RemoveAt(i);
            }
            string msg = string.Format("離開({0})", user_name);
            Console.WriteLine(msg);

            // 向其他人廣播
            Broadcast(msg, current);
        }
    }

    private void Broadcast(string msg, Ice.Current current)
    {
        string user_name = current.ctx["user_name"];
        foreach (var user in _users)
        {
            if (user.Name == user_name)
            {
                continue;
            }
            user.Cp.Response(msg, current.ctx);
        }
    }

    private UserList _users = new UserList();
}
