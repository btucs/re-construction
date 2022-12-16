#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Sirenix.OdinInspector;

public class DrawLineEvent : UnityEvent<LineControllerUI> {}

public class DrawLineUI : MonoBehaviour {

  public LineUISO lineSO;
  public DrawLineEvent onEndDraw = new DrawLineEvent();

  [Required]
  public LineControllerUI linePrefab;
  [Required]
  public LineUISO hintLine;

  [Required]
  public RectTransform lineContainer;

  [Required]
  public KonstruktorDrawController drawController;

  private readonly BehaviorSubject<bool> drawIsActiveSource = new BehaviorSubject<bool>(false);
  private IObservable<bool> drawIsActive;

  private KonstructorDrawPointController[] drawnItems;

  private ValueTuple<string, Vector2, KonstructorDrawPointController> startTuple;
  private ValueTuple<string, Vector2, KonstructorDrawPointController> endTuple;
  private ValueTuple<string, Vector2, KonstructorDrawPointController> enteredTuple;
  private LineControllerUI currentLine;
  private LineControllerUI currentHintLine;

  private Canvas drawCanvas;

  public void ToggleDraw(bool isActive) {

    drawIsActiveSource.OnNext(isActive);
  }

  private void Start() {

    drawCanvas = lineContainer.GetComponent<Canvas>();

    drawIsActive = drawIsActiveSource.AsObservable();
    drawIsActive
      .SelectMany((bool isActive) => {

        if(isActive == false) {

          return Observable.Empty<ValueTuple<string, Vector2, KonstructorDrawPointController>>();
        } else {

          drawnItems = drawController.GetDrawnItems();
          Debug.Log("Is Active " + isActive);
          IObservable<ValueTuple<string, Vector2, KonstructorDrawPointController>> onFingerDown = Observable.Empty<ValueTuple<string, Vector2, KonstructorDrawPointController>>();
          IObservable<ValueTuple<string, Vector2, KonstructorDrawPointController>> onDragStart = Observable.Empty<ValueTuple<string, Vector2, KonstructorDrawPointController>>();
          IObservable<ValueTuple<string, Vector2, KonstructorDrawPointController>> onDrag = Observable.Empty<ValueTuple<string, Vector2, KonstructorDrawPointController>>();
          IObservable<ValueTuple<string, Vector2, KonstructorDrawPointController>> onFingerUp = Observable.Empty<ValueTuple<string, Vector2, KonstructorDrawPointController>>();
          IObservable<ValueTuple<string, Vector2, KonstructorDrawPointController>> onFingerEnter = Observable.Empty<ValueTuple<string, Vector2, KonstructorDrawPointController>>();
          IObservable<ValueTuple<string, Vector2, KonstructorDrawPointController>> onFingerExit = Observable.Empty<ValueTuple<string, Vector2, KonstructorDrawPointController>>();

          foreach(KonstructorDrawPointController pointContoller in drawnItems) {

            onFingerDown = onFingerDown.Merge(EnhanceObservable("OnFingerDown", pointContoller.OnFingerDown, pointContoller));
            onDragStart = onDragStart.Merge(EnhanceObservable("OnDragStart", pointContoller.OnDragStart, pointContoller));
            onDrag = onDrag.Merge(EnhanceObservable("OnDrag", pointContoller.OnDrag, pointContoller));
            onFingerUp = onFingerUp.Merge(EnhanceObservable("OnFingerUp", pointContoller.OnFingerUp, pointContoller));
            onFingerEnter = onFingerEnter.Merge(EnhanceObservable("OnFingerEnter", pointContoller.OnFingerEnter, pointContoller));
            onFingerExit = onFingerExit.Merge(EnhanceObservable("OnFingerExit", pointContoller.OnFingerExit, pointContoller));
          }

          onDragStart = onDragStart.Do(StartLine);
          onDrag = onDrag.Do(DrawLine);
          onFingerUp = onFingerUp.Do(EndLine);

          onFingerEnter = onFingerEnter
            .Where(tuple => tuple.Item3 != startTuple.Item3)
            .Do(OnEnterOtherPoint)
          ;

          onFingerExit = onFingerExit.Do(OnExitOtherPoint);

          // when a finger down happens wait for the other events to come
          return onFingerDown
            .SelectMany(_ => onDragStart
              .Merge(onDrag)
              // should be only in TakeUntil, otherwise fires twice
              //.Merge(onFingerUp)
              .Merge(onFingerEnter)
              .Merge(onFingerExit)
              .TakeUntil(onFingerUp)
            )
            .TakeUntil(drawIsActive.Where((bool innerIsActive) => innerIsActive == false))
          ;
        }
      })
      .Subscribe()
      .AddTo(this)
    ;
  }
  
