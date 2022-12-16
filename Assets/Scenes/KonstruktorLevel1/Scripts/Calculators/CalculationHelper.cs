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
using Mathematics.LA;

public static class CalculationHelper {

  public static ScalarValue Sqrt(ScalarValue value) {

    return NthRoot(value, 2);
  }

  public static ScalarValue NthRoot(ScalarValue value, int exponent) {

    Unit targetUnit;

    if(value.Unit is DimensionlessUnit) {

      targetUnit = value.Unit;
    } else if(value.Unit is ExponentUnit exp && exp.Exponent % exponent == 0) {

      exp.Exponent = exp.Exponent / exponent;
      targetUnit = exp;
    } else {

      return CreateScalarError();
    }

    return new ScalarValue(targetUnit, Math.Pow(value.Value, 1.0 / exponent));
  }

  public static ScalarValue Cos(ScalarValue value) {

    ScalarValue rad = Deg2Rad(value);

    return new ScalarValue(new Dimensionless(), Math.Cos(rad.Value));
  }

  public static ScalarValue Sin(ScalarValue value) {

    ScalarValue rad = Deg2Rad(value);

    return new ScalarValue(new Dimensionless(), Math.Sin(rad.Value));
  }

  public static ScalarValue Arctan(ScalarValue value) {

    ScalarValue rad = Deg2Rad(value);

    return new ScalarValue(new Dimensionless(), Math.Atan(rad.Value));
  }

  public static ScalarValue CreateScalarError() {

    return new ScalarValue(new Dimensionless(), Double.NaN);
  }

  public static VectorValue CreateVectorError() {

    return new VectorValue(new Dimensionless(), Vector3D.NaN);
  }

  public static ScalarValue Deg2Rad(ScalarValue degrees) {

    return degrees * Math.PI / 180;
  }
}