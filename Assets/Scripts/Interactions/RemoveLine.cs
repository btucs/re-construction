#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using System;

[RequireComponent(typeof(SingleFingerInputTrigger))]
public class RemoveLine: MonoBehaviour {

  public bool IsActive = true;
  public CommandInvoker invoker;

  private void Start() {

    SingleFingerInputTrigger trigger = gameObject.GetComponent<SingleFingerInputTrigger>();

    IObservable<Vector2> obs = trigger.OnSingleFingerDownAsObservable();
    obs
      .Where((_) => IsActive == true)
      .Buffer(obs.Throttle(TimeSpan.FromMilliseconds(250)))
      .Where(xs => xs.Count >= 2)
      .Do(xs => {

        Command command = new RemoveLineCommand(transform.parent.gameObject);

        if (invoker != null) {

          invoker.ExecuteCommand(command);
        } else {

          command.Execute();
          command.CleanupResources();
        }
      })
      .Subscribe()
      .AddTo(this)
    ;
  }
}
