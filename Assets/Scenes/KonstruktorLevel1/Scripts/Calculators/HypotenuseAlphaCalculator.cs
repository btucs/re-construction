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

public class HypotenuseAlphaCalculator : CalculatorAbstract<ScalarValue> {

  public Tuple<string, CalculatorParameterType>[] expectedParameterTypes = new Tuple<string, CalculatorParameterType>[] {
    Tuple.Create("alpha", CalculatorParameterType.Angle),
    Tuple.Create("b", CalculatorParameterType.Any),
  };

  public override ScalarValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = GetParameters();

    calculationParams.TryGetValue("alpha", out PhysicalValue alpha);
    calculationParams.TryGetValue("b", out PhysicalValue b);

    ScalarValue alphaScalar = alpha as ScalarValue;
    ScalarValue bScalar = b as ScalarValue;

    try {

      ScalarValue result = bScalar / CalculationHelper.Cos(alphaScalar);

      return result;
    } catch(Exception) {

      return CalculationHelper.CreateScalarError();
    }
  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {

    return expectedParameterTypes;
  }
}