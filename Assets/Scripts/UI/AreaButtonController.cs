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

public class AreaButtonController : MonoBehaviour
{
	private string areaSceneString;
  private AreaDataSO currentArea;
  private GameController controller;

  public void LoadAreaScene()
  {
    controller = GameController.GetInstance();

    string sceneLoadName = AreaIntroCheck();
    if(Application.CanStreamedLevelBeLoaded(sceneLoadName))
    {
      if(controller.playerScript != null) {

        Vector3 pos = controller.playerScript.transform.position;
        controller.gameState.gameworldData.SetPlayerPos(pos);
      }

      controller.gameState.gameworldData.lastScene = areaSceneString;
      controller.SaveGame();
      SceneManager.LoadScene(sceneLoadName);
    } 
    else
    {
    	Debug.Log("No Scene with the name " + areaSceneString + "can be loaded.");
    } 
  }

  public string AreaIntroCheck()
  {
    string sceneToLoad = areaSceneString;
    Debug.Log("Checking for intro scene");
    Debug.Log("Target Scene " + currentArea.sceneName);
    if(currentArea != null && currentArea.sceneName == areaSceneString)
    {

      if(controller.gameState.gameworldData.WasSceneVisited(currentArea.introSceneName) == false && currentArea.introSceneName.Length > 1)
      {
        controller.gameState.gameworldData.MarkSceneVisited(currentArea.introSceneName);
        sceneToLoad = currentArea.introSceneName;
      }
    }

    return sceneToLoad;
  }

  public void SetSelectedArea(AreaDataSO newArea)
  {
     currentArea = newArea;
     areaSceneString = newArea.sceneName;
  }
}
