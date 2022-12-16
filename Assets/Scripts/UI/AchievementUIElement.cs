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

public class AchievementUIElement : MonoBehaviour
{
    public Image iconBackground;
    public Image achievementIcon;
    public Text nameText;
    public Text progressText;
    private AchievementSO currentData;
    private GameController controller;
    private PortfolioContentResolver uiManager;

    public Sprite lvl1Background;
    public Sprite lvl2Background;
    public Sprite lvl3Background;
    public Sprite lvl4Background;

    public void SetContent(AchievementSO data, PortfolioContentResolver portfolioUI)
    {
    	uiManager = portfolioUI;
    	controller = GameController.GetInstance();
    	currentData = data;
    	
    	int level = currentData.GetCurrentLevel(controller.gameState);
    	nameText.text = CreateNameString();
    	progressText.text = CreateProgressString();
    	achievementIcon.sprite = currentData.icon;

    	iconBackground.color = data.tint;
    	SetBackgroundGraphic(level);
    }

    public void OpenDetails()
    {
    	uiManager.DisplayAchievementDetails(this);
    }

    public AchievementSO GetData()
    {
    	return currentData;
    }

    public string CreateNameString()
    {
    	int _level = currentData.GetCurrentLevel(controller.gameState);
    	return currentData.title + ": " + currentData.levels[_level].levelName;
    }

    public string CreateProgressString()
    {
    	int progressInt = currentData.GetProgressAmount(controller.gameState);
    	int nextLevelInt = currentData.GetNextLevelThreshold(progressInt);
    	string returnText = progressInt.ToString() + " / " + nextLevelInt.ToString() + ' ' + currentData.progressText;

    	return returnText;
    }

    private void SetBackgroundGraphic(int _level)
    {
    	switch(_level)
    	{
    		case 3:
    			iconBackground.sprite = lvl4Background;
    			break;
    		case 2:
    			iconBackground.sprite = lvl3Background;
    			break;
    		case 1:
    			iconBackground.sprite = lvl2Background;
    			break;
    		default:
    			iconBackground.sprite = lvl1Background;
    			break;
    	}
    }
}
