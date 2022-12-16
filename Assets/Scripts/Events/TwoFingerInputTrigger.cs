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

public interface ITwoFingerHandler {

  void HandleTwoFingerDragStart(TwoFingerEventData data);
  void HandleTwoFingerDragEnd(TwoFingerEventData data);
  void HandleTwoFingerDrag(DragEventData dataa);
}

[DisallowMultipleComponent]
public class TwoFingerInputTrigger: ObservableTriggerBase, IPointerDownHandler, IPointerUpHandler, IDragHandler {

  private Dictionary<int, Vector2> touches = new Dictionary<int, Vector2>();

  Subject<TwoFingerEventData> onTwoFingerStart = new Subject<TwoFingerEventData>();
  Subject<TwoFingerEventData> onTwoFingerEnd = new Subject<TwoFingerEventData>();
  Subject<PointerEventData> onDrag = new Subject<PointerEventData>();

  BehaviorSubject<bool> isDragActiveSource = new BehaviorSubject<bool>(false);
  // fix problems when OnTwoFingerDrawAsOvservable is called before Start
  ReplaySubject<IObservable<bool>> isDragActive = new ReplaySubject<IObservable<bool>>(1);

  private void Start() {

    isDragActive.OnNext(isDragActiveSource.AsObservable());
  }

  public void OnPointerDown(PointerEventData data) {

    touches[data.pointerId] = data.position;

    if (touches.Count == 2) {
      
      onTwoFingerStart.OnNext(TouchesToEventData());
      isDragActiveSource.OnNext(true);
    }
  }

  public void OnPointerUp(PointerEventData data) {

    int beforeUpCount = touches.Count;
    touches.Remove(data.pointerId);

    if (beforeUpCount == 2) {

      onTwoFingerEnd.OnNext(TouchesToEventData());
      isDragActiveSource.OnNext(false);
    }     
  }

  public void OnDrag(PointerEventData data) {

    onDrag.OnNext(data);
  }

  public IObservable<TwoFingerEventData> OnTwoFingerDragStartAsObservable() {

    return onTwoFingerStart.AsObservable();
  }

  public IObservable<TwoFingerEventData> OnTwoFingerDragEndAsObservable() {

    return onTwoFingerEnd.AsObservable();
  }

  public IObservable<DragEventData> OnTwoFingerDragAsObservable() {

    return isDragActive
      .Switch()
      .Where(isActive => isActive == true)
      .SelectMany((bool _) => {

        IObservable<DragEventData> firstTouch = CalculateDelta(onDrag, 0);
        IObservable<DragEventData> secondTouch = CalculateDelta(onDrag, 1);

        return firstTouch.Zip(
          secondTouch,
          (DragEventData left, DragEventData right) => DragEventData.Avg(left, right)
        ).TakeUntil(isDragActive.Switch().Where(isActive => isActive == false));
      })
      .Buffer(Observable.EveryUpdate())
      .Where(list => list.Count > 0)
      .Select((list) => {

        DragEventData sum = new DragEventData();

        foreach (DragEventData item in list) {

          sum = new DragEventData(
            sum.delta + item.delta,
            sum.direction + item.direction
          );
        }

        return sum;
      })
      .DistinctUntilChanged(new IEqualityComparerDragEventData())
    ;
  }

  private IObservable<DragEventData> CalculateDelta(IObservable<PointerEventData> dragEvent, int pointerId) {

    return dragEvent
      .Where(data => data.pointerId == pointerId)
      .Select(data => data.position)
      .Buffer(2, 1)
      .Select((IList<Vector2> list) => new DragEventData(
        Vector2.Distance(list[0], list[1]),
        list[1] - list[0]
      ))
    ;
  }

  private TwoFingerEventData TouchesToEventData() {

    TwoFingerEventData eventData;
    touches.TryGetValue(0, out eventData.firstTouch);
    touches.TryGetValue(1, out eventData.secondTouch);

    return eventData;
  }

  protected override void RaiseOnCompletedOnDestroy() {

    onTwoFingerStart.OnCompleted();
    onDrag.OnCompleted();
    onTwoFingerEnd.OnCompleted();
    isDragActiveSource.OnCompleted();
  }
}
