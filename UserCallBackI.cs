using System;
using Demo;

public sealed class UserCallBackI : UserCallBackDisp_
{
    public override void Response(string content, Ice.Current current)
    {
        Console.Out.WriteLine("{0}:{1}", current.ctx["user_name"], content);
    }

    public override void ResponseNode(string content, string nodeId, string nodeText, string parentId, Ice.Current current)
    {
        Console.Out.WriteLine($"{current.ctx["user_name"]}:{content}    Id:{nodeId}   NodeText:{nodeText}   ParetnId:{parentId}");
    }
}
