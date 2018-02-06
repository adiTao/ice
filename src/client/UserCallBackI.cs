using System;
using Demo;

public sealed class UserCallBackI : UserCallBackDisp_
{
    public override void Response(string content, Ice.Current current)
    {
        Console.Out.WriteLine("{0}:{1}", current.ctx["user_name"], content);
    }

    public override void ResponseNode(string content, MyNode node, Ice.Current current)
    {
        Console.Out.WriteLine($"{current.ctx["user_name"]}:{content}    Id:{node.NodeId}   NodeText:{node.NodeText}   ParetnId:{node.ParentId}");
    }

    public override void ResponseGraph(string content, string graphName, Ice.Current current)
    {
        Console.Out.WriteLine("{0}:{1} : {2}", current.ctx["user_name"], content, graphName);
    }
}
