// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

using System;
using System.Linq;
using Demo;

public class UserMapI : UserMapDisp_
{
    public override void SendGreeting(string msg, Ice.Current current)
    {
        string user_name = current.ctx["user_name"];
        Console.Out.WriteLine("{0}:{1}", user_name, msg);
        // 向其他人廣播
        Broadcast(msg, current);
    }

    public override void shutdown(Ice.Current current)
    {
        Console.Out.WriteLine("Shutting down...");
        current.adapter.getCommunicator().shutdown();
    }

    public override void SetupCallback(UserCallBackPrx cp, Ice.Current current)
    {
        if (current != null && current.ctx["user_name"] != null)
        {
            string user_name = current.ctx["user_name"];
            Console.Out.WriteLine("使用者 {0} 已註冊", user_name);
            UserUtils.OnlineUser.Add(new User(user_name, cp));
        }
        else
        {
            throw new System.Exception("註冊失敗!!");
        }
    }

    public override bool Register(string name, Ice.Current current)
    {
        Console.Out.WriteLine("驗證 {0} 是否登入!", name);
        if (UserUtils.OnlineUser.Count == 0)
            return true;
        return UserUtils.OnlineUser.All(user => user.Name != name);
    }

    public override void Unregister(Ice.Current current)
    {
        string user_name = current.ctx["user_name"];
        if (user_name != null)
        {
            for (int i = UserUtils.OnlineUser.Count - 1; i >= 0; i--)
            {
                if (UserUtils.OnlineUser[i].Name == user_name)
                    UserUtils.OnlineUser.RemoveAt(i);
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
        foreach (var user in UserUtils.OnlineUser)
        {
            if (user.Name == user_name)
            {
                continue;
            }
            //user.Cp.Response(msg, current.ctx);
            TaskUtils.AddTask(user.Cp.ResponseAsync(msg, current.ctx));
        }
    }
}
