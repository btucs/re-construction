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


public class IntPreferenceToggle : MonoBehaviour
{
    public GameSettingsController settings;
    public int qualityIndex = 0;
    private Toggle thisToggleRef;

    private void OnEnable()
    {
    	Debug.Log("Toggle values has been set to match saved value");
    	thisToggleRef = GetComponent<Toggle>();
    	thisToggleRef.isOn = (GameController.GetInstance().gameState.settings.graphicsQualityIndex == this.qualityIndex);
    	thisToggleRef.group.NotifyToggleOn(thisToggleRef);
    }

    public void OnToggle(bool isOn)
    {
    	if(isOn)
    	{
    		settings.SetGraphicsQuality(qualityIndex);
    	}
    }
}
