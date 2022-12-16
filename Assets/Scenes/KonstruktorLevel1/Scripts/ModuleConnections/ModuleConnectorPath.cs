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

[Serializable]
public class ModuleConnectorPath {

  [SerializeField, HideInInspector]
  public List<Vector2> points;

  public ModuleConnectorPath(Vector2 from, Vector2 to) {

    Vector2 center = (to - from) / 2;

    points = new List<Vector2> {
      from,
      from - (Vector2.left * center.x),
      to - (Vector2.right * center.x),
      to,
    };
  }

  public Vector2 this[int i] {

    get {
      return points[i];
    }
  }

  private int LoopIndex(int i) {

    return (i + points.Count) % points.Count;
  }

  public void MovePoint(int i, Vector2 pos) {

    Vector2 deltaMove = pos - points[i];
    points[i] = pos;

    if(i % 3 == 0) {

      if(i + 1 < points.Count) {

        points[i + 1] += deltaMove;
      }

      if(i - 1 >= 0) {

        points[i - 1] += deltaMove;
      }
    } else {

      bool nextPointIsAnchor = (i + 1) % 3 == 0;
      int correspondingControlIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
      int anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;

      if(correspondingControlIndex >= 0 && correspondingControlIndex < points.Count) {

        float dst = (points[anchorIndex] - points[correspondingControlIndex]).magnitude;
        Vector2 dir = (points[anchorIndex] - pos).normalized;
        points[correspondingControlIndex] = points[anchorIndex] + dir * dst;
      }
    }
  }

  public Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1) {

    List<Vector2> evenlySpacedPoints = new List<Vector2>();
    evenlySpacedPoints.Add(points[0]);
    Vector2 previousPoint = points[0];
    float dstSinceLastEvenPoint = 0;

    Vector2[] p = points.ToArray();
    float controlNetLength = Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
    float estimatedCurveLength = Vector2.Distance(p[0], p[3]) + controlNetLength / 2f;
    int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
    float t = 0;

    while(t <= 1) {

      t += 1f / divisions;
      Vector2 pointOnCurve = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], t);
      dstSinceLastEvenPoint += Vector2.Distance(previousPoint, pointOnCurve);

      while(dstSinceLastEvenPoint >= spacing) {

        float overshootDst = dstSinceLastEvenPoint - spacing;
        Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;
        evenlySpacedPoints.Add(newEvenlySpacedPoint);
        dstSinceLastEvenPoint = overshootDst;
        previousPoint = newEvenlySpacedPoint;
      }

      previousPoint = pointOnCurve;
    }

    if(evenlySpacedPoints[evenlySpacedPoints.Count - 1] != points[3]) {

      evenlySpacedPoints.Add(points[3]);
    }

    return evenlySpacedPoints.ToArray();
  }
}
