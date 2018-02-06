// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************
#include <..\..\..\slice\NodeMap.ice>
#pragma once

module Demo
{
	//�ߵ�Client
	interface UserCallBack
	{
		void Response(string content);
		void ResponseNode(string content, Node node);
		void ResponseGraph(string content, string graph);
	};

	interface UserMap
    {
	    void SendGreeting(string msg); //���۩I
		void SetupCallback(UserCallBack * cp); //���U�q���ƥ�
		bool Register(string name);//���U�b��
		void Unregister();//�h�X
        void shutdown();
    };
};
