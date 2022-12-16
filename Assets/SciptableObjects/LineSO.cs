#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Line")]
public class LineSO : ScriptableObject {

  public Color lineColor;
  public GameObject LinePrefab;
  public HandleTypeSO startHandle;
  public HandleTypeSO endHandle;
  public float lineWidth;
}
