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
using UniRx;

[RequireComponent(typeof(SingleFingerInputTrigger))]
public class DrawLine : MonoBehaviour {

  public LineSO lineSO;

  public CommandInvoker invoker;
  public Transform lineContainer;

  public bool MoveUpdatesRelatedHandles = true;
  public bool AllowDelete = true;
  public bool IsActive = true;
  
  [HideInInspector]
  public GameObject currentLine;
  private Line currentLineScript;
  private GameManager gameManager;
  private GridManager gridManager;

  private SingleFingerInputTrigger trigger;
  private CompositeDisposable subscriptions = new CompositeDisposable();

  private readonly BehaviorSubject<bool> drawIsAciveSource = new BehaviorSubject<bool>(false);
  private IObservable<bool> drawIsActive;

  // indicates if someone else is already handling
  private bool handling = false;

  private float gridStep;

  private void Start() {

    trigger = gameObject.GetComponent<SingleFingerInputTrigger>();
    gameManager = GameManager.instance;
    gridManager = GridManager.instance;

    GenerateGridExtended gridScript = gridManager.grid.GetComponent<GenerateGridExtended>();
    gridStep = gridScript.GetSmallestGridStep();

    drawIsActive = drawIsAciveSource.AsObservable();
    drawIsActive
      .Where((bool isActive) => isActive == true)
      .SelectMany((bool isActive) => gameManager.selectedObject
        .Where((GameObject selectedObject) => selectedObject != null)
        .Where((_) => handling == false)
        .Do((_) => handling = true)
        .TakeUntil(
          // end dispose observable when drawBalken is disabled
          drawIsActive.Where((bool isDrawActive) => isDrawActive == false)
        )
        .SelectMany((GameObject selectedObject) => {

          Vector3 startPos = selectedObject.transform.position;
          HashSet<GameObject> startPool = gameManager.GetPoolForPos(startPos);
          int startSortingOrder = startPool.Count;
          StartLine(startPos, true, startSortingOrder, false);

          SingleFingerInputTrigger handleDragTrigger = selectedObject.GetComponent<SingleFingerInputTrigger>();

          SingleFingerLongPressTrigger lpTrigger = selectedObject.GetComponent<SingleFingerLongPressTrigger>();

          // merge drag triggers on the selectedObject with drag trigger of grid background to handle both together independed of where the finger is
          IObservable<Vector2> mergedDrag = handleDragTrigger.OnSingleFingerDragAsObservable()
            .Merge(trigger.OnSingleFingerDragAsObservable())
          ;

          IObservable<Vector2> mergedUp = handleDragTrigger.OnSingleFingerUpAsObservable()
            .Merge(new IObservable<Vector2>[] {
              trigger.OnSingleFingerUpAsObservable(),
              lpTrigger.OnSingleFingerLongPress(200f)
            })
            .Do((_) => handling = false)
          ;

          return HandleDrawObservable(startPos, mergedDrag, mergedUp);
        })
      )
      .Subscribe()
      .AddTo(this)
    ;
  }

  public void StartLine(Vector3 position, bool isLocalPos) {

    StartLine(position, isLocalPos, 0, false);
  }

