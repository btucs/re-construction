#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class IEqualityComparerDragEventData : IEqualityComparer<DragEventData>
{

  public bool Equals(DragEventData x, DragEventData y) {

    return x.direction == y.direction && x.delta == y.delta;
  }

  public int GetHashCode(DragEventData obj) {

    return obj.GetHashCode();
  }
}
