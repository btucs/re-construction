#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SelectHandle : MonoBehaviour {

  private GameManager gameManager;

  private void Start() {

    SingleFingerInputTrigger trigger = GetComponent<SingleFingerInputTrigger>();
    trigger.OnSingleFingerDownAsObservable().Subscribe(OnSingleFingerTap).AddTo(this);

    gameManager = GameManager.instance;
  }

  private void OnSingleFingerTap(Vector2 position) {
    
    bool isSelected = gameManager.selectedObjectSource.Value == gameObject;
    gameManager.selectedObjectSource.OnNext(isSelected ? null : gameObject);
  }
}
