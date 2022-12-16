#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUnits.Physics.Values;

public enum CalculatorParameterType
{
  Any,
  Acceleration,
  Mass,
  Length,
  Angle,
  Point,
  Force,
  Vector,
}

public class CalculatorException : Exception
{
  public CalculatorException(string message): base(message) {}
}

public abstract class CalculatorAbstract<T> where T: PhysicalValue {

  protected Dictionary<string, PhysicalValue> parameters;

  public abstract T Calculate();

  public abstract Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes();

  public virtual void SetParameters(Dictionary<string, PhysicalValue> parameters) {

    this.parameters = parameters;
  }

  public virtual void SetParameters(Dictionary<string, MathMagnitude> parameters) {

    this.parameters = parameters.ToDictionary(
      (KeyValuePair<string, MathMagnitude> parameter) => parameter.Key,
      (KeyValuePair<string, MathMagnitude> parameter) => {
        TaskInputVariable input = (TaskInputVariable)parameter.Value.Value;

        return input.type == TaskVariableType.Scalar ? input.GetScalarValue() as PhysicalValue : input.GetVectorValue() as PhysicalValue;
      }
    );
  }
  
  protected Dictionary<string, PhysicalValue> GetParameters() {

    return parameters;
  }
}
