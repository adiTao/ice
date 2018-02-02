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
        NodeSeq GetAllNodes(string graphName); //�C�X�Ҧ��Ϥ��`�I
		GraphSeq GetAllMaps(); //�C�X�Ҧ���
		bool CreateGraph(string graphName);  //�s�W��
		bool EditGraph(string graphName, string newGraphName);  //�s���
		bool DeleteGraph(string graphName);  //�R����
    };
};
