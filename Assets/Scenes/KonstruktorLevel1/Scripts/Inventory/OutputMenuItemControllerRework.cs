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

public class OutputMenuItemControllerRework : MonoBehaviour {

  [Required]
  public ParticleSystem solvedParticleSystem;
  [Required]
  public DropAreaUI dropArea;
  [Required]
  public GameObject SearchedItemHighlight;

  [Required]
  public KonstruktorVariableAnalysisController variableAnalysisController;
  // GameObjects that will be SetActive(false) when VariableAnalysis GameObject will be set to active
  public UIPanelManager uiPanelManager;

  public BoolReactiveProperty ShowHighlight = new BoolReactiveProperty(false);

  private int stepIndex = -1;
  private GameController controller;

  private InventoryItem droppedItem;
  
  private void Start() {

    // counts hidden objects as well
    stepIndex = transform.GetSiblingIndex() - 1;
    controller = GameController.GetInstance();

    ShowHighlight
      .Do((bool showHighLight) => SearchedItemHighlight.SetActive(showHighLight))
      .Subscribe()
      .AddTo(this)
    ;
  }

  [Obsolete("Probably not used anymore")]
  public bool ValidateSolution() {
    KonstruktorSceneData sceneData = controller.gameState.konstruktorSceneData;
    ConverterResultData[] results = sceneData.converterResults;

    ConverterResultData connectedResult = /*results.FirstOrDefault((ConverterResultData result) => result.step == stepIndex);*/ null;

    if(connectedResult != null) {

      GetComponent<Image>().color = new Color32(81, 112, 94, 255);
      solvedParticleSystem.Play();

      return true;
    }

    return false;
  }

  public void HandleDroppedItem(InventoryItem droppedItem) {

    if(this.droppedItem != null) {

      Destroy(this.droppedItem.gameObject);
    }

    this.droppedItem = droppedItem;
    droppedItem.enableDrag = false;
    droppedItem.transform.SetParent(transform, true);
    droppedItem.transform.localPosition = Vector3.zero;
    droppedItem.OnTapEvent.AddListener(ShowAnalysisPanel);
  }

  private void ShowAnalysisPanel(InventoryItem item) {

    uiPanelManager.Show(variableAnalysisController.name);
    variableAnalysisController.SetDisplayContent(item);
  }
}
