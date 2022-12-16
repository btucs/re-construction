

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
using TMPro;

public class StartVideoAnimationTrigger : MonoBehaviour
{
    public bool overrideMLEText = false;
    public string introTextHeadline = "MLE Name";

    public MLEController mleVideoScript;
    public TMP_Text mleNameTextMesh;
    public UnityEvent mleIntroStartEvent;
    public UnityEvent mleIntroEndEvent;


    public void StartMLEVideo()
    {
        OnAnimationEnd();
    	mleVideoScript.StartVideo();
    	this.gameObject.SetActive(false);
    }

    public void SetNameText()
    {
        OnAnimationStart();
    	mleNameTextMesh.text = mleVideoScript.MLEContent.mleName;
    	//Debug.Log(mleVideoScript.MLEContent.mleName + " in " + mleNameTextMesh.gameObject.name);
        if(overrideMLEText)
            mleNameTextMesh.text = introTextHeadline;
    }

    public void OnAnimationStart()
    {
        if(mleIntroStartEvent != null)
            mleIntroStartEvent.Invoke();
    }

    public void OnAnimationEnd()
    {
        if(mleIntroEndEvent != null)
            mleIntroEndEvent.Invoke();
    }
}
