// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************
#pragma once

module Demo
{
	["clr:serializable:Demo.MyNode"] sequence<byte> Node;

    sequence<Node> NodeSeq;

	interface NodeMap
    {
        NodeSeq GetAllNodes(); //列出所有節點
		bool CreateNode(string graphName, Node n);  //新增節點, 節點可儲存資料(string)
		bool EditNode(string graphName, Node n);  //編輯節點資料
		bool DeleteNode(string graphName, string nodeId);  //刪除節點
		bool MoveNode(string graphName, Node n);  //搬移節點
    };
};
