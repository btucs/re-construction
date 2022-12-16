#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using MathUnits.Physics.Values;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FormularVarController))]
public class MathVarHandler : MonoBehaviour
{
  public DropAreaUI connectedDropArea;
  public Canvas varCanvas;

  public ScalarValue currentValue;
  public IntermediateResultHandler linkedResult;

  public InventoryItem droppedItem;

  private MathConstantHandler multipliedBy;
  private Canvas constantUIElement;
  private int exponent = 1;
  private TrigonometryType trigonometry = TrigonometryType.none;
  //private Unit[] acceptedValueTypes;


  public void SetSortingOrder(int newIndex)
  {
    varCanvas.sortingOrder = newIndex;
    if(multipliedBy != null) 
    {
      Canvas constCanvas = multipliedBy.valueDisplay.GetComponent<Canvas>();
      constCanvas.sortingOrder = newIndex;
    }
  }

  public void SetUp(string varString) {
    connectedDropArea.itemDropped.AddListener(UpdateValueOnDrop);
    //set expectedValueType depending on FormularEntry
    CheckForExponent(varString);
    CheckForTrigonometry(varString);
  }

  public void SetConstant(MathConstantHandler constRef) {
    multipliedBy = constRef;
  }

  public void UpdateValueOnDrop(InventoryItem dropItem) {

    droppedItem = dropItem;

    if(dropItem.magnitude.Value is TaskInputVariable) {
      TaskInputVariable inputValue = dropItem.magnitude.Value as TaskInputVariable;
      currentValue = inputValue.GetScalarValue();
    }
    if(dropItem.magnitude.Value is TaskOutputVariable) {
      currentValue = null;
    }
    //Debug.Log("New Item Value is: " + (float)currentValue.Value);
  }

  public void CompareInputToExpectedUnit() {
    //get value from DropAreaUI
    //bool accepted = false;
    //for(int i = 0; i < acceptedValueTypes.Count; i++)
    //{
    //	if(currentValue.unit == acceptedValueTypes[i])
    //		accepted = true;
    //}

  }

  public ScalarValue ReturnValue() {
    ScalarValue result;
    if(linkedResult != null) {
      //Debug.Log("Linked Result Found: " + linkedResult.Value.Value.ToString());
      result = linkedResult.Value;
    } else {
      result = currentValue ^ exponent;

      if(multipliedBy != null) {
        result = result * multipliedBy.GetValue();
      }

      result.Value = CalculateTrigonometry(result.Value);
      currentValue = result;
    }
    //Debug.Log("Value: " + result.Value + " returned");
    return result;
  }

  public float GetExponentValue() {
    return (float)exponent;
  }

  public void UpdateValue() {
    //send new Value to all linked Vars
  }

  public void Reset() {

    linkedResult = null;
    UpdateValueOnDrop(droppedItem);
  }

  private void ResolveUnitFromString(string formularString) {
    //acceptedValueTypes.Clear();
    //if(formularString.Contains("m"))
    //{
    //	return value.Unit is MassUnit;
    //}
  }

  private void CheckForTrigonometry(string varString) {
    if(varString.Contains("-")) {
      if(varString.Contains("cos")) {
        trigonometry = TrigonometryType.cosinus;
      } else if(varString.Contains("sin")) {
        trigonometry = TrigonometryType.sinus;
      } else if(varString.Contains("tan")) {
        trigonometry = TrigonometryType.tangens;
      }
    }
  }

  private void CheckForExponent(string varString) {
    if(varString.Contains("^")) {
      string[] myStringArray = varString.Split('^');
      if(Int32.TryParse(myStringArray[1], out int j)) {
        exponent = j;
      } else {
        Console.WriteLine("String could not be parsed.");
      }
    }
  }

  private double CalculateTrigonometry(double valDouble) {
    double result = valDouble;
    float valFloat = (float)valDouble;

    switch(trigonometry) {
      case TrigonometryType.cosinus:
        result = (double)Mathf.Cos(valFloat * Mathf.Deg2Rad);
        break;

      case TrigonometryType.sinus:
        result = (double)Mathf.Sin(valFloat * Mathf.Deg2Rad);
        break;

      case TrigonometryType.tangens:
        result = (double)Mathf.Sin(valFloat * Mathf.Deg2Rad);
        break;

      default:
        result = valDouble;
        break;
    }

    return result;
  }
}

public enum TrigonometryType
{
  none, sinus, cosinus, tangens
}