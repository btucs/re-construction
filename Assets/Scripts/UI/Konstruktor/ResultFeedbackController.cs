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

public class ResultFeedbackController : MonoBehaviour
{
	public Text textHolder;
	public Button sendResultButton;

    private int nrOfResults;
    private int nrOfTaskSteps;
    public void UpdateResultText()
    {
    	GetResultData();
    	
    	if(nrOfResults < nrOfTaskSteps)
    	{
    		textHolder.text = "Eine Zielvariable wurde erstellt. Wiederhole jetzt den Vorgang für den anderen gesuchten Wert.";
    	} else {
    		textHolder.text = "Eine Zielvariable wurde erstellt. Jetzt kannst du die Aufgabe abgeben.";
    	}
    }

    private void GetResultData()
    {
    	GameController controller = GameController.GetInstance();

    	KonstruktorSceneData konstruktorSceneData = controller.gameState.konstruktorSceneData;
    	nrOfResults = konstruktorSceneData.converterResults.Length;
    	nrOfTaskSteps = konstruktorSceneData.taskData.steps.Length;
    }
}
