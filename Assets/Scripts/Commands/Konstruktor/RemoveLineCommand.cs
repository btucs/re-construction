#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveLineCommand: Command {

  private GameObject lineToRemove;

  public RemoveLineCommand(GameObject lineToRemove) {

    this.lineToRemove = lineToRemove;
  }

  public override void Execute() {

    lineToRemove.SetActive(false);
  }

  public override void Undo() {

    lineToRemove.SetActive(true);
  }

  public override void CleanupResources() {

    if (lineToRemove.activeSelf == false) {

      Object.Destroy(lineToRemove);
      lineToRemove = null;
    }
  }
}
