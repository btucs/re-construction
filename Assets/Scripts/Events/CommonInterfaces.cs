#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;

public struct TwoFingerEventData {
  public Vector2 firstTouch;
  public Vector2 secondTouch;
}

public class DragEventData {

  public float delta;
  public Vector3 direction;

  public DragEventData() {}
  public DragEventData(float delta, Vector3 direction) {

    this.delta = delta;
    this.direction = direction;
  }

  public static DragEventData Avg(
    DragEventData left,
    DragEventData right
  ) {

    return new DragEventData(
      (left.delta + right.delta) / 2,
      (left.direction + right.direction) / 2
    );
  }
}