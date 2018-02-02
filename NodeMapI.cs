// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

using System;
using System.Linq;
using Demo;

public class NodeMapI : NodeMapDisp_
{
    public override MyNode[] GetAllNodes(Ice.Current current)
    {
        var query = Neo4jConfig.GraphClient.Cypher
            .Match("(node:Node)")
            .Return(node => node.As<MyNode>());
        var data = query.Results.ToArray();
        
        return data;
    }

    public override bool CreateNode(string graphName, MyNode newNode, Ice.Current current)
    {
        if (newNode.ParentId == "root") //Root
        {
            Neo4jConfig.GraphClient.Cypher
                .Merge("(node:Node { NodeText: {nodeText} })")
                .OnCreate()
                .Set("node = {newNode}")
                .WithParams(new
                {
                    nodeText = newNode.NodeText,
                    newNode
                })
                .ExecuteWithoutResults();
        }
        else //Branch
        {
            Neo4jConfig.GraphClient.Cypher
                .Match("(parent:Node)")
                .Where((MyNode parent) => parent.NodeId == newNode.ParentId)
                .Create("(parent)-[:HASCHILD]->(child:Node {newNode})")
                .WithParam("newNode", newNode)
                .ExecuteWithoutResults();
        }

        Neo4jConfig.GraphClient.Cypher
            .Match("(node:Node)", "(graph:Graph)")
            .Where((MyNode node) => node.NodeId == newNode.NodeId)
            .AndWhere((MyGraph graph) => graph.GraphName == graphName)
            .Create("(graph)-[:CONTAINS]->(node)")
            .ExecuteWithoutResults();

        // 向其他人廣播
        BroadcastNode("新建節點", newNode, current);
        return true;
    }

    public override bool EditNode(string graphName, MyNode editNode, Ice.Current current)
    {
        Neo4jConfig.GraphClient.Cypher
            .Match("(node:Node)-[r]-(graph:Graph)")
            .Where((MyNode node) => node.NodeId == editNode.NodeId)
            .AndWhere((MyGraph graph) => graph.GraphName == graphName)
            .Set("node.NodeText = {nodeText}")
            .WithParam("nodeText", editNode.NodeText)
            .ExecuteWithoutResults();
        // 向其他人廣播
        BroadcastNode("編輯節點", editNode, current);
        return true;
    }

    public override bool DeleteNode(string graphName, string nodeId, Ice.Current current)
    {
        Neo4jConfig.GraphClient.Cypher
            .Match("(node:Node)-[r]-(graph:Graph)")
            .Where((MyNode node) => node.NodeId == nodeId)
            .AndWhere((MyGraph graph) => graph.GraphName == graphName)
            .DetachDelete("node")
            .ExecuteWithoutResults();

        MyNode newnode = new MyNode { NodeId = nodeId};
        // 向其他人廣播
        BroadcastNode("刪除節點", newnode, current);
        return true;
    }

    public override bool MoveNode(string graphName, MyNode moveNode, Ice.Current current)
    {
        //Remove relation
        Neo4jConfig.GraphClient.Cypher
            .OptionalMatch("(node:Node)<-[r:HASCHILD]-()")
            .Where((MyNode node) => node.NodeId == moveNode.NodeId)
            .Delete("r")
            .ExecuteWithoutResults();

        //Connect new parent
        Neo4jConfig.GraphClient.Cypher
            .Match("(node:Node)")
            .Where((MyNode node) => node.NodeId == moveNode.NodeId)
            .Set("node.ParentId = {parentId}")
            .WithParam("parentId", moveNode.ParentId)
            .ExecuteWithoutResults();

        //Connect new parent relation
        Neo4jConfig.GraphClient.Cypher
            .Match("(node1:Node)", "(node2:Node)")
            .Where((MyNode node1) => node1.NodeId == moveNode.ParentId)
            .AndWhere((MyNode node2) => node2.NodeId == moveNode.NodeId)
            .Create("(node1)-[:HASCHILD]->(node2)")
            .ExecuteWithoutResults();

        // 向其他人廣播
        BroadcastNode("搬移節點", moveNode, current);
        return true;
    }

    private void BroadcastNode(string msg, MyNode node, Ice.Current current)
    {
        string user_name = current.ctx["user_name"];
        foreach (var user in _users)
        {
            if (user.Name == user_name)
            {
                continue;
            }
            user.Cp.ResponseNode(msg, node, current.ctx);
        }
    }


    private UserList _users = new UserList();
}
