#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NPCDialogController : MonoBehaviour
{
	public CharacterSO characterData;
    public string startNode;
    public YarnProgram scriptToLoad;
    public DialogController dialogueController;
    public Transform dialogPositionTransform;
    private MenuUIController uiController;
    private DialogueRunner yarnRunner;
    private NPCSpeechBubble connectedSpeechBubble;
    private DialogSoundResolver soundResolver;
    private WorldSceneCameraController camController;

    private void Start()
    {
        uiController = MenuUIController.Instance;
    	yarnRunner = uiController.dialogueManager.yarnRunner;
        soundResolver = uiController.dialogueManager.soundResolver;
        camController = Camera.main.gameObject.GetComponent<WorldSceneCameraController>();
    	if(scriptToLoad != null && yarnRunner != null)
    	{
            Debug.Log("Yarn Programm added");
    		yarnRunner.Add(scriptToLoad);

            connectedSpeechBubble = uiController.worldSpaceUI.CreateNPCSpeechBubble(this);
    	}
    }

    public void StartDialogue()
    {
        bool hasDialogue = string.IsNullOrEmpty (startNode) == false;
        if(soundResolver != null)
            soundResolver.SetVoiceType(characterData.voice);

        if (hasDialogue && yarnRunner != null)
        {
            uiController.EnterDialogue();
            dialogueController.SetNameText(characterData);
            dialogueController.SetTargetTransform(this.transform);
            FocusCamera();
            // Kick off the dialogue at this node.
            yarnRunner.StartDialogue(startNode);
        } else {
            Debug.Log("Could not start dialogue");
        }
    }

    private void FocusCamera()
    {
        Vector3 camGoal = dialogueController.GetDialogWorldPos();
        camGoal.y -= 1f;
        if(camController != null)
            camController.SetGoalPosition(camGoal);
    }
}
