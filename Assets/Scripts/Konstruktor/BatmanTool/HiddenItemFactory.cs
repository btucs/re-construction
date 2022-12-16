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

public class HiddenItemFactory : MonoBehaviour
{
    //public TaskDialogVariableHandler variableController;
    public GameObject itemPrefab;
    public Canvas itemContainer;
    private TaskDataSO taskData;
    private TaskObjectSO taskObj;


    public InventoryItem CreateItem(TaskVariable variable)
    {
    	InventoryItem inventoryItemScript = InventoryItemFactory.Instantiate(
	      itemPrefab,
	      itemContainer,
	      variable,
	      taskData,
	      taskObj
	    );

	    return inventoryItemScript;
    }

    public void Setup()
    {
    	GameController controller = GameController.GetInstance();
    	KonstruktorSceneData konstructorData = controller.gameState.konstruktorSceneData;
    	taskData = konstructorData.taskData;
    	taskObj = konstructorData.taskObject;
    }
}
