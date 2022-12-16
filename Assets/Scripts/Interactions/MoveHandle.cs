#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(SingleFingerLongPressTrigger), typeof(SingleFingerInputTrigger), typeof(Handle))]
public class MoveHandle: MonoBehaviour {

  public float waitUntilMoveMillis = 300f;
  public bool IsActive = true;
  public CommandInvoker invoker;

  private bool isMoving = false;
  private new SpriteRenderer renderer;
  private Highlighter highlighter;
  private GameManager gameManager;
  private GridManager gridManager;
  private Handle handle;

  private Vector3 startPos;
  private float gridStep;
  private Transform otherHandle;

  private void Start() {

    SingleFingerInputTrigger trigger = GetComponent<SingleFingerInputTrigger>();
    SingleFingerLongPressTrigger lpTrigger = GetComponent<SingleFingerLongPressTrigger>();

    gridManager = GridManager.instance;

    GenerateGridExtended gridScript = gridManager.grid.GetComponent<GenerateGridExtended>();
    gridStep = gridScript.GetSmallestGridStep();

    int index = transform.GetSiblingIndex();

    lpTrigger.OnSingleFingerLongPress(waitUntilMoveMillis)
      .Where((_) => IsActive == true)
      .Where((_) => transform.parent.childCount >= 3)
      .Do((_) => otherHandle = transform.parent.GetChild(index == 1 ? 2 : 1))
      .Do(OnLongPress)
      .SelectMany((_) => trigger
        .OnSingleFingerDragAsObservable()
        .TakeUntil(
          trigger.OnSingleFingerUpAsObservable().Do(OnSingleFingerUp)
        )
      )
      .Subscribe(OnSingleFingerDrag)
      .AddTo(this)
    ;

    renderer = GetComponentInChildren<SpriteRenderer>();
    highlighter = GetComponentInChildren<Highlighter>();
    gameManager = GameManager.instance;
    handle = GetComponent<Handle>();
  }

  public void MoveTo(Vector2 to) {
        
    Vector3 currentPosition = transform.position;
    transform.Translate(to, Space.World);

    Line lineScript = transform.parent.GetComponent<Line>();
    lineScript.UpdateLinePosAndRotation();
  }

  private void OnLongPress(Vector2 position) {

    isMoving = true;
    //highlighter.Highlight();
    ForAllInHandlePool((GameObject handle) => {

      handle.GetComponentInChildren<Highlighter>().Highlight();
    });

    startPos = transform.localPosition;
  }
 
  private void OnSingleFingerUp(Vector2 position) {

    isMoving = false;

    Vector3 diff = GridManager.instance.GetCameraPosition(position, gameObject) - transform.position;
    diff.z = 0;

    ForAllInHandlePool((GameObject handle) => {

      handle.GetComponentInChildren<Highlighter>().UnHighlight();
      MoveHandle moveHandle = handle.GetComponent<MoveHandle>();
      moveHandle.MoveTo(diff);
    });

    if (invoker != null) {

      Vector3 distanceMoved = transform.localPosition - startPos;
      distanceMoved.z = 0;

      if (distanceMoved != Vector3.zero) {

        Command command = new MoveHandleCommand(gameObject, distanceMoved);
        invoker.ExecuteCommand(command);
      }
    }    
  }

  private void OnSingleFingerDrag(Vector2 position) {
    
    if (isMoving == true) {

      Vector3 localPos = GridManager.instance.GetCameraPosition(position, gameObject, false);
      Vector3 diff = localPos - transform.position;
      diff.z = 0;

      // distance between handles of the same line
      float magnitude = (otherHandle.position - localPos).magnitude;
      
      if (diff != Vector3.zero && magnitude >= gridStep) {

        ForAllInHandlePool((GameObject handle) => {

          MoveHandle moveHandle = handle.GetComponent<MoveHandle>();
          moveHandle.MoveTo(diff);
        });
      }
    }
  }

  private void ForAllInHandlePool(Action<GameObject> forEachMethod) {

    HashSet<GameObject> pool = gameManager.GetPoolForPos(transform.position);

    foreach (GameObject handle in pool) {

      forEachMethod(handle);
    }
  }
}
