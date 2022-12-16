#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestlogController : MonoBehaviour
{
	public Text selectedQuestName;
    public Text selectedQuestDescription;
    public Text selectedQuestProgress;

    public Text questSubscriptionHint;

    public GameObject questListContainer;
    public GameObject questDetailContainer;

    public GameObject activeQuestPrefab;
    public GameObject completedQuestPrefab;

	public Transform activeQuestContainer;

	public CanvasGroup activeQuestTab;
	public CanvasGroup completedQuestTab;

	private GameController controller;
	private ActiveQuestDisplay currentQuestSubscription;

	public void OpenQuestUI()
	{
		controller = GameController.GetInstance();
		CreateQuestList();
	}

	public void SubscribeToQuest(ActiveQuestDisplay newSubscription)
	{
		if(controller == null)
			controller = GameController.GetInstance();

		controller.gameState.profileData.subscribedQuest = newSubscription.questData.UID;
		controller.SaveGame();
		if(currentQuestSubscription != null)
			currentQuestSubscription.Unsubscripe();

		currentQuestSubscription = newSubscription;
		UpdateQuestSubHint();
	}

    private void CreateQuestList()
	{
    	ProfileData profileData = controller.gameState.profileData;
    	List<MainQuestSO> newAddedQuests = new List<MainQuestSO>();
    	//clear list
    	foreach(Transform child in activeQuestContainer) {
    	  GameObject.Destroy(child.gameObject);
    	}

    	//add entry for each active Quest
    	for(int i = profileData.activeQuests.Count - 1; i >= 0; i--)
    	{
    		string activeQuest = profileData.activeQuests.Keys.ElementAt(i);
    		MainQuestSO questData = controller.gameAssets.FindQuestByID(activeQuest);

     		if(questData.Solved(controller))
     		{
        		profileData.OnQuestFinished(questData);
        		string finishedMessage = "Quest abgeschlossen: " + questData.title;
        		MenuUIController.Instance.notificationSystem.QueueTempMessage(finishedMessage, 2.5f);
        		if(questData.followUpQuest != null)
        		{
        			newAddedQuests.Add(questData.followUpQuest);
        		}
    		} else {
        		GameObject activeQuestObj = Instantiate(activeQuestPrefab, activeQuestContainer);
        		ActiveQuestDisplay QuestScript = activeQuestObj.GetComponent<ActiveQuestDisplay>();
        		QuestScript.questData = questData;
        		QuestScript.questUIController = this;
        		QuestScript.DisplayQuestData();        
    		}
    	}

    	foreach(MainQuestSO newQuest in newAddedQuests)
    	{
    		GameObject activeQuestObj = Instantiate(activeQuestPrefab, activeQuestContainer);
    		ActiveQuestDisplay QuestScript = activeQuestObj.GetComponent<ActiveQuestDisplay>();
    		QuestScript.questData = newQuest;
    		QuestScript.questUIController = this;
    		QuestScript.DisplayQuestData();    
		}
	}

	public void DisplayCompletedQuest()
	{
		activeQuestTab.alpha = 0.5f;
		completedQuestTab.alpha = 1f;

		List<string> completedQuests = controller.gameState.profileData.finishedQuests;
		foreach(string completedQuest in completedQuests)
		{
			//MainQuestSO questData = contr
		}
	}

	public void DisplayQuestDetails(ActiveQuestDisplay questController)
	{
        selectedQuestName.text = questController.questData.title;
        selectedQuestDescription.text = questController.questData.description;
        selectedQuestProgress.text = questController.GetProgressMSG();

        questListContainer.SetActive(false);
        questDetailContainer.SetActive(true);
	}

	public void UpdateQuestSubHint()
	{
		controller = GameController.GetInstance();

		MainQuestSO subQuest = controller.gameAssets.FindQuestByID(controller.gameState.profileData.subscribedQuest);
		if(subQuest != null && controller.gameState.profileData.IsQuestActive(subQuest) == true)
		{
			questSubscriptionHint.text = subQuest.GetProgressText(controller);
			questSubscriptionHint.transform.parent.gameObject.SetActive(true);
		} else 
		{
			controller.gameState.profileData.subscribedQuest = "";
			questSubscriptionHint.transform.parent.gameObject.SetActive(false);
		}
	}
}
