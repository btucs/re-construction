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

public class VectorCalculator : CalculatorAbstract<VectorValue> {

  private readonly Tuple<string, CalculatorParameterType>[] expectedParameterTypes = new Tuple<string, CalculatorParameterType>[] {
    Tuple.Create("p1", CalculatorParameterType.Point),
    Tuple.Create("p2", CalculatorParameterType.Point),
  };

  public override VectorValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = GetParameters();

    calculationParams.TryGetValue("p1", out PhysicalValue p1);
    calculationParams.TryGetValue("p2", out PhysicalValue p2);

    VectorValue p1Vector = p1 as VectorValue;
    VectorValue p2Vector = p2 as VectorValue;

    try {

      return VectorValueHelper.Round(p2Vector - p1Vector, 2);
    } catch(Exception) {

      return CalculationHelper.CreateVectorError();
    }
  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {

    return expectedParameterTypes;
  }
}
