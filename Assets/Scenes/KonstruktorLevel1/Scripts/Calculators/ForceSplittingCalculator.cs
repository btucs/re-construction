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

public class ForceSplittingCalculator : DirectionalCalculatorAbstract {

  public ForceSplittingCalculator(DirectionEnum direction): base(direction) { }

  public override ScalarValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = GetParameters();

    calculationParams.TryGetValue("F", out PhysicalValue F);
    calculationParams.TryGetValue("alpha", out PhysicalValue alpha);

    try {

      ScalarValue angle = (direction == DirectionEnum.X ?
        CalculationHelper.Cos(alpha as ScalarValue) :
        CalculationHelper.Sin(alpha as ScalarValue))
      ;

      ScalarValue FScalar = F as ScalarValue;

      ScalarValue result = FScalar.Value * angle;

      return result;
    } catch(Exception) {

      return CalculationHelper.CreateScalarError();
    }
  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {
    throw new System.NotImplementedException();
  }
}
