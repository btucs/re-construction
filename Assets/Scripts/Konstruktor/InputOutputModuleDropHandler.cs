#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputOutputModuleDropHandler : MonoBehaviour {

  public GameObject dropAreaPlaceholderSpriteObject;
  public GameObject bottomHalfObject;
  public SpriteRenderer dropAreaDroppedSprite;
  public SpriteRenderer backgroundSprite;
  public TextMeshPro dropAreaVariable;

  private InventoryItem droppedItem;

  public void HandleDroppedItem(InventoryItem item) {

    dropAreaPlaceholderSpriteObject.SetActive(false);
    dropAreaDroppedSprite.sprite = item.GetSprite();
    CalculateAndSetScale(dropAreaDroppedSprite);
    dropAreaDroppedSprite.gameObject.SetActive(true);
    dropAreaVariable.text = item.magnitude.Value.textMeshProName;
    dropAreaVariable.gameObject.SetActive(true);
    bottomHalfObject.SetActive(true);

    Image image = item.GetComponent<Image>();
    backgroundSprite.color = image.color;

    droppedItem = item;
  }

  public InventoryItem GetDroppedItem() {

    return droppedItem;
  }

  private void CalculateAndSetScale(SpriteRenderer spriteRenderer) {

    Transform parent = spriteRenderer.transform.parent;
    SpriteRenderer parentSpriteRenderer = parent.GetComponent<SpriteRenderer>();

    Vector3 parentBoundsSize = parentSpriteRenderer.sprite.bounds.size;

    float xScale = parentBoundsSize.x / spriteRenderer.sprite.bounds.size.x * 0.6f;
    float yScale = parentBoundsSize.y / spriteRenderer.sprite.bounds.size.y * 0.6f;

    float minScale = Mathf.Min(xScale, yScale);

    spriteRenderer.transform.localScale = new Vector3(minScale, minScale, 1);
  }
}
