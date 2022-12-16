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

public class ActiveQuestDisplay : MonoBehaviour
{
    public Text questHeadline;
    public Text questDescription;
    public Text progressText;
    public MainQuestSO questData;
    public QuestlogController questUIController;
    public GameObject subscriptionMarker;

    public void DisplayQuestData()
    {
        string questTypeString = questData.isMainQuest ? " (Kampagne)" : " ";
    	questHeadline.text = questData.title + questTypeString;
    	questDescription.text = questData.description;

    	if(questData.UID == GameController.GetInstance().gameState.profileData.subscribedQuest)
            SetQuestSubscription();

    	progressText.text = GetProgressMSG();
    }

    public void SetQuestSubscription()
    {
        questUIController.SubscribeToQuest(this);
        subscriptionMarker.SetActive(true);
    }

    public void Unsubscripe()
    {
        subscriptionMarker.SetActive(false);
    }

    public string GetProgressMSG()
    {
        GameController controller = GameController.GetInstance();
    	
    	return questData.GetProgressText(controller);
    } 

    public void OpenQuestDetails()
    {
        questUIController.DisplayQuestDetails(this);
    }
}
