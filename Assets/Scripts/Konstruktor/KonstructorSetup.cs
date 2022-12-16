#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class KonstructorSetup : MonoBehaviour {

  [Required]
  public Camera mainCamera;

  [Required]
  public TaskDialogHandler taskDialogHandler;

  [Required]
  public InputMenuController inputMenuController;
  [Required]
  public OutputMenuController outputMenuController;

  [Required]
  public KonstruktorVariableAnalysisController analysisController;

  [Required]
  public KonstruktorClassificationController classificationController;

  [Required]
  public KonstruktorDrawController drawController;

  [Required]
  [AssetsOnly]
  public GameObject npcCharacterPrefab;

  [Required]
  [AssetsOnly]
  public GameObject npcSpeechBubblePrefab;

  [Required]
  public GameObject hiddenInventoryItemPrefab;

  [Required]
  public HiddenItemFactory hiddenItemSpawner;

  public Text taskNameText;
  public TextMeshProUGUI taskDescriptionText;

  [Required]
  public Transform environmentContainer; 

  public GameObject postProcessingObj;

  private Transform speechBubbleContainer;

  private GameController controller;
  private List<GameObject> npcObjects = new List<GameObject>();
  private List<GameObject> speechBubbleObjects = new List<GameObject>();

  private void Awake()
  {
  	controller = GameController.GetInstance();
    if(controller.IsLoaded == false) {

      controller.LoadGame();
    }
  }

  private void Start() {
    speechBubbleContainer = environmentContainer.GetChild(0);

    KonstruktorSceneData konstructorData = controller.gameState.konstruktorSceneData;
    konstructorData.currentStep = -1;
    PositionCamera(konstructorData);
    PlaceBackgroundAndInteractables(konstructorData);
    PlaceNPCCharacters(konstructorData);
    SetUIContent(konstructorData);
    PlaceHiddenInventoryItems(konstructorData);
    InitializeAnalysisPanel();

    bool displayVFX = (konstructorData.returnSceneName == "TeachAndPlayScene");
    if(postProcessingObj != null)
    {
      postProcessingObj.SetActive(displayVFX);
    }
  }

  private void SetUIContent(KonstruktorSceneData konstructorData)
  {
    if(taskNameText != null) {

      taskNameText.text = konstructorData.taskData.taskName;
    }

    if(taskDescriptionText != null) {

      taskDescriptionText.text = konstructorData.taskData.fullDescription;
    }
  }

  private void PlaceBackgroundAndInteractables(KonstruktorSceneData konstructorData) {

    Instantiate(konstructorData.backgroundPrefab, environmentContainer);

    foreach(KonstruktorSceneData.InteractableData data in konstructorData.interactablesPrefabs) {

      GameObject interactableInstance = Instantiate(data.taskObject.objectPrefab, environmentContainer);
      interactableInstance.transform.position = data.position;

      ReplacementModelController replacementModelController = interactableInstance.GetComponent<ReplacementModelController>();
      if(replacementModelController != null) {

        classificationController.replacementModelController = replacementModelController;
      }

      SpriteTransparencyController spriteTransparency = interactableInstance.GetComponent<SpriteTransparencyController>();
      if(spriteTransparency != null) {

        drawController.spriteTransparencyController = spriteTransparency;
      }
    }
  }

  private void PositionCamera(KonstruktorSceneData konstructorData) {

    mainCamera.transform.position = new Vector3(konstructorData.cameraPosition.x, konstructorData.cameraPosition.y, konstructorData.cameraPosition.z);
    mainCamera.orthographicSize = 4f / konstructorData.cameraZoomFactor;
  }

  private void PlaceNPCCharacters(KonstruktorSceneData konstructorData) {

    TaskDataSO taskData = konstructorData.taskData;
    TaskNPC[] npcs = taskData.taskNPCs;

    KonstruktorSceneData.InteractableData interactable = GetTaskObjectInteractable(konstructorData.taskObject, konstructorData.interactablesPrefabs);
    Vector3 interactablePosition = interactable.position;
    
    BoxCollider interactableCollider = konstructorData.taskObject.objectPrefab.GetComponent<BoxCollider>();
    float currentDistance = 0;
    if(interactableCollider != null) {

      currentDistance = (interactableCollider.size.x / 2) + 0.5f;
    } else {
      // use bounds in case there is no collider 
      Bounds interactableBounds = GetMaxBounds(konstructorData.taskObject.objectPrefab);
      currentDistance = (interactableBounds.size.x / 2) + 0.5f;
    }

    float npcSpacing = 1.5f;

    for(int i = 0; i < npcs.Length; i++) {

      TaskNPC taskNPC = npcs[i];
      int leftRight = i % 2 == 0 ? -1 : 1;
      float npcXPos = interactablePosition.x + (leftRight * currentDistance);

      if(i % 2 == 1) {
        // next npc character on the left side will be moved further to the left
        currentDistance += npcSpacing;
      }

      characterGraphicsUpdater graphicsUpdaterComponent = npcCharacterPrefab.GetComponentInChildren<characterGraphicsUpdater>();
      graphicsUpdaterComponent.characterSOData.Value = taskNPC.npc;

      GameObject npcCharacterInstance = Instantiate(npcCharacterPrefab, environmentContainer);
      npcCharacterInstance.name = "NPC - " + taskNPC.npc.characterName;
      npcCharacterInstance.transform.position = new Vector3(npcXPos, 0, 0);
      npcCharacterInstance.transform.localScale = new Vector3(konstructorData.npcScale, konstructorData.npcScale, 1f);
      npcObjects.Add(npcCharacterInstance);
      

      //Destroy(npcCharacterInstance.GetComponent<characterMovement>());
      PlaceSpeechBubble(npcCharacterInstance.transform, taskData, taskNPC);
    }
  }

  private void PlaceSpeechBubble(Transform npcBody, TaskDataSO taskData, TaskNPC taskNPC) {

    Vector3 bubblePos = new Vector3(npcBody.position.x - 0.13f, 3.7f * npcBody.localScale.y, 0f);
    if(npcSpeechBubblePrefab == null) {

      return;
    }

    GameObject bubbleInstance = Instantiate(npcSpeechBubblePrefab, speechBubbleContainer);
    speechBubbleObjects.Add(bubbleInstance);
    bubbleInstance.transform.position = bubblePos;
    ClickHandler clickHandler = bubbleInstance.GetComponent<ClickHandler>();
    clickHandler.OnClick.AddListener((PointerEventData) => {

      // taskData has to be first, is used by taskNPC
      taskDialogHandler.taskData.Value = taskData;
      taskDialogHandler.taskNPC.Value = taskNPC;
      taskDialogHandler.Show();
    });
  }

  private void PlaceHiddenInventoryItems(KonstruktorSceneData konstructorData)
  {
    KonstruktorSceneData.InteractableData interactable = GetTaskObjectInteractable(konstructorData.taskObject, konstructorData.interactablesPrefabs);
    Vector3 interactablePosition = interactable.position;

    TaskDataSO currentTask = konstructorData.taskData;
    TaskObjectSO currentTaskObj = konstructorData.taskObject;

    hiddenItemSpawner.Setup();

    foreach(ObjectTaskData taskInfo in currentTaskObj.taskInfos)
    {
      if(taskInfo.task == currentTask)
      {
       foreach(HiddenTaskVariable hiddenVarData in taskInfo.hiddenVariables)
       {
        GameObject hiddenObj = Instantiate(hiddenInventoryItemPrefab);
        
        Vector3 itemPos = new Vector3(
          interactable.position.x + hiddenVarData.localAnalyzePosition.x, 
          interactable.position.y + hiddenVarData.localAnalyzePosition.y, 
          0f);

        hiddenObj.transform.position = itemPos;
        
        HiddenItem hiddenItemScript = hiddenObj.GetComponent<HiddenItem>();
        hiddenItemScript.Setup(hiddenVarData.itemData, hiddenItemSpawner);
       } 
      }
    } 
  }

  private void InitializeAnalysisPanel() {

    if(inputMenuController != null) {

      inputMenuController.onDrop.AddListener(HandleItemDrop);
    }

    if(outputMenuController != null) {

      outputMenuController.onDrop.AddListener(HandleItemDrop);
    }
  }

  private KonstruktorSceneData.InteractableData GetTaskObjectInteractable(TaskObjectSO taskObject, KonstruktorSceneData.InteractableData[] interactables) {

    return interactables.FirstOrDefault((KonstruktorSceneData.InteractableData interactable) => interactable.taskObject == taskObject);
  }

  private Bounds GetMaxBounds(GameObject parent) {

    Bounds bounds = default(Bounds);
    Renderer[] childRenderers = parent.GetComponentsInChildren<Renderer>();
    if(childRenderers.Length > 0) {

      bounds = childRenderers[0].bounds;
      for(int i = 1; i < childRenderers.Length; i++) {

        bounds.Encapsulate(childRenderers[i].bounds);
      }
    }
    
    return bounds;
  }

    public List<characterMovement> GetNPCReferences()
    {
        List<characterMovement> moveControllers = new List<characterMovement>();
        foreach(GameObject npc in npcObjects)
        {
            characterMovement moveController = npc.GetComponent<characterMovement>();
            if (moveController != null)
            {
                moveControllers.Add(moveController);
            }
        }
        return moveControllers;
    }

  private void HandleItemDrop(InventoryItem item) {

    item.OnTapEvent.AddListener(ShowAnalysisPanel);
  }

  private void ShowAnalysisPanel(InventoryItem item) {

    analysisController.SetDisplayContent(item);
    analysisController.gameObject.SetActive(true);
  }

  public void SetSpeechBubblesActive(bool nowActive)
  {
    foreach(GameObject bubbleObj in speechBubbleObjects)
    {
      bubbleObj.SetActive(nowActive);
    }
  }
}
