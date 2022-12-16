#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldTaskButtonHandler : MonoBehaviour {

  public TaskDataSO taskData;
  public TaskObjectSO taskObject;
  public string konstructorSceneName = "KonstruktorTest";

  public Button startButton;
  public Text taskName;
  public Text teaser;

  private void Start() {
    startButton.onClick.AddListener(HandleClick);

    name = "Aufgabenbutton - " + taskData.taskName;
    taskName.text = taskData.taskName;
    teaser.text = taskData.teaserDescription;
  }

  private void HandleClick() {

    GameController controller = GameController.GetInstance();
    controller.gameState.konstruktorSceneData.taskData = taskData;
    controller.gameState.konstruktorSceneData.taskObject = taskObject;

    // save data, such that it can be loaded again on the next scene
    // gameobjects are getting destroyed when changing and they are
    // created again on load
    controller.SaveGame();
    SceneManager.LoadScene(konstructorSceneName);
  }
}
