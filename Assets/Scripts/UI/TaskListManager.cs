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
using UnityEngine.SceneManagement;

public class TaskListManager : MonoBehaviour
{
  public Text objectDescriptionBody;
  public PlayerActionButton examineButton;
  public Transform taskContainer;
  public GameObject MLEPreviewContainer;
  
  public GameObject taskEntryPrefab;
	public ButtonToggleMenu buttonGroupReference;
	public MLESceneLoader mleLoader;

  [HideInInspector]
	public List<ObjectTaskEntry> taskListEntries;

  public GameObject taskDetailContainer;
  public GameObject relatedMLEButton;
  public Text taskNameBody;
  public Text taskFullDescriptionBody;
  public Text rewardEPAmount;
  public Text rewardCoinAmount;
  public Text mleNameHolder;
  public Text mlePointHolder;
  public Text hashtagText;

  [Obsolete("backgroundPrefab not used")]
  public GameObject backgroundPrefab;
  public KonstruktorSceneData.InteractableData[] interactablePrefabs;
  
  private List<TaskDataSO> currentObjTasks;
  private TaskHistoryData taskHistory;

  private int MLEMaxPoints;
  private int MLEArchievedPoints;

  private TaskDataSO currentTaskData;
  private TaskObjectSO currentTaskObject;
  private GameWorldObject currentWorldObj;

  private MenuUIController UIManager;
  private GameController controller;
  private string[] objectExaminedTexts = new string[] {"Alles beim Alten: " , "Hier hat sich nichts verändert. ", ""};

  public void SetKonstruktorObject(GameWorldObject obj)
  {
    Debug.Log("Updating Konstruktor Object");
    currentWorldObj = obj;
    currentTaskObject = obj.objectData;
    UpdateKonstruktorData(currentWorldObj);
  }

  public void UpdateObjectSelectionUI(GameWorldObject newObject)
  {
    SetReferences();
    TaskObjectSO objectData = newObject.objectData;

    currentWorldObj = newObject;

    objectDescriptionBody.text = "";
    if(currentWorldObj.GetExaminedState() == true)
    {
      int randomTextIndex = UnityEngine.Random.Range(0, objectExaminedTexts.Length);

      objectDescriptionBody.text = objectExaminedTexts[randomTextIndex];
      examineButton.UpdateButtonText("Verstanden");
    }

    objectDescriptionBody.text += objectData.objectDescription;
    mleLoader.objectRef = objectData;

    //ResetTaskList();
    CloseMLEPreview();

    //CreateTaskButtonsForObject(objectData, relatedGameObject);
    
  }

  public void UpdateKonstruktorData(GameWorldObject _taskObject)
  {
    interactablePrefabs = new KonstruktorSceneData.InteractableData[] {
      new KonstruktorSceneData.InteractableData() {
        taskObject = _taskObject.objectData,
        position = _taskObject.transform.position,
      }
    };
  }

  public void CreateTaskUIMarkers(GameWorldObject taskObject, bool justUnlocked = false)
  {
    SetReferences();

    foreach(ObjectTaskData taskInfo in taskObject.objectData.taskInfos)
    {
      TaskDataSO taskData = taskInfo.task;
      TaskState taskState = taskHistory.GetStateOfTask(taskData);
        
      bool displayTask = true;
      if(taskData.requiredTask != null)
        displayTask = taskHistory.ExistsEntry(taskData.requiredTask);
          
      if(displayTask)
      {
        GameObject newTaskObject = UIManager.worldSpaceUI.CreateTaskInteractionMarker(taskObject, taskInfo);
        newTaskObject.GetComponent<ObjectTaskEntry>().Setup(this, taskData, taskObject, taskState, justUnlocked);
      }
    }  
  }

  public void ExamineSelectedObject()
  {
    currentWorldObj.Examine();
  }

