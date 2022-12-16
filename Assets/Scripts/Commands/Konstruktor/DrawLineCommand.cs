#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class DrawLineCommand : Command {

  private DrawLine script;

  private GameObject currentLine;
  private GameObject toDestroy;

  public DrawLineCommand(
    DrawLine script,
    GameObject currentLine
  ) {

    this.script = script;
    this.currentLine = currentLine;
  }

  public override void Execute() {

    if (toDestroy != null) {

      currentLine = script.currentLine = toDestroy;
      currentLine.SetActive(true);
    }
  }

  public override void Undo() {

    currentLine.SetActive(false);
    toDestroy = currentLine;
    GameManager.instance.selectedObjectSource.OnNext(null);
  }

  public override void CleanupResources() {

    if (toDestroy != null) {

      Object.Destroy(toDestroy);
      toDestroy = null;
    }
  }
}
