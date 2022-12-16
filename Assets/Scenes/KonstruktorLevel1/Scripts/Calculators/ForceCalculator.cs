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

public class ForceCalculator : CalculatorAbstract<ScalarValue> {

  private Tuple<string, CalculatorParameterType>[] expectedParameterTypes = new Tuple<string, CalculatorParameterType>[] {
    Tuple.Create("m", CalculatorParameterType.Mass),
    Tuple.Create("a", CalculatorParameterType.Acceleration),
  };

  public override ScalarValue Calculate() {

    Dictionary<string, PhysicalValue> calculationParams = GetParameters();

    calculationParams.TryGetValue("m", out PhysicalValue m);
    calculationParams.TryGetValue("a", out PhysicalValue a);

    ScalarValue mScalar = m as ScalarValue;
    ScalarValue aScalar = a as ScalarValue;

    try {

      ScalarValue result = mScalar * aScalar;

      try {

        bool isConvertable = Unit.Convertible(result.Unit, new Newton());
        if(isConvertable == true) {

          result.Unit = new Newton();
        }
      } catch(UnitsUnconvertibleException) {
        // ignore
      }

      return result;

    } catch(Exception) {

      return CalculationHelper.CreateScalarError();
    }    
  }

  public override Tuple<string, CalculatorParameterType>[] GetExpectedParameterTypes() {

    return expectedParameterTypes;
  }
}
 