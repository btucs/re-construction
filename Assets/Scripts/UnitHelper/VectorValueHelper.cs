#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUnits.Physics.Values;
using MathUnits.Physics.Units;
using Mathematics.LA;

public static class VectorValueHelper {

  public static VectorValue Convert(Vector2 vector) {

    return new VectorValue(new Dimensionless(), new Vector3D(vector.x, vector.y, 0));
  }

  public static Vector2 Convert(VectorValue vector) {

    float x = System.Convert.ToSingle(vector.Value.X1);
    float y = System.Convert.ToSingle(vector.Value.X2);

    if(float.IsNaN(x)) {

      x = 0;
    }

    if(float.IsNaN(y)) {

      y = 0;
    }

    return new Vector2(x, y);
  }

  public static string ToString(Vector2 vector) {

    return Convert(vector).ToString(CultureInfo.InvariantCulture);
  }

  public static string ToString(VectorValue vector) {

    return vector.ToString(CultureInfo.InvariantCulture);
  }

  public static VectorValue Round(VectorValue value, int decimals) {

    Vector3D rounded = new Vector3D() {
      X1 = Math.Round(value.Value.X1, decimals, MidpointRounding.AwayFromZero),
      X2 = Math.Round(value.Value.X2, decimals, MidpointRounding.AwayFromZero),
      X3 = Math.Round(value.Value.X3, decimals, MidpointRounding.AwayFromZero),
    };

    return new VectorValue(value.Unit, rounded);
  }

  public static bool IsNaN(VectorValue value) {

    return double.IsNaN(value.Value.X1) || double.IsNaN(value.Value.X2) || double.IsNaN(value.Value.X3);
  }
}