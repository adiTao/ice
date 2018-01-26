// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

#pragma once

module Demo
{
	["clr:serializable:Demo.MyNode"] sequence<byte> Node;
	
	//拋給Client
	interface CallBack
	{
		void Response(string content);
		void ResponseNode(string content, Node n);
	};

	interface NodeMap
    {
	    idempotent void SendGreeting(string msg); //打招呼
        bool CreateMap(string mapName); //新建圖
		bool CreateNode(Node n);  //新增節點, 節點可儲存資料(string)
		bool EditNode(Node n);  //編輯節點資料
		bool DeleteNode(string nodeId);  //刪除節點
		bool MoveNode(Node n);  //搬移節點
		void SetupCallback(CallBack * cp); //註冊通知事件
		bool Register(string name);//註冊帳號
		void Unregister();//退出
        void shutdown();
    };
};
