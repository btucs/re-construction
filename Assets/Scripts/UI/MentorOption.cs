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
using Sirenix.OdinInspector;

public class MentorOption : MonoBehaviour
{
    public string mentorName;
    public string mentorDescription;

    public Color activeColor;
    public Color defaultColor;

    public Image mentorBGIMG;
    public MentorSelector controller;

    public bool isDefaultOption = false;

    private CharacterPresetController dataWriter;

    private void Start()
    {
    	controller.Subscribe(this);
        dataWriter = this.gameObject.GetComponent<CharacterPresetController>();
    	if(isDefaultOption)
    	{
            OnSelect();
            dataWriter.SetTargetCharacter();
        }	

    	else {
    		SetSelected(false);
    	}
    }

    public void SetSelected(bool toBeActive)
    {
    	mentorBGIMG.color = toBeActive ? activeColor : defaultColor;
    	this.transform.localScale = toBeActive ? new Vector3(1.1f, 1.1f, 1f) : new Vector3(0.8f, 0.8f, 1f);
    }

    public void OnSelect()
    {
    	controller.SetSelectedMentor(this);
    }
}
