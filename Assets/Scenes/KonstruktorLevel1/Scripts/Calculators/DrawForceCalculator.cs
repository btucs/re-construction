#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using MathUnits.Physics.Values;
using MathUnits.Physics.Units;
using Mathematics.LA;

public class DrawForceCalculator : CalculatorAbstract<VectorValue> {

  public float scale = 100f;

  private readonly Tuple<string, CalculatorParameterType>[] expectedParameterTypes = new Tuple<string, CalculatorParameterType>[] {
    Tuple.Create("P", CalculatorParameterType.Point),
    Tuple.Create("alpha", CalculatorParameterType.Angle),
    Tuple.Create("F", CalculatorParameterType.Force),
  };

  public override VectorValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = GetParameters();

    calculationParams.TryGetValue("P", out PhysicalValue P);
    calculationParams.TryGetValue("alpha", out PhysicalValue alpha);
    calculationParams.TryGetValue("F", out PhysicalValue F);

    VectorValue PVector = P as VectorValue;
    ScalarValue alphaScalar = alpha as ScalarValue;
    ScalarValue FScalar = F as ScalarValue;
    

    try {

      ScalarValue xScalar = FScalar * CalculationHelper.Cos(alphaScalar) / scale;
      ScalarValue yScalar = FScalar * CalculationHelper.Sin(alphaScalar) / scale;

      VectorValue endPos = new VectorValue(new Dimensionless(), new Vector3D(xScalar.Value, yScalar.Value, 0));

      return VectorValueHelper.Round(endPos, 2);
    } catch(Exception) {

      return CalculationHelper.CreateVectorError();
    }
  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {

    return expectedParameterTypes;
  }
}
