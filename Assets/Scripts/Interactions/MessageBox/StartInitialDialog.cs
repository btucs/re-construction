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
using UniRx;

public class StartInitialDialog : MonoBehaviour {

  public int delayInSeconds = 2;
  public string trigger = "start";

  void Start() {

    Observable
      .Return(trigger)
      .Delay(TimeSpan.FromSeconds(delayInSeconds))
      .Do((string eventName) => MonologManager.instance.Trigger(eventName))
      .Subscribe()
      .AddTo(this)
    ;
  }
}
