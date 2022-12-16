#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(SingleFingerInputTrigger))]
[Obsolete()]
public class PlaceHandle: MonoBehaviour {

  public GameObject handlePrefab;

  private GameManager gameManager;
  private GridManager gridManager;
  
  void Start() {

    gameManager = GameManager.instance;
    gridManager = GridManager.instance;

    SingleFingerInputTrigger trigger = gameObject.GetComponent<SingleFingerInputTrigger>();

    trigger.OnSingleFingerTapAsObservable().Subscribe(OnSingleFingerUp).AddTo(this);
  }

  private void OnSingleFingerUp(Vector2 position) {

    if (gameManager.contextMenu.gameObject.activeInHierarchy == true) {

      gameManager.contextMenu.gameObject.SetActive(false);
    } else {

      Vector3 localPosition = gridManager.GetCameraPosition(position, gameObject);
      localPosition.z = -0.1f;
      GameObject handleInstance = Instantiate<GameObject>(handlePrefab, localPosition, Quaternion.identity);

      //gameManager.selectedObject = handleInstance;
      gameManager.contextMenu.gameObject.SetActive(true);
    }
  }
}
