// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

#pragma once

module Demo
{
	["clr:serializable:Demo.MyNode"] sequence<byte> Node;

    sequence<Node> NodeList;

	interface NodeMap
    {
        NodeList GetAllNodes(); //�C�X�Ҧ��`�I
		bool CreateNode(Node n);  //�s�W�`�I, �`�I�i�x�s���(string)
		bool EditNode(Node n);  //�s��`�I���
		bool DeleteNode(string nodeId);  //�R���`�I
		bool MoveNode(Node n);  //�h���`�I
    };
};
