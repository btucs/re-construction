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

public class KonstruktorObjectFeedbackSetup : MonoBehaviour
{
	[Required]
	public Camera mainCamera;

	[Required]
	[AssetsOnly]
	public GameObject npcCharacterPrefab;

	[Required]
	[AssetsOnly]
	public GameObject npcSpeechBubblePrefab;

	[Required]
	public Button ContinueButton;

	public KonstruktorDialogController dialogController;
	public ScriptedEventDataSO mapUnlock;
	public TaskTweetResolver tweetController;
	public WorldMapController mapController;
	public MLEQuestionController questionController;
	public MLEFeedbackController feedbackController;
	public GameObject questionUI;

	public RectTransform worldMapObj;
	public GameObject tweetObj;

	private SceneManagement SceneManagement;
	private GameController controller;
	private TaskHistoryData saveData;
	private List<string> NPCFeedbackTexts = new List<string>();
	private RequiredTaskState currentTaskState;
	private string returnToSceneName;
	private int partOfFeedback = 0;
	private GameObject speechBubbleObjRef; 

	private void Start() {

		controller = GameController.GetInstance();
		saveData = controller.gameState.taskHistoryData;
		SceneManagement = this.GetComponent<SceneManagement>();

		KonstruktorSceneData konstructorData = controller.gameState.konstruktorSceneData;
		PositionCameraAndBoundaries(konstructorData);
		PlaceBackgroundAndInteractables(konstructorData); //currentTaskState is set here
		PlaceNPCCharacters(konstructorData);
		//dialogController.StartDialog();
    	returnToSceneName = konstructorData.returnSceneName;

    	//ContinueButton.onClick.AddListener(() => {
    	//	controller.gameState.konstruktorSceneData = new KonstruktorSceneData();
    	//	controller.SaveGame();
    	//	SceneManagement.LoadScene(returnToSceneName);
    	//});
    	worldMapObj.anchoredPosition = new Vector2 (worldMapObj.anchoredPosition.x + 2000f, worldMapObj.anchoredPosition.y);
    	questionController.OnSubmit.AddListener(OnSubmitSingleChoiceAnswer);
	}

	public void HandleContinueButtonClick()
	{
		partOfFeedback++;
		if(returnToSceneName == "TeachAndPlayScene")
		{
			if(controller.gameState.konstruktorSceneData.followUpQuestions != null && controller.gameState.konstruktorSceneData.followUpQuestions.Length >= partOfFeedback)
			{
				questionController.SetQuestion(controller.gameState.konstruktorSceneData.followUpQuestions[partOfFeedback-1]);
				questionController.gameObject.SetActive(true);
				questionUI.SetActive(true);
				feedbackController.gameObject.SetActive(false);
				ContinueButton.gameObject.SetActive(false);
				return;
			} else {
				controller.gameState.konstruktorSceneData = new KonstruktorSceneData();
	    		controller.SaveGame();
	    		SceneManagement.LoadScene(returnToSceneName);
			}
		}

		switch(partOfFeedback)
		{
			case(1): 
				speechBubbleObjRef.SetActive(false);
				tweetController.UpdateTweetDisplay();
				tweetObj.SetActive(true);
				break;
			case(2):
			if(controller.gameState.onboardingData.EventEntryExists(mapUnlock.UID))
			{
				bool improve = currentTaskState == RequiredTaskState.taskSolvedCorrect;
				worldMapObj.anchoredPosition = new Vector2 (worldMapObj.anchoredPosition.x - 2000f, worldMapObj.anchoredPosition.y);
    			mapController.QueueActiveAreaChange(20, improve);
				mapController.UpdateWorldState();
				break;
			} else {
				controller.gameState.konstruktorSceneData = new KonstruktorSceneData();
    			controller.SaveGame();
    			SceneManagement.LoadScene(returnToSceneName);
    			break;
			}
				
			default:
				controller.gameState.konstruktorSceneData = new KonstruktorSceneData();
    			controller.SaveGame();
    			SceneManagement.LoadScene(returnToSceneName);
    			break;
		}
	}

	private void OnSubmitSingleChoiceAnswer(MLEQuiz quiz, MLEQuizChoice selectedAnswer) {

	questionController.gameObject.SetActive(false);

    feedbackController.gameObject.SetActive(true);
    feedbackController.SetView(
      quiz,
      selectedAnswer
    );

    if(selectedAnswer.isAnswer) {
      //reward
      //MLEAnimationController.DisplayCorrectAnswer();
    	Debug.Log("display correct answer feedback");
    } else {
      //MLEAnimationController.DisplayWrongAnswer();
    	Debug.Log("display wrong answer feedback");
    }
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
    			if(konstructorData.returnSceneName == "TeachAndPlayScene")
    			{
    				currentTaskState = objectUpdater.GetSolveStateFromCourseData(konstructorData.taskData, saveData);
    			} else {
					currentTaskState = objectUpdater.GetSolveStateOfTask(konstructorData.taskData, saveData);
    			}
    			NPCFeedbackTexts = objectUpdater.GetFeedbackTextFromSolvedTask(konstructorData.taskData, currentTaskState);
    			objectUpdater.ResolveObjectStateFromSolvedTask(konstructorData.taskData, currentTaskState);
    		}
		}
	}

	private void PositionCameraAndBoundaries(KonstruktorSceneData konstructorData)
	{
		mainCamera.transform.position = new Vector3(konstructorData.cameraPosition.x, konstructorData.cameraPosition.y, konstructorData.cameraPosition.z);
		mainCamera.orthographicSize = 4f / konstructorData.cameraZoomFactor;
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
	    	npcCharacterInstance.transform.localScale = new Vector3(konstructorData.npcScale, konstructorData.npcScale, 1f);

	    	if(i==0)
	    		PlaceFeedbackSpeechBubble(npcCharacterInstance.transform, taskData, taskNPC);
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

	private void PlaceFeedbackSpeechBubble(Transform npcTransform, TaskDataSO taskData, TaskNPC taskNPC) {

		Vector3 bubblePos = new Vector3(npcTransform.position.x - 0.13f, 3.85f * npcTransform.localScale.y, 0);
		if(npcSpeechBubblePrefab == null) {
			return;
		}

    	speechBubbleObjRef = Instantiate(npcSpeechBubblePrefab);
    	speechBubbleObjRef.transform.position = bubblePos;

    	dialogController.SetDialogContent(NPCFeedbackTexts, speechBubbleObjRef, npcTransform);

    	ClickHandler clickHandler = speechBubbleObjRef.GetComponent<ClickHandler>();
	    clickHandler.OnClick.AddListener((PointerEventData) => {
	    	dialogController.StartDialog();
	    });
	    
	    dialogController.StartDialog();
	}

	private void OnDestroy()
	{
    	questionController.OnSubmit.RemoveListener(OnSubmitSingleChoiceAnswer);
  	}

}
