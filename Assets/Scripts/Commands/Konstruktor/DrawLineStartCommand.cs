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
using Constants;

[Obsolete]
public class DrawLineStartCommand: Command {

  private DrawLine script;
  private LineSO lineSO;
  private CommandInvoker invoker;
  private Transform lineContainer;

  private Vector3 position;
  private bool isLocalPos;
  private int sortingOrder;
  private bool select;

  private GameObject currentLine;

  // keep currentLine reference active
  private GameObject toDestroy;

  public DrawLineStartCommand(
    DrawLine script,
    LineSO lineSO,
    CommandInvoker invoker,
    Transform lineContainer,
    Vector3 position,
    bool isLocalPos,
    int sortingOrder,
    bool select
  ) {

    this.script = script;
    this.lineSO = lineSO;
    this.invoker = invoker;
    this.lineContainer = lineContainer;

    this.position = position;
    this.isLocalPos = isLocalPos;
    this.sortingOrder = sortingOrder;
    this.select = select;
  }

  public override void Execute() {

    if (toDestroy != null) {

      currentLine = script.currentLine = toDestroy;
      currentLine.SetActive(true);
      Transform handle = currentLine.transform.GetChild(1);
      GameManager.instance.selectedObjectSource.OnNext(handle.gameObject);
      toDestroy = null;
    } else {

      SetupLine();
    }
    
  }

  public override void Undo() {

    currentLine.SetActive(false);
    toDestroy = currentLine;
    GameManager.instance.selectedObjectSource.OnNext(null);
    script.currentLine = null;
  }

  public override void CleanupResources() {

    if (toDestroy != null) {

      UnityEngine.Object.Destroy(toDestroy);
      toDestroy = null;
    }
  }

  private void SetupLine() {

    currentLine = script.currentLine = UnityEngine.Object.Instantiate<GameObject>(lineSO.LinePrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
    currentLine.layer = Layers.interactable;

    if (lineContainer != null) {

      currentLine.transform.SetParent(lineContainer);
    }

    Transform lineSprite = currentLine.transform.GetChild(0);
    lineSprite.GetComponent<SpriteRenderer>().color = lineSO.lineColor; 

    Vector3 startPos = (isLocalPos ? position : GridManager.instance.GetCameraPosition(position, currentLine));

    Line currentLineScript = currentLine.GetComponent<Line>();
    currentLineScript.invoker = invoker;
    currentLineScript.lineWidth = lineSO.lineWidth;
    currentLineScript.AddPosition(startPos, sortingOrder, lineSO.startHandle, select);
  }
}
