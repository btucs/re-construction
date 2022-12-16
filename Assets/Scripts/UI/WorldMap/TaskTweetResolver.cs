#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTweetResolver : MonoBehaviour
{
    public WorldMapController worldMap;
    public TweetDisplay tweetUI;
    private GameController saveDataController;
    private TweetData[] possibleTweets;
    private TweetDataInstance identifiedTweet;
    //public KonstruktorObjectFeedbackSetup feedbackController;
    private WorldObjectResolver taskStateControllerRef;

    public void UpdateTweetDisplay()
    {
    	saveDataController = GameController.GetInstance();
    	GetTweets();

    	TaskDataSO currentTask = saveDataController.gameState.konstruktorSceneData.taskData;
    	taskStateControllerRef = gameObject.AddComponent<WorldObjectResolver>();
    	RequiredTaskState currentTaskState = taskStateControllerRef.GetSolveStateOfTask(currentTask, saveDataController.gameState.taskHistoryData);
    	
    	IdentifyTaskTweet(currentTask, currentTaskState);
    	if(identifiedTweet != null)
    	{
    		tweetUI.Setup(identifiedTweet);
    		AddTweetToSaveGame(identifiedTweet);

    	} else { Debug.Log("no tweet identified."); }
    }

    private void GetTweets()
    {
    	possibleTweets = saveDataController.gameAssets.GetTweets();

    }

    private void IdentifyTaskTweet(TaskDataSO identTask, RequiredTaskState identCondition)
    {
    	Debug.Log("Found " + possibleTweets.Length + " different task tweet datasets for " + identTask.taskName);
    	for(int i = 0; i < possibleTweets.Length; i++)
    	{
    		if(possibleTweets[i].IsTaskFeedback(identTask, identCondition))
    		{
	    		identifiedTweet = new TweetDataInstance(possibleTweets[i]);
    		} 
    	}
    }

    private void AddTweetToSaveGame(TweetDataInstance newTweet)
    {
    	saveDataController.gameState.gameworldData.tweets.Add(newTweet);
    	saveDataController.SaveGame();
    }
    
}
