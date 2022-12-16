#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UniRx;

public class OutputMenuItemController : MonoBehaviour {

  [Required]
  public ParticleSystem solvedParticleSystem;
  [Required]
  public DropAreaUI outputDropArea;
  [Required]
  public DropAreaUI resultDropArea;
  [Required]
  public GameObject SearchedItemHighlight;

  [Required]
  public KonstruktorVariableAnalysisController variableAnalysisController;
  // GameObjects that will be SetActive(false) when VariableAnalysis GameObject will be set to active
  public UIPanelManager uiPanelManager;

  public BoolReactiveProperty ShowHighlight = new BoolReactiveProperty(false);

  public InventoryItem droppedItem;

  private GameController controller;

  public int StepIndex { get; private set; } = -1;

  public void Initialize() {

    controller = GameController.GetInstance();

    outputDropArea.Initialize();
    outputDropArea.itemDropped.AddListener(HandleDroppedItem);

    resultDropArea.Initialize();
    resultDropArea.itemDropped.AddListener(HandleResultDrop);
  }

  private void Start() {

    // counts hidden objects as well
    StepIndex = transform.GetSiblingIndex() - 1;
    controller = GameController.GetInstance();

    ShowHighlight
      .Do((bool showHighLight) => SearchedItemHighlight.SetActive(showHighLight))
      .Subscribe()
      .AddTo(this)
    ;
  }

  public void HandleDroppedItem(InventoryItem droppedItem) {

    TaskOutputVariable droppedVariable = droppedItem.magnitude.Value as TaskOutputVariable;
    if(this.droppedItem != null) {

      KonstruktorSceneData konstruktorSceneData = controller.gameState.konstruktorSceneData;
      ConverterResultData resultData = konstruktorSceneData.FindResultFor(this.droppedItem.magnitude.Value as TaskOutputVariable);
      if(resultData != null) {

        resultData.resultFor = null;
      }

      Destroy(this.droppedItem.gameObject);
    }

    this.droppedItem = droppedItem;
    droppedItem.enableDrag = false;
    droppedItem.transform.SetParent(transform, true);
    droppedItem.transform.localPosition = Vector3.zero;
    droppedItem.OnTapEvent.AddListener(ShowAnalysisPanel);
    if(droppedItem.hasResult) {

      droppedItem.variableName.text = droppedVariable.name;
    }

    controller.SaveGame();
  }

  public void HandleResultDrop(InventoryItem resultItem) {

    if(droppedItem == null) {

      return;
    }

    TaskOutputVariable currentItem = droppedItem.magnitude.Value as TaskOutputVariable;
    KonstruktorSceneData sceneData = controller.gameState.konstruktorSceneData;

    ConverterResultData converterResult = sceneData.FindResultFor(resultItem.magnitude);

    if(converterResult != null) {

      if(droppedItem.hasResult == true) {

        ConverterResultData previousConnectedResult = sceneData.FindResultFor(currentItem);
        if(previousConnectedResult != null) {

          previousConnectedResult.resultFor = null;
        }
      }

      droppedItem.hasResult = true;
      converterResult.resultFor = currentItem;
      droppedItem.GetComponent<Image>().color = new Color32(81, 112, 94, 255);
      droppedItem.variableName.text = resultItem.variableName.text;
    }

    resultItem.ResetPosition();

    controller.SaveGame();
  }

  private void ShowAnalysisPanel(InventoryItem item) {

    uiPanelManager.Show(variableAnalysisController.name);
    variableAnalysisController.SetDisplayContent(item);
  }
}