  private void SetReferences()
  {
    if(controller == null) {
      controller = GameController.GetInstance();
    }
    taskHistory = controller.gameState.taskHistoryData;

    if(UIManager == null)
      UIManager = MenuUIController.Instance;
  }

    /*private void CreateTaskButton(GameWorldObject _worldObj, TaskDataSO _theTask, bool _solved)
    {
        GameObject newTaskObject = UIManager.CreateTaskInteractionMarker(_worldObj, _theTask);
        ObjectTaskEntry currentTaskScript = newTaskObject.GetComponent<ObjectTaskEntry>();
        taskListEntries.Add(currentTaskScript);
        Debug.Log(taskListEntries);

        //obsolete?
        SingleMenuButton currentButtonScript = newTaskObject.GetComponent<SingleMenuButton>();
        currentButtonScript.buttonGroup = buttonGroupReference;
        buttonGroupReference.menuButtons.Add(currentButtonScript);

        currentTaskScript.Setup(this, _theTask, _solved);
    }*/

   
  public void UpdateTaskData(TaskDataSO _task, TaskState taskState)
  {
    currentTaskData = _task;

    taskNameBody.text = _task.taskName;
    hashtagText.text = _task.GetHashtagText(); 
    taskFullDescriptionBody.text = GetTaskText(_task, taskState);

    if (taskNameBody.text == "Kraftarten identifizieren")
    {
      GetComponent<OnboardingTaskDisplayer>().SetOnboardingText(taskFullDescriptionBody.text);
    }

    VariableInfoSO info = controller.gameAssets.variableInfo;

    int maxExp = MaxExpHelper.GetMaxExp(info, _task);

    TaskHistoryData history = controller.gameState.taskHistoryData;
    FinishedTaskData finishedMaxTask = history.FindFinishedMaxPoints(_task);
    int currentPoints = finishedMaxTask != null ? finishedMaxTask.achievedPoints : 0;

    rewardEPAmount.text = currentPoints + " / " + maxExp;
    rewardCoinAmount.text = "+ " + (maxExp*20).ToString();

    VariableInfoSO.VariableInfoEntry infoEntry = null;
    int stepCount = _task.steps.Length;
    if(stepCount > 0)
    {

      TaskOutputVariableUnit outputUnit = _task.steps[stepCount - 1].output.unit;
      infoEntry = info.GetInfoFor(outputUnit);
    }

    if(infoEntry == null || infoEntry.mle == null)
    {

      relatedMLEButton.SetActive(false);
    } else {

      relatedMLEButton.SetActive(true);

      MLEInfo mleInfo = GetPointsOfMLE(infoEntry.mle);
      mlePointHolder.text = mleInfo.currentPoints + " / " + mleInfo.maxPoints;

      if(mleInfo.currentPoints == mleInfo.maxPoints)
      {
        mleNameHolder.text = "In dieser Aufgabe geht es um das Thema " + infoEntry.mle.mleName + ". Du hast unser Training zu diesem Thema bereits erfolgreich abgeschlossen. Willst du deine Kenntnisse vor der Aufgabe noch einmal auffrischen?";
      }
      else if(mleInfo.currentPoints > 0)
      {
        mleNameHolder.text = "In dieser Aufgabe geht es um das Thema " + infoEntry.mle.mleName + ". Scheinbar hast du unser Training zu diesem Thema bereits durchlaufen. Möchtest du deine Performance vom letzten Mal verbessern?";
      } else {
        mleNameHolder.text = "In dieser Aufgabe geht es um das Thema " + infoEntry.mle.mleName + ". Falls das Thema neu für dich ist, kannst du unser Training absolvieren, bevor du mit der Aufgabe loslegst.";
      }

      mleLoader.currentMLEData = infoEntry.mle;
      mleLoader.taskRef = _task;
    }
  }

