#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using MathUnits.Physics.Values;
using MathUnits.Physics.Units;

public static class ScalarValueHelper {

  public static ScalarValue Round(ScalarValue value, int decimals) {

    if(double.IsNaN(value.Value)) {

      return value;
    }

    ScalarValue rounded = new ScalarValue(value.Unit, Math.Round(value.Value, decimals, MidpointRounding.AwayFromZero));

    return rounded;
  }
}