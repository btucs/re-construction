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

public class mlePreviewButton : MonoBehaviour
{
    public Text nameHolder;
    public Text descriptionHolder;
    public Text achievedPointsHolder;
    public MLEDataSO relatedData;
    public MLESceneLoader mleLoadingScript;

    public void SetContent(List<FinishedMLEData> solvedMLEs)
    {
    	nameHolder.text = relatedData.mleName;
        if(descriptionHolder != null)
    	   descriptionHolder.text = relatedData.mleIntroText;

    	//check if the player has done this MLE, how much 
    	//points he/she got and how much he could get
    	int recievedPoints = 0;
    	foreach(FinishedMLEData oneMLE in solvedMLEs)
    	{
    		if(oneMLE.solvedMLE.mleName == relatedData.mleName)
    		{
    			recievedPoints = oneMLE.achievedPoints;
    		}
    	}
    	int maxPoints = 0;
    	for(int i = 0; i < relatedData.videoData.Length; i++)
    	{
    		maxPoints += 2 * relatedData.videoData[i].questions.Length;	
    	}
    	string achievedPointsString = recievedPoints + " / " + maxPoints + " Punkte erhalten.";

        achievedPointsHolder.text = achievedPointsString;
    }

    public void StartMLE()
    {
    	mleLoadingScript.currentMLEData = relatedData;
    	mleLoadingScript.LoadMLESceneFromList();
    }

}
