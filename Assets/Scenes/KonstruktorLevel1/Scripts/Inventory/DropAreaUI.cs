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
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class DropAreaUI : MonoBehaviour, IDropArea, IDropHandler {

  public Image backgroundColorRenderer;
  
  public Color acceptColor;
  public Color denyColor;

  public InventoryItemEvent itemDropped;

  public bool highlightOnEnterLeave = false;

  [ValueDropdown("acceptedTypeOptions")]
  public string acceptedTypeString;
  private Type acceptedType;

  private InventoryItem droppedItem;

  private Sprite defaultIcon;
  private Color defaultColor;

  private Type[] acceptedTypes = new Type[] {
    typeof(TaskInputVariable),
    typeof(TaskOutputVariable),
    typeof(TaskVariable),
  };

  private AbstractCondition[] conditions = new AbstractCondition[0];

  public ParticleSystem successParticleSystem;

  private void Start() {

    Initialize();
  }

  private IEnumerable acceptedTypeOptions() {

    return acceptedTypes.Select((Type type) => type.ToString());
  }

  public void Initialize() {

    defaultColor = backgroundColorRenderer.color;
    acceptedType = acceptedTypes.FirstOrDefault((Type type) => type.ToString() == acceptedTypeString);
  }

  public void AddCondition(AbstractCondition condition) {

    conditions = conditions.Append(condition).ToArray();
  }

  public bool IsValidTarget(InventoryItem dropItem) {

    Type toDropItemType = dropItem.magnitude.Value.GetType();

    if(CheckType(toDropItemType) == false) {

      return false;
    }

    bool result = conditions.All((AbstractCondition condition) => {
      condition.SetCompareToValue(dropItem);

      return condition.IsFullfilled();
    });

    return result;
  }

  public void DropItem(InventoryItem item) {

    DropItem(item, true);
  }

  public void DropItem(InventoryItem item, bool showIndicatorOnDrop) {

    droppedItem = item;
    backgroundColorRenderer.color = defaultColor;
    item.HasBeenDropped(this);
    item.ResetPosition();
    itemDropped.Invoke(item);

    //Debug.Log("dropped");

    if(showIndicatorOnDrop == true && successParticleSystem != null) {

      //successParticleSystem.Stop();
      successParticleSystem.Play();
    }
  }

  public void Clear() {

    droppedItem = null;
  }

  public void OnEnter(InventoryItem item) {

    if(highlightOnEnterLeave == true) {

      backgroundColorRenderer.color = (IsValidTarget(item) ? acceptColor : denyColor);
    }
  }

  public void OnLeave() {

    if(highlightOnEnterLeave == true) {

      backgroundColorRenderer.color = defaultColor;
    }
  }

  public bool HasDroppedItem() {

    return droppedItem != null;
  }

  private bool CheckType(Type itemType) {

    if(acceptedType == typeof(TaskVariable)) {

      return itemType.IsSubclassOf(typeof(TaskVariable));
    }

    return itemType == acceptedType;
  }

  public void OnDrop(PointerEventData eventData) {

    if(eventData.pointerDrag != null && eventData.used == false) {

      InventoryItem item = eventData.pointerDrag.GetComponent<InventoryItem>();
      if(item != null && IsValidTarget(item)) {

        eventData.Use();
        DropItem(item);
      }
    }
  }
}
