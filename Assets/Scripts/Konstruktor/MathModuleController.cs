#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using MathUnits.Physics.Values;
using UnityEngine;

public class MathModuleController : MonoBehaviour
{
  public MathFunctionType type;
  public int priority;
  public GameObject iconGraphicObj;
  private MathVarHandler valueA;
  private MathVarHandler valueB;

  public void Setup(MathFunctionType moduleType, int konstruktorPosition) {
    type = moduleType;
    priority = 100 - konstruktorPosition;
    if(type == MathFunctionType.multiply || type == MathFunctionType.devide) {
      priority = priority * 10;
    }
  }

  public void AssignValueLinks(MathVarHandler _a, MathVarHandler _b) {
    valueA = _a;
    valueB = _b;
  }

  public ScalarValue ExecuteCalculation() {
    ScalarValue result = null;
    if(valueA != null && valueB != null) {
      ScalarValue valA = valueA.ReturnValue();
      ScalarValue valB = valueB.ReturnValue();
      //Do the math with Values A and B
      //Debug.Log("MathFUnctionType is: " + type);
      switch(type) {
        case MathFunctionType.multiply:
          result = valA * valB;
          //Debug.Log("value 1: " + valA.Value + " - value 2:" + valB.Value + " - result: " + result.Value);
          break;

        case MathFunctionType.addition:
          result = ScalarAPlusB(valA, valB);
          break;

        case MathFunctionType.subtract:
          result = ScalarAMinusB(valA, valB);
          break;

        case MathFunctionType.devide:
          result = valA / valB;
          break;

        default:
          result = valueA.currentValue;
          break;
      }
      //Send Result to Handler A and B, so the other modules continue with the result.
      SendResultToLinkedVars(result);
    } else {
      Debug.Log("Error: This module can not calculate because it does not have two values connected");
    }
    return result;
  }

  public MathModuleController ResetResults() {

    if(valueA != null) {

      valueA.Reset();
    }

    if(valueB != null) {

      valueB.Reset();
    }

    return this;
  }

  private ScalarValue ScalarATimesB(ScalarValue a, ScalarValue b) {
    return a;
  }

  private ScalarValue ScalarAPlusB(ScalarValue a, ScalarValue b) {
    ScalarValue result = a;
    double f1 = a.Value;
    double f2 = b.Value;
    result.Value = f1 + f2;
    //Debug.Log("value 1: " + f1 + " - value 2:" + f2 + " - result: " + result.Value);
    return result;
  }

  private ScalarValue ScalarAMinusB(ScalarValue a, ScalarValue b) {
    ScalarValue result = a;
    double f1 = a.Value;
    double f2 = b.Value;
    result.Value = f1 - f2;
    //Debug.Log("value 1: " + f1 + " - value 2:" + f2 + " - result: " + result.Value);
    return result;
  }

  private void SendResultToLinkedVars(ScalarValue resultScalar) {
    //Debug.Log("sending Value: " + resultScalar.Value);
    IntermediateResultHandler zwischenErgebnis;
    if(valueA.linkedResult != null && valueB.linkedResult == null) {
      valueA.linkedResult.linkedVars.Add(valueB);
      zwischenErgebnis = valueA.linkedResult;
    } else if(valueB.linkedResult != null && valueA.linkedResult == null) {
      valueB.linkedResult.linkedVars.Add(valueA);
      zwischenErgebnis = valueB.linkedResult;
    } else if(valueA.linkedResult != null && valueB.linkedResult != null) {
      valueA.linkedResult.MergeWith(valueB.linkedResult);
      zwischenErgebnis = valueA.linkedResult;
    } else {
      //Debug.Log("AAAAAAAAAAAAAAAAAAa Creating new Result");
      zwischenErgebnis = new IntermediateResultHandler();
      zwischenErgebnis.linkedVars.Add(valueA);
      zwischenErgebnis.linkedVars.Add(valueB);
    }
    zwischenErgebnis.Value = new ScalarValue(resultScalar.Unit, resultScalar.Value);
    //Debug.Log("BBBBBBBBBBBBBBBB Has send Value: " + zwischenErgebnis.Value);
    zwischenErgebnis.UpdateAllLinkedVars();
  }
}

public enum MathFunctionType
{
  multiply, addition, subtract, devide, equals
}