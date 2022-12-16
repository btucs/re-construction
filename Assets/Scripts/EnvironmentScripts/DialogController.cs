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
using Yarn.Unity;

public class DialogController : MonoBehaviour
{
	public Transform target;
	public Vector2 offSet;
	public Text NPCNameText;
	public Text dialogContentText;
  public GameObject continueButton;
  public Canvas dialogCanvas;
  public DialogueRunner yarnRunner;
  public DialogSoundResolver soundResolver;

  [HideInInspector]
	public ScriptedEventManager DialogEventManager;

	private RectTransform dialogPanel;
	private Vector3 prevTargetPos, prevCamPos;
  private Camera activeCamera;

  void Start()
  {
    UpdateUIPosition();
  }

  void LateUpdate()
  {
      if(prevTargetPos != target.position || activeCamera.transform.position != prevCamPos)
      {
        UpdateUIPosition();
      }
  }


  void UpdateUIPosition()
  {
    Initialize();
    if(target != null)
    {
    	Vector3 ScreenPos = activeCamera.WorldToScreenPoint(target.position);
    	ScreenPos.x = ScreenPos.x / dialogCanvas.scaleFactor;
    	ScreenPos.y = ScreenPos.y / dialogCanvas.scaleFactor;
      dialogPanel.anchoredPosition = new Vector2(ScreenPos.x + offSet.x, ScreenPos.y + offSet.y);
      prevTargetPos = target.position;
      prevCamPos = activeCamera.transform.position;
    }
  }
  
  private void Initialize()
  {
    if(activeCamera == null || dialogPanel == null)
    {
      activeCamera = Camera.main;
      dialogPanel = GetComponent<RectTransform>();      
    }
  }

  public void SetTargetTransform(Transform newTarget)
  { 
    target = newTarget;
  }

  public Vector3 GetDialogWorldPos()
  {
    UpdateUIPosition();
    Vector3[] worldCorners = new Vector3[4];
    dialogPanel.GetWorldCorners(worldCorners);
    float xPos = (worldCorners[0].x + worldCorners[3].x)/2;
    float yPos = (worldCorners[0].y + worldCorners[1].y)/2;
    return new Vector3(xPos, yPos, 0);;
  }

  public void SetNameText(CharacterSO speaker)
  {
    NPCNameText.text = speaker.characterName;
  }

/*  public void UpdateDialogContent(DialogEvent myDialog)
  {
    if(myDialog.showContinueButton == false)
    {
      continueButton.SetActive(false);
    } else
    {
      continueButton.SetActive(true);
    }
    dialogContentText.text = myDialog.text;

    //To Do: Possibility to chose MentorData when selecting a NPC Data for Dialogs 
    GameController saveController = GameController.GetInstance();
    //NPCNameText.text = myDialog.speaker.characterName;
    NPCNameText.text = saveController.gameState.characterData.mentor.characterName;
  } */

  public void ContinueEvent()
  {
    //DialogEventManager.eventStepFinished = true;
  }
}
