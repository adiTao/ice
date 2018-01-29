// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

#pragma once

module Demo
{
	//�ߵ�Client
	interface UserCallBack
	{
		void Response(string content);
		void ResponseNode(string content, string nodeId, string nodeText, string parentId);
	};

	interface UserMap
    {
	    idempotent void SendGreeting(string msg); //���۩I
		void SetupCallback(UserCallBack * cp); //���U�q���ƥ�
		bool Register(string name);//���U�b��
		void Unregister();//�h�X
        void shutdown();
    };
};
