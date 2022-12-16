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
using Constants;

public class KonstructorAssessmentSetup : MonoBehaviour {

  [Required]
  public Camera mainCamera;
  [Required]
  public Button ContinueButton;
  [Required]
  public SceneManagement SceneManagement;

  private GameController controller;

  private void Start() {

    controller = GameController.GetInstance();

    KonstruktorSceneData konstructorData = controller.gameState.konstruktorSceneData;
    PositionCameraAndBoundaries(konstructorData);
    PlaceBackgroundAndInteractables(konstructorData);

    string returnToSceneName = konstructorData.returnSceneName;

    ContinueButton.onClick.AddListener(() => {
      controller.SaveGame();
      //SceneManagement.LoadScene(returnToSceneName);
    });
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

  private void PositionCameraAndBoundaries(KonstruktorSceneData konstructorData)
  {
    mainCamera.transform.position = new Vector3(konstructorData.cameraPosition.x, konstructorData.cameraPosition.y, konstructorData.cameraPosition.z);
    mainCamera.orthographicSize = 4f / konstructorData.cameraZoomFactor;
  }
}
