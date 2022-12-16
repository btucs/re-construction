#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float IdleAnimOffset = 10.0f;
    private Animator npcAnimator;
    // Update is called once per frame
    void Start()
    {
    	npcAnimator = GetComponent<Animator>();
	    InvokeRepeating("RandomIdleAnimation", IdleAnimOffset, IdleAnimOffset);

	    //CancelInvoke(); zum abbrechen
    }

    void RandomIdleAnimation()
    {
    	float whichAnimation = Random.Range(0.0f, 1.0f);
    	if(whichAnimation > 0.5)
    	{
    		npcAnimator.SetTrigger("IdleScratchTrigger");
    	} 
    	else if (whichAnimation < 0.5)
    	{
    		npcAnimator.SetTrigger("Idle2Trigger");
    	} 

    }
}
