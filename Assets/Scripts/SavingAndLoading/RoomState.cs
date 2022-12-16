#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class RoomState : MonoBehaviour
{
  public WorldSceneCameraController cameraScript;
  public playerScript interactionScript;
  public SceneIntroduction sceneIntroController;
  public GameObject NPCPrefab;

  public List<Transform> areaWaypointsLeft;
  public List<Transform> areaWaypointsRight;
  [HideInInspector]
  public int activeLeft = 0;
  [HideInInspector]
  public int activeRight = 0;
  public float cameraBorderOffset = 2.5f;
  public float cameraStartYOffset = 1f;
  public float animDuration = 2f;
  public Transform leftBorder;
  public Transform rightBorder;

  public GameObject moveScreenHintObj;

  private Vector3 currentLeftPos;
  private Vector3 currentRightPos;
  private new bool animation = false;
  private bool camObservation = false;
  private Vector3 camStartPos;
  private float camOffsetValue = 1f;

  private Camera sceneCamera;
  private float timeSinceAnimStart;

  public List<GameWorldObject> objectScripts = new List<GameWorldObject>();
  private TaskHistoryData playerTaskHistory;
  private WorldAreaData areaSaveData;

  public UnlockDataSO unlockData;
  private List<UnlockSpecification> areaStateConditions;
  private string currentSceneName;
  private GameController saveController;

  private UnityAction camMoveAction;

  public List<GameObject> sceneCharacters = new List<GameObject>();

  private static RoomState instance;
  public static RoomState Instance {
    get { return instance; }
  }

  private void Awake ()
  {
    if (instance == null) { instance = this; }
    else if (instance != this) { Destroy(this.gameObject); }
  }

  private void Update() {
    if(animation)
    {
      if(CompareFloats(currentRightPos.x, rightBorder.position.x) == false || CompareFloats(currentLeftPos.x, leftBorder.position.x) == false)
      {
        timeSinceAnimStart += Time.deltaTime;
        if(timeSinceAnimStart > animDuration) {
          timeSinceAnimStart = animDuration;
        }

        rightBorder.position = new Vector3(
          Mathf.Lerp(rightBorder.position.x, currentRightPos.x, timeSinceAnimStart / animDuration),
          rightBorder.position.y,
          rightBorder.position.z);

        leftBorder.position = new Vector3(
          Mathf.Lerp(leftBorder.position.x, currentLeftPos.x, timeSinceAnimStart / animDuration),
          leftBorder.position.y,
          leftBorder.position.z);
      } else {
        animation = false;
        sceneIntroController.onHold = false;
        interactionScript.EnableInteraction();
      }
    }

    
  }

  private bool CompareFloats(float a, float b)
  {
    return (a > b - 0.05f && a < b + 0.05f);
  }


  private void ShowCamMoveTutorial()
  {
    if(moveScreenHintObj != null && activeRight == 1)
    {
      moveScreenHintObj.SetActive(true);
      camMoveAction = null;
      camMoveAction += HideCamMoveTutorial;
      interactionScript.onDragCamera.AddListener(camMoveAction);
      MenuUIController.Instance.worldSpaceUI.SetWorldSpaceUIActive(false);
    }
  }

  private void HideCamMoveTutorial()
  {
    moveScreenHintObj.SetActive(false);
    interactionScript.onDragCamera.RemoveListener(camMoveAction);
    MenuUIController.Instance.worldSpaceUI.SetWorldSpaceUIActive(true);
  }

  //This function is crucial and has to be called when loading the scene.
  //it is currently called in SceneIntroduction-Script to handle the sequence in which different Scripts execute when loading the scene
  public void InitiateGameWorld()
  {
    saveController = GameController.GetInstance();
    playerTaskHistory = saveController.gameState.taskHistoryData;
    
    currentSceneName = SceneManager.GetActiveScene().name;

    sceneCamera = cameraScript.GetComponent<Camera>();
    areaStateConditions = unlockData.GetRulesBySceneName(currentSceneName);

    TryFindAreaSaveData(saveController.gameState.gameworldData);
    LoadSceneCharacterStates();
    SetWorldObjectStates();
    SetActiveBorders();
    UpdateRoomBorders();
    SetCameraPosToPlayer();
  }

  public bool HasAreaNewContent(AreaDataSO area)
  {
    List<UnlockSpecification> unlockRulesForArea = unlockData.GetRulesBySceneName(area.sceneName);
    foreach(UnlockSpecification rule in unlockRulesForArea)
    {

    }
    return false;
  }

  public List<UnlockSpecification> GetUnlockRulesFor(AreaDataSO area)
  {
    List<UnlockSpecification> returnData = unlockData.GetRulesBySceneName(area.sceneName);
    return returnData;
  }

  private void SetCameraPosToPlayer() {
    Vector3 goalPos = new Vector3(
      interactionScript.transform.position.x,
      interactionScript.transform.position.y + cameraStartYOffset,
      sceneCamera.transform.position.z
    );

    sceneCamera.transform.position = goalPos;
    
    if(cameraScript)
    {
        cameraScript.SetGoalPosition(goalPos);
    }
  }

  public void TryFindAreaSaveData(WorldData _gameworldData) {
    bool areaDataFound = false;
    
    foreach(WorldAreaData singleWorldArea in _gameworldData.areas) {
      if(singleWorldArea.sceneName == currentSceneName) {
        areaSaveData = singleWorldArea;
        Debug.Log("Set to existing");
        areaDataFound = true;
      }
    }
    if(!areaDataFound) {
      Debug.Log("No Area Data Found");
      areaSaveData = new WorldAreaData();
      areaSaveData.sceneName = currentSceneName;
      //areaSaveData.NPCs = new List<NPCData>();
    }
  }

  public void LoadSceneCharacterStates() {
    foreach(GameObject sceneCharacter in sceneCharacters) {
      foreach(NPCData NPCSaveData in areaSaveData.NPCs) {
        if(NPCSaveData.characterName == sceneCharacter.name) {
          //Debug.Log("Found save Data for Character for " + NPCSaveData.characterName);
          Vector3 scenePosition = sceneCharacter.transform.position;
          // use only stored x-position to avoid floating characters
          sceneCharacter.transform.position = new Vector3(NPCSaveData.worldPos.x, scenePosition.y);
          //break; 
          characterMovement moveScript = sceneCharacter.GetComponent<characterMovement>();
          if(moveScript) {
            moveScript.goalPosition = sceneCharacter.transform.position;
          }
        }
      }

    }
  }

  public void SaveSceneState() {
    GameController saveController = GameController.GetInstance();

    UpdateCurrentAreaSaveData();

    bool areaSaveFound = false;
    foreach(WorldAreaData singleAreaData in saveController.gameState.gameworldData.areas) {
      //Debug.Log("Found Save-Entry for this Area");
      if(singleAreaData.sceneName == areaSaveData.sceneName) {
        singleAreaData.unlockedLeft = activeLeft;
        singleAreaData.unlockedRight = activeRight;
        singleAreaData.NPCs = areaSaveData.NPCs;
        singleAreaData.objects = areaSaveData.objects;
        areaSaveFound = true;
      }
    }
    if(!areaSaveFound) {
      //Debug.Log("No prev save-Entry for this area");
      saveController.gameState.gameworldData.areas.Add(areaSaveData);
    }
    saveController.SaveGame();
  }

  public void SetActiveBorders() {
    //TO DO: mle data gegen taskdata austauschen
    /*foreach(UnlockAreaCondition areaStateCondition in areaStateConditions)
    {
        foreach(FinishedTaskData finishedTask in playerTaskHistory.taskHistoryData)
        {
            //To DO: zusätzlich Bedingung, wie Mindestpunktzahl hinzufügen
            if(finishedTask.solvedTask.taskName == areaStateCondition.requiredTask.taskName)
            {
                if(areaStateCondition.unlockLeft > activeLeft)
                    activeLeft = areaStateCondition.unlockLeft;
                if(areaStateCondition.unlockRight > activeRight)
                    activeRight = areaStateCondition.unlockRight;
            }
        }
    }*/
    activeLeft = areaSaveData.unlockedLeft;
    activeRight = areaSaveData.unlockedRight;

  }

  public void UpdateRoomBorders() {

    int limitedRight = activeRight < areaWaypointsRight.Count ? activeRight : areaWaypointsRight.Count - 1;
    rightBorder.position = new Vector3 (areaWaypointsRight[limitedRight].position.x, rightBorder.position.y, 0);
    currentRightPos = rightBorder.position;

    int limitedLeft = activeLeft < areaWaypointsLeft.Count ? activeLeft : areaWaypointsLeft.Count - 1;
    leftBorder.position = new Vector3 (areaWaypointsLeft[limitedLeft].position.x, leftBorder.position.y, 0);
    currentLeftPos = leftBorder.position;
    UpdateCameraBounds();
  }

  public void UpdateRoomBorderToMapUnlock() {
    rightBorder.position = areaWaypointsRight[2].position;
    currentRightPos = rightBorder.position;
    leftBorder.position = areaWaypointsLeft[0].position;
    currentLeftPos = leftBorder.position;
    UpdateCameraBounds();
  }

  public bool CheckForNewLeftArea()
  {
    foreach(UnlockSpecification unlockRule in areaStateConditions) {
      if(unlockRule.direction == AreaDirection.left && unlockRule.index > activeLeft)
      { 
        if(unlockRule.condition.ConditionMet(saveController))
          return true;
      }
    }
    return false;
  }

  public bool CheckForNewRightArea()
  {
    foreach(UnlockSpecification unlockRule in areaStateConditions) {
      if(unlockRule.direction == AreaDirection.right && unlockRule.index > activeRight)
      { 
        if(unlockRule.condition.ConditionMet(saveController))
          return true;
      }
    }
    return false;
  }

  public void UnlockLeftArea() {
    if(areaWaypointsLeft.Count > activeLeft + 1) {
      activeLeft++;
      timeSinceAnimStart = 0f;
      animation = true;
      currentLeftPos = new Vector3(areaWaypointsLeft[activeLeft].position.x, leftBorder.position.y, leftBorder.position.z);

      UpdateCameraBounds();

      cameraScript.SetGoalPosition(leftBorder.position);
      SaveSceneState();
    }
  }

  public void UnlockRightArea() {
    if(areaWaypointsRight.Count > activeRight + 1) {
      activeRight++;
      timeSinceAnimStart = 0f;
      animation = true;
      currentRightPos = new Vector3(areaWaypointsRight[activeRight].position.x, rightBorder.position.y, rightBorder.position.z);

      UpdateCameraBounds();

      cameraScript.SetGoalPosition(rightBorder.position);
      ShowCamMoveTutorial();
      SaveSceneState();
    }
  }

  public void UpdateCameraBounds() {
    float cameraWidth = sceneCamera.aspect * sceneCamera.orthographicSize;
    Vector2 boundsVector = new Vector2(currentLeftPos.x - cameraBorderOffset + cameraWidth, currentRightPos.x + cameraBorderOffset - cameraWidth);
    cameraScript.UpdateCameraBoundsX(boundsVector);
  }

  public void SetWorldObjectStates() {
    foreach(GameWorldObject objectScript in objectScripts)
    {
      WorldObjectData saveData = areaSaveData.GetSaveDataOfObject(objectScript);
      objectScript.Setup(playerTaskHistory, saveData);
    }
  }

  private void UpdateCurrentAreaSaveData()
  {
    foreach(GameObject sceneCharacter in sceneCharacters) {
      bool characterSaveFound = false;
      foreach(NPCData NPCSaveData in areaSaveData.NPCs) {
        if(NPCSaveData.characterName == sceneCharacter.name) {
          //Debug.Log("Found existing characterData while Saving");
          NPCSaveData.worldPos = sceneCharacter.transform.position;
          characterSaveFound = true;
        }
      }
      if(!characterSaveFound) {
        //Debug.Log("Creating new Character to save");
        NPCData newCharacter = new NPCData();
        newCharacter.characterName = sceneCharacter.name;
        newCharacter.worldPos = sceneCharacter.transform.position;

        areaSaveData.NPCs.Add(newCharacter);
      }
    }
    areaSaveData.unlockedLeft = activeLeft;
    areaSaveData.unlockedRight = activeRight;
    
    foreach(GameWorldObject sceneObject in objectScripts)
    {
      areaSaveData.UpdateOrAddObjectData(sceneObject);
    }

  }
  
}

[System.Serializable]
public class CharacterRef
{
  public Vector3 worldPos;
  public string characterName;
}

