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

public class ObjectTaskEntry : MonoBehaviour
{
	public TaskDataSO taskData;
	public TaskListManager taskManager;
    public GameWorldObject worldObject;
    public ParticleSystem unlockEffect;
    private TaskState state = TaskState.unsolved;
    private bool isExpanded = false;
    private CustomAnimationController animController;

    public void ActivateTaskSelection()
    {
        taskManager.SetKonstruktorObject(worldObject);
        taskManager.UpdateTaskData(taskData, state);
        taskManager.EnableTaskPreview();
    }

    public void Setup(TaskListManager _manager, TaskDataSO _task, GameWorldObject _taskObj, TaskState displayState, bool newUnlock = false)
    {
        animController = GetComponent<CustomAnimationController>();
        
        taskManager = _manager;
        taskData = _task;
        worldObject = _taskObj;
        state = displayState;

        RefreshAppearance();

        if(newUnlock && unlockEffect != null)
        {
            unlockEffect.Play();
        }
    }

    public void RefreshAppearance()
    {
        switch(state)
        {
            case TaskState.unsolved :
            animController.AddActiveAnimationState("showHighlight");
            break;

            case TaskState.solvedCorrect :
            animController.AddActiveAnimationState("showCheckMark");
            break;

            case TaskState.solvedWrong :
            animController.AddActiveAnimationState("showCross");
            break;
        }
    }

    public void MovePlayerToObject()
    {
        playerScript playerController = playerScript.Instance;
        if(playerController != null && playerController.GetPlayerMovementScript() != null)
        {
            Vector3 markerWorldPos = this.transform.position;
            playerController.GetPlayerMovementScript().SetXGoalPosition(markerWorldPos);
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCol)
    {
        if(otherCol.gameObject.CompareTag("Player") && !isExpanded)
        {
            animController.AddActiveAnimationState("activate");
            isExpanded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCol)
    {
        if(otherCol.gameObject.CompareTag("Player") && isExpanded && this.gameObject.activeSelf)
        {
            animController.AddActiveAnimationState("deactivate");
            isExpanded = false;
        }
    }
}

