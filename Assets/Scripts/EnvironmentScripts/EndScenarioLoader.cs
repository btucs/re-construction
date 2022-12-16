#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScenarioLoader : MonoBehaviour
{
    public ProgressRequirement gameEndRequirement;
    public ProgressRequirement alternativeRequirement;
    private SceneManagement sceneLoader;

    private void Start()
    {
    	CheckForGameEnd();
    }

    public void CheckForGameEnd()
    {
        sceneLoader = this.gameObject.GetComponent<SceneManagement>();
    	GameController controller = GameController.GetInstance();

        Debug.Log("End cinemativ played " + controller.gameState.onboardingData.endCinematicPlayed);

    	if(controller.gameState.onboardingData.endCinematicPlayed == false &&
         (gameEndRequirement.ConditionMet(controller) || alternativeRequirement.ConditionMet(controller)))
    	{
            controller.gameState.onboardingData.endCinematicPlayed = true;
            controller.SaveGame();
    		float averagePoints = controller.gameState.taskHistoryData.GetPlayerPerformance();
    		
    		if(averagePoints > 2.5f)
    		{
    			sceneLoader.LoadSceneWithFade("Ending - Positive");
    		} else {
    			sceneLoader.LoadSceneWithFade("Ending - Negative");
    		}

    	}
    }
}
