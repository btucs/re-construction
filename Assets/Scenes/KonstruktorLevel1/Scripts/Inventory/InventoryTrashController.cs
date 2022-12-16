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
using Sirenix.OdinInspector;
using UniRx;

public class InventoryTrashController : MonoBehaviour {

  [Required]
  public ParticleSystem trashedParticleSystem;

  [Required]
  public DropAreaUI dropArea;

  [Required]
  public GameObject SearchedItemHighlight;

  public BoolReactiveProperty ShowHighlight = new BoolReactiveProperty(false);

  public void HandleDroppedItem(InventoryItem droppedItem) {

    droppedItem.OnDestroyEvent?.Invoke(droppedItem);
    Destroy(droppedItem.gameObject);
  }

  private void Start() {

    ShowHighlight
      .Do((bool showHighLight) => SearchedItemHighlight.SetActive(showHighLight))
      .Subscribe()
      .AddTo(this)
    ;

    if(dropArea != null) {

      dropArea.itemDropped.AddListener(HandleDroppedItem);
    }
  }

  public void SetEnabled(bool enabled)
  {
    dropArea.gameObject.SetActive(enabled);
  }

  private void OnDestroy() {

    if(dropArea != null) {

      dropArea.itemDropped.RemoveListener(HandleDroppedItem);
    }
  }
}
