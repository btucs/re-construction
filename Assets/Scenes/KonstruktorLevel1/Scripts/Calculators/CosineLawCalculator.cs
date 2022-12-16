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
using MathUnits.Physics.Units;

public class CosineLawCalculator : CalculatorAbstract<ScalarValue> {

  private readonly Tuple<string, CalculatorParameterType>[] expectedParameterTypes = new Tuple<string, CalculatorParameterType>[] {
    Tuple.Create("gamma", CalculatorParameterType.Angle),
    Tuple.Create("a", CalculatorParameterType.Length),
    Tuple.Create("b", CalculatorParameterType.Length),
  };

  public override ScalarValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = ReMapParamsToMatchTypes(GetParameters());

    calculationParams.TryGetValue("a", out PhysicalValue a);
    calculationParams.TryGetValue("b", out PhysicalValue b);
    calculationParams.TryGetValue("gamma", out PhysicalValue gamma);

    ScalarValue aScalar = a as ScalarValue;
    ScalarValue bScalar = b as ScalarValue;
    ScalarValue gammaScalar = gamma as ScalarValue;

    try {
      ScalarValue result = CalculationHelper.Sqrt(
        (aScalar ^ 2) + (bScalar ^ 2) - (2 * aScalar * bScalar * CalculationHelper.Cos(gammaScalar))
      );

      return result;
    } catch(Exception) {

      return CalculationHelper.CreateScalarError();
    }
  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {

    return expectedParameterTypes;
  }

  private Dictionary<string, PhysicalValue> ReMapParamsToMatchTypes(Dictionary<string, PhysicalValue> parameters) {

    List<PhysicalValue> values = parameters.Values.ToList();
    Dictionary<string, PhysicalValue> remapped = new Dictionary<string, PhysicalValue>();
    foreach(Tuple<string, CalculatorParameterType> tuple in expectedParameterTypes) {

      int foundIndex = values.FindIndex((PhysicalValue value) => IsMatchingUnit(tuple.Item2, value));
      if(foundIndex != -1) {

        remapped.Add(tuple.Item1, values[foundIndex]);
        values.RemoveAt(foundIndex);
      }
    }

    return remapped;
  }

  private bool IsMatchingUnit(CalculatorParameterType parameterType, PhysicalValue value) {

    switch(parameterType) {

      case CalculatorParameterType.Angle: return value.Unit is DimensionlessUnit || value.Unit is Degree;
      case CalculatorParameterType.Any: return value.Unit is Unit;
      case CalculatorParameterType.Length: return value.Unit is LengthUnit;
      case CalculatorParameterType.Mass: return value.Unit is MassUnit;
      // acceleration is a derived unit
      //case CalculatorParameterType.Acceleration: return typeof()
    }

    return false;
  }
}
