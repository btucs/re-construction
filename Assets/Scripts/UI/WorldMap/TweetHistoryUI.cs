#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweetHistoryUI : MonoBehaviour
{
    public Transform tweetContainer;
    public GameObject tweetPrefab;
    public TweetData defaultTweetData;
    private List<TweetDataInstance> tweetDataList = new List<TweetDataInstance>();
    private GameController saveDataController;

    void Start()
    {
        GetTweetData();

    	if(tweetDataList.Count == 0)
    	{
    		TweetDataInstance defaultEntry = new TweetDataInstance(defaultTweetData);
    		tweetDataList.Add(defaultEntry);
            SaveTweetData();
    	}

    	CreateTweetEntries();
    }

    private void CreateTweetEntries()
    {
    	foreach(Transform child in tweetContainer)
    	{
    		Destroy(child.gameObject);
    	}

    	foreach(TweetDataInstance post in tweetDataList)
    	{
    		CreateNewTweetEntry(post);
    	}
    }

    private void CreateNewTweetEntry(TweetDataInstance dataToDisplay)
    {
    	GameObject newTweetObj = Instantiate(tweetPrefab, tweetContainer);
    	newTweetObj.GetComponent<TweetDisplay>().Setup(dataToDisplay);
    }

    private void GetTweetData()
    {
        CheckForSaveController();
        tweetDataList = saveDataController.gameState.gameworldData.tweets;
    }

    private void SaveTweetData()
    {
        CheckForSaveController();
        saveDataController.gameState.gameworldData.tweets = tweetDataList;
        saveDataController.SaveGame();
    }

    private void CheckForSaveController()
    {
        if(saveDataController == null)
        {
            saveDataController = GameController.GetInstance();
        }
    }
}
