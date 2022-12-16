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
using MathUnits.Physics.Values;
using MathUnits.Physics.Units;
using FullSerializer;
using Sirenix.OdinInspector;

[Serializable]
public class ReplacementModelMapping {

  public string variableName;
  public ReplacementModelType selectedType;
  public ReplacementModelType expectedType;
  public ReplacementModelType variableType;
}

[Serializable]
[fsObject(Converter = typeof(MathMagnitudeConverter))]
public class MathMagnitude {

  public TaskVariable Value;
  // InfoBox has to be on another field because TaskVariable is an abstract class and is not processed by unity
  [InfoBox(@"@"" Value: "" + (this.Value != null ? this.Value.ToString() : ""null"")")]
  public GameObject Source;
  public DirectionEnum Direction;

  public TaskDataSO taskData;
  public TaskObjectSO taskObject;

  public ReplacementModelMapping[] replacementModelMapping = new ReplacementModelMapping[0];

  public MathVariableType GetValueType()
  {
    if(replacementModelMapping != null && replacementModelMapping.Length > 0)
    {
      return MathVariableType.replacementmodel;
    }

    if(Value is TaskInputVariable)
    {
      TaskInputVariable inputVal = Value as TaskInputVariable;
      
      if(inputVal.type == TaskVariableType.Vector)
      {
        if(inputVal.startPointText == null || inputVal.startPointText.Length == 0){
          return MathVariableType.point;
        }
        else {
          //VectorValue forceValue = inputVal.GetVectorValue();
          //if(forceValue != null && forceValue.Unit is Newton){
          //  return MathVariableType.forcevector;
          //}
          //else {
            return MathVariableType.vector;
          //}
        }
      }

      if(inputVal.type == TaskVariableType.Scalar)
      {
        ScalarValue scalarValue = inputVal.GetScalarValue();
        if(scalarValue != null && scalarValue.Unit is LengthUnit) {
          return MathVariableType.length;
        } 
        else if (scalarValue != null && scalarValue.Unit is MassUnit){
          return MathVariableType.mass;
        } 
        else if (scalarValue != null && scalarValue.Unit is Newton){
          return MathVariableType.force;
        }
        else if (scalarValue != null && scalarValue.Unit is PlaneAngleUnit){
          return MathVariableType.angle;
        }
        else {
          return MathVariableType.error;
        }
      }
    }

    return MathVariableType.none;
  }

  public enum MathVariableType
  {
    point, vector, length, mass, forcevector, force, replacementmodel, angle, error, none
  }
}
