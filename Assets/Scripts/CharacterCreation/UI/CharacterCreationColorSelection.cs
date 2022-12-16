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

public class CharacterCreationColorSelection : MonoBehaviour
{
	private Color hairTint;
	private Color bodyTint;
	public GameObject colorContainer;
	public GameObject graphicContainer;

    // Start is called before the first frame update
    

    public void EnableColorMenu()
    {
    	
    	colorContainer.SetActive(true);
    	graphicContainer.SetActive(false);
    	
    	
    }
 
}
