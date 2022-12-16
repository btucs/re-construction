#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier {

  public static Vector2 EvaluateLinear(Vector2 a, Vector2 b, float t) {

    return a + t * (b - a);
  }

  public static Vector2 EvaluateQuadratic(Vector2 a, Vector2 b, Vector2 c, float t) {

    Vector2 p0 = Vector2.Lerp(a, b, t);
    Vector2 p1 = Vector2.Lerp(b, c, t);

    return Vector2.Lerp(p0, p1, t);
  }

  public static Vector2 EvaluateCubic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t) {

    Vector2 p0 = EvaluateQuadratic(a, b, c, t);
    Vector2 p1 = EvaluateQuadratic(b, c, d, t);

    return Vector2.Lerp(p0, p1, t);
  }
}