  private IObservable<ValueTuple<string, Vector2, KonstructorDrawPointController>> EnhanceObservable(
    string type,
    IObservable<Vector2> observable,
    KonstructorDrawPointController controller
  ) {
    // create a tuple to keep information about which observable of which controller fired
    return observable.Select((Vector2 pos) => ValueTuple.Create(
      type, 
      ConvertLocalSourceToLocalTargetCoordinate(pos, controller.transform),
      controller
    ));
  }

  private void StartLine(ValueTuple<string, Vector2, KonstructorDrawPointController> tuple) {

    if(currentLine == null) {

      startTuple = tuple;

      currentLine = Instantiate(linePrefab, lineContainer);
      currentLine.name = "Line";
      currentLine.ConfigureLine(lineSO);
      currentLine.PositionLine(startTuple.Item2, startTuple.Item2);
    }
  }

  private void DrawLine(ValueTuple<string, Vector2, KonstructorDrawPointController> tuple) {

    if(currentLine != null) {

      endTuple = tuple;

      currentLine.PositionLine(startTuple.Item2, endTuple.Item2);
    }
  }

  private void OnEnterOtherPoint(ValueTuple<string, Vector2, KonstructorDrawPointController> tuple) {

    if(currentLine != null) {

      enteredTuple = tuple;

      currentHintLine = Instantiate(linePrefab, lineContainer);
      currentHintLine.name = "HintLine";
      currentHintLine.ConfigureLine(hintLine);
      currentHintLine.PositionLine(startTuple.Item2, tuple.Item2);
    }
  }

  private void OnExitOtherPoint(ValueTuple<string, Vector2, KonstructorDrawPointController> tuple) {

    if(currentHintLine != null) {

      Destroy(currentHintLine.gameObject);
      currentHintLine = null;
      enteredTuple = default;
    }
  }

  private void EndLine(ValueTuple<string, Vector2, KonstructorDrawPointController> tuple) {

    if(currentLine != null) {

      if(enteredTuple != default) {

        endTuple = enteredTuple;
        currentLine.PositionLine(startTuple.Item2, enteredTuple.Item2);
        // set as first sibling such that it is placed behind the points
        //currentLine.transform.SetAsFirstSibling();
      } else {

        Destroy(currentLine.gameObject);
        currentLine = null;
      }
    }

    if(currentHintLine != null) {

      Destroy(currentHintLine.gameObject);
      currentHintLine = null;
      enteredTuple = default;
    }

    onEndDraw?.Invoke(currentLine);
  }

  private Vector2 ConvertLocalSourceToLocalTargetCoordinate(Vector2 sourcePos, Transform sourceTransform) {

    RectTransform sourceTransformRt = sourceTransform as RectTransform;
    RectTransform sourceParent = sourceTransformRt.parent as RectTransform;

    Vector3 targetPos = sourcePos;
    while(sourceParent != lineContainer) {
      
      targetPos = sourceParent.localRotation * targetPos;
      targetPos += sourceParent.localPosition;
      sourceParent = sourceParent.parent as RectTransform;
    }

    return targetPos;
  }
}