  public void StartLine(Vector3 position, bool isLocalPos, int sortingOrder, bool select) {

    currentLine = Instantiate<GameObject>(lineSO.LinePrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
    currentLine.layer = Layers.interactable;

    if (lineContainer != null) {

      currentLine.transform.SetParent(lineContainer);
    }

    Transform lineSprite = currentLine.transform.GetChild(0);
    lineSprite.GetComponent<SpriteRenderer>().color = lineSO.lineColor;

    Vector3 startPos = (isLocalPos ? position : GridManager.instance.GetCameraPosition(position, currentLine));

    currentLineScript = currentLine.GetComponent<Line>();
    currentLineScript.invoker = invoker;
    currentLineScript.lineWidth = lineSO.lineWidth;
    currentLineScript.AddPosition(startPos, sortingOrder, lineSO.startHandle, select);
  }

  public void EndLine(Vector3 position) {

    EndLine(position, false, 0, false);
  }

  public void EndLine(Vector3 position, bool isLocalPos) {

    EndLine(position, isLocalPos, 0, false);
  }

  public void EndLine(Vector3 position, bool isLocalPos, int sortingOrder, bool select) {

    Vector3 endPos = (isLocalPos ? position : GridManager.instance.GetCameraPosition(position, currentLine));

    Line currentLineScript = currentLine.GetComponent<Line>();
    currentLineScript.invoker = invoker;
    currentLineScript.AddPosition(
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

    currentLine = null;
    GameManager.instance.selectedObjectSource.OnNext(null);
    subscriptions.Dispose();
  }

  public void ToggleDraw(bool isActive) {

    drawIsAciveSource.OnNext(isActive);
    if (isActive == true) {

      BeginDraw();
    } else {

      gameManager.selectedObjectSource.OnNext(null);
      subscriptions.Dispose();
    }
  }

  public void BeginDraw() {

    subscriptions.Dispose();
    subscriptions = new CompositeDisposable();

    trigger.OnSingleFingerDownAsObservable()
      .Select((Vector2 startPos) => {

        StartLine(startPos, false);

        return gridManager.GetCameraPosition(startPos, currentLine);
      })
      .Do((_) => handling = true)
      .SelectMany((Vector3 startPos) => HandleDrawObservable(startPos))
      .Do((_) => handling = false)
      .Subscribe()
      .AddTo(this)
      .AddTo(subscriptions)
    ;
  }

  private void CreateAndExecuteDrawLineCommand(
    GameObject currentLine
  ) {

    Command command = new DrawLineCommand(
      this,
      currentLine
    );

    if (AllowDelete == true) {

      if (invoker != null) {

        invoker.ExecuteCommand(command);
      }
    } else {

      command.Execute();
      command.CleanupResources();
    }
  }

  private IObservable<Vector3> HandleDrawObservable(Vector3 startPos) {

    IObservable<Vector2> dragObservable = trigger.OnSingleFingerDragAsObservable();
    IObservable<Vector2> upObservable = trigger.OnSingleFingerUpAsObservable();

    return HandleDrawObservable(startPos, dragObservable, upObservable);
  }

  private IObservable<Vector3> HandleDrawObservable(
    Vector3 startPos,
    IObservable<Vector2> dragObservable,
    IObservable<Vector2> upObservable
  ) {

    bool isFirst = true;
    MoveHandle moveScript = null;

    IObservable<Vector3> dragObservableV3 = dragObservable
      .Select((Vector2 dragPos) => gridManager.GetCameraPosition(dragPos, currentLine, false))
      .Where((Vector3 dragPos) => (dragPos - startPos).magnitude >= gridStep)
      .Do((Vector3 dragPos) => {

        if (isFirst == true) {
          
          // add a dummy handle to move around
          GameObject handle = currentLineScript.AddPosition(dragPos, 0, lineSO.endHandle, true);
          moveScript = handle.GetComponent<MoveHandle>();
          isFirst = false;
        }
      })
      .Do((Vector3 dragPos) => {

        Vector3 diff = dragPos - moveScript.transform.position;
        diff.z = 0;

        if (diff != Vector3.zero) {

          moveScript.MoveTo(diff);
        }
      })
    ;

    IObservable<Vector3> upObservableV3 = upObservable
      .Select((Vector2 endPos) => gridManager.GetCameraPosition(endPos, currentLine))
      .Do((Vector3 endPos) => {

        if ((endPos - startPos).magnitude >= gridStep) {

          // destroy the dummy handle before adding the real endpos
          Destroy(moveScript.gameObject);
          moveScript = null;

          CreateAndExecuteDrawLineCommand(currentLine);
          EndLine(endPos, true);
        } else {

          Destroy(currentLine);
          currentLine = null;
        }

        if (drawIsAciveSource.Value == true) {

          BeginDraw();
        }
      })
    ;

    return Observable.Merge(dragObservableV3, upObservableV3).TakeUntil(upObservable);
  }
}
