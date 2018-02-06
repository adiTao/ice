// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************
#include <..\..\..\slice\NodeMap.ice>
#pragma once

module Demo
{
	//拋給Client
	interface UserCallBack
	{
		void Response(string content);
		void ResponseNode(string content, Node node);
		void ResponseGraph(string content, string graph);
	};

	interface UserMap
    {
	    void SendGreeting(string msg); //打招呼
		void SetupCallback(UserCallBack * cp); //註冊通知事件
		bool Register(string name);//註冊帳號
		void Unregister();//退出
        void shutdown();
    };
};