  public void EnableTaskPreview()
  {
    taskDetailContainer.SetActive(true);
    objectDescriptionBody.transform.parent.gameObject.SetActive(false);

    CloseMLEPreview();

    if (taskNameBody.text == "Kraftarten identifizieren")
    {
      GetComponent<OnboardingTaskDisplayer>().DisplayOnboardingTask();
    }
  }

  public void CloseTaskPreview()
  {
    taskDetailContainer.SetActive(false);
    objectDescriptionBody.transform.parent.gameObject.SetActive(true);
    //buttonGroupReference.DeactivateAllButtons();
  }

  public void OpenMLEPreview()
  {
    taskDetailContainer.SetActive(false);
    MLEPreviewContainer.SetActive(true);
  }

  public void CloseMLEPreview()
  {
    taskDetailContainer.SetActive(true);
    MLEPreviewContainer.SetActive(false);
  }

  /*public void ResetTaskList()
  {
    ClearTaskButtons();
    buttonGroupReference.menuButtons.Clear();
  }*/

  private void ClearTaskButtons()
  {
    foreach (ObjectTaskEntry taskListEntry in taskListEntries)
    {
      Destroy(taskListEntry.gameObject);
    }
    taskListEntries.Clear();
  }

  private string GetTaskText(TaskDataSO _task, TaskState taskState)
  {
    string resultText = _task.teaserDescription;
    if(SceneManager.GetActiveScene().name == "TeachAndPlayScene")
      return _task.teachAndPlayDescription;

    if(taskState == TaskState.solvedCorrect && _task.teaserSolvedCorrect.Length > 5)
      return _task.teaserSolvedCorrect;

    if(taskState == TaskState.solvedWrong && _task.teaserSolvedWrong.Length > 5)
      return _task.teaserSolvedWrong;

    return resultText;
  }

  public MLEInfo GetPointsOfMLE(MLEDataSO activeMLE) {

    TaskHistoryData history = controller.gameState.taskHistoryData;
    FinishedMLEData maxPointsMLE = history.FindFinishedMaxPoints(activeMLE);
    int achievedPoints = maxPointsMLE != null ? maxPointsMLE.achievedPoints : 0;
    int maxPoints = activeMLE.GetMaxPoints();

    return new MLEInfo(activeMLE, maxPoints, achievedPoints);
  }

  public void SetKonstruktorQuestionData(MLEQuiz[] questions)
  {
    controller.gameState.konstruktorSceneData.followUpQuestions = questions;
    controller.SaveGame();
  }

  public void StartSelectedTask() {

    KonstruktorSceneData konstruktorSceneData = controller.gameState.konstruktorSceneData;
    konstruktorSceneData.Reset();

    konstruktorSceneData.cameraPosition = currentWorldObj.transform.position;
    konstruktorSceneData.cameraPosition += currentWorldObj.objectData.cameraOffset;
    konstruktorSceneData.cameraPosition.z = Camera.main.transform.position.z;
    konstruktorSceneData.cameraZoomFactor = currentWorldObj.objectData.cameraZoom;

    if(currentWorldObj.areaBackground != null)
      konstruktorSceneData.backgroundPrefab = currentWorldObj.areaBackground;

    konstruktorSceneData.taskData = currentTaskData;
    konstruktorSceneData.taskObject = currentTaskObject;
    konstruktorSceneData.npcScale = MenuUIController.Instance.playerController.transform.localScale.y;

    konstruktorSceneData.interactablesPrefabs = interactablePrefabs;
    konstruktorSceneData.returnSceneName = SceneManager.GetActiveScene().name;

    controller.SaveGame();
  }

  private void Start() {

    controller = GameController.GetInstance();
  }
}

public class MLEInfo {

  public MLEDataSO mle;
  public int maxPoints = 0;
  public int currentPoints = 0;

  public MLEInfo(MLEDataSO mle, int maxPoints, int currentPoints) {

    this.mle = mle;
    this.maxPoints = maxPoints;
    this.currentPoints = currentPoints;
  }
}
