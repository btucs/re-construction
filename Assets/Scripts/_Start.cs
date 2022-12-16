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
using UnityEngine.SceneManagement;

public class _Start : MonoBehaviour {

  public string startScene;
  public string continueScene;

  private void Start() {
    Screen.sleepTimeout = SleepTimeout.NeverSleep;
    if(Application.CanStreamedLevelBeLoaded("WelcomeScreen"))
    {
      SceneManager.LoadScene("WelcomeScreen");
    } else {
      LoadPreviewsGameScene();
    }
    
  }

  private void LoadPreviewsGameScene()
  {
    GameController gameController = GameController.GetInstance();

    bool savegameExists = gameController.SavegameExists();

    if(savegameExists == true) {

      if(gameController.IsLoaded == false) {

        gameController.LoadGame();
      }

      bool isNewPlayer = String.IsNullOrEmpty(gameController.gameState.profileData.playerName);

      string lastScene = gameController.gameState.gameworldData.lastScene;
      if(lastScene == "OfficeStartScene") {

        lastScene = continueScene;
      }

      if(lastScene != null && Application.CanStreamedLevelBeLoaded(lastScene)) {

        continueScene = lastScene;
      }

      SceneManager.LoadScene(isNewPlayer == true ? startScene : continueScene);
    } else {

      SceneManager.LoadScene(startScene);
    }
  }
}
