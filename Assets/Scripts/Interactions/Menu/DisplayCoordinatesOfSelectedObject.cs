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
using TMPro;

public class DisplayCoordinatesOfSelectedObject : MonoBehaviour {

  TextMeshProUGUI textComponent;

  private void Start() {

    textComponent = GetComponent<TextMeshProUGUI>();

    GameManager.instance.selectedObject
      .SelectMany(DisplayCoordinates)
      .Subscribe()
      .AddTo(this)
    ;
  }

  private IObservable<GameObject> DisplayCoordinates(GameObject selectedObject) {

    if (selectedObject == null) {

      textComponent.text = "";

      return Observable.Return(selectedObject);
    }

    return selectedObject.transform
      .ObserveEveryValueChanged((Transform transform) => transform.position)
      .Do((Vector3 position) => 
        textComponent.text = $"({ position.x };{position.y})"
      )
      .Select((_) => selectedObject)
    ;
  }
}
