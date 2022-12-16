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

public static class Vector3Extension {

  /// <summary>
  /// Inverts a scale vector by dividing 1 by each component
  /// </summary>
  public static Vector3 Invert(this Vector3 vec) {
    return new Vector3(1 / vec.x, 1 / vec.y, 1 / vec.z);
  }

  /// <summary>
  /// Rounds Vector3.
  /// </summary>
  /// <param name="vector3"></param>
  /// <param name="decimalPlaces"></param>
  /// <returns></returns>
  public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2) {
    float multiplier = 1;
    for(int i = 0; i < decimalPlaces; i++) {
      multiplier *= 10f;
    }
    return new Vector3(
        Mathf.Round(vector3.x * multiplier) / multiplier,
        Mathf.Round(vector3.y * multiplier) / multiplier,
        Mathf.Round(vector3.z * multiplier) / multiplier);
  }

  public static Vector3 Round(this Vector3 vector3, MidpointRounding rounding) {

    return new Vector3(
      (float) Math.Round(vector3.x, rounding),
      (float) Math.Round(vector3.y, rounding),
      (float) Math.Round(vector3.z, rounding)
    );
  }
}