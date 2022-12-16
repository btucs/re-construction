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
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class OutputVariableChoiceController : MonoBehaviour {

  [Required]
  public GameObject VariableHolderPanel;
  [Required]
  public OutputVariableAnalysisController outputVariableAnalysisController;
  [Required]
  public Button GoToConverterButton;
  [Required]
  public string GoToConverterButtonSceneTarget;

  private GameObject currentItemCopy;
  private InventoryItem originalItem;
  private int stepIndex = -1;

  public void SetDisplayContent(InventoryItem item) {

    if(currentItemCopy != null) {

      Destroy(currentItemCopy);
      currentItemCopy = null;
      originalItem = null;
    }

    originalItem = item;
    currentItemCopy = Instantiate(item.gameObject, VariableHolderPanel.transform);
    RectTransform currentItemTransform = (RectTransform)currentItemCopy.transform;
    currentItemTransform.sizeDelta = new Vector2(80, 80);

    GameController controller = GameController.GetInstance();
    int currentStep = controller.gameState.konstruktorSceneData.currentStep;

    // hidden template is also count
    stepIndex = item.transform.parent.GetSiblingIndex() - 1;

    GoToConverterButton.interactable = stepIndex != currentStep;
  }

  public void GoToConverter() {

    GameController controller = GameController.GetInstance();
    KonstruktorSceneData sceneData = controller.gameState.konstruktorSceneData;
    sceneData.currentStep = stepIndex;
    controller.SaveGame();

    SceneManager.LoadScene(GoToConverterButtonSceneTarget);
  }

  public void GotoAnalysis() {

    outputVariableAnalysisController.gameObject.SetActive(true);
    outputVariableAnalysisController.SetDisplayContent(originalItem);
    gameObject.SetActive(false);
  }
}
