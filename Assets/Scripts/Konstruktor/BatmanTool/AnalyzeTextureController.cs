#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyzeTextureController : MonoBehaviour
{
    public Transform effector;
    public Material analyzeMaterial;
    public Material hiddenObjMaterial;
    public Material hiddenTextMaterial;

    public bool update = true;
    private bool displace = false;
    private Camera mainCam;

    private void Start()
    {
        SetAreaToOffset();
    }

    void Update()
    {
    	if(update)
    	{
    		if(!displace)
    		{
    			analyzeMaterial.SetVector("_EffectorWorldPos", effector.position);
    			hiddenObjMaterial.SetVector("_EffectorWorldPos", effector.position);
                hiddenTextMaterial.SetVector("_EffectorWorldPos", effector.position);
    		} else {
    			SetAreaToOffset();  			
    		}
    	}
    }

    public void ToggleDisplaceAnalyzeArea(bool shouldDisplaced)
    {
    	mainCam = Camera.main;
    	displace = shouldDisplaced;
    	SetAreaToOffset();
    }

    private void SetAreaToOffset()
    {
    	Vector3 offsetVector = mainCam.transform.position;
    			offsetVector.x += 20f;
		analyzeMaterial.SetVector("_EffectorWorldPos", offsetVector);
		hiddenObjMaterial.SetVector("_EffectorWorldPos", offsetVector);
        hiddenTextMaterial.SetVector("_EffectorWorldPos", offsetVector);
    }
}
