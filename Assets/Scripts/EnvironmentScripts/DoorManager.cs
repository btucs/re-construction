#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;

public class DoorManager : MonoBehaviour
{
  public Vector3 newAreaGoalPos = new Vector3();
  public string targetSceneName;
  public GameObject playerObj;
  public SceneManagement mySceneManager;
  public GameObject noAccessPopup;

  public void PrepareAndLoadTargetScene() {

    GameController controller = GameController.GetInstance();
    WorldAreaData targetAreaData = controller.gameState.gameworldData.FindAreaData(targetSceneName);
    if(targetAreaData == null) {

      targetAreaData = CreateWorldAreaData(targetSceneName);
      controller.gameState.gameworldData.areas.Add(targetAreaData);
    }

    SetPlayerGoalPos(targetAreaData);
    controller.gameState.gameworldData.lastScene = targetSceneName;
    
    RoomState areaController = RoomState.Instance;
    if(areaController != null)
    {
      areaController.SaveSceneState();
    } else {
      controller.SaveGame();
    }
    //Debug.Log("Now Loading Scene");
    mySceneManager.LoadSceneWithFade(targetSceneName);
  }

  public void TryAccessTargetScene()
  {
    if(GameController.GetInstance().gameState.course != null)
    {
      PrepareAndLoadTargetScene();
    } else {
      noAccessPopup.SetActive(true);
    }
  }

  private void SetPlayerGoalPos(WorldAreaData targetAreaData) {
    bool characterSaveFound = false;
    foreach(NPCData NPCSaveData in targetAreaData.NPCs) {
      if(NPCSaveData.characterName == playerObj.name) {
        NPCSaveData.worldPos = newAreaGoalPos;
        characterSaveFound = true;
      }
    }
    if(!characterSaveFound) {
      NPCData newCharacter = new NPCData();
      newCharacter.characterName = playerObj.name;
      newCharacter.worldPos = newAreaGoalPos;
      targetAreaData.NPCs.Add(newCharacter);
    }
  }

  private WorldAreaData CreateWorldAreaData(string targetSceneName) {

    return new WorldAreaData() {
      sceneName = targetSceneName
    };
  }
}
