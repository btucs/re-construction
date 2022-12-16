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
using UnityEngine.EventSystems;

public class KonstruktorVarDropHandler : MonoBehaviour, IKonstruktorDropHandler {

  private InventoryItem droppedItem;
  private InventoryItem linkedItem;

  public int StepIndex {
    get; private set;
  }

  public void Initialize() {

  }

  public void HandleDroppedItem(InventoryItem droppedItem) {

    if(this.droppedItem != null) {
      Destroy(this.droppedItem.gameObject);
    }

    InventoryItem clone = Instantiate(droppedItem, transform);
    clone.transform.localPosition = Vector3.zero;
    clone.enableDrag = false;

    if(clone.magnitude.Value is TaskInputVariable) {

      OutputMenuItemController outputController = droppedItem.ParentTransform.GetComponentInParent<OutputMenuItemController>();
      if(outputController != null) {

        StepIndex = outputController.StepIndex;
      }
    }

    this.droppedItem = clone;
    linkedItem = droppedItem;
  }
}
