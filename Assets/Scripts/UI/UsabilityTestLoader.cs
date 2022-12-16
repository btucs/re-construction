#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UsabilityTestLoader : MonoBehaviour
{
  [Required]
  public AreaDataSO ersatzmodellArea;
  [Required]
  public AreaDataSO drawForceArea;
  [Required]
  public AreaDataSO batmanArea;
  [Required]
  public UnlockDataSO worldUnlockData;

  public InputField developerConsole;
  private GameController controller;
  private string playerName;


  public void Start() {
    controller = GameController.GetInstance();
    developerConsole.onEndEdit.AddListener(delegate {
      LockInput(developerConsole);
    });
  }

  private void LockInput(InputField input) {
    string inputString = input.text.ToLower();
    if(inputString.Length > 2) {
      switch(inputString) {
        case "modell":
          LoadErsatzModellTask();
          break;
        case "zeichnen":
          LoadDrawForceTask();
          break;
        case "batman":
          LoadAnalyzeToolTask();
          break;
        case "godmode":
          OpenAllAreas();
          break;
        case "multiplayer":
          SceneManager.LoadScene("StartMultiplayer");
          break;
        case "savegame":
          DownloadSaveGame();
          break;

        default:
          Debug.Log("No entry found for entered string: " + inputString);
          break;
      }
    }
  }

  private void DownloadSaveGame()
  {
    controller.SaveGameInDocuments();
  }

  private void LoadErsatzModellTask() {
    Vector3 newPlayerPosition = new Vector3(-4f, 0f, 0f);
    SetupSceneForUXTest(ersatzmodellArea.sceneName, newPlayerPosition, 0, 0);
  }

  private void LoadDrawForceTask() {
    Vector3 newPlayerPosition = new Vector3(26f, 0f, 0f);
    SetupSceneForUXTest(drawForceArea.sceneName, newPlayerPosition, 0, 0);
  }

  private void LoadAnalyzeToolTask() {
    Vector3 newPlayerPosition = new Vector3(-16f, 0f, 0f);
    controller.gameState.onboardingData.konstruktorData.analyzeToolUnlocked = true;
    SetupSceneForUXTest(batmanArea.sceneName, newPlayerPosition, 3, 2);
  }

  private void OpenAllAreas() {
    // start probably not used anymore
    controller.gameState.onboardingData.bibUnlocked = true;
    controller.gameState.onboardingData.mapUnlocked = true;
    controller.gameState.onboardingData.profileUnlocked = true;
    controller.gameState.onboardingData.officeWelcomeFinished = true;
    // end probably not used anymore
    controller.gameState.onboardingData.konstruktorData.analyzeToolUnlocked = true;
    controller.gameState.onboardingData.konstruktorData.givenSearchedOnboardingFinished = true;
    controller.gameState.onboardingData.konstruktorData.konstruktorIntroductionFinished = true;
    controller.gameState.onboardingData.konstruktorData.modulesOnboardingFinished = true;

    controller.gameState.gameworldData.areas = CreateCompleteWorldAreaData();

    ScriptedEventDataSO[] events = controller.gameAssets.GetScriptedEvents();
    controller.gameState.onboardingData.finishedEvents = events
      .Select((ScriptedEventDataSO eventData) => eventData.UID)
      .ToList()
    ;

    controller.SaveGame();
    SceneManager.LoadScene("_Start");
  }

  private void SetPlayerPosition(WorldAreaData targetAreaData, Vector3 targetPosition) {
    playerName = playerScript.Instance.gameObject.name;
    bool characterSaveFound = false;
    foreach(NPCData NPCSaveData in targetAreaData.NPCs) {
      if(NPCSaveData.characterName == playerName) {
        NPCSaveData.worldPos = targetPosition;
        characterSaveFound = true;
      }
    }
    if(!characterSaveFound) {
      NPCData newCharacter = new NPCData();
      newCharacter.characterName = playerName;
      newCharacter.worldPos = targetPosition;
      targetAreaData.NPCs.Add(newCharacter);
    }
  }

  private void SetupSceneForUXTest(string areaName, Vector3 targetPlayerPos, int leftRoomBorder, int rightRoomBorder) {
    WorldAreaData targetAreaData = controller.gameState.gameworldData.FindAreaData(areaName);

    if(targetAreaData == null) {
      targetAreaData = CreateWorldAreaData(areaName);
    }

    SetPlayerPosition(targetAreaData, targetPlayerPos);
    SetRoomBorders(targetAreaData, leftRoomBorder, rightRoomBorder);

    UpdateControllerData(targetAreaData);
    controller.SaveGame();
    SceneManager.LoadScene(areaName);
  }

  private void SetRoomBorders(WorldAreaData targetAreaData, int left, int right) {
    targetAreaData.unlockedLeft = left;
    targetAreaData.unlockedRight = right;
  }

  private void UpdateControllerData(WorldAreaData targetAreaData) {
    bool areaSaveFound = false;
    foreach(WorldAreaData singleAreaData in controller.gameState.gameworldData.areas) {
      if(singleAreaData.sceneName == targetAreaData.sceneName) {
        singleAreaData.unlockedLeft = targetAreaData.unlockedLeft;
        singleAreaData.unlockedRight = targetAreaData.unlockedRight;
        singleAreaData.NPCs = targetAreaData.NPCs;
        areaSaveFound = true;
      }
    }
    if(!areaSaveFound) {
      controller.gameState.gameworldData.areas.Add(targetAreaData);
    }
  }

  private WorldAreaData CreateWorldAreaData(string targetSceneName) {

    return new WorldAreaData() {
      sceneName = targetSceneName
    };

  }

  private List<WorldAreaData> CreateCompleteWorldAreaData() {
    Dictionary<string, WorldAreaData> worldAreas = new Dictionary<string, WorldAreaData>();
    foreach(UnlockSpecification rule in worldUnlockData.unlockRules) {
      worldAreas.TryGetValue(rule.area.sceneName, out WorldAreaData area);
      if(area == null) {
        area = new WorldAreaData();
        area.sceneName = rule.area.sceneName;
        area.unlockedLeft = 0;
        area.unlockedRight = 0;
      }

      switch(rule.direction) {
        case AreaDirection.left:
          area.unlockedLeft = Mathf.Max(area.unlockedLeft, rule.index);
          break;
        case AreaDirection.right:
          area.unlockedRight = Mathf.Max(area.unlockedRight, rule.index);
          break;
      }

      worldAreas[rule.area.sceneName] = area;
    }

    return worldAreas.Values.ToList();
  }

}