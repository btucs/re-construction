#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion

/** based on @link https://stackoverflow.com/a/40591301/1244727 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

[DisallowMultipleComponent]
public class SingleFingerInputTrigger : ObservableTriggerBase, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

  Subject<Vector2> onSingleFingerDown = new Subject<Vector2>();
  Subject<Vector2> onSingleFingerBeginDrag = new Subject<Vector2>();
  Subject<Vector2> onSingleFingerDrag = new Subject<Vector2>();
  Subject<Vector2> onSingleFingerUp = new Subject<Vector2>();
  
  private int currentSingleFinger = -1;
  private int kountFingersDown = 0;

  private bool startDragSent = false;

  public void OnPointerDown(PointerEventData data) {

    kountFingersDown = kountFingersDown + 1;

    if (
      currentSingleFinger == -1 &&
      kountFingersDown == 1
    ) {

      currentSingleFinger = data.pointerId;
      onSingleFingerDown.OnNext(data.position);
    }
  }

  public IObservable<Vector2> OnSingleFingerDownAsObservable() {

    return onSingleFingerDown.AsObservable();
  }

  public void OnDrag(PointerEventData data) {

    if (
      currentSingleFinger == data.pointerId &&
      kountFingersDown == 1
    ) {

      onSingleFingerDrag.OnNext(data.position);
    }
  }

  public IObservable<Vector2> OnSingleFingerDragAsObservable() {

    return onSingleFingerDrag.AsObservable()
      .Buffer(Observable.EveryUpdate())
      .Where(list => list.Count > 0)
      .Select((list) => list[list.Count - 1])
      .DistinctUntilChanged()
      .Do((Vector2 position) => {
        if(startDragSent == false) {

          onSingleFingerBeginDrag.OnNext(position);
          startDragSent = true;
        }
      })
    ;
  }

  public void OnPointerUp(PointerEventData data) {

    kountFingersDown = kountFingersDown - 1;
    if (currentSingleFinger == data.pointerId) {
      
      currentSingleFinger = -1;

      if (kountFingersDown == 0) {

        onSingleFingerUp.OnNext(data.position);
      }
    }
  }

  public IObservable<Vector2> OnSingleFingerUpAsObservable() {

    return onSingleFingerUp.AsObservable();
  }

  public IObservable<Vector2> OnSingleFingerBeginDragAsObservable() {

    return onSingleFingerBeginDrag.AsObservable();
  }

  public IObservable<Vector2> OnSingleFingerEndDragAsObservable() {

    return onSingleFingerUp.AsObservable()
      .Where((Vector2 data) => {

        if(startDragSent == true) {

          startDragSent = false;

          return true;
        }

        return false;
      })
    ;
  }

  public IObservable<Vector2> OnSingleFingerTapAsObservable() {

    return OnSingleFingerDownAsObservable()
      .SelectMany((_) =>
        OnSingleFingerUpAsObservable()
          .Timeout(TimeSpan.FromMilliseconds(200))
          .Catch<Vector2, TimeoutException>((TimeoutException) => Observable.Empty<Vector2>())
          .TakeUntil(OnSingleFingerDragAsObservable())
      )
    ;
  }

  protected override void RaiseOnCompletedOnDestroy() {

    onSingleFingerDown.OnCompleted();
    onSingleFingerBeginDrag.OnCompleted();
    onSingleFingerDrag.OnCompleted();
    onSingleFingerUp.OnCompleted();
  }
}
