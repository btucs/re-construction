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

public class WeightForceCalculator : CalculatorAbstract<ScalarValue> {

  private readonly ScalarValue gAcceleration = new ScalarValue(new StandardGravity(), 1);

  public override ScalarValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = GetParameters();

    calculationParams.TryGetValue("m", out PhysicalValue m);

    try {

      ScalarValue result = (m as ScalarValue) * gAcceleration;

      return result;
    } catch(Exception) {

      return CalculationHelper.CreateScalarError();
    }
  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {
    throw new NotImplementedException();
  }
}
