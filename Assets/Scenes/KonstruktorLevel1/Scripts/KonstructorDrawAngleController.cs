#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;

public class KonstructorDrawAngleController : KonstructorDrawEntityAbstract {

  [Required]
  public KonstructorDrawPointController attackPoint;
  [Required]
  public KonstructorDrawPointController amountPoint;
  [Required]
  public Transform wirkungslinie;
  public RectTransform angleIndicator;

  public TextMeshProUGUI displayValues;

  public PositionEvent onMove = new PositionEvent();

  private string variableName;
  private string angleName;
  private string forceName;
  private float screenToScaleMultiplier = 1;
  private float minForcePosition = 0;

  public void ShowValueText(bool toShow)
  {
    displayValues.gameObject.SetActive(toShow);
  }

  public void ShowHighlights(bool toShow)
  {
    attackPoint.ShowHighlight(toShow);
    amountPoint.ShowHighlight(toShow);
  }

  public void SetPointName(string name) {

    variableName = name;
    attackPoint.gameObject.SetActive(true);    
  }

  public void SetAngleName(string name) {

    angleName = name;
    amountPoint.gameObject.SetActive(true);
    if(amountPoint.transform.localPosition == Vector3.zero) {

      SetForcePosition(minForcePosition);
    }
  }

  public void SetForceName(string name) {

    forceName = name;
    amountPoint.gameObject.SetActive(true);
    if(amountPoint.transform.localPosition == Vector3.zero) {

      SetForcePosition(minForcePosition);
    }
  }

  public void SetScaleMultiplier(float multiplier) {

    screenToScaleMultiplier = multiplier;
  }

  public void SetMinForcePosition(float pos) {

    minForcePosition = pos;
  }

  public Vector3 Position {
    get {
      return transform.localPosition;
    }
  }

  public double Force {
    get {
      return CalculateForceAmount();
    }
  }

  public float Angle {
    get;
    private set;
  } = 0;  

  public override void RenderPosition() {

    List<string> texts = new List<string>();

    if(variableName != null) {

      float xPos = transform.localPosition.x * multiplier.x;
      float yPos = transform.localPosition.y * multiplier.y;
      //attackPoint.text.text = SymbolHelper.GetSymbol(variableName) + " = (" + xPos + " | " + yPos + " )";
      texts.Add(SymbolHelper.GetSymbol(variableName) + " (" + Math.Round(xPos, 2) + " | " + Math.Round(yPos, 2) + " )");
    }

    if(angleName != null) {

      texts.Add(SymbolHelper.GetSymbol(angleName) + " = " + Angle + "°");
    }

    if(forceName != null) {

      double value = CalculateForceAmount();

      //amountPoint.text.text = SymbolHelper.GetSymbol(forceName) + " " + value.ToString(CultureInfo.InstalledUICulture) + " N";
      texts.Add(SymbolHelper.GetSymbol(forceName) + " = " + value.ToString(CultureInfo.InstalledUICulture) + " N");
    }

    displayValues.text = String.Join("\n", texts);
  }

  public override void SetEditable(bool isEditable) {

    attackPoint.SetEditable(isEditable);
    attackPoint.ShowHighlight(isEditable);
    angleIndicator.gameObject.SetActive(false);
  }

  public void SetAngleEditable(bool isEditable) {

    amountPoint.SetEditable(isEditable);
    amountPoint.ShowHighlight(isEditable);
    amountPoint.transform.localScale = (isEditable == true) ? Vector3.one * 1.5f : Vector3.one;
    angleIndicator.gameObject.SetActive(isEditable);

    float forceDistance = amountPoint.transform.localPosition.x;
    float radius = forceDistance * 2;

    angleIndicator.sizeDelta = new Vector2(radius, radius);

    Image indicatorImage = angleIndicator.GetComponent<Image>();
    indicatorImage.material.SetFloat("Radius", radius);
    indicatorImage.material.SetColor("_Color", indicatorImage.color);
  }

  public void SetForceEditable(bool isEditable) {

    amountPoint.SetEditable(isEditable);
    amountPoint.ShowHighlight(isEditable);
    amountPoint.transform.localScale = (isEditable == true) ? Vector3.one * 1.5f : Vector3.one;
    angleIndicator.gameObject.SetActive(false);
  }

  public void SetPosition(DirectionEnum direction, float value) {

    Vector3 currentPos = transform.localPosition;
    transform.localPosition = CalculatePosition(currentPos, direction, value);
    onMove?.Invoke(transform.localPosition);
  }

  public void SetPosition(Vector3 pos) {

    transform.localPosition = pos;
    onMove?.Invoke(transform.localPosition);
  }

  public void SetForcePosition(float pos) {

    pos = Mathf.Max(minForcePosition, pos);

    amountPoint.transform.localPosition = new Vector3(pos, 0, 0);
  }

  public void SetAnglePosition(Vector2 localPos) {
  
    Vector2 globalPos = amountPoint.transform.TransformVector(localPos);
    
    float angle = Vector2.Angle(Vector2.right, globalPos);
    
    if(globalPos.y < 0) {

      angle *= -1;
    }

    angle = Mathf.RoundToInt(angle);

    Angle = angle;

    wirkungslinie.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
  }

  private double CalculateForceAmount() {

    float distance = Vector2.Distance(attackPoint.transform.localPosition, amountPoint.transform.localPosition);
    double value = Math.Round((distance * screenToScaleMultiplier), 1, MidpointRounding.AwayFromZero);

    return value;
  }
}
