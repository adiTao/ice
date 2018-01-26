using System;
using Demo;

public sealed class CallBackI : CallBackDisp_
{
    public override void Response(string content, Ice.Current current)
    {
        Console.Out.WriteLine("{0}:{1}", current.ctx["user_name"], content);
    }

    public override void ResponseNode(string content, MyNode n, Ice.Current current)
    {
        Console.Out.WriteLine("{0}:{1}", current.ctx["user_name"], content);
    }
}
