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

[Obsolete]
public class DrawLineEndCommand: Command {

  private DrawLine script;
  private LineSO lineSO;
  private CommandInvoker invoker;
  private bool MoveUpdatesRelatedHandles;
  private bool AllowDelete;

  private Vector3 position;
  private bool isLocalPos;
  private int sortingOrder;
  private bool select;

  private GameObject currentLine;
  private GameObject currentHandle;

  private GameObject toDestroy;

  public DrawLineEndCommand(
    DrawLine script,
    GameObject currentLine,
    LineSO lineSO,
    CommandInvoker invoker,
    bool MoveUpdatesRelatedHandles,
    bool AllowDelete,
    Vector3 position,
    bool isLocalPos,
    int sortingOrder,
    bool select
   ) {

    this.script = script;
    this.currentLine = currentLine;
    this.lineSO = lineSO;
    this.invoker = invoker;
    this.MoveUpdatesRelatedHandles = MoveUpdatesRelatedHandles;
    this.AllowDelete = AllowDelete;

    this.position = position;
    this.isLocalPos = isLocalPos;
    this.sortingOrder = sortingOrder;
    this.select = select;
  }

  public override void Execute() {

    if (toDestroy != null) {

      ReactivateExistingHandle();
    } else {

      CreateEndHandle();
    }
  }

  private void ReactivateExistingHandle() {

    currentHandle = toDestroy;
    currentHandle.transform.parent = currentLine.transform;
    currentHandle.SetActive(true);

    Line lineScript = currentLine.GetComponent<Line>();
    lineScript.UpdateLinePosAndRotation();

    toDestroy = null;
    script.currentLine = null;
    GameManager.instance.selectedObjectSource.OnNext(null);
  }

  private void CreateEndHandle() {

    Vector3 endPos = (isLocalPos ? position : GridManager.instance.GetCameraPosition(position, currentLine));

    Line currentLineScript = currentLine.GetComponent<Line>();
    currentLineScript.invoker = invoker;
    currentHandle = currentLineScript.AddPosition(
      endPos, sortingOrder, lineSO.endHandle, select
    );

    Transform lineSprite = currentLineScript.transform.GetChild(0);
    MoveLine moveScript = lineSprite.GetComponent<MoveLine>();
    if (moveScript != null) {

      moveScript.MoveUpdatesRelatedHandles = MoveUpdatesRelatedHandles;
      moveScript.invoker = invoker;
    }

    RemoveLine removeLineScript = lineSprite.GetComponent<RemoveLine>();
    if (removeLineScript != null) {

      removeLineScript.IsActive = AllowDelete;
      removeLineScript.invoker = invoker;
    }

    script.currentLine = null;
    GameManager.instance.selectedObjectSource.OnNext(null);
  }

  public override void Undo() {

    Transform lineSprite = currentLine.transform.GetChild(0);
    lineSprite.position = currentLine.transform.GetChild(1).position;
    Vector3 currentScale = lineSprite.localScale;
    currentScale.x = 0;
    lineSprite.localScale = currentScale;

    currentHandle.SetActive(false);
    currentHandle.transform.parent = GameManager.instance.transform;
    toDestroy = currentHandle;

    currentHandle = null;

    Transform startTransform = currentLine.transform.GetChild(1);
    GameManager.instance.selectedObjectSource.OnNext(startTransform.gameObject);

    script.currentLine = currentLine;
  }

  public override void CleanupResources() {

    if (toDestroy != null) {

      UnityEngine.Object.Destroy(toDestroy);
    }
  }

  private Vector3 GetLocalPos(Vector3 point) {

    Vector3 localPos = currentLine.transform.InverseTransformPoint(point);
    localPos.z = 0;

    return localPos;
  }
}
