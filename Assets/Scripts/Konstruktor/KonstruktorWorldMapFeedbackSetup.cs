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
using Constants;

public class KonstruktorWorldMapFeedbackSetup : MonoBehaviour
{
	[Required]
	public Camera mainCamera;

	[Required]
	[AssetsOnly]
	public GameObject npcCharacterPrefab;

	[Required]
	public Button ContinueButton;

	private SceneManagement SceneManagement;
	private GameController controller;
	private TaskHistoryData saveData;
	private List<string> NPCFeedbackTexts = new List<string>();

	private void Start() {

		controller = GameController.GetInstance();
		saveData = controller.gameState.taskHistoryData;
		SceneManagement = this.GetComponent<SceneManagement>();

		KonstruktorSceneData konstructorData = controller.gameState.konstruktorSceneData;
		PositionCameraAndBoundaries(konstructorData);
		PlaceBackgroundAndInteractables(konstructorData);
		PlaceNPCCharacters(konstructorData);
		//dialogController.StartDialog();
    	string returnToSceneName = konstructorData.returnSceneName;

    	ContinueButton.onClick.AddListener(() => {
    		controller.gameState.konstruktorSceneData = new KonstruktorSceneData();
    		controller.SaveGame();
    		SceneManagement.LoadScene(returnToSceneName);
    	});
	}

	private void PlaceBackgroundAndInteractables(KonstruktorSceneData konstructorData) {

		GameObject background = Instantiate(konstructorData.backgroundPrefab);
		HelperFunctions.MoveToLayer(background.transform, Layers.background);
		foreach(KonstruktorSceneData.InteractableData data in konstructorData.interactablesPrefabs) {

    		GameObject interactableInstance = Instantiate(data.taskObject.objectPrefab);
    		HelperFunctions.MoveToLayer(interactableInstance.transform, Layers.background);
    		interactableInstance.transform.position = data.position;

    		WorldObjectResolver objectUpdater = interactableInstance.GetComponent<WorldObjectResolver>();
    		if(objectUpdater != null)
    		{
    			RequiredTaskState currentTaskState = objectUpdater.GetSolveStateOfTask(konstructorData.taskData, saveData);
    			Debug.Log(currentTaskState);
    			NPCFeedbackTexts = objectUpdater.GetFeedbackTextFromSolvedTask(konstructorData.taskData, currentTaskState);
    			objectUpdater.ResolveObjectStateFromSolvedTask(konstructorData.taskData, currentTaskState);
    		}
		}
	}

	private void PositionCameraAndBoundaries(KonstruktorSceneData konstructorData) {

		mainCamera.transform.position = new Vector3(konstructorData.cameraPosition.x, -1.23f, konstructorData.cameraPosition.z);
		mainCamera.orthographicSize = 3.6f;
	}

    private void PlaceNPCCharacters(KonstruktorSceneData konstructorData) {

		TaskDataSO taskData = konstructorData.taskData;
		TaskNPC[] npcs = taskData.taskNPCs;

		KonstruktorSceneData.InteractableData interactable = GetTaskObjectInteractable(konstructorData.taskObject, konstructorData.interactablesPrefabs);
    	Vector3 interactablePosition = interactable.position;

    	//Bounds interactableBounds = GetMaxBounds(konstructorData.taskObject.objectPrefab);
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

	    	GameObject npcCharacterInstance = Instantiate(npcCharacterPrefab);
	    	npcCharacterInstance.name = "NPC - " + taskNPC.npc.characterName;
	    	npcCharacterInstance.transform.position = new Vector3(npcXPos, 0, 0);
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

}
