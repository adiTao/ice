// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************
#include <..\..\..\NodeMap.ice>
#pragma once

module Demo
{
	["clr:serializable:Demo.MyGraph"] sequence<byte> Graph;
	sequence<Graph> GraphSeq;
	interface GraphMap
    {
        NodeSeq GetAllNodes(string graphName); //列出所有圖內節點
		GraphSeq GetAllMaps(); //列出所有圖
		bool CreateGraph(string graphName);  //新增圖
		bool EditGraph(string graphName, string newGraphName);  //編輯圖
		bool DeleteGraph(string graphName);  //刪除圖
    };
};
