// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

using System;
using System.Linq;
using Demo;

public class GraphMapI : GraphMapDisp_
{
    public override MyNode[] GetAllNodes(string graphName, Ice.Current current)
    {
        var query = Neo4jConfig.GraphClient.Cypher
            .Match("(node:Node)<-[CONTAINS]-(graph:Graph)")
            .Where((MyGraph graph) => graph.GraphName == graphName)
            .Return(node => node.As<MyNode>());
        var data = query.Results.ToArray();

        return data;
    }

    public override MyGraph[] GetAllMaps(Ice.Current current)
    {
        var query = Neo4jConfig.GraphClient.Cypher
            .Match("(graph:Graph)")
            .Return(graph => graph.As<MyGraph>());
        var data = query.Results.ToArray();

        return data;
    }

    public override bool CreateGraph(string graphName, Ice.Current current)
    {
        MyGraph newGraph = new MyGraph {GraphName = graphName};
        Neo4jConfig.GraphClient.Cypher
            .Merge("(graph:Graph { GraphName: {graphName} })")
            .OnCreate()
            .Set("graph = {newGraph}")
            .WithParams(new
            {
                graphName = newGraph.GraphName,
                newGraph
            })
            .ExecuteWithoutResults();

        // 向其他人廣播
        BroadcastGraph("新建圖", graphName, current);
        return true;
    }

    public override bool EditGraph(MyGraph oldGraph, string newGraphName, Ice.Current current)
    {
        Neo4jConfig.GraphClient.Cypher
            .Match("(graph:Graph)")
            .Where((MyGraph graph) => graph.GraphName == oldGraph.GraphName)
            .Set("graph.GraphName = {graphName}")
            .WithParam("graphName", newGraphName)
            .ExecuteWithoutResults();
        // 向其他人廣播
        BroadcastGraph("編輯圖", newGraphName, current);
        return true;
    }

    public override bool DeleteGraph(string graphName, Ice.Current current)
    {
        //Neo4jConfig.GraphClient.Cypher
        //    .Match("(graph:Graph)")
        //    .Where((MyGraph graph) => graph.GraphName == graphName)
        //    .DetachDelete("graph")
        //    .ExecuteWithoutResults();
        Neo4jConfig.GraphClient.Cypher
            .OptionalMatch("(graph:Graph)-[CONTAINS]->(node:Node)")
            .Where((MyGraph graph) => graph.GraphName == graphName)
            .DetachDelete("graph, node")
            .ExecuteWithoutResults();

        Neo4jConfig.GraphClient.Cypher
            .OptionalMatch("(graph:Graph)")
            .Where((MyGraph graph) => graph.GraphName == graphName)
            .DetachDelete("graph")
            .ExecuteWithoutResults();

        // 向其他人廣播
        BroadcastGraph("刪除圖", graphName, current);
        return true;
    }

    private void BroadcastGraph(string msg, string graphName, Ice.Current current)
    {
        string user_name = current.ctx["user_name"];
        foreach (var user in UserUtils.OnlineUser)
        {
            if (user.Name == user_name)
            {
                continue;
            }
            //user.Cp.ResponseGraph(msg, graphName, current.ctx);
            TaskUtils.AddTask(user.Cp.ResponseGraphAsync(msg, graphName, current.ctx));
        }
    }
}
