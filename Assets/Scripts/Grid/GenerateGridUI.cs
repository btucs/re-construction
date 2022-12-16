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
using TMPro;
using System;

public class GridInfoEvent : UnityEvent<GridInfo> {}

[Serializable]
public struct GridDimensions
{
  [LabelText("+X")]
  public int posX;
  [LabelText("+Y")]
  public int posY;
  [LabelText("-X")]
  public int negX;
  [LabelText("-Y")]
  public int negY;
}

public struct GridInfo {
  public float unitSize;
  public int intermediateSteps;
  public GridDimensions dimensions;
  public bool isDrawFinished;
}

public class GenerateGridUI : MonoBehaviour {

  [Required]
  public RectTransform gridContainer;
  [Required]
  public RectTransform secondaryAxisContainer;
  [Required]
  public RectTransform intermediateAxisContainer;

  [Required]
  public GameObject secondaryAxisTemplate;
  [Required]
  public GameObject intermediateAxisTemplate;
  [Required]
  public GameObject textTemplate;

  [Required]
  public int unitSize = 100;
  [Required]
  public int intermediateSteps = 5;

  public GridInfoEvent onDrawFinished = new GridInfoEvent();

  public GridDimensions dimensions = new GridDimensions() { posX = 10, negX = 10, posY = 10, negY = 10 };

  /// <summary>
  /// <returns>GridInfo</returns>
  /// </summary>
  public GridInfo CurrentGridInfo {
    get; private set;
  } = new GridInfo() {
    //unitSize = unitSize,
    //intermediateSteps = intermediateSteps,
    isDrawFinished = false
  };

  public void Configure(Vector2 canvasOrigin, GridDimensions dimensions, int unitSize, int intermediateSteps) {

    this.dimensions = dimensions;

    gridContainer.sizeDelta = CalculateDimensions(dimensions, unitSize);

    Vector2 origin = CalculateOrigin(dimensions, unitSize);
    origin.Scale(new Vector2(-1, 1));

    gridContainer.anchoredPosition = origin + canvasOrigin;

    this.unitSize = unitSize;
    this.intermediateSteps = intermediateSteps;

    DrawGrid();
  }

  private Vector2 CalculateDimensions(GridDimensions dimensions, int unitSize) {

    int x = (dimensions.posX + dimensions.negX) * unitSize;
    int y = (dimensions.posY + dimensions.negY) * unitSize;

    return new Vector2(x, y);
    ;
  }

  private Vector2 CalculateOrigin(GridDimensions dimensions, int unitSize) {

    return new Vector2(dimensions.posX * unitSize, dimensions.negY * unitSize);
  }

