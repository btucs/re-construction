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
using UnityEngine.Events;

public class SystemNotificationController : MonoBehaviour
{
	public CanvasGroup tempMessageContainer;
	public Text tempNotificationText;
	public GameObject secondLine;
	public Text secondLineText;

	public GameObject notificationPanel;
	public Text messageText;
	public Text messageHeadline;
	public Button confirmButton;
	public Text confirmButtonText;
	public SceneIntroduction introManager;

	public RewardMessage rewardMessager;
	public ExpMessager expMessager;
	public RoomState roomController;

	public UnityEvent onNotificationSend = new UnityEvent();
	public UnityEvent onAreaUnlock = new UnityEvent();
	public UnityEvent onCashGained = new UnityEvent();
	public UnityEvent onLevelUp = new UnityEvent();

	public UnityAction passFunction;
	private bool onHold = true;
	private bool tempMSGActive = false;
	private float targetAlpha = 0f;
	private float startAlpha = 0f;
	private float timeSinceAnimStart = 0f;
	private float animDuration = 1f;
	private List<TempMessageEntry> tempMessageQueue = new List<TempMessageEntry>();
	private TempMessageEntry activeMessageElement;

	//private float queueDuration = 0f;
	private void Update()
	{
		Animate();
	}

	public void AllowTempMessages(bool isAllowed)
	{
		//Debug.Log("Temp messages now send: " + isAllowed);
		if(isAllowed)
		{
			tempMSGActive = true;
			Invoke("SetOnHoldFalse", 0.8f);
		}
		else
			SetOnHoldTrue();
	}

	public bool DisplayingMessage()
	{
		return tempMSGActive;
	}

	private void SetOnHoldTrue()
	{
		onHold = true;
		tempMSGActive = false;
	}

	private void SetOnHoldFalse()
	{
		onHold = false;
		tempMSGActive = true;
	}

	public void QueueRewardMSGs(int currencyGained, int expGained)
	{
		TempMessageEntry cashMSG = new TempMessageEntry(currencyGained, NotificationMessageType.cashGainedMSG, 4f);
		tempMessageQueue.Add(cashMSG);

		if(expGained > 0)
			QueueExpMessage(expGained);
		else
			Debug.Log("Gained 0 EXP so no exp message is displayed.");
		  
	}

	public void QueueExpMessage(int expRecieved)
	{
		TempMessageEntry expMSG = new TempMessageEntry(expRecieved, NotificationMessageType.expGained, 4f);
		tempMessageQueue.Add(expMSG);
	}

	public void DisplayMLESuccessMSG(MLEDataSO _mleData)
	{
		passFunction = new UnityAction(ConfirmMLESuccess);
		confirmButton.onClick.AddListener(passFunction);
		messageHeadline.text = "Systemnachricht:";
		messageText.text = "Videolektion zum Thema " + _mleData.mleName + " erfolgreich beendet!";
		confirmButtonText.text = "Okay";

		AllowTempMessages(false);
		notificationPanel.SetActive(true);

		if(onNotificationSend != null)
			onNotificationSend.Invoke();
	}

	public void DisplayAreaUnlockMSG(bool _left)
	{
		if(_left)
			passFunction = new UnityAction(ConfirmLeftAreaUnlock);
		else
			passFunction = new UnityAction(ConfirmRightAreaUnlock);
		
		confirmButton.onClick.AddListener(passFunction);
		messageHeadline.text = "Systemnachricht:";
		messageText.text = "Ein neuer Bereich wurde freigeschaltet!";
		confirmButtonText.text = "Anzeigen";
		notificationPanel.SetActive(true);
		if(onNotificationSend != null)
			onNotificationSend.Invoke();
	}

	private void SetTempMessageContent(string message, string subMessage = "")
	{
		if(subMessage.Length > 2)
		{
			secondLineText.text = subMessage;
			secondLine.SetActive(true);
		} else {
			secondLine.SetActive(false);
		}

		tempNotificationText.text = message;
	}

	public void QueueTempMessage(string message, float displayTime, string subMessage = "")
	{
		Debug.Log("NEW NOTIFICATION MESSAGE QUEUED");
		TempMessageEntry addMessage = new TempMessageEntry(message, subMessage, displayTime);
		tempMessageQueue.Add(addMessage);
		//queueDuration += displayTime;
		//tempNotificationText.text = message;
		//Invoke("DisplayTempMessage", );
		//Invoke("FadeOutTempMessage", displayTime);
	}



	public void DisplayQuestProgressMessage(MainQuestSO _questData)
	{
        string questProgressMessage = "Der Quest " + _questData.title + " wurde aktualisiert";
        string substring = "Der neue Auftrag kann im Profil eingesehen werden."; 
        QueueTempMessage(questProgressMessage, 3.5f, substring);
	}

	public void DisplayQuestFinishedMessage(MainQuestSO _questData)
	{
		string finishedMessage = "Quest abgeschlossen: " + _questData.title;
        QueueTempMessage(finishedMessage, 2.5f);
    	if(_questData.followUpQuest != null)
        {
        	DisplayNewQuestMessage(_questData.followUpQuest);
        }
	}

