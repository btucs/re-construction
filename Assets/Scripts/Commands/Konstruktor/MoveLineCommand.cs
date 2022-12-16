#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLineCommand: Command {

  private GameObject lineToMove;
  private bool MoveUpdatesRelatedHandles;
  private Vector3 distanceMoved;

  private bool waitForUndo = true;

  public MoveLineCommand(
    GameObject lineToMove,
    bool MoveUpdatesRelatedHandles,
    Vector3 distanceMoved
  ) {

    this.lineToMove = lineToMove;
    this.MoveUpdatesRelatedHandles = MoveUpdatesRelatedHandles;
    this.distanceMoved = distanceMoved;
  }

  public override void Execute() {

    // don't execute until Undo was called first
    // everything was already moved when command was created
    if (waitForUndo == true || distanceMoved == Vector3.zero) {

      return;
    }

    MoveBy(distanceMoved);
  }

  public override void Undo() {

    if (distanceMoved == Vector3.zero) {

      return;
    }

    MoveBy(-distanceMoved);
    waitForUndo = false;
  }

  private void MoveBy(Vector3 distance) {

    if (MoveUpdatesRelatedHandles == true) {

      GameManager gameManager = GameManager.instance;


      for (int i = 0; i < lineToMove.transform.childCount; i++) {

        Transform childTransform = lineToMove.transform.GetChild(i);
        GameObject child = childTransform.gameObject;
        HashSet<GameObject> pool = gameManager.GetPoolForPos(childTransform.position);

        foreach (GameObject handle in pool) {

          // ignore children of this gameobject since they already moved
          if (handle != child) {

            MoveHandle moveHandle = handle.GetComponent<MoveHandle>();
            moveHandle.MoveTo(distance);
          }
        }
      }
    }

    lineToMove.transform.Translate(distance);
  }
}
