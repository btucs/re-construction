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

public class DropAreaKonstruktor : MonoBehaviour, IDropArea {

  public InventoryItem droppedItem;
  public Image backgroundColorRenderer;
  
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

  private ConstructorManager moduleManager;
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

    if(dropItem.magnitude.Value.GetType() != acceptedType) {

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
    //backgroundColorRenderer.color = defaultColor;
    itemDropped.Invoke(item);

    Debug.Log("dropped");

    if(showIndicatorOnDrop == true) {

      //successParticleSystem.Stop();
      successParticleSystem.Play();
    }
  }

  public void OnEnter(InventoryItem item) {

    //backgroundColorRenderer.color = (IsValidTarget(item) ? acceptColor : denyColor);
  }

  public void OnLeave() {

    //backgroundColorRenderer.color = defaultColor;
  }

  public void SetConstructorManager(ConstructorManager _manager)
  {
  	moduleManager = _manager;
  }
}