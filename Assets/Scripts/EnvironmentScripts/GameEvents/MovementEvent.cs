#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[AddComponentMenu("Cutscene/CharacterMovement")]
public class MovementEvent : EventAction
{
	public characterMovement npc;
	public Vector3 goalPos;
    public Transform goalTransform;
    public bool teleport = false;
    private UnityAction onTargetReached;

    public override void Invoke(ScriptedEventManager eventManager)
    {
    	manager = eventManager;
    	Vector3 npcPrevPos = npc.transform.position;
        Vector3 npcGoal = npcPrevPos;
        if(goalTransform != null)
        {
            float xGoalVal = goalTransform.position.x;
            if(xGoalVal > npcPrevPos.x)
            {
                xGoalVal -= 1.3f;   
            } else {
                xGoalVal += 1.3f;
            }
            xGoalVal += goalPos.x;
            npcGoal = new Vector3(xGoalVal, npcPrevPos.y, npcPrevPos.z);
        } else {
            npcGoal = new Vector3(goalPos.x, npcPrevPos.y, npcPrevPos.z);
        }

        npc.SetGoalPosition(npcGoal);
        
        if(teleport)
        {
        	npc.transform.position = npcGoal;
        	SetActionFinished();
        } else {
        	onTargetReached += SetActionFinished;
        	npc.onReachGoalPos.AddListener(onTargetReached);
        }

    }

    public override void SetActionFinished()
    {
        Debug.Log("Movement finished");
    	base.SetActionFinished();
        if(!teleport)
    	   npc.onReachGoalPos.RemoveListener(onTargetReached);
    }
}