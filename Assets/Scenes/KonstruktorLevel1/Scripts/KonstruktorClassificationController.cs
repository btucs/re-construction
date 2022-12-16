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
using Sirenix.OdinInspector;

public class KonstruktorClassificationController : StepControllerAbstract {

  [Required, FoldoutGroup("Configuration", expanded: false)]
  public UIPanelManager uiManager;
  [Required, FoldoutGroup("Configuration")]
  public GameObject givenAndSearchedItemsCanvas;
  [Required, FoldoutGroup("Configuration"), ChildGameObjectsOnly]
  public RectTransform leftSidebarContent;
  [Required, FoldoutGroup("Configuration"), ChildGameObjectsOnly]
  public RectTransform rightSidebarContent;
  [Required, FoldoutGroup("Configuration"), ChildGameObjectsOnly]
  public RectTransform placedItemsContainer;
  [Required, FoldoutGroup("Configuration"), ChildGameObjectsOnly]
  public Text taskNameText;
  [Required, FoldoutGroup("Configuration"), ChildGameObjectsOnly]
  public Button backButton;
  [Required, FoldoutGroup("Configuration"), ChildGameObjectsOnly]
  public Button continueButton;
  [Required, FoldoutGroup("Configuration")]
  public InputMenuController inputMenuController;

  [Required, FoldoutGroup("Configuration")]
  public ReplacementModelController replacementModelController;

  [Required, FoldoutGroup("Templates", expanded: false)]
  public InputOutputDrawController inputOutputPlaceholderTemplate;
  [Required, FoldoutGroup("Templates")]
  public ReplacementModelDropAreaController replacementModelDropAreaTemplate;

  private KonstructorModuleSO currentModuleSO;
  private AbstractClassificationHandler currentHandler;

  public override void SetupAndOpen(KonstructorModuleSO currentModuleSO) {

    uiManager.Show(name);
    this.currentModuleSO = currentModuleSO;
    taskNameText.text = currentModuleSO.title;

    if(currentHandler != null) {

      // just destroy the script itself, not the GameObject
      Destroy(currentHandler);
      currentHandler = null;
    }

    currentHandler = ClassificationHandlerFactory.CreateAndAddClassificationHandler(currentModuleSO.moduleType, this);
  }

  public void HandleContinueButtonPressed() {

    currentHandler.CreateAndSaveResult();
  }

  public void HandleBackButtonPressed() {

    Debug.Log("back button pressed");
  }

  public int GetDroppedItemCount() {

    return currentHandler.GetDroppedItemCount();
  }

  public bool AllItemsDropped() {

    return currentHandler.AllItemsDropped();
  }

  public void Hide() {

    uiManager.Hide(name);
  }
}
