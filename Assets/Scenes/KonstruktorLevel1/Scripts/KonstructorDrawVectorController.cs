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
using TMPro;

public class KonstructorDrawVectorController : KonstructorDrawEntityAbstract {

  [Required]
  public KonstructorDrawPointController attackPoint;
  [Required]
  public LineControllerUI vectorLine;
  [Required]
  public Transform wirkungslinie;

  [Required]
  public LineUISO vectorLineSO;

  [Required]
  public TextMeshProUGUI displayValues;

  private string vectorName;
  private float screenToScaleMultiplier = 1;
  private float minVectorLength = 1;
  private Vector3 currentVector = Vector3.zero;

  public void ShowValueText(bool toShow)
  {
    displayValues.gameObject.SetActive(toShow);
  }

  public void SetVectorName(string name) {

    vectorName = name;
  }

  public void SetScaleMultiplier(float multiplier) {

    screenToScaleMultiplier = multiplier;
  }

  public void SetMinVectorLength(float length) {

    minVectorLength = length;
  }

  public Vector3 Position {

    get {
      return transform.localPosition;
    }
  }

  public Vector3 Vector {

    get {
      return currentVector;
    }
  }

  public void SetPosition(DirectionEnum direction, float value) {

    Vector3 currentPos = transform.localPosition;
    transform.localPosition = CalculatePosition(currentPos, direction, value);
  }

  public void SetPosition(Vector2 pos) {

    transform.localPosition = pos;
  }

  public void SetVectorLine(Vector2 vector) {

    SetVectorLine(vector, Vector2.zero);
  }

  public void SetVectorLine(Vector2 vector, Vector2 startPoint) {

    transform.localPosition = startPoint / multiplier;

    Vector2 localPos = transform.localPosition;
    currentVector = vector / multiplier;

    if(currentVector == Vector3.zero) {

      currentVector = new Vector3(minVectorLength, 0, 0);
    }

    vectorLine.PositionLine(Vector2.zero, currentVector);

    wirkungslinie.localRotation = vectorLine.transform.localRotation;
  }

  public override void RenderPosition() {

    List<string> texts = new List<string>();

    if(vectorName != null) {

      float xPos = transform.localPosition.x * multiplier.x;
      float yPos = transform.localPosition.y * multiplier.y;

      texts.Add(SymbolHelper.GetSymbol(vectorName) + " (" + Math.Round(xPos, 2) + " | " + Math.Round(yPos, 2) + " )");

      float xVector = currentVector.x * multiplier.x;
      float yVector = currentVector.y * multiplier.y;

      texts.Add(SymbolHelper.GetSymbol(vectorName) + " (" + Math.Round(xVector, 2) + " | " + Math.Round(yVector, 2) + " )");
    }

    displayValues.text = String.Join("\n", texts);
  }

  public override void SetEditable(bool isEditable) {

    attackPoint.SetEditable(isEditable);
    attackPoint.transform.localScale = (isEditable == true) ? Vector3.one * 1.5f : Vector3.one;
    attackPoint.gameObject.SetActive(isEditable);
  }

  protected override void Start() {

    base.Start();
    vectorLine.ConfigureLine(vectorLineSO);
  }
}
