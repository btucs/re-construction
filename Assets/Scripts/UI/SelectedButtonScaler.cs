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

public class SelectedButtonScaler : MonoBehaviour
{
	public List<Button> buttonGroup = new List<Button>();
	public float scaleX = 1.2f;
	public float scaleY = 1.2f;
    
    public void SetThisButtonActive()
    {
        foreach(Button oneButton in buttonGroup)
        {
        	if(oneButton != this.GetComponent<Button>())
        	{
        		oneButton.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
        	} 
        	else 
        	{
        		oneButton.GetComponent<RectTransform>().localScale = new Vector3 (scaleX, scaleY, 1f);
        	}
        }
    }
}
