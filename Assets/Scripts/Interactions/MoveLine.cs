#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(SingleFingerInputTrigger), typeof(SingleFingerLongPressTrigger), typeof(Highlighter))]
public class MoveLine: MonoBehaviour {

  public float waitUntilMoveMillis = 300f;
  public bool MoveUpdatesRelatedHandles = true;
  public bool IsActive = true;

  public CommandInvoker invoker;

  private bool inEditMode = false;

  private Line line;

  private Highlighter highlighter;
  private GridManager gridManager;
  private GameManager gameManager;

  // diff between startPoint and touchPoint
  private Vector3 diff;
  private Vector3 touchPos;
  
  public void Start() {

    line = gameObject.GetComponentInParent<Line>();
    highlighter = gameObject.GetComponent<Highlighter>();

    gridManager = GridManager.instance;
    gameManager = GameManager.instance;

    SingleFingerInputTrigger trigger = gameObject.GetComponent<SingleFingerInputTrigger>();

    SingleFingerLongPressTrigger lpTrigger = gameObject.GetComponent<SingleFingerLongPressTrigger>();

    lpTrigger.OnSingleFingerLongPress(waitUntilMoveMillis)
      .Where((_) => IsActive == true)
      .Do(OnLongPress)
      .SelectMany((_) => {

        IObservable<Vector2> fingerUpObservable = trigger.OnSingleFingerUpAsObservable()
          .Do(OnSingleFingerUp)
        ;

        IObservable<Vector2> dragObservable = trigger.OnSingleFingerDragAsObservable()
          .Do(OnSingleFingerDrag)
        ;

        return dragObservable.TakeUntil(fingerUpObservable);
      })
      .Subscribe()
      .AddTo(this)
    ;
  }
  
  private void OnLongPress(Vector2 position) {
    
    touchPos = gridManager.GetCameraPosition(position, line.gameObject);
    diff = GetLocalPoint(touchPos);
    EnabledMove();
  }

  private void OnSingleFingerDrag(Vector2 position) {

    if (inEditMode == true) {

      Vector3 newPos = GetLocalPoint(gridManager.GetCameraPosition(position, line.gameObject)) - diff;
      newPos.z = 0f;

      MoveTo(newPos);
    } else {

      DisableMove();
    }
  }

  private void OnSingleFingerUp(Vector2 position) {

    inEditMode = false;
    DisableMove();

    if (invoker != null) {

      Vector3 distanceMoved = gridManager.GetCameraPosition(position, line.gameObject) - touchPos;
      distanceMoved.z = 0;

      if (distanceMoved != Vector3.zero) {

        Command command = new MoveLineCommand(
          line.gameObject,
          MoveUpdatesRelatedHandles,
          distanceMoved
        );

        invoker.ExecuteCommand(command);
      }
    }
  }

  private Vector3 GetLocalPoint(Vector3 point) {

    Vector3 localPos = line.transform.InverseTransformPoint(point);
    localPos.z = 0f;

    return localPos;
  }

  private void EnabledMove() {

    inEditMode = true;
    highlighter.Highlight();
  }

  private void DisableMove() {

    inEditMode = false;
    highlighter.UnHighlight();
  }

  public void MoveTo(Vector2 direction) {

    if (direction == Vector2.zero) {

      return;
    }

    if (MoveUpdatesRelatedHandles == true) {

      for (int i = 1; i < line.transform.childCount; i++) {

        Transform childTransform = line.transform.GetChild(i);
        GameObject child = childTransform.gameObject;
        HashSet<GameObject> pool = gameManager.GetPoolForPos(childTransform.position);

        foreach (GameObject handle in pool) {

          // ignore children of this gameobject since they already moved
          if (handle != child) {

            MoveHandle moveHandle = handle.GetComponent<MoveHandle>();
            moveHandle.MoveTo(direction);
          }
        }
      }
    }

    line.transform.Translate(direction);
  }
}