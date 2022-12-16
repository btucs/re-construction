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

[System.Serializable]
[AddComponentMenu("Cutscene/Dialog")]
public class DialogEvent : EventAction
{
	public CharacterSO speaker;
    public Transform speakerTransform;
	public bool speakerIsMentor = false;
	public YarnProgram textData;
	public string startNode = "Start";
    private DialogController dialogManager;
	private DialogueRunner dialogueRunner;

	public override void Invoke(ScriptedEventManager eventManager)
    {
        manager = eventManager;

    	dialogManager = MenuUIController.Instance.dialogueManager;
        dialogueRunner = dialogManager.yarnRunner;

    	if(speakerIsMentor)
    	{
    		speaker = GameController.GetInstance().gameState.characterData.mentor;
    	}
    	if(textData != null && dialogueRunner != null)
    	{
            if(dialogueRunner.Dialogue.NodeExists(startNode) == false)
            {
                dialogueRunner.Add(textData);
                //dialogueRunner.Dialogue.SetProgram(textData); would be better but gives strange compiler error
            }
    	}
    	bool hasDialogue = string.IsNullOrEmpty (startNode) == false;

        TryFaceCharacters();

        dialogManager.SetNameText(speaker);
        dialogManager.SetTargetTransform(speakerTransform);
        
        DialogSoundResolver soundResolver = dialogManager.soundResolver;
        if(soundResolver != null)
            soundResolver.SetVoiceType(speaker.voice);

        WorldSceneCameraController camController = Camera.main.gameObject.GetComponent<WorldSceneCameraController>();
        if(camController)
        {
            Vector3 camTarget = dialogManager.GetDialogWorldPos();
            camTarget.y -= 1f;
            camController.SetGoalPosition(camTarget);
        }
        dialogueRunner.StartDialogue(startNode);

        SetupContinueCondition();
    }

    private void TryFaceCharacters()
    {
        Transform playerTransform = MenuUIController.Instance.playerController.GetComponent<Transform>();
        if(speakerTransform != null && playerTransform != null)
        {
            characterMovement playerMoveController = playerTransform.GetComponent<characterMovement>();
            characterMovement speakerMoveController = speakerTransform.GetComponent<characterMovement>();

            if(playerMoveController)
                playerMoveController.FaceTowards(speakerTransform);

            if(speakerMoveController)
                speakerMoveController.FaceTowards(playerTransform);
        }
    }

}