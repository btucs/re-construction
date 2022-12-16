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
using Sirenix.OdinInspector;

public class LineControllerUI : MonoBehaviour{

  [Required, PreviewField]
  public Image lineImage;
  [Required, PreviewField]
  public Image startImage;
  [Required, PreviewField]
  public Image endImage;

  public Vector2 startPos {
    get; private set;
  }

  public Vector2 endPos {
    get; private set;
  }

  private RectTransform rectTransform => transform as RectTransform;
  private LineUISO currentSettings;

  public void ConfigureLine(LineUISO lineSO) {

    gameObject.SetActive(true);

    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, lineSO.lineWidth);

    lineImage.sprite = lineSO.lineSprite;
    lineImage.color = lineSO.lineColor;
    lineImage.pixelsPerUnitMultiplier = lineSO.pixelPerUnit;
    lineImage.type = Image.Type.Tiled;
    RectTransform lineImageTransform = lineImage.transform as RectTransform;
    Vector2 startEndMargin = Vector2.zero;

    startImage.sprite = lineSO.startSprite;
    if(lineSO.startSprite == null) {

      startImage.color = Color.clear;
      startEndMargin.x = 0;
    } else {

      startImage.color = lineSO.lineColor;
      startEndMargin.x = (startImage.transform as RectTransform).sizeDelta.x / 2;
    }
    
    endImage.sprite = lineSO.endSprite;
    if(lineSO.endSprite == null) {

      endImage.color = Color.clear;
      startEndMargin.y = 0;
    } else {

      endImage.color = lineSO.lineColor;
      startEndMargin.y = (endImage.transform as RectTransform).sizeDelta.x / 2;
    }

    // set left distance of stretch, corresponding to the beginning of the line
    lineImageTransform.offsetMin = new Vector2(startEndMargin.x, lineImageTransform.offsetMin.y);
    // set right distance of stretch, corresponding to the end of the line
    lineImageTransform.offsetMax = new Vector2(-startEndMargin.y, lineImageTransform.offsetMax.y);
  }

  public void PositionLine(Vector2 startPos, Vector2 endPos) {

    this.startPos = startPos;
    this.endPos = endPos;

    Vector2 direction = (endPos - startPos).normalized;
    Vector2 center = (startPos + endPos) / 2;
    float length = Vector2.Distance(endPos, startPos);
    
    rectTransform.localPosition = center;
    rectTransform.localRotation = Quaternion.FromToRotation(Vector3.right, direction);
    rectTransform.sizeDelta = new Vector2(length, rectTransform.sizeDelta.y);
  }
}
