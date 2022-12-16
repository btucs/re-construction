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

[RequireComponent(typeof(SingleFingerInputTrigger))]
public class InputModuleSwitch : MonoBehaviour {

  public float fillInMs = 1000;
  public float emptyInMs = 750;

  private Material mat;
  private float fillPercentage = 0;

  private float increaseAmount;
  private float decreaseAmount;

  private void Start() {

    mat = GetComponent<Renderer>().material;
    mat.SetFloat("_Fillpercentage", fillPercentage);

    increaseAmount = 1f / fillInMs * 1000 * Time.fixedDeltaTime;
    decreaseAmount = 1f / emptyInMs * 1000 * Time.fixedDeltaTime;

    SingleFingerInputTrigger ipTrigger = GetComponent<SingleFingerInputTrigger>();

    ipTrigger.OnSingleFingerUpAsObservable()
      .SelectMany((_) => Observable.EveryFixedUpdate()
        .TakeWhile((__) => fillPercentage != 1)
        .Select((__) => fillPercentage = Mathf.Clamp01(fillPercentage - decreaseAmount))
        .Do(__ => mat.SetFloat("_Fillpercentage", fillPercentage))
        .TakeWhile((float fillPercentage) => fillPercentage > 0)
        .TakeUntil(ipTrigger.OnSingleFingerDownAsObservable())
      )
      .Subscribe()
      .AddTo(this)
    ;

    ipTrigger.OnSingleFingerDownAsObservable()
      .SelectMany((_) => Observable.EveryFixedUpdate()
        .Select((__) => fillPercentage = Mathf.Clamp01(fillPercentage + increaseAmount))
        .Do((__) => mat.SetFloat("_Fillpercentage", fillPercentage))
        .TakeWhile((float fillPercentage) => fillPercentage < 1)
        .TakeUntil(ipTrigger.OnSingleFingerUpAsObservable())
      )
      .Subscribe()
      .AddTo(this)
    ;
  }
}
