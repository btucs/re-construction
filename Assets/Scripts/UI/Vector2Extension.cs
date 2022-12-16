#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;

public static class Vector2Extension
{
  public static Vector2Int ToInt2(this Vector2 v)
	{
		return new Vector2Int((int)v.x, (int)v.y);
	}

  /// <summary>
  /// Rounds Vector3.
  /// </summary>
  /// <param name="vector3"></param>
  /// <param name="decimalPlaces"></param>
  /// <returns></returns>
  public static Vector2 Round(this Vector2 vector2, int decimalPlaces = 2) {
    float multiplier = 1;
    for(int i = 0; i < decimalPlaces; i++) {
      multiplier *= 10f;
    }

    return new Vector2(
      Mathf.Round(vector2.x * multiplier) / multiplier,
      Mathf.Round(vector2.y * multiplier) / multiplier
    );
  }
}
