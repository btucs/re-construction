#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DropArea : MonoBehaviour, IDropArea {

  public InventoryItem droppedItem;
  public SpriteRenderer backgroundColorRenderer;
  public SpriteRenderer iconRenderer;

  public Color acceptColor;
  public Color denyColor;

  public InventoryItemEvent itemDropped;

  [ValueDropdown("acceptedTypeOptions")]
  public string acceptedTypeString;
  private Type acceptedType;

  private Sprite defaultIcon;
  private Color defaultColor;

  private Type[] acceptedTypes = new Type[] {
    typeof(TaskInputVariable),
    typeof(TaskOutputVariable),
  };
 
  private void Start() {

    defaultIcon = iconRenderer.sprite;
    defaultColor = backgroundColorRenderer.color;
    acceptedType = acceptedTypes.FirstOrDefault((Type type) => type.ToString() == acceptedTypeString);
  }

  private IEnumerable acceptedTypeOptions() {

    return acceptedTypes.Select((Type type) => type.ToString());
  }

  public bool IsValidTarget(InventoryItem dropItem) {

    if(acceptedType != null && dropItem.magnitude.Value.GetType() != acceptedType) {

      return false;
    }

    bool result = true;

    return result;
  }

  public void DropItem(InventoryItem item) {

    droppedItem = item;

    //iconRenderer.sprite = item.GetSprite();
    backgroundColorRenderer.color = defaultColor;

    itemDropped.Invoke(item);
  }

  public void OnEnter(InventoryItem item) {

    backgroundColorRenderer.color = (IsValidTarget(item) ? acceptColor : denyColor);
  }

  public void OnLeave() {

    backgroundColorRenderer.color = defaultColor;
  }
}
