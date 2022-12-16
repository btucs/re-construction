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

public class KonstruktorDialogController : MonoBehaviour
{
	public Transform target;
	public Vector2 offSet;
	//public Text NPCNameText;
	public Text dialogContentText;
	public GameObject continueButton;
	private GameObject activeSpeechBubble;
	public Canvas dialogCanvas;

	private RectTransform dialogPanel;
	private Vector3 prevTargetPos, prevCamPos;
	private Camera activeCamera;
	private int activeLine;
	private List<string> textLines = new List<string>();

	void Start()
	{
		if(target == null)
			{target = this.transform;}
    	activeCamera = Camera.main;
    	dialogPanel = GetComponent<RectTransform>();
    	UpdateUIPosition();
	}

	void LateUpdate()
	{
    	if(prevTargetPos != target.position || activeCamera.transform.position != prevCamPos)
    	{
    		UpdateUIPosition();
    	}
	}

	private void UpdateUIPosition()
	{
		if(target != null)
		{
			Vector3 targetPosition = target.position;
			targetPosition.y += 3.8f*GameController.GetInstance().gameState.konstruktorSceneData.npcScale; //
    		Vector3 ScreenPos = activeCamera.WorldToScreenPoint(targetPosition);
    		ScreenPos.x = ScreenPos.x / dialogCanvas.scaleFactor;
    		ScreenPos.y = ScreenPos.y / dialogCanvas.scaleFactor;
    		dialogPanel.anchoredPosition = new Vector2(ScreenPos.x + offSet.x, ScreenPos.y + offSet.y);
    		prevTargetPos = target.position;
    		prevCamPos = activeCamera.transform.position;
    	}
	}
    
	public void SetDialogContent(List<string> dialogText, GameObject speechBubbleObj, Transform targetTransform)
	{
		target = targetTransform;
		textLines = dialogText;
		activeSpeechBubble = speechBubbleObj;
	}

	public void StartDialog()
	{
		activeLine = 0;
		UpdateDialogContent(activeLine);
		activeSpeechBubble.SetActive(false);
		if(dialogPanel == null)
			dialogPanel = this.GetComponent<RectTransform>();
		dialogPanel.gameObject.SetActive(true);
	}

	public void ContinueDialog()
	{
		activeLine++;
		UpdateDialogContent(activeLine);
	}

	public void UpdateDialogContent(int lineNr)
	{
    	if(lineNr >= textLines.Count)
    	{
    		if(dialogPanel == null)
				dialogPanel = this.GetComponent<RectTransform>();
    		dialogPanel.gameObject.SetActive(false);
    		activeSpeechBubble.SetActive(true);
    		return;
    	}
    	dialogContentText.text = textLines[lineNr];

		//GameController saveController = GameController.GetInstance();
		//NPCNameText.text = saveController.gameState.characterData.mentor.characterName;
	}
}
