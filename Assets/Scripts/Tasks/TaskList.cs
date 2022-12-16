#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;


public class TaskList {

  private Queue<Task> tasks = new Queue<Task>();

  private Task currentTask;

  public void AddTask(Task task) {

    tasks.Enqueue(task);
  }

  public string[] GetCurrentDescription() {

    if (tasks.Count == 0 && currentTask == null) {

      return new string[0];
    }

    if (currentTask == null) {

      currentTask = tasks.Dequeue();
    }

    return currentTask.GetDescription();
  }

  public TaskAnalyzerResultAbstract CheckCurrentTask() {

    if (currentTask == null) {

      currentTask = tasks.Dequeue();
    }

    TaskAnalyzerResultAbstract result = currentTask.Validate();

    if (typeof(PositiveTaskAnalyzerResult) == result.GetType()) {

      currentTask = tasks.Count > 0 ? tasks.Dequeue() : null;
    }

    return result;
  }

  public bool IsCompleted() {

    return tasks.Count == 0 && currentTask == null;
  }
}
