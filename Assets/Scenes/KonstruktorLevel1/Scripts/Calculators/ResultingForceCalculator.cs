#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using MathUnits.Physics.Values;

public class ResultingForceCalculator : DirectionalCalculatorAbstract {

  public ResultingForceCalculator(DirectionEnum direction): base(direction) { }

  public override ScalarValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = GetParameters();

    try {

      ScalarValue result = calculationParams.Values.Aggregate(null, (ScalarValue agg, PhysicalValue current) => agg + (current as ScalarValue));

      return result;
    } catch(Exception) {

      return CalculationHelper.CreateScalarError();
    }
  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {
    throw new System.NotImplementedException();
  }
}
