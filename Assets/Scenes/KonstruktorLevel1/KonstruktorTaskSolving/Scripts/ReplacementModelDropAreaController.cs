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
using Sirenix.OdinInspector;
using TMPro;
using UniRx;

[RequireComponent(typeof(SingleFingerInputTrigger))]
public class ReplacementModelDropAreaController : MonoBehaviour, IKonstruktorDropHandler {

  [Required]
  public DropAreaUI dropArea;
  [Required]
  public ReplacementModelPopupController popupController;
  [Required]
  public Image replacementTypeImage;

  [Required]
  public Sprite replacementTypeMass;
  [Required]
  public Sprite replacementTypeRope;
  [Required]
  public Sprite replacementTypeRod;
  [Required]
  public Sprite replacementTypeBeam;

  public ReplacementModelType expectedType;
  public ReplacementModelType selectedType;

  public InventoryItem droppedItem;

  private SingleFingerInputTrigger trigger;

  private Dictionary<ReplacementModelType, Sprite> typeSpriteMapping;

  public void Initialize() {

    dropArea.itemDropped.AddListener(HandleDroppedItem);
  }

  private void Start() {
    trigger = GetComponent<SingleFingerInputTrigger>();
    trigger.OnSingleFingerUpAsObservable().Subscribe(HandleTap);

    typeSpriteMapping = new Dictionary<ReplacementModelType, Sprite>() {
      { ReplacementModelType.Beam, replacementTypeBeam },
      { ReplacementModelType.Mass, replacementTypeMass },
      { ReplacementModelType.Rod, replacementTypeRod },
      { ReplacementModelType.Rope, replacementTypeRope },
    };
  }

  private void HandleTap(Vector2 pos) {

    if(droppedItem != null) {

      HandleDroppedItem(droppedItem);
    }
  }

  private void HandleDroppedItem(InventoryItem item) {

    if(droppedItem != null) {

      Destroy(droppedItem.gameObject);
    }

    InventoryItem clone = Instantiate(item, transform);
    clone.enableDrag = false;
    clone.transform.localPosition = Vector3.zero;
    clone.magnitude.Value = item.magnitude.Value;

    clone.gameObject.SetActive(false);

    droppedItem = clone;

    popupController.ShowPopup();
    popupController.onClick.AddListener(HandleTypeSelection);
  }

  private void HandleTypeSelection(ReplacementModelType type) {

    selectedType = type;

    typeSpriteMapping.TryGetValue(type, out Sprite selectedSprite);

    replacementTypeImage.sprite = selectedSprite;
  }
}