  private void DrawGrid() {

    GameObject go;
    RectTransform rectTransform;
    TMP_Text text;

    float containerWidth = gridContainer.rect.width;
    float containerHeight = gridContainer.rect.height;

    Vector2 origin = CalculateOrigin(dimensions, unitSize);

    float intermediateSize = unitSize / intermediateSteps;

    // up
    for(int i = 0; i <= dimensions.posY; i++) {

      float currentDistance = i * unitSize;

      if(i > 0) {

        // secondary axis line
        go = Instantiate(secondaryAxisTemplate, secondaryAxisContainer, false);
        (go.transform as RectTransform).anchoredPosition = new Vector2(0, currentDistance);
        go.name = "Secondary Axis " + currentDistance;
        go.SetActive(true);

        // secondary axis text
        go = Instantiate(textTemplate, secondaryAxisContainer, false);
        (go.transform as RectTransform).anchoredPosition = new Vector2(0, currentDistance);
        go.name = "Axis Text " + currentDistance;
        text = go.GetComponent<TMP_Text>();
        text.text = "" + i;
        text.alignment = TextAlignmentOptions.MidlineRight;
        go.SetActive(true);
      }      

      for(int j = 1; j < intermediateSteps; j++) {

        float currentIntermediateDistance = currentDistance + (j * intermediateSize);
        go = Instantiate(intermediateAxisTemplate, intermediateAxisContainer, false);
        (go.transform as RectTransform).anchoredPosition = new Vector2(0, currentIntermediateDistance);
        go.name = "Intermediate Axis " + currentIntermediateDistance;
        go.SetActive(true);
      }
    }

    // down
    for(int i = 0; i >= -dimensions.negY; i--) {

      float currentDistance = i * unitSize;

      if(i < 0) {
        
        // secondary axis line
        go = Instantiate(secondaryAxisTemplate, secondaryAxisContainer, false);
        (go.transform as RectTransform).anchoredPosition = new Vector2(0, currentDistance);
        go.name = "Secondary Axis " + currentDistance;
        go.SetActive(true);

        // secondary axis text
        go = Instantiate(textTemplate, secondaryAxisContainer, false);
        (go.transform as RectTransform).anchoredPosition = new Vector2(0, currentDistance);
        go.name = "Axis Text " + currentDistance;
        text = go.GetComponent<TMP_Text>();
        text.text = "" + i;
        text.alignment = TextAlignmentOptions.MidlineRight;
        go.SetActive(true);
      }

      for(int j = 1; j < intermediateSteps; j++) {

        float currentIntermediateDistance = currentDistance - (j * intermediateSize);
        go = Instantiate(intermediateAxisTemplate, intermediateAxisContainer, false);
        (go.transform as RectTransform).anchoredPosition = new Vector2(0, currentIntermediateDistance);
        go.name = "Intermediate Axis " + currentIntermediateDistance;
        go.SetActive(true);
      }
    }

    // right
    for(int i = 0; i <= dimensions.posX; i++) {

      float currentDistance = i * unitSize;

      if(i > 0) {

        // secondary axis line
        go = Instantiate(secondaryAxisTemplate, secondaryAxisContainer, false);
        rectTransform = go.transform as RectTransform;
        rectTransform.anchoredPosition = new Vector2(currentDistance, 0);
        rectTransform.Rotate(new Vector3(0, 0, 90));
        go.name = "Secondary Axis " + currentDistance;
        go.SetActive(true);
      
        // secondary axis text
        go = Instantiate(textTemplate, secondaryAxisContainer, false);
        (go.transform as RectTransform).anchoredPosition = new Vector2(currentDistance, 0);
        go.name = "Axis Text " + currentDistance;
        text = go.GetComponent<TMP_Text>();
        text.text = "" + i;
        text.alignment = TextAlignmentOptions.Bottom;
        go.SetActive(true);
      }

      for(int j = 1; j < intermediateSteps; j++) {

        float currentIntermediateDistance = currentDistance + (j * intermediateSize);
        go = Instantiate(intermediateAxisTemplate, intermediateAxisContainer, false);
        rectTransform = go.transform as RectTransform;
        rectTransform.anchoredPosition = new Vector2(currentIntermediateDistance, 0);
        rectTransform.Rotate(new Vector3(0, 0, 90));
        go.name = "Intermediate Axis " + currentIntermediateDistance;
        go.SetActive(true);
      }
    }

    //left
    for(int i = 0; i >= -dimensions.negX; i--) {

      float currentDistance = i * unitSize;

      if(i < 0) {

        // secondary axis line
        go = Instantiate(secondaryAxisTemplate, secondaryAxisContainer, false);
        rectTransform = go.transform as RectTransform;
        rectTransform.anchoredPosition = new Vector2(currentDistance, 0);
        rectTransform.Rotate(new Vector3(0, 0, 90));
        go.name = "Secondary Axis " + currentDistance;
        go.SetActive(true);

        // secondary axis text
        go = Instantiate(textTemplate, secondaryAxisContainer, false);
        (go.transform as RectTransform).anchoredPosition = new Vector2(currentDistance, 0);
        go.name = "Axis Text " + currentDistance;
        text = go.GetComponent<TMP_Text>();
        text.text = "" + i;
        text.alignment = TextAlignmentOptions.Bottom;
        go.SetActive(true);
      }

      for(int j = 1; j < intermediateSteps; j++) {

        float currentIntermediateDistance = currentDistance - (j * intermediateSize);
        go = Instantiate(intermediateAxisTemplate, intermediateAxisContainer, false);
        rectTransform = go.transform as RectTransform;
        rectTransform.anchoredPosition = new Vector2(currentIntermediateDistance, 0);
        rectTransform.Rotate(new Vector3(0, 0, 90));
        go.name = "Intermediate Axis " + currentIntermediateDistance;
        go.SetActive(true);
      }
    }

    CurrentGridInfo = new GridInfo {
      intermediateSteps = intermediateSteps,
      unitSize = unitSize,
      dimensions = dimensions,
      isDrawFinished = true,
    };

    onDrawFinished?.Invoke(CurrentGridInfo);
  }

  private void OnDestroy() {

    HelperFunctions.DestroyChildren(intermediateAxisContainer, true);
    HelperFunctions.DestroyChildren(secondaryAxisContainer, true);
  }
}
