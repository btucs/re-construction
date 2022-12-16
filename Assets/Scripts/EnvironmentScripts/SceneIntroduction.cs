#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneIntroduction : MonoBehaviour
{
  #pragma warning disable 0649
  [SerializeField]
	private UIStateLoader pastUIRecoverScript;
  #pragma warning restore 0649
  public OnboardingController onboardingScript;
	public SystemNotificationController systemMessageScript;
	public RoomState gameWorldLoader;
	public MLEDataSO solvedMLE;

	[HideInInspector]
    public bool onHold = false;
    private bool unlockAreaMessageSend = false;
    private bool eventsFinished = false;
    private bool notificationStarted = false;
    private GameController controller;
	private playerScript interactionScript;


    private bool unlockLeft;
    private bool unlockRight;
    public bool MLEsuccessMessageSend;

    void Start()
    {
        controller = GameController.GetInstance();
        controller.gameState.gameworldData.activeScene = SceneManager.GetActiveScene().name;
        interactionScript = playerScript.Instance;
    	//We check if we come back from a MLE

    	gameWorldLoader.InitiateGameWorld();
    	onboardingScript.InitializeMenuUI();

        systemMessageScript.AllowTempMessages(false);
    	
        CheckForMLESuccess();
        CheckForRewardData();
        CheckForAreaUnlock();
        CheckForMapUnlock();
        CheckForQuestProgress();
    	eventsFinished = false;
    }

    void Update()
    {
    	//Debug.Log("onhold is " + onHold);
    	if(!onHold)
    	{
    		if(!MLEsuccessMessageSend)
    		{
    			onHold = true;
                if(solvedMLE != null)
    			 systemMessageScript.DisplayMLESuccessMSG(solvedMLE);
                interactionScript.menuOpen = true;
    			return;
    		}

            if(!eventsFinished)
            {
                bool eventStartet = onboardingScript.CheckForEvents();
                if(eventStartet)
                    onHold = true;
                eventsFinished = !eventStartet;
                return;
            }

            if(!unlockAreaMessageSend)
            {
                if(!notificationStarted)
                {
                    notificationStarted = true;
                    systemMessageScript.AllowTempMessages(true);
                    return;
                }

                if(systemMessageScript.DisplayingMessage())
                    return;

    			if(unlockRight)
                {
                    systemMessageScript.DisplayAreaUnlockMSG(false);
                    interactionScript.menuOpen = true;
                    unlockRight = false;
                    onHold = true;
                    return;
                }

                if(unlockLeft)
                {
                    systemMessageScript.DisplayAreaUnlockMSG(true);
                    interactionScript.menuOpen = true;
                    unlockLeft = false;
                    onHold = true;
                    return;
                }

    			unlockAreaMessageSend = true;
    		}

    		
    		//Debug.Log("Went to end.");
    	}
    }


    public void ConfirmNotificationClose()
    {
        interactionScript.menuOpen = false;
    }

    private void CheckForRewardData()
    {
        GameObject rewardDataObj = GameObject.FindWithTag("RewardData");
        if(rewardDataObj)
        {
            Debug.Log("Reward data found");
            RewardNotificationData dataTransporter = rewardDataObj.GetComponent<RewardNotificationData>();
            systemMessageScript.QueueRewardMSGs(dataTransporter.currencyGained, dataTransporter.expGained);
            Destroy(rewardDataObj);
        }
    }

    public void CheckForMLESuccess()
    {
        MLEsuccessMessageSend = true;
        //unlockAreaMessageSend = true;

    	GameObject UISaveObject = GameObject.FindWithTag("MLEDataObj");
    	if(UISaveObject)
    	{
    		Debug.Log("Save Object Found: " + UISaveObject.name);
    		MLEDataTransporter mleReturnData = UISaveObject.GetComponent<MLEDataTransporter>();
    		if(mleReturnData)
    		{
    			//If there is no entry in player History 
    			// we add Exp to playerExp and mle to history and Save Game
    			if(mleReturnData.achievedPoints > 0)
                {
                    SaveMLEProgress(mleReturnData.mleTransportData, mleReturnData.achievedPoints);
                    solvedMLE = mleReturnData.mleTransportData;

                    unlockAreaMessageSend = false;
                    MLEsuccessMessageSend = false; 
                }

                //check if we want to open the scene with some active UI we return to
                pastUIRecoverScript.OpenPreviousUI(mleReturnData);
                
    		}
    		
    		Destroy(UISaveObject);
    		//interactionScript.interactionEnabled = false;
    	}
    }

    private void CheckForMapUnlock()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if(controller.gameState.gameworldData.mapData.UnlockArea(currentSceneName))
        {
            string mapUnlockedMessage = "Neues Gebiet entdeckt!";
            string substring = "Dieses Gebiet wird jetzt auf der Weltkarte angezeigt."; 
            systemMessageScript.QueueTempMessage(mapUnlockedMessage, 3f, substring);
            controller.SaveGame();
        }
        //
        //WorldMapData newData = saveDataController.gameState.gameworldData.mapData;
    }
    
    public void CheckForAreaUnlock()
    {
        unlockLeft = gameWorldLoader.CheckForNewLeftArea();
        unlockRight = gameWorldLoader.CheckForNewRightArea();

        unlockAreaMessageSend = false;
    }


    public void CheckForQuestProgress()
    {
        ProfileData profileData = controller.gameState.profileData;

        for(int i = profileData.activeQuests.Count - 1; i >= 0; i--)
        {
            string questName = profileData.activeQuests.ElementAt(i).Key;
            MainQuestSO questData = controller.gameAssets.FindQuest(questName);

            int saveIndex = profileData.activeQuests[questName];
            int currentIndex = questData.GetCurrentStep(controller, saveIndex);
            bool isSolved = questData.Solved(currentIndex);
            if(currentIndex > saveIndex && isSolved == false)
            { 
                systemMessageScript.DisplayQuestProgressMessage(questData);                    
                profileData.activeQuests[questName] = currentIndex;
                controller.gameState.profileData = profileData;
                controller.SaveGame();
            }
            else if(isSolved)
            {
                profileData.OnQuestFinished(questData);
                systemMessageScript.DisplayQuestFinishedMessage(questData);
            }

        }
        
        if(MenuUIController.Instance != null)
            MenuUIController.Instance.questUIController.UpdateQuestSubHint();

    }

    public void SaveMLEProgress(MLEDataSO correspondingMLE, int points)
    {
   		controller.gameState.taskHistoryData.mleHistory.Add(new FinishedMLEData(correspondingMLE, points));
     	controller.SaveGame();	
    }
}
