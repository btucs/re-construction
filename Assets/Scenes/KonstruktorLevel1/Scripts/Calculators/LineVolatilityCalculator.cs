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
using Sirenix.OdinInspector;

public class LineVolatilityCalculator : CalculatorAbstract<VectorValue>
{

  private readonly Tuple<string, CalculatorParameterType>[] expectedParameterTypes = new Tuple<string, CalculatorParameterType>[] {
    Tuple.Create("F1", CalculatorParameterType.Vector),
  };

  /* Dummy calculator since the vector stays the same */
  public override VectorValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = GetParameters();

    calculationParams.TryGetValue("F1", out PhysicalValue F1);

    return F1 as VectorValue;
  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {

    return expectedParameterTypes;
  }
}
