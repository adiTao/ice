// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

#pragma once

module Demo
{
	["clr:serializable:Demo.MyNode"] sequence<byte> Node;
	
	//�ߵ�Client
	interface CallBack
	{
		void Response(string content);
		void ResponseNode(string content, Node n);
	};

	interface NodeMap
    {
	    idempotent void SendGreeting(string msg); //���۩I
        bool CreateMap(string mapName); //�s�ع�
		bool CreateNode(Node n);  //�s�W�`�I, �`�I�i�x�s���(string)
		bool EditNode(Node n);  //�s��`�I���
		bool DeleteNode(string nodeId);  //�R���`�I
		bool MoveNode(Node n);  //�h���`�I
		void SetupCallback(CallBack * cp); //���U�q���ƥ�
		bool Register(string name);//���U�b��
		void Unregister();//�h�X
        void shutdown();
    };
};
