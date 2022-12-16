#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCColorButton : MonoBehaviour
{
	public bool body = true; 
	public Color buttonColor;
	public characterGraphicsUpdater graphicsController;


    void Start()
    {
        
    }

    public void SetColor()
    {
    	if(body)
    	{
    		graphicsController.SetBodyColor(buttonColor);
    	} 
    	else
    	{
    		graphicsController.SetHairColor(buttonColor);	
    	}
    }

}
