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
using Sirenix.OdinInspector;
using TMPro;
using Constants;

public class TaskSolvingSetup : MonoBehaviour {

  [Required]
  public Text taskNameText;
  [Required]
  public TextMeshProUGUI taskDescriptionText;
  [Required]
  public Camera backgroundCamera;
  [Required]
  public DropArea outputModuleArea;
  [Required]
  public OutputMenuController outputMenuController;
  [Required]
  public GameObject converterPanel;

  private GameController controller;

  private void Start() {

    controller = GameController.GetInstance();

    KonstruktorSceneData konstructorData = controller.gameState.konstruktorSceneData;

    PositionCameraAndBoundaries(konstructorData);
    PlaceBackgroundAndInteractables(konstructorData);
    SetUIContent(konstructorData);
    PlaceOutputItem(konstructorData);

    converterPanel.SetActive(true);
  }

  private void SetUIContent(KonstruktorSceneData konstructorData) {
    taskNameText.text = konstructorData.taskData.taskName;
    taskDescriptionText.text = konstructorData.taskData.fullDescription;
  }

  private void PlaceBackgroundAndInteractables(KonstruktorSceneData konstructorData) {

    GameObject background = Instantiate(konstructorData.backgroundPrefab);
    HelperFunctions.MoveToLayer(background.transform, Layers.background);

    foreach(KonstruktorSceneData.InteractableData data in konstructorData.interactablesPrefabs) {

      GameObject interactableInstance = Instantiate(data.taskObject.objectPrefab);
      HelperFunctions.MoveToLayer(interactableInstance.transform, Layers.background);
      interactableInstance.transform.position = data.position;
    }
  }

  private void PositionCameraAndBoundaries(KonstruktorSceneData konstructorData) {

    backgroundCamera.transform.position = konstructorData.cameraPosition;
    backgroundCamera.orthographicSize = 2.6f;
  }

  private void PlaceOutputItem(KonstruktorSceneData konstructorData) {

    if(konstructorData.currentStep != -1) {

      OutputMenuItemController[] outputItemController = outputMenuController.GetOutputItemControllers();
      InventoryItem currentStepItem = outputItemController[konstructorData.currentStep].droppedItem;

      outputModuleArea.DropItem(currentStepItem);
    }
  }
}
