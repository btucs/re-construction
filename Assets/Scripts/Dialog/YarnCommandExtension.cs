#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnCommandExtension : MonoBehaviour
{
	private OnboardingController onboardingController;
	private MenuUIController uiController;
    public DialogueRunner yarnRunner;
    public DialogueUI yarnUI;

    private bool autoContinue = false;
    private GameController saveController;

    private void Awake()
    {
        // Create a new command
	    yarnRunner.AddCommandHandler(
	            "event_continue",
	            ContinueEvent
	    );

	    yarnRunner.AddCommandHandler(
	    	"AutoContinue",
	    	AutoMarkLineComplete
	    );

        yarnRunner.AddCommandHandler(
            "AddQuest",
            AddQuest
        );

        yarnRunner.AddCommandHandler(
            "ContinueQuest",
            ContinueQuest
        );

	    yarnUI.onLineFinishDisplaying.AddListener(AutoContinueCheck);
    }

    private void Start()
    {
    	onboardingController = OnboardingController.Instance;
    	uiController = MenuUIController.Instance;
        saveController = GameController.GetInstance();
    }

    // The method that gets called when '<<event_continue>>' is run.
    private void ContinueEvent(string[] parameters)
    {
        onboardingController.ContinueActiveEvent();
    }

    private void AutoMarkLineComplete(string[] parameters)
    {
    	autoContinue = true;
    }

    private void AutoContinueCheck()
    {
    	if(autoContinue)
    	{
    		autoContinue = false;
    		yarnUI.MarkLineComplete();
    	}
    }

    private void AddQuest(string[] parameters)
    {
        string questUID = parameters[0];
        MainQuestSO questToAdd = saveController.gameAssets.FindQuestByShortID(questUID);
        saveController.gameState.profileData.AddActiveQuest(questToAdd);
        saveController.SaveGame();
    }

    private void ContinueQuest(string[] parameters)
    {
        string questUID = parameters[0];
        MainQuestSO questData = saveController.gameAssets.FindQuestByShortID(questUID);
        int index = saveController.gameState.profileData.GetStepOfActiveQuest(questData);
        if(index>=0)
        {
            index++;
            if(questData.objectives.Count > index)
            {
                if(MenuUIController.Instance != null)
                    MenuUIController.Instance.notificationSystem.DisplayQuestProgressMessage(questData);

                saveController.gameState.profileData.activeQuests[questData.UID] = index;
            } else {
                if(MenuUIController.Instance != null)
                    MenuUIController.Instance.notificationSystem.DisplayQuestFinishedMessage(questData);

                saveController.gameState.profileData.OnQuestFinished(questData);
            }
        }
        if(MenuUIController.Instance != null)
            MenuUIController.Instance.questUIController.UpdateQuestSubHint();
        saveController.SaveGame();
    }
}