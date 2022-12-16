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

public class MoveHandleCommand: Command {

  private GameObject handleToMove;
  private Vector3 distanceToMove;

  private bool waitForUndo = true;

  public MoveHandleCommand(GameObject handleToMove, Vector3 distanceToMove) {

    this.handleToMove = handleToMove;
    this.distanceToMove = distanceToMove;
  }

  public override void Execute() {

    // don't execute until Undo was called first
    // everything was already moved when command is created
    if (waitForUndo == true || distanceToMove == Vector3.zero) {

      return;
    }

    ForAllInHandlePool((GameObject handle) => {

      MoveHandle moveHandle = handle.GetComponent<MoveHandle>();
      MoveBy(moveHandle.transform, distanceToMove);
    });
  }

  public override void Undo() {

    if (distanceToMove == Vector3.zero) {

      return;
    }

    ForAllInHandlePool((GameObject handle) => {

      MoveHandle moveHandle = handle.GetComponent<MoveHandle>();
      MoveBy(moveHandle.transform, -distanceToMove);
    });

    waitForUndo = false;
  }

  private void MoveBy(Transform handleToMove, Vector3 distance) {

    handleToMove.Translate(distance, Space.World);

    Line lineScript = handleToMove.parent.GetComponent<Line>();
    lineScript.UpdateLinePosAndRotation();
  }

  private void ForAllInHandlePool(Action<GameObject> forEachMethod) {

    HashSet<GameObject> pool = GameManager.instance.GetPoolForPos(handleToMove.transform.position);

    foreach (GameObject handle in pool) {

      forEachMethod(handle);
    }
  }
}
