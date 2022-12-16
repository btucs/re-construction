#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using System.Collections;

public static class PrerequisiteFactory {

  public static Task CreateAndAdd(Task task, Vector3 value) {

    task.AddPrerequisite(new Vector3Prerequisite(value));

    return task;
  }

  public static Task CreateAndAdd(Task task, Vector3 startPos, Vector3 endPos, LineSO lineSO) {

    task.AddPrerequisite(new BalkenPrerequisite(startPos, endPos, lineSO));

    return task;
  }

  public static Task CreateAndAddFromArray(Task task, Vector3[] values) {

    for (int i = 0; i < values.Length; i++) {

      task.AddPrerequisite(new Vector3Prerequisite(values[i]));
    }

    return task;
  }
}
