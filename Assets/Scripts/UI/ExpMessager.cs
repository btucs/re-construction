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

public class ExpMessager : MonoBehaviour
{
    public Canvas notificationCanvas;
    public Text currentRankName;
    public Text expGainAmount;
    public RectTransform progressBarFill;

    public Vector2 offSet;

    private float waitTimeStart = 1f;
    private float waitTimeEnd = 1f;

    private int prevEXP = 0;

    private Transform target;
    private RectTransform msgTransform;
    private Camera activeCamera;
    private Vector3 prevTargetPos, prevCamPos;
    private GameController controller;
    private bool isAnimating = false;

    private TempMessageEntry messageData;

    void LateUpdate()
    {
        UpdateUIPosition();
    }

    public void UpdateDisplay(float timeSinceStart)
    {
        if(isAnimating)
        {
            if(timeSinceStart > waitTimeStart && timeSinceStart < (messageData.duration-waitTimeEnd))
            {
                float progressionValue = (timeSinceStart - waitTimeStart) / (messageData.duration - (waitTimeStart + waitTimeEnd));
                float interpolatedValue = Mathf.Lerp(0f, (float)messageData.rewardAmount, progressionValue);
                int rounded = Mathf.FloorToInt(interpolatedValue);

                RankInfo currentRankInfo = controller.gameAssets.playerRanks.GetRankInfo(rounded);
                UpdateProgressBar(interpolatedValue, currentRankInfo); 
                currentRankName.text = currentRankInfo.currentRank.title;

                int expToAdd = messageData.rewardAmount - rounded;
                expGainAmount.text = "+" + expToAdd;
                
            } else if (timeSinceStart > (messageData.duration-waitTimeEnd)) {
                isAnimating = false;
            }
        }
    }

    public void StartDisplay(TempMessageEntry _messageData)
    {
        messageData = _messageData;
        msgTransform = this.GetComponent<RectTransform>();
        activeCamera = Camera.main;
        target = MenuUIController.Instance.playerController.transform;

        controller = GameController.GetInstance();
        prevEXP = controller.gameState.taskHistoryData.CalculateCurrentEXP();
        prevEXP -= _messageData.rewardAmount;
        RankInfo prevRankInfo = controller.gameAssets.playerRanks.GetRankInfo(prevEXP); 

        currentRankName.text = prevRankInfo.currentRank.title;
        
        msgTransform.gameObject.SetActive(true);
        isAnimating = true;
    }

    public void StopDisplay()
    {
        msgTransform.gameObject.SetActive(false);
        isAnimating = false;
    }

    void UpdateUIPosition()
    {
        if(target != null && prevTargetPos != target.position || activeCamera.transform.position != prevCamPos)
        {
            Vector3 targetPos = new Vector3(target.position.x + offSet.x, target.position.y + offSet.y, 0);
            Vector3 ScreenPos = activeCamera.WorldToScreenPoint(targetPos);
            ScreenPos.x = ScreenPos.x / notificationCanvas.scaleFactor;
            ScreenPos.y = ScreenPos.y / notificationCanvas.scaleFactor;
            msgTransform.anchoredPosition = new Vector2(ScreenPos.x, ScreenPos.y);
            prevTargetPos = target.position;
            prevCamPos = activeCamera.transform.position;
        }
    }

    private void UpdateProgressBar(float currentExp, RankInfo rankInfo) {

    Vector2 fillStretch = new Vector2(0.05f, 1f);
    if(rankInfo.nextRank == null) {

      fillStretch.x = 1;
    } else {

      fillStretch.x = (currentExp - (float)rankInfo.currentRank.expRequirement)/(float)(rankInfo.nextRank.expRequirement - rankInfo.currentRank.expRequirement);
    }
    progressBarFill.anchorMax = fillStretch;
  }

}
