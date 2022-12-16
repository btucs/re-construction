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

public class PythagorasCalculator : CalculatorAbstract<ScalarValue> {

  private Tuple<string, CalculatorParameterType>[] expectedParameterTypes = new Tuple<string, CalculatorParameterType>[] {
    Tuple.Create("a", CalculatorParameterType.Length),
    Tuple.Create("b", CalculatorParameterType.Length),
  };

  public override ScalarValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = GetParameters();

    calculationParams.TryGetValue("a", out PhysicalValue a);
    calculationParams.TryGetValue("b", out PhysicalValue b);

    ScalarValue aScalar = a as ScalarValue;
    ScalarValue bScalar = b as ScalarValue;

    try {

      ScalarValue result = CalculationHelper.Sqrt((aScalar ^ 2) + (bScalar ^ 2));

      return result;
    } catch(Exception) {

      return CalculationHelper.CreateScalarError();
    }
  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {

    return expectedParameterTypes;
  }
}
