#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopicEntry : MonoBehaviour
{
	public Image iconUI;
    public RectTransform progressBarFill;
    public Sprite lockedIconSprite;
    public Text topicNameText;
    public Text progressText;
    public Text lockedText;
    public TopicDisplayManager topicManager;
    private TopicSO topicData;
    private bool isUnlocked = true;

    public void OpenDetailWindow()
    {
        topicManager.OpenTopicDetails(topicData);
    }

    public void SetupAsUnlocked(TopicSO newData)
    {
    	lockedText.gameObject.SetActive(false);
    	topicData = newData;
    	iconUI.sprite = topicData.icon;
    	
    	float progressVal = topicData.GetProgressValue();
   		int level = Mathf.FloorToInt(progressVal);
    	
    	//Debug.Log("level " + progressVal + " for " + topicData.name);
		progressText.text = (level < 1) ? "Fortschritt: " + Math.Round((progressVal - (float)level)*100) + '%' : "Abgeschlossen";
    	topicNameText.text = topicData.name;
    	progressBarFill.anchorMax = (progressVal >= 1f) ? new Vector2(1f, 1f) : new Vector2(progressVal, 1f);
    }

    public void SetupAsPreview(TopicSO newData)
    {  
        isUnlocked = false;
    	topicData = newData;
    	progressText.gameObject.SetActive(false);
    	lockedText.gameObject.SetActive(true);
    	lockedText.text = topicData.GetRequirementString();
    	progressBarFill.transform.parent.gameObject.SetActive(false);
    	topicNameText.text = topicData.name;
    	iconUI.sprite = lockedIconSprite;
    }
}
