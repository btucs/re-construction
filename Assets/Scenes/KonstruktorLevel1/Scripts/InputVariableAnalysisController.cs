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
using MathUnits.Physics.Values;
using Mathematics.LA;

public class InputVariableAnalysisController : MonoBehaviour {

  [Required]
  public GameObject variableHolderPanel;
  [Required]
  public GameObject descriptionPanel;
  [Required]
  public TextMeshProUGUI valueInfoText;
  [Required]
  public TextMeshProUGUI shortDescriptionText;
  [Required]
  public TextMeshProUGUI definitionText;

  [Required]
  public GameObject analysisPanel;

  public KonstruktorNavTitle breadcrum;

  private InventoryItem currentItemCopy;

  public void SetDisplayContent(InventoryItem item) {

    if(currentItemCopy != null) {

      Destroy(currentItemCopy.gameObject);
      currentItemCopy = null;
    }

    currentItemCopy = Instantiate(item, variableHolderPanel.transform);
    currentItemCopy.enableDrag = false;
    currentItemCopy.OnTapEvent.AddListener(HandleTap);
    
    RectTransform currentItemTransform = (RectTransform)currentItemCopy.transform;
    currentItemTransform.sizeDelta = new Vector2(80, 80);

    TaskInputVariable input = (TaskInputVariable)item.magnitude.Value;

    if(input.type == TaskVariableType.Scalar) {

      valueInfoText.text = SymbolHelper.GetSymbol(input.textMeshProName) + " = " + input.textMeshProValue.Replace('.', ',');
    } else {

      VectorValue vectorValue = input.GetVectorValue();
      Vector3D vector = vectorValue.Value;

      valueInfoText.text = SymbolHelper.GetSymbol(input.textMeshProName) + " = " + vector.X1.ToString().Replace('.', ',') + ";" + vector.X2.ToString().Replace('.', ',');

      VectorValue startPoint = input.GetStartPoint();
      if(VectorValueHelper.IsNaN(startPoint) == false) {

        Vector3D startPointValue = startPoint.Value;
        valueInfoText.text += "\nP<sub>start</sub> = " + startPointValue.X1.ToString().Replace('.', ',') + ";" + startPointValue.X2.ToString().Replace('.', ',');
      }

    }
    
    shortDescriptionText.text = input.shortDescription;
    definitionText.text = input.definition;

    if (breadcrum != null)
    {
      breadcrum.SetPrevLayerText();
    }
  }

  private void HandleTap(InventoryItem item) {

    gameObject.SetActive(false);
    analysisPanel.SetActive(false);
  }
}
