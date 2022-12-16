#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using MathUnits.Physics.Values;
using MathUnits.Physics.Units;

public class DrawEquilibriumCalculator : CalculatorAbstract<VectorValue> {

  private readonly Tuple<string, CalculatorParameterType>[] expectedParameterTypes = new Tuple<string, CalculatorParameterType>[] {
    Tuple.Create("F1", CalculatorParameterType.Vector),
  };

  public override VectorValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = GetParameters();

    calculationParams.TryGetValue("F1", out PhysicalValue F1);

    VectorValue F1Vector = F1 as VectorValue;

    try {

      return F1Vector * -1;
    } catch(Exception) {

      return CalculationHelper.CreateVectorError();
    }

  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {

    return expectedParameterTypes;
  }
}