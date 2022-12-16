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
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

[DisallowMultipleComponent]
public class SingleFingerLongPressTrigger : ObservableTriggerBase, IPointerDownHandler,
  IPointerUpHandler, IDragHandler
{

  private Subject<PointerEventData> onPointerDownSource = new Subject<PointerEventData>();
  private IObservable<PointerEventData> onPointerDown;

  private Subject<PointerEventData> onDragSource = new Subject<PointerEventData>();
  private IObservable<PointerEventData> onDrag;

  private Subject<PointerEventData> onPointerUpSource = new Subject<PointerEventData>();
  private IObservable<PointerEventData> onPointerUp;

  private void Awake() {

    onDrag = onDragSource.AsObservable();
    onPointerUp = onPointerUpSource.AsObservable();
    onPointerDown = onPointerDownSource.AsObservable();
  }

  public void OnPointerDown(PointerEventData data) {

    onPointerDownSource.OnNext(data);
  }

  public void OnPointerUp(PointerEventData data) {

    onPointerUpSource.OnNext(data);
  }

  public void OnDrag(PointerEventData data) {

    onDragSource.OnNext(data);
  }

  public IObservable<Vector2> OnSingleFingerLongPress(float delay) {

    return onPointerDown
      .SelectMany(pointerEvent => Observable.Return(pointerEvent)
        .Delay(TimeSpan.FromMilliseconds(delay))
        .TakeUntil(
          onPointerUp
            .Merge<PointerEventData>(onDrag)
            .Merge<PointerEventData>(onPointerDown)
        )
      )
      .Select(data => data.position)
    ;
  }

  protected override void RaiseOnCompletedOnDestroy() {

    onDragSource.OnCompleted();
    onPointerUpSource.OnCompleted();
    onPointerDownSource.OnCompleted();
  }
}
