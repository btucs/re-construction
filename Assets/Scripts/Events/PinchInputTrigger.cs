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

/** based on @link https://stackoverflow.com/a/40591301/1244727 */

public interface IPinchHander {
  void HandleOnPinchStart();
  void HandleOnPinchEnd();
  void HandleOnPinchZoom(float gapDelta);
}

[DisallowMultipleComponent]
public class PinchInputTrigger: ObservableTriggerBase, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

  Subject<TwoFingerEventData> OnPinchStartSource = new Subject<TwoFingerEventData>();
  Subject<TwoFingerEventData> OnPinchEndSource = new Subject<TwoFingerEventData>();
  Subject<float> OnPinchZoomSource = new Subject<float>();

  private Dictionary<int, Vector2> touches = new Dictionary<int, Vector2>();

  private float previousDistance = 0f;
  private float delta = 0f;

  public void OnPointerDown(PointerEventData data) {

    touches[data.pointerId] = data.position;

    if (touches.Count == 2) {

      FigureDelta();
      OnPinchStartSource.OnNext(TouchesToEventData());
    }
  }

  public void OnPointerUp(PointerEventData data) {

    int beforeUpCount = touches.Count;
    touches.Remove(data.pointerId);

    if (beforeUpCount == 2) {

      OnPinchEndSource.OnNext(TouchesToEventData());
    }
  }

  public void OnDrag(PointerEventData data) {

    touches[data.pointerId] = data.position;
    FigureDelta();

    if (touches.Count == 2) {

      OnPinchZoomSource.OnNext(delta);
    }
  }

  public IObservable<TwoFingerEventData> OnPinchStartAsObservable() {

    return OnPinchStartSource.AsObservable();
  }

  public IObservable<TwoFingerEventData> OnPinchEndAsObservable() {

    return OnPinchEndSource.AsObservable();
  }

  public IObservable<float> OnPinchZoomAsObservable() {

    return OnPinchZoomSource.AsObservable()
      .Buffer(Observable.EveryFixedUpdate())
      .Where(list => list.Count > 0)
      .Select(list => {
        float sum = 0f;

        foreach (float listItem in list) {
          sum += listItem;
        }

        return sum;
      })
      .DistinctUntilChanged()
    ;
  }

  protected override void RaiseOnCompletedOnDestroy() {

    OnPinchStartSource.OnCompleted();
    OnPinchEndSource.OnCompleted();
    OnPinchZoomSource.OnCompleted();
  }

  private TwoFingerEventData TouchesToEventData() {

    TwoFingerEventData eventData;
    touches.TryGetValue(0, out eventData.firstTouch);
    touches.TryGetValue(1, out eventData.secondTouch);

    return eventData;
  }

  private void FigureDelta() {

    float newDistance = 0;
    try {

      newDistance = Vector2.Distance(touches[0], touches[1]);
    } catch (KeyNotFoundException) { /* newDistance stays 0 */ }

    delta = newDistance - previousDistance;
    previousDistance = newDistance;
  }
}
