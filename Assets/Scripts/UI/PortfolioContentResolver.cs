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

public class PortfolioContentResolver : MonoBehaviour
{
  public Text playerNameHolder;
  public Text rankHolder;
  public Text expAmountHolder;
  public RectTransform progressBarFill;
  public Image avatarImg;

  public GameObject achievementDetails;
  public Text achievementDetailText;
  public Text achievementNameText;
  public Text achievementProgressText;

  private GameController controller;

  public GameObject achievementPrefab;
  public GameObject achievementUI;
  public Transform AchievementContainer;
  public ShopController shopController;

  public List<AchievementSO> achievements = new List<AchievementSO>();

  public void UpdateProfileContent()
  {
    controller = GameController.GetInstance();
    avatarImg.sprite = controller.gameState.characterData.player.thumbnail;

    ProfileData profileData = controller.gameState.profileData;
    TaskHistoryData taskHistory = controller.gameState.taskHistoryData;
    PlayerRankSO rankManager = controller.gameAssets.playerRanks;

    int currentExp = taskHistory.CalculateCurrentEXP();
    RankInfo rankInfo = rankManager.GetRankInfo(currentExp);

    playerNameHolder.text = profileData.playerName;
    rankHolder.text = "Rang: " + rankInfo.currentRank.title;

    string nextRank = rankInfo.nextRank != null ? rankInfo.nextRank.expRequirement + " EP" : "Max";
    string expString = currentExp + " / " + nextRank;
    expAmountHolder.text = expString;

    UpdateProgressBar(currentExp, rankInfo);
    shopController.UpdateCurrencyDisplay();

    CreateAchievementEntries();
  }

  public void OpenAchievementList()
  {
    string achievementHeadline = "Achievements";
    MenuUIController.Instance.breadcrumController.openSecondLayer(achievementUI, achievementHeadline, this.gameObject);
  }

  private void UpdateProgressBar(int currentExp, RankInfo rankInfo) {

    Vector2 fillStretch = new Vector2(0.05f, 1f);
    if(rankInfo.nextRank == null) {

      fillStretch.x = 1;
    } else {

      fillStretch.x = (float)(currentExp - rankInfo.currentRank.expRequirement)/(rankInfo.nextRank.expRequirement - rankInfo.currentRank.expRequirement);
    }
    progressBarFill.anchorMax = fillStretch;
  }

  private void Start() {
    UpdateProfileContent();
  }

  private void CreateAchievementEntries()
  {
    foreach(Transform child in AchievementContainer)
    {
      GameObject.Destroy(child.gameObject);
    }
    Debug.Log("Creating achievement entries");
    foreach(AchievementSO achievement in achievements)
    {
      //check for display condition?
      if(achievement.GetProgressAmount(controller.gameState) > 0)
      {
        AchievementUIElement newUIElement = Instantiate(achievementPrefab, AchievementContainer).GetComponent<AchievementUIElement>();
        newUIElement.SetContent(achievement, this);
      }
    }
  }

  public void DisplayAchievementDetails(AchievementUIElement entryToDisplay)
  {
    achievementDetailText.text = entryToDisplay.GetData().description;
    achievementNameText.text = entryToDisplay.CreateNameString();
    achievementProgressText.text = entryToDisplay.CreateProgressString();

    achievementDetails.SetActive(true);
  }

  public void SaveGame()
  {
    if(controller == null) {

      controller = GameController.GetInstance();
    }

    controller.SaveGame();
  }
  /*public void AddAchievementsToList()
  {
    foreach(AchievementEntry currentAchievement in currentPlayerData.achievements)
    {
      GameObject archivementObject = Instantiate(achievementPrefab, AchievementContainer);
      
    }
  }*/
}