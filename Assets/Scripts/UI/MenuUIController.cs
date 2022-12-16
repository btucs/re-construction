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
using UnityEngine.UI;
using Yarn.Unity;
using Sirenix.OdinInspector;

public class MenuUIController : MonoBehaviour
{
  public playerScript playerController;
  public WorldSceneCameraController cameraController;
  public BreadCrumManager breadcrumController;

  public GameObject headlineContainer;
  public Text headlineText;
  public TaskListManager objectUIManager;
  public DialogController dialogueManager;
  public PortfolioContentResolver portfolioUIManager;
  public QuestlogController questUIController;

  public GameObject objectSelectionUI;

  public GameObject bibUI;
  public string bibUIHeadline;

  public GameObject worldMapUI;
  public string worldMapUIHeadline;

  public GameObject portfolioUI;
  public string portfolioUIHeadline;

  public GameObject questlogUI;
  public string questUIHeadline;

  public GameObject settingsUI;
  public GameObject gameMenu;

  public List<GameObject> disableOnMenuClose = new List<GameObject>();

  public GameObject mainMenuPanel;
  public GameObject optionButtonPanel;

  public GameObject mentorDialogButton;

  public SystemNotificationController notificationSystem;
  public WorldSpaceUIController worldSpaceUI;

  private bool worldUIActiveState = true;
  private MenuUIState state = MenuUIState.closed;

  private GameController controller;

  private static MenuUIController instance;
  public static MenuUIController Instance
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
    controller = GameController.GetInstance();
    questUIController.UpdateQuestSubHint();

  }

  public MenuUIState State()
  {
    return state;
  }

  public bool IsClosed()
  {
    return (state == MenuUIState.closed);
  }

  public void ActivateBibUI()
  {
    ChangeHeadlineText(bibUIHeadline);
    DisableContent();
    bibUI.SetActive(true);
    state = MenuUIState.bib;
  }
  
  public void ActivateQuestlogUI()
  {
    ChangeHeadlineText(questUIHeadline);
    DisableContent();
    questlogUI.SetActive(true);
    state = MenuUIState.questlog;
  }

  public void ActivateWorldMapUI()
  {
    ChangeHeadlineText(worldMapUIHeadline);
    DisableContent();
    worldMapUI.SetActive(true);
    state = MenuUIState.worldmap;
  }

  public void ActivatePortfolioUI()
  {
  	string headlineString = portfolioUIHeadline + " von " + controller.gameState.profileData.playerName;
    ChangeHeadlineText(headlineString);
    portfolioUIManager.UpdateProfileContent();
    
    DisableContent();
    portfolioUI.SetActive(true);
    state = MenuUIState.profile;
  }

  public void ActivateObjectSelectionUI(GameWorldObject selectedObj) {

    ChangeHeadlineText(selectedObj.objectData.objectName);
    objectUIManager.UpdateObjectSelectionUI(selectedObj);
    DisableContent();
    //worldSpaceUI.SetWorldSpaceUIActive(false);
    objectSelectionUI.SetActive(true);
    state = MenuUIState.objectSelection;
  }

  public void OpenGameMenu()
  {
  	DisableContent();
    EnterMenuState();
    gameMenu.SetActive(true);
    if(cameraController != null)
      cameraController.isMovable = false;
  }

  public void OpenSettingsMenu()
  {
    DisableContent();
    EnterMenuState();
    settingsUI.SetActive(true);
    if(cameraController != null)
      cameraController.isMovable = false;
  }

  /*private void PositionTaskButtonInstance(GameObject taskButtonInstance, int index) {

    RectTransform rectTransform = taskButtonInstance.GetComponent<RectTransform>();

    float height = rectTransform.rect.height;
    float width = rectTransform.rect.width;
    float y = -height * (index + 1);
    rectTransform.offsetMin = new Vector2(-width / 2, y);
    rectTransform.offsetMax = new Vector2(width / 2, y + height);

    taskButtonInstance.SetActive(true);
  }*/

  private void ClearChildren(Transform parent) {

    foreach(Transform child in parent) {
      Destroy(child.gameObject);
    }
  }

/*  private void CreateTaskButtons( TaskObjectSO taskObject) {

    WorldTaskButtonHandler handler = taskTemplatePrefab.GetComponent<WorldTaskButtonHandler>();

    for(int i = 0; i < taskObject.taskInfos.Count; i++) {

      TaskDataSO task = taskObject.taskInfos[i].task;
      handler.taskData = task;
      handler.taskObject = taskObject;
      GameObject taskButton = Instantiate(taskTemplatePrefab, taskContainer.transform);
      PositionTaskButtonInstance(taskButton, i);
    }
  }*/

  public void EnterCutsceneMode()
  {

  }

  public void EnterDialogue()
  {
    //cameraController.isMovable = false;
    state = MenuUIState.dialogue;
    worldUIActiveState = worldSpaceUI.GetActiveState();
    worldSpaceUI.SetWorldSpaceUIActive(false);
    notificationSystem.AllowTempMessages(false);
    mentorDialogButton.SetActive(false);
  }

  public void CloseDialogue()
  {
    ExitMenu();
    if(OnboardingController.Instance.IsEventActive() == false)
      mentorDialogButton.SetActive(true);
  }

  public void DisableContent()
  {
    settingsUI.SetActive(false);
    objectSelectionUI.SetActive(false);
    bibUI.SetActive(false);
    worldMapUI.SetActive(false);
    portfolioUI.SetActive(false);
    questlogUI.SetActive(false);

    foreach(GameObject disableObject in disableOnMenuClose)
    {
      disableObject.SetActive(false);
    }
  }

  public void ChangeHeadlineText (string newText)
  {
    headlineText.text = newText;
  }

  public void OpenMainMenu()
  {
    headlineContainer.SetActive(true);
    if(cameraController != null)
      cameraController.isMovable = false;
    if(worldSpaceUI != null)
      worldUIActiveState = worldSpaceUI.GetActiveState();
    notificationSystem.AllowTempMessages(false);
  }

  public void ExitMenu()
  {
    CloseMainMenu();
    if(OnboardingController.Instance.IsEventActive() == false)
    {
      if(cameraController != null)
        cameraController.SetToWorldMode();
      if(worldSpaceUI != null)
        worldSpaceUI.SetWorldSpaceUIActive(true);
    }
    ToggleMenuPanelVisibility(true);
  }

  public void CloseMainMenu()
  {
    state = MenuUIState.closed;
    DisableContent();
    headlineContainer.SetActive(false);
    notificationSystem.AllowTempMessages(true);
  }

  public void ToggleMenuPanelVisibility(bool show)
  {
    mainMenuPanel.SetActive(show);
    optionButtonPanel.SetActive(show);
  }

  public void EnterMenuState()
  {
    notificationSystem.AllowTempMessages(false);
    if(worldSpaceUI != null)
      worldSpaceUI.SetWorldSpaceUIActive(false);
  }

  [Obsolete("ExitMenuState is called. Redundant and should be replaced with ExitMenu")]
  public void ExitMenuState()
  {
    if(worldSpaceUI != null)
      worldSpaceUI.SetWorldSpaceUIActive(true);
    notificationSystem.AllowTempMessages(true);
    state = MenuUIState.closed;
    if(playerController != null)
      playerController.EnableInteraction();
  }
}

public enum MenuUIState
{
  closed, profile, bib, questlog, worldmap, objectSelection, dialogue
}