#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InputMenuSizeExpander : MonoBehaviour
{
  public RectTransform expandTransform;
  public RectTransform childTransform;
  public int maxSize;

  public Sprite singleEntrySprite;
  public Sprite multipleEntrySprite;
  private HorizontalLayoutGroup offSet;
  private Image containerImg;

  // Start is called before the first frame update
  void Start() {
    StartSetup();
  }

  private void OnEnable() {
    StartSetup();
  }

  private void StartSetup() {
    offSet = childTransform.GetComponent<HorizontalLayoutGroup>();
    containerImg = expandTransform.GetComponent<Image>();
    UpdateTransformSize();
  }

  public void UpdateTransformSize() {
    float horizontalPadding = GetPaddingAmount();
    //Debug.Log("padding left and right: " + (float)offSet.padding.horizontal + "and between: " + (float)childTransform.childCount*offSet.spacing);
    float childSize = GetXSizeOfChildren(childTransform);
    childSize += horizontalPadding;
    childTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, childSize);
    if(childSize > maxSize) {
      childSize = maxSize;
    }

    expandTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, childSize);
    //CheckForSpriteSwap();
  }

  public void UpdateTransformSizeDelayed() {

    StartCoroutine("UpdateNextFrame");
  }

  private IEnumerator UpdateNextFrame() {

    yield return 0;

    UpdateTransformSize();
  }

  private float GetPaddingAmount() {
    offSet = childTransform.GetComponent<HorizontalLayoutGroup>();
    float xPadding = (float)offSet.padding.horizontal;

    if(childTransform.childCount > 0) {
      for(int i = 0; i < childTransform.childCount; i++) {
        if(childTransform.GetChild(i).gameObject.activeSelf && childTransform.GetChild(i).GetComponent<LayoutElement>() == null) {
          xPadding += offSet.spacing;
        }
      }
      xPadding -= offSet.spacing;
    }
    Debug.Log("padding ist " + xPadding);
    return xPadding;
  }

  private float GetXSizeOfChildren(Transform container) {
    float size = 0f;

    for(int i = 0; i < container.childCount; i++) {
      RectTransform currentTransform = container.GetChild(i).GetComponent<RectTransform>();
      if(currentTransform.gameObject.activeSelf && currentTransform.GetComponent<LayoutElement>() == null) {
        size += currentTransform.rect.width;
      }
    }
    Debug.Log("size is " + size);
    return size;
  }

  private void CheckForSpriteSwap() {

    if(containerImg == null || childTransform == null)
    {
      Debug.Log("Check if item menu is configured correctly.");
      return;
    }

    if(childTransform.childCount >= 1) {
      containerImg.sprite = multipleEntrySprite;
      containerImg.type = Image.Type.Sliced;
    } else {
      containerImg.sprite = singleEntrySprite;
      containerImg.type = Image.Type.Simple;
    }
  }
}
