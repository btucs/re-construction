#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardMessage : MonoBehaviour
{
	public TextMeshProUGUI bankCashText;
	public TextMeshProUGUI addCashText;
	public GameObject cashMSG;
	public Canvas notificationCanvas;

	public Vector2 offSet;

	private bool isAnimating = false;
	private int cashOrigin = 0;
	private int cashTemp = 0;
	private int cashToAdd = 0;
	private TempMessageEntry messageData;

	private float waitTimeStart = 1f;
	private float waitTimeEnd = 1f;

	private Transform target;
	private RectTransform msgTransform;
	private Camera activeCamera;
	private Vector3 prevTargetPos, prevCamPos;

	public void StartDisplayCashMSG(TempMessageEntry _messageData)
	{
		messageData = _messageData;

		activeCamera = Camera.main;
		msgTransform = cashMSG.GetComponent<RectTransform>();
		target = MenuUIController.Instance.playerController.transform;

		GameController controller = GameController.GetInstance();
		int currencyNow = controller.gameState.profileData.inventory.currencyAmount;
		//int expNow = controller.gameState.taskHistoryData.CalculateCurrentEXP();
		cashOrigin = (currencyNow - messageData.rewardAmount);

		isAnimating = true;
		cashMSG.SetActive(true);
		UpdateCashTexts(cashOrigin, messageData.rewardAmount);
	} 
    
    public void UpdateDisplay(float timeSinceStart)
    {
    	if(isAnimating)
    	{
	        if(timeSinceStart > waitTimeStart && timeSinceStart < (messageData.duration-waitTimeEnd))
	        {
	        	float progressionValue = (timeSinceStart - waitTimeStart) / (messageData.duration - (waitTimeStart + waitTimeEnd));
	        	int interpolatedValue = Mathf.FloorToInt(Mathf.Lerp(0f, (float)messageData.rewardAmount, progressionValue));
	        	cashToAdd = messageData.rewardAmount - interpolatedValue;
	        	cashTemp = cashOrigin + interpolatedValue;
	        	UpdateCashTexts(cashTemp, cashToAdd);
	        } else if (timeSinceStart > (messageData.duration-waitTimeEnd)) {
	        	isAnimating = false;
	        	UpdateCashTexts(cashOrigin+messageData.rewardAmount, 0);
	        }
    	}
    }

    void LateUpdate()
	{
    	UpdateUIPosition();
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

    public void StopDisplayCashMSG()
    {
    	cashMSG.SetActive(false);
    	isAnimating = false;
    }

    private void UpdateCashTexts(int totalCash, int addCash)
    {
    	bankCashText.text = totalCash.ToString();
		addCashText.text = '+' + addCash.ToString();
    }
}
