#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using MathUnits.Physics.Values;
using Mathematics.LA;

public static class InventoryItemFactory
{

  public static InventoryItem Instantiate(
    GameObject inventoryItemPrefab,
    Canvas parentCanvas,
    MathMagnitude magnitude,
    bool isResult = false
  ) {
    GameObject instance = Object.Instantiate(inventoryItemPrefab, parentCanvas.transform);
    InventoryItem script = instance.GetComponent<InventoryItem>();

    script.magnitude = magnitude;
    Sprite sprite = magnitude.taskObject.objectThumbnail;
    if(magnitude.Value.icon != null) {
      sprite = magnitude.Value.icon;
    } else {
      VariableInfoSO varInfo = GameController.GetInstance().gameAssets.variableInfo;
      Sprite varIconSprite = varInfo.GetVariableIcon(magnitude.GetValueType());
      if(varIconSprite != null){
        sprite = varIconSprite;
      }
    }

    script.objectImage.sprite = sprite;

    //Set Text ----------->
    if(magnitude.Value is TaskInputVariable) {
      TaskInputVariable inputValue = magnitude.Value as TaskInputVariable;
      if(inputValue.type == TaskVariableType.Scalar) {
        script.variableName.text = inputValue.textMeshProValue;
        //ScalarValue scalarVal = inputValue.GetScalarValue();
        //script.variableName.text = scalarVal.Value.ToString();
        //script.variableUnitName.text = scalarVal.Unit.Symbol;
      } else {
        VectorValue value = inputValue.GetVectorValue();
        Vector3D vector = value.Value;
        script.variableName.text = vector.X1 + ";" + vector.X2;
      }
    } else if(magnitude.Value is TaskOutputVariable) {
      TaskOutputVariable outValue = magnitude.Value as TaskOutputVariable;
      script.variableName.text = "? " + outValue.textMeshProUnit;
    }
    //<--------

    script.isResult = isResult;

    return script;
  }

  public static InventoryItem Instantiate(
    GameObject inventoryItemPrefab,
    Canvas parentCanvas,
    TaskVariable taskVariable,
    TaskDataSO taskData,
    TaskObjectSO taskObject,
    bool isResult = false
  ) {

    MathMagnitude magnitude = new MathMagnitude() {
      Value = taskVariable,
      taskData = taskData,
      taskObject = taskObject
    };

    return Instantiate(inventoryItemPrefab, parentCanvas, magnitude, isResult);
  }
}
