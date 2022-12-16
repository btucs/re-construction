#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UniRx;

public class InputMenuItemController : MonoBehaviour {

  [Required]
  public DropAreaUI dropArea;
  [Required]
  public GameObject SearchedItemHighlight;

  [Required]
  public KonstruktorVariableAnalysisController variableAnalysisController;
  public UIPanelManager uiPanelManager;

  public BoolReactiveProperty ShowHighlight = new BoolReactiveProperty(false);

  public InventoryItem droppedItem;

  private InputMenuController parentController;

  private void Start() {

    ShowHighlight
      .Do((bool showHighlight) => SearchedItemHighlight.SetActive(showHighlight))
      .Subscribe()
      .AddTo(this)
    ;
  }

  public void HandleDroppedItem(InventoryItem droppedItem) {

    if(this.droppedItem != null) {

      Destroy(this.droppedItem.gameObject);
    }

    this.droppedItem = droppedItem;
    droppedItem.transform.SetParent(transform, true);
    droppedItem.transform.localPosition = Vector3.zero;
    droppedItem.OnTapEvent.AddListener(ShowAnalysisPanel);
    droppedItem.OnDestroyEvent.AddListener(HandleDestroy);
  }

  public void ShowAnalysisPanel(InventoryItem item) {

    uiPanelManager.Show(variableAnalysisController.name);
    variableAnalysisController.SetDisplayContent(item);
  }

  public void SetShowHighlight(bool shouldShow) {

    ShowHighlight.Value = shouldShow;
  }

  public void SetParent(InputMenuController parent) {

    parentController = parent;
  }

  private void HandleDestroy(InventoryItem item) {

    droppedItem = null;
    parentController.RemoveInputController(this);
  }
}
