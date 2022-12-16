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
using Yarn.Unity;

public class MentorHelpMessenger : MonoBehaviour
{
    public MentorDialogManager dialogController;
    public OutputMenuController outputMenu;
    public Button submitModuleButton;
    public TaskDataSO tutorialTask;
    public TaskDataSO dummyValueTutorialTask;

    private string startNodeName = "Konstruktor.Multistep";
    private GameController controller; 
    //private bool shouldBeActive = true;
    public KonstruktorHelpEntry searchedVarHelp = new KonstruktorHelpEntry();
    public KonstruktorHelpEntry multistepHelp = new KonstruktorHelpEntry();
    public KonstruktorHelpEntry vektorHelp = new KonstruktorHelpEntry();
    public KonstruktorHelpEntry submitSolutionHelp = new KonstruktorHelpEntry();
    public KonstruktorHelpEntry dummyValueHelp = new KonstruktorHelpEntry();
    private KonstruktorHelpEntry activeHelpEntry;

    private bool searchVarAssigned = false;
    private bool vectorModuleSelected = false;

    private void Start()
    {
		controller = GameController.GetInstance();
        CheckForHelpStart();

    }

    private bool SearchedVarIdentified()
    {
        return (outputMenu.GetOutputItemControllers()[0].droppedItem != null);
    }

    private void CheckForHelpStart()
    {
        int taskAmount = controller.gameState.taskHistoryData.CountDistinctTasks();
        if(taskAmount < 2 && controller.gameState.konstruktorSceneData.taskData.UID == tutorialTask.UID)
        {
            StartHelp(searchedVarHelp);
            outputMenu.onDrop.AddListener(OnSearchedVarSuccess);
            submitModuleButton.onClick.AddListener(OnSelectModuleSuccess);
        } else if (controller.gameState.konstruktorSceneData.taskData.UID == dummyValueTutorialTask.UID) {
            StartHelp(dummyValueHelp);
        }
    }

    public void OnSelectModuleSuccess()
    {
        if(!vectorModuleSelected)
        {
            vectorModuleSelected = true;
            EndCurrentHelp();
            StartHelp(vektorHelp);
        }
    }

    public void EndCurrentHelp()
    {
    	if(activeHelpEntry != null)
        {
            activeHelpEntry.OnFinish();
            activeHelpEntry = null;
        }
    }

    private void OnSearchedVarSuccess(InventoryItem item)
    {
        if(!searchVarAssigned)
        {
            searchVarAssigned = true;
            EndCurrentHelp();
            StartHelp(multistepHelp);
        }
    }

    private void StartHelp(KonstruktorHelpEntry helpToStart)
    {
        activeHelpEntry = helpToStart;
        helpToStart.StartHelp(dialogController);
    } 
}

[System.Serializable]
public class KonstruktorHelpEntry
{
    public string dialogNodeName = "Nodename";
    public List<GameObject> hideOnStart = new List<GameObject>();
    public List<GameObject> enableOnEnd = new List<GameObject>();
    private MentorDialogManager dialogManager;

    public void StartHelp(MentorDialogManager dialogController)
    {
        dialogManager = dialogController;
        dialogController.StartAtNode(dialogNodeName);

        foreach(GameObject hideObj in hideOnStart)
        {
            hideObj.SetActive(false);
        }
    }

    public void OnFinish()
    {
        foreach(GameObject enableObj in enableOnEnd)
        {
            enableObj.SetActive(true);
        }
    }

    public bool Equals(KonstruktorHelpEntry compareData)
    {
        return (compareData.dialogNodeName==this.dialogNodeName);
    }
}
