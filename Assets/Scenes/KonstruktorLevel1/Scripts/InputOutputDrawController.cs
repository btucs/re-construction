#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class InputOutputDrawControllerEvent : UnityEvent<InputOutputDrawController> {

}

public class InputOutputDrawController : MonoBehaviour, IKonstruktorDropHandler
{
  [Required]
  public DropAreaUI dropArea;
  [Required]
  public GameObject highlight;
  [Required]
  // set for input paramerts
  public CalculatorParameterType expectedParameterType = CalculatorParameterType.Any;
  [Required]
  // set for output parameters
  public TaskOutputVariableUnit expectedOutputUnit = TaskOutputVariableUnit.Unspecified;

  public InputOutputDrawControllerEvent onTap = new InputOutputDrawControllerEvent();
  public InputOutputDrawControllerEvent onItemDropped = new InputOutputDrawControllerEvent();

  public InventoryItem droppedItem;
  public bool disableTap = false;

  public DrawVariableData varData;

  public bool IsHighlighted {
    get;
    private set;
  }

  public int StepIndex {
    get;
    private set;
  } = -1;

  public void Clear() {

    if(droppedItem != null) {

      dropArea.Clear();
      Destroy(droppedItem.gameObject);
      droppedItem = null;
    }
  }

  public void Initialize() {

    dropArea.itemDropped.AddListener(HandleDroppedItem);
  }

  private void Start() {

    IsHighlighted = highlight.activeSelf;
    highlight.SetActive(IsHighlighted);
  }

  private void HandleDroppedItem(InventoryItem item) {

    if(droppedItem != null) {

      Destroy(droppedItem.gameObject);
    }
    
    InventoryItem clone = Instantiate(item, transform);
    clone.enableDrag = false;
    clone.transform.localPosition = Vector3.zero;
    // magnitude does not get cloned automatically
    clone.magnitude.Value = item.magnitude.Value;

    if(clone.magnitude.Value is TaskInputVariable) {

      clone.OnTapEvent.AddListener(HandleTap);
      if(disableTap == false) {

        ShowHighlight(true);
      }
    } else {

      // TaskOutputVariable
      OutputMenuItemController outputController = item.ParentTransform.GetComponentInParent<OutputMenuItemController>();
      // current source parent of the item is an OutputMenuItemController
      if(outputController != null) {

        StepIndex = outputController.StepIndex;
      }
    }

    droppedItem = clone;
    onItemDropped?.Invoke(this);
  }

  private void HandleTap(InventoryItem item) {

    if(disableTap == false) {

      ToggleHighlight();
      onTap?.Invoke(this);
    }
  }

  public void ToggleHighlight() {

    IsHighlighted = !IsHighlighted;
    highlight.SetActive(IsHighlighted);
  }

  public void ShowHighlight(bool show) {

    IsHighlighted = show;
    highlight.SetActive(show);
  }

  private void OnDestroy() {

    if(droppedItem != null) {

      droppedItem.OnTapEvent.RemoveListener(HandleTap);
    }
  }
}
