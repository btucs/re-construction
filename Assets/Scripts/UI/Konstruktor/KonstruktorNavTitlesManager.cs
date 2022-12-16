#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KonstruktorNavTitlesManager : MonoBehaviour
{
	public GameObject moduleSelectionCanvas;
	public GameObject konstruktorCanvas;
	public ConstructorSolutionController konstruktorController;

	private GameController controller;

	void Start()
	{
		controller = GameController.GetInstance();
	}

    public string ReturnPrevLayerName()
    {
    	string layerName;
        layerName = controller.gameState.konstruktorSceneData.taskData.taskName;

        if (konstruktorCanvas!=null && konstruktorCanvas.activeSelf == true)
    	{
    		layerName = konstruktorController.GetKonstruktorName();
    	}
    	else if(moduleSelectionCanvas != null && moduleSelectionCanvas.activeSelf == true)
    	{
    		layerName = "Lösungsbereich";
    	}

    	return layerName;
    }

    public string ReturnActiveTaskName()
    {
    	string name = controller.gameState.konstruktorSceneData.taskData.taskName;
    	return name;
    }

    public string ReturnActiveKonstruktorName()
    {
    	string konstruktorName = konstruktorController.GetKonstruktorName();
    	return konstruktorName;
    }
}
