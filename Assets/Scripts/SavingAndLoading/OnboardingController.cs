#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingController : MonoBehaviour
{
	public MainMenuOnboardingController onboardingMenuManager;

  public SceneIntroduction sceneIntroController;

	public ScriptedEventDataSO unlockProfileEvent;
	public ScriptedEventDataSO unlockMapEvent;
  public ScriptedEventDataSO unlockBibEvent;
  public List<ScriptedEventManager> events = new List<ScriptedEventManager>();

  private int numberOfMLEsDone = 0;
  private OnboardingData currentOnboardingData;
  private GameController controller;
  private WorldSpaceUIController worldSpaceUI;
  private ScriptedEventManager activeEvent;
  private WorldSceneCameraController camScript;

  private static OnboardingController instance;
  public static OnboardingController Instance
  {
    get { return instance; }
  }

  private void Awake ()
  {
    if (instance == null) { instance = this; }
    else if (instance != this) { Destroy(this.gameObject); }
  }

  private void Start()
  {
    camScript = Camera.main.GetComponent<WorldSceneCameraController>();
  }

  public bool IsEventActive()
  {
    return (activeEvent!=null);
  }

  public void InitializeMenuUI()
  {
    worldSpaceUI = MenuUIController.Instance.worldSpaceUI;
    GetOnboardingData();
    onboardingMenuManager.UpdateSideMenuUI(currentOnboardingData);
  }

  public void SetActiveEvent(ScriptedEventManager newActive)
  {
    activeEvent = newActive;
  }

  public void ContinueActiveEvent()
  {
    if(activeEvent != null)
    {
      activeEvent.Continue();
    }
  }

  public bool CheckForEvents()
  {
    GetOnboardingData();

    foreach(ScriptedEventManager currentEvent in events)
    {
      if(currentEvent.type == GameEventType.onSceneStart)
      {
        if(CheckForEvent(currentEvent))
          return true;
      }
    }

    return false;
  }

  public void EnterEventMode()
  {
    worldSpaceUI.SetWorldSpaceUIActive(false);
    MenuUIController.Instance.mentorDialogButton.SetActive(false);
  }

  public bool CheckForEvent(ScriptedEventManager checkEvent)
  {
    if(controller == null)
      GetOnboardingData();

    if(currentOnboardingData.EventEntryExists(checkEvent.eventData.UID) == false && checkEvent.RequirementsMet(controller))
    {
      checkEvent.StartEvent();
      return true;
    }
    return false;
  }

  public void GetOnboardingData()
  {
    controller = GameController.GetInstance();
    currentOnboardingData = controller.gameState.onboardingData;
    numberOfMLEsDone = controller.gameState.taskHistoryData.taskHistoryData.Count;
  }

  public void SaveOnboardingData()
  {
    //GameController controller = GameController.GetInstance();
    controller.gameState.onboardingData = currentOnboardingData;
    controller.SaveGame();	
  }

  public void OnEventFinished()
  {
    activeEvent = null;
    sceneIntroController.onHold = false;
    SaveOnboardingData();
    sceneIntroController.CheckForAreaUnlock();

    worldSpaceUI.SetWorldSpaceUIActive(true);
    MenuUIController.Instance.mentorDialogButton.SetActive(true);
    
    if(MenuUIController.Instance.IsClosed())
      camScript.SetToWorldMode();

    GameController.GetInstance().SaveGame();
  }

  public void OnEventFinished(ScriptedEventManager eventManager)
  {
    activeEvent = null;
    sceneIntroController.onHold = false;
    currentOnboardingData.finishedEvents.Add(eventManager.eventData.UID);
    onboardingMenuManager.UpdateSideMenuUI(currentOnboardingData);
    SaveOnboardingData();

    if(CheckForEvents()==false) //starts new followup event, if one is found
    {
      worldSpaceUI.SetWorldSpaceUIActive(true);
      MenuUIController.Instance.mentorDialogButton.SetActive(true);
      
      sceneIntroController.CheckForAreaUnlock();
      if(MenuUIController.Instance.IsClosed())
        camScript.SetToWorldMode();
    }

    RoomState.Instance. SaveSceneState();

  }

  public void UnlockProfileMenu()
  {
    onboardingMenuManager.AddProfileButton();
  }

  public void UnlockBibMenu()
  {
    onboardingMenuManager.AddBibButton();
  }

  public void UnlockMapMenu()
  {
    onboardingMenuManager.AddMapButton();
  }

  public void UnlockQuestMenu()
  {
    onboardingMenuManager.AddQuestlogButton();
  }

  public void UnlockAnalyzeTool()
  {
    Debug.Log("BatmanTool unlocked");
    controller.gameState.onboardingData.konstruktorData.analyzeToolUnlocked = true;
    controller.SaveGame();
  }

  [Obsolete]
  public void ResetOnboardingData()
  {
    currentOnboardingData.officeWelcomeFinished = true;
    currentOnboardingData.profileUnlocked = true;
    currentOnboardingData.bibUnlocked = true;
    currentOnboardingData.mapUnlocked = false;
    currentOnboardingData.outroEventFinished = false;
    currentOnboardingData.finishedEvents = new List<string>();
    SaveOnboardingData();
  }
}
