﻿#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using System.Collections;

public class Vector3Prerequisite: PrerequisiteAbstract {

  public Vector3 Value {
    get; private set;
  }

  public Vector3Prerequisite(Vector3 value) {

    this.Value = value;
  }
}
