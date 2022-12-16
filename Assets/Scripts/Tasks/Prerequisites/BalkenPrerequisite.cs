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


public class BalkenPrerequisite: PrerequisiteAbstract {

  public Vector3 StartPos {
    get; private set;
  }
  public Vector3 EndPos {
    get; private set;
  }
  public LineSO LineSO {
    get; private set;
  }

  public BalkenPrerequisite(Vector3 startPos, Vector3 endPos, LineSO lineSO) {

    this.StartPos = startPos;
    this.EndPos = endPos;
    this.LineSO = lineSO;
  }
}