	public void DisplayNewQuestMessage(MainQuestSO _questData)
	{
		string newQuestText = "Neuen Quest erhalten: " + _questData.title;
        QueueTempMessage(newQuestText, 2.5f);
	}

	private void FadeOutTempMessage()
	{
		startAlpha = tempMessageContainer.alpha;
		targetAlpha = 0f;
		timeSinceAnimStart = 0f;
	}

	public void ConfirmMLESuccess()
	{
		introManager.MLEsuccessMessageSend = true;
		introManager.onHold = false;
		this.onHold = false;
		confirmButton.onClick.RemoveListener(passFunction);
		notificationPanel.SetActive(false);
		AllowTempMessages(true);
	}

	public void ConfirmLeftAreaUnlock()
	{
		roomController.UnlockLeftArea();
		confirmButton.onClick.RemoveListener(passFunction);
		notificationPanel.SetActive(false);
		introManager.ConfirmNotificationClose();
		AllowTempMessages(true);
		if(onAreaUnlock != null)
			onAreaUnlock.Invoke();
	}

	public void ConfirmRightAreaUnlock()
	{
		roomController.UnlockRightArea();
		confirmButton.onClick.RemoveListener(passFunction);
		notificationPanel.SetActive(false);
		introManager.ConfirmNotificationClose();
		AllowTempMessages(true);
		if(onAreaUnlock != null)
			onAreaUnlock.Invoke();
	}

	private void Animate()
	{
		if(!onHold)
		{
			if(activeMessageElement == null)
			{
				TrySetupNextMSG();
			}

			timeSinceAnimStart += Time.deltaTime;

			if(activeMessageElement != null && activeMessageElement.type == NotificationMessageType.textMSG)
			{
					if(tempMessageContainer.alpha != targetAlpha)
					{
						tempMessageContainer.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeSinceAnimStart / animDuration);
					} else {
						//REACHED TARGET VALUE
						if(targetAlpha == 1f && timeSinceAnimStart > (animDuration + activeMessageElement.duration))
						{
							FadeOutTempMessage();
						} else if (targetAlpha == 0f){
							tempMessageContainer.gameObject.SetActive(false);
							activeMessageElement = null;
						}
					}
			} else if (activeMessageElement != null && activeMessageElement.type == NotificationMessageType.cashGainedMSG) {
				rewardMessager.UpdateDisplay(timeSinceAnimStart);
				if(timeSinceAnimStart > activeMessageElement.duration)
				{
					rewardMessager.StopDisplayCashMSG();
					activeMessageElement = null;
				}
			} else if (activeMessageElement != null && activeMessageElement.type == NotificationMessageType.expGained) {
				expMessager.UpdateDisplay(timeSinceAnimStart);
				if(timeSinceAnimStart > activeMessageElement.duration)
				{
					expMessager.StopDisplay();
					activeMessageElement = null;
				}
			}
		}
	}

	private void TrySetupNextMSG()
	{
		if(tempMessageQueue.Count > 0)
		{
			Debug.Log("DISPLAYING NOTIFICATION MESSAGE");
			activeMessageElement = tempMessageQueue[0];
			tempMessageQueue.RemoveAt(0);

			timeSinceAnimStart = 0f;

			if(activeMessageElement.type == NotificationMessageType.textMSG)
			{
				tempMessageContainer.gameObject.SetActive(true);
				tempMessageContainer.alpha = 0f;
				startAlpha = 0f;
				targetAlpha = 1f;
				SetTempMessageContent(activeMessageElement.mainMessageString, activeMessageElement.subMessageString);

				if(onNotificationSend != null)
					onNotificationSend.Invoke();
			}
			else if(activeMessageElement.type == NotificationMessageType.cashGainedMSG)
			{
				rewardMessager.StartDisplayCashMSG(activeMessageElement);

				if(onCashGained != null)
					onCashGained.Invoke();
			}
			else if(activeMessageElement.type == NotificationMessageType.expGained)
			{
				expMessager.StartDisplay(activeMessageElement);

				//if(onCashGained != null)
				//	onCashGained.Invoke();
			}
				
		} else {
			Debug.Log("All Messages send. Queue empty");
			onHold = true;
			tempMSGActive = false;
		}
	}
}

public class TempMessageEntry
{
	public string mainMessageString = "Message";
	public string subMessageString = "";
	public float duration = 2f;
	public NotificationMessageType type = NotificationMessageType.textMSG;
	public int rewardAmount = 0;

	public TempMessageEntry (string main, string sub, float time)
	{
		mainMessageString = main;
		subMessageString =sub;
		duration = time;
		type = NotificationMessageType.textMSG;
	}

	public TempMessageEntry(int reward, NotificationMessageType msgType, float time = 2f)
	{
		rewardAmount = reward;
		type = msgType;
		duration = time;
	}
}

public enum NotificationMessageType
{
	textMSG, cashGainedMSG, expGained
}