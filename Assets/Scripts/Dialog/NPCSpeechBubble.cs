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

public class NPCSpeechBubble : MonoBehaviour
{
    public Vector2 positionOffset = new Vector2(0f, 3.86f); 
	
    private NPCDialogController dialogueData;
	private Transform target;
	private CustomAnimationController animController;
	private bool isExpanded = false;
    private Button buttonRef;
    private Vector3 targetPrevPos;
    private characterMovement charMovement;
    private bool interactable = false;
    private CanvasGroup alphagroup;

    private void LateUpdate()
    {
        if(target.position != targetPrevPos)
        {
            if(charMovement != null)
            {
                float directedXOffset = charMovement.facingRight==true ? positionOffset.x : positionOffset.x*-1;
                this.transform.position = new Vector3(target.position.x + directedXOffset, target.position.y + positionOffset.y, this.transform.position.z);
            } else 
            {
                this.transform.position = new Vector3(target.position.x, target.position.y + positionOffset.y, this.transform.position.z);
            }
            targetPrevPos = target.position;
        }
    }

	public void Setup(NPCDialogController _dialogueData, Vector2 offset)
	{
        dialogueData = _dialogueData;
        positionOffset = offset;

        buttonRef = GetComponent<Button>();
        target = dialogueData.transform;
        targetPrevPos = target.position;
        this.transform.position = new Vector3(target.position.x + positionOffset.x, target.position.y + positionOffset.y, this.transform.position.z);
        charMovement = dialogueData.GetComponent<characterMovement>();
        alphagroup = transform.GetComponent<CanvasGroup>();
        alphagroup.alpha = 0.5f;
	}

    public void StartDialogue()
    {
        if(interactable)
        {
        	Debug.Log("Starting Dialogue");
        	dialogueData.StartDialogue();            
        } else {
            playerScript playerController = playerScript.Instance;
            if(playerController != null && playerController.GetPlayerMovementScript() != null)
            {
                Vector3 markerWorldPos = this.transform.position;
                playerController.GetPlayerMovementScript().SetXGoalPosition(markerWorldPos);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D otherCol)
    {
        if(otherCol.gameObject.CompareTag("Player") && !isExpanded)
        {
            //animController.AddActiveAnimationState("activate");
            interactable = true;
            isExpanded = true;
            alphagroup.alpha = 1f;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCol)
    {
        if(otherCol.gameObject.CompareTag("Player") && isExpanded && this.gameObject.activeSelf)
        {
            //animController.AddActiveAnimationState("deactivate");
            interactable = false;
            isExpanded = false;
            alphagroup.alpha = 0.5f;
        }
    }
}
