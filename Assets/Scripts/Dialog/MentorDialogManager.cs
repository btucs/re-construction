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
using TMPro;


public class MentorDialogManager : MonoBehaviour
{
	public YarnProgram mentorDialog;
	public string startNode;
	public DialogueRunner yarnRunner;
  public ButtonSelectionUIFeedback buttonUI;
   	public TextMeshProUGUI nameText;
   	public Image mentorImg;
   	public GameObject dialogPanel;
   	private CharacterSO mentorData;
   	private bool dialogueIsActive = false;
   	private bool dialogueStarted = false;

   	private void Start()
   	{
   		mentorData = GameController.GetInstance().gameState.characterData.mentor;
    	nameText.text = mentorData.characterName;
   		mentorImg.sprite = mentorData.thumbnail;

   		if(mentorDialog != null && yarnRunner != null)
    	{
    		yarnRunner.Add(mentorDialog);
    	}
   	}

    public void StartAtNode(string nodeName)
    {
      SetDialogueInactive();
      yarnRunner.StartDialogue(nodeName);
      dialogueStarted = true;
      dialogueIsActive = true;
      if(buttonUI != null)
        buttonUI.SetSelected(true);
    }

   	public void StartDialogue()
    {
    	yarnRunner.StartDialogue(startNode);
    	dialogueStarted = true;
    	dialogueIsActive = true;
      if(buttonUI != null)
        buttonUI.SetSelected(true);

    }

    public void SetDialogueInactive()
    {
      dialogueIsActive = false;
      dialogueStarted = false;
      yarnRunner.Stop();
      if(buttonUI != null)
        buttonUI.SetSelected(false);
    }

    public void ToggleDialogue()
    {
    	if(!dialogueIsActive)
    	{
    		if(!dialogueStarted)
    		{
	    		StartDialogue();
    		} else {
	    		dialogPanel.SetActive(true);
	    		dialogueIsActive = true;
	    	}
    	} else {
        yarnRunner.Stop();
    		dialogPanel.SetActive(false);
    		dialogueIsActive = false;
        dialogueStarted = false;
    	}
    }
}
