#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "HandleTypes/FollowRotation")]
public class HandleTypeFollowRotation : HandleTypeSO {

  private Dictionary<GameObject, SerialDisposable> handleSubscriptions = new Dictionary<GameObject, SerialDisposable>();


  public override void SetSpriteOnHandle(GameObject handle) {

    base.SetSpriteOnHandle(handle);

    Transform line = handle.transform.parent;

    IDisposable positionSubscription = handle.transform
      .ObserveEveryValueChanged((Transform transform) => transform.position)
      .Where((_) => line.transform.childCount == 3)
      .DistinctUntilChanged()
      .TakeUntilDestroy(handle)
      .Subscribe(
        (_) => {
          int currentIndex = handle.transform.GetSiblingIndex();
          Transform sibling = line.GetChild((currentIndex == 1 ? 2 : 1));
          TransformExtensions.LookAtOrAway(handle.transform, sibling);
          TransformExtensions.LookAtOrAway(sibling, handle.transform);
        },
        (_) => {
          handleSubscriptions.Remove(handle);
        }
      )      
    ;

    SerialDisposable disposable = new SerialDisposable();
    if (handleSubscriptions.ContainsKey(handle)) {

      handleSubscriptions.TryGetValue(handle, out disposable);
    }

    disposable.Disposable = positionSubscription;
  }
}
