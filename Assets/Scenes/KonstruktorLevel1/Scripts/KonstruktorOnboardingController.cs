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
using UnityEngine.SceneManagement;

public class KonstruktorOnboardingController : MonoBehaviour
{
	private KonstruktorOnboardingData saveData;

  public List<GameObject> blendOutBeforeIntroTutorial; 
	public List<GameObject> blendOutBeforeGivenSearchedTutorial;
	public List<GameObject> blendOutBeforeConverterTutorial;

  public GameObject onboardingPanel;
  public KonstruktorTutorial taskIntroTutorial;
  public KonstruktorTutorial givenSearchedTutorial;
  public KonstruktorTutorial converterTutorial;

  [HideInInspector]
  public KonstruktorTutorial activeTutorial;
    
  void Start()
  {
    Scene currentScene = SceneManager.GetActiveScene();

    GetKonstruktorOnboardingData();
    BlendInOutUIElements();
    if(currentScene.name == "KonstruktorNewUI")
    {
      if(saveData.konstruktorIntroductionFinished == false)
      {
        StartTaskIntroTutorial();
      } else if (saveData.givenSearchedOnboardingFinished == false)
      {
        StartGivenSearchedTutorial();
      }
    }

    if(saveData.modulesOnboardingFinished == false && currentScene.name == "TaskSolvingGraphicsUpdate")
    	StartConverterTutorial();

    if(taskIntroTutorial != null)
    	taskIntroTutorial.onboardingController = this;
    if(givenSearchedTutorial != null)
    	givenSearchedTutorial.onboardingController = this;
    if(converterTutorial != null)
    	converterTutorial.onboardingController = this;
  }

  public void GetKonstruktorOnboardingData()
  {
    saveData = GameController.GetInstance().gameState.onboardingData.konstruktorData;
  }

  public void SaveKonstruktorOnboardingData()
  {
    GameController.GetInstance().gameState.onboardingData.konstruktorData = saveData;
  }

  public void BlendInOutUIElements()
  {
    foreach(GameObject blendOutObj in blendOutBeforeIntroTutorial)
    {
    	if(saveData.konstruktorIntroductionFinished == false)
    		blendOutObj.SetActive(false);
    	else
    		blendOutObj.SetActive(true);
    }

    foreach(GameObject blendOutObj in blendOutBeforeGivenSearchedTutorial)
    {
    	if(saveData.givenSearchedOnboardingFinished == false)
    		blendOutObj.SetActive(false);
    	else
    		blendOutObj.SetActive(true);
    }

    foreach(GameObject blendOutObj in blendOutBeforeConverterTutorial)
    {
    	if(saveData.modulesOnboardingFinished == false)
    		blendOutObj.SetActive(false);
    	else
    		blendOutObj.SetActive(true);
    }
  }

  public void StartTaskIntroTutorial()
  {
    taskIntroTutorial.StartEvent();
    activeTutorial = taskIntroTutorial;
  }

  public void StartGivenSearchedTutorial()
  {
    givenSearchedTutorial.StartEvent();
    activeTutorial = givenSearchedTutorial;
  }

  public void StartConverterTutorial()
  {
    converterTutorial.StartEvent();
    activeTutorial = converterTutorial;
  }

  public void ContinueActiveEvent()
  {
    activeTutorial.ContinueEvent();
  }

  public void ActiveTutorialFinished()
  {
    Debug.Log("Tutorial Finished: " + activeTutorial);
    if(activeTutorial == taskIntroTutorial)
    {
    	saveData.konstruktorIntroductionFinished = true;
    		
    }
    else if(activeTutorial == givenSearchedTutorial)
    {
    	saveData.givenSearchedOnboardingFinished = true;
    }
    else if(activeTutorial == converterTutorial)
    {
      saveData.givenSearchedOnboardingFinished = true;
    	saveData.modulesOnboardingFinished = true;
    }

    SaveTutorialState();

    BlendInOutUIElements();

    if(activeTutorial == taskIntroTutorial){
    	StartGivenSearchedTutorial();
    }
  }

  public void SaveTutorialState()
  {
    GameController controller = GameController.GetInstance();
    controller.gameState.onboardingData.konstruktorData = saveData;
    controller.SaveGame();
  }
}
