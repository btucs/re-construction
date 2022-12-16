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

public class CommandInvoker: MonoBehaviour {

  private Stack<Command> undoStack = new Stack<Command>();
  private Stack<Command> redoStack = new Stack<Command>();

  public ReactiveProperty<bool> canUndo = new ReactiveProperty<bool>(false);
  public ReactiveProperty<bool> canRedo = new ReactiveProperty<bool>(false);

  public void ExecuteCommand(Command command) {

    undoStack.Push(command);
    command.Execute();
    canUndo.Value = true;

    CleanupResources(redoStack);
    canRedo.Value = false;
  }

  public bool CanUndo() {

    return undoStack.Count > 0;
  }

  public bool CanRedo() {

    return redoStack.Count > 0;
  }

  public void Undo() {

    int stackCount = undoStack.Count;
    if (stackCount > 0) {

      Command command = undoStack.Pop();
      command.Undo();
      redoStack.Push(command);

      canUndo.Value = stackCount - 1 > 0 ? true : false;
      canRedo.Value = true;
    }
  }

  public void Redo() {

    int stackCount = redoStack.Count;
    if (stackCount > 0) {

      Command command = redoStack.Pop();
      command.Execute();
      undoStack.Push(command);

      canRedo.Value = stackCount - 1 > 0 ? true : false;
      canUndo.Value = true;
    }
  }

  private void CleanupResources(Stack<Command> stackToCleanup) {

    while (stackToCleanup.Count > 0) {

      Command command = stackToCleanup.Pop();
      command.CleanupResources();
    }
  }
}
