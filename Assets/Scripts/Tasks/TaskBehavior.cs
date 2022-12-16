#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBehavior : MonoBehaviour {

  public Vector3[] prerequisites;
  public string[] description;
  public string[] successMessage;

  private Task task;

  public void Awake() {

    task = new Task();
    task.SetDescription(description);
    task.SetSuccessMessage(successMessage);

    PrerequisiteFactory.CreateAndAddFromArray(task, prerequisites);
    TaskAnalyzerBehavior[] behaviors = GetComponents<TaskAnalyzerBehavior>();
    for (int i = 0; i < behaviors.Length; i++) {

      task.AddTaskAnalyzer(behaviors[i].GetAnalyzer());
    }
  }

  public Task GetTask() {

    return task;
  }
}
