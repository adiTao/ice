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
        NodeSeq GetAllNodes(); //�C�X�Ҧ��`�I
		bool CreateNode(string graphName, Node n);  //�s�W�`�I, �`�I�i�x�s���(string)
		bool EditNode(string graphName, Node n);  //�s��`�I���
		bool DeleteNode(string graphName, string nodeId);  //�R���`�I
		bool MoveNode(string graphName, Node n);  //�h���`�I
    };
};
