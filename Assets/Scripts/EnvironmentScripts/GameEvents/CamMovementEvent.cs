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
[AddComponentMenu("Cutscene/CameraMovement")]
public class CamMovementEvent : EventAction
{
	public Vector3 camTargetPos;
	public Transform targetTransform;

	public override void Invoke(ScriptedEventManager eventManager)
    {
    	manager = eventManager;
      Camera camera = Camera.main;
      WorldSceneCameraController camController = camera.GetComponent<WorldSceneCameraController>();
    	float camZ = camera.transform.position.z;
    	Vector3 targetPos = new Vector3(0f, 0f, camZ); 
    	if(targetTransform != null)
    	{
    		targetPos = targetTransform.position;
    		targetPos.z = camZ;
    	}

    	targetPos.x += camTargetPos.x;
    	targetPos.y += camTargetPos.y;

    	camController.ForceMoveTowards(targetPos);

    	SetupContinueCondition();
    }
}
