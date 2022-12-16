#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUnits.Physics.Values;
using System;

public class AngleOnXAxisCalculator : CalculatorAbstract<ScalarValue> {

  public override ScalarValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = GetParameters();

    calculationParams.TryGetValue("Rx", out PhysicalValue Rx);
    calculationParams.TryGetValue("Ry", out PhysicalValue Ry);

    ScalarValue rxScalar = Rx as ScalarValue;
    ScalarValue ryScalar = Ry as ScalarValue;

    try {

      ScalarValue result = CalculationHelper.Arctan(rxScalar / ryScalar);

      return result;
    } catch(Exception) {

      return CalculationHelper.CreateScalarError();
    }    
  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {
    throw new System.NotImplementedException();
  }
}
