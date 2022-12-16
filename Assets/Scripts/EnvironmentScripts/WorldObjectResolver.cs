#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WorldObjectResolver : MonoBehaviour
{
  public List<WorldObjectState> objectStates = new List<WorldObjectState>();

  void Start() {
    GameObject saveGameObj = GameObject.Find("GlobalState");
    Scene activeScene = SceneManager.GetActiveScene();
    if(saveGameObj != null) {
      if(activeScene.name == "KonstruktorRework" || activeScene.name == "TaskSolvingGraphicsUpdate" || activeScene.name == "KonstruktorMultistep") {
        TaskDataSO currentTask = saveGameObj.GetComponent<GameController>().gameState.konstruktorSceneData.taskData;
        if(currentTask != null) {
          ResolveObjectStateFromActiveTask(currentTask);
        }
      }
    }
  }

  public void ResolveObjectStateFromTaskHistory(TaskHistoryData _playerTaskData) {

    foreach(FinishedTaskData finishedTask in _playerTaskData.taskHistoryData) {
      foreach(WorldObjectState objectState in objectStates) {
        //check if it is the task, that this objectstate relates to
        if(finishedTask.solvedTask.taskName == objectState.correspondingTask.taskName) {
          //check, if saved task state equals required task state 
          RequiredTaskState stateOfTask = GetSolveStateOfTaskSave(finishedTask);
          if(objectState.taskState == stateOfTask) {
            SetState(objectState);
          }
        }
      }
    }
  }

  public void ResolveObjectStateFromSolvedTask(TaskDataSO solvedTask, RequiredTaskState _taskState) {
    foreach(WorldObjectState objectState in objectStates) {
      if(objectState.correspondingTask.taskName == solvedTask.taskName && objectState.taskState == _taskState) {
        SetState(objectState);
        return;
      }
    }
  }

  public List<string> GetFeedbackTextFromSolvedTask(TaskDataSO solvedTask, RequiredTaskState _taskState) {
    List<string> textList = new List<string>();
    foreach(WorldObjectState objectState in objectStates) {
      if(objectState.correspondingTask.taskName == solvedTask.taskName && objectState.taskState == _taskState) {
        textList = objectState.feedbackTexts;
      }
    }
    return textList;
  }

  public void ResolveObjectStateFromActiveTask(TaskDataSO currentTask) {
    foreach(WorldObjectState objectState in objectStates) {
      if(objectState.correspondingTask.taskName == currentTask.taskName) {
        if(objectState.taskState == RequiredTaskState.taskActive) {
          SetState(objectState);
        }
      }
    }
  }

  public void SetState(WorldObjectState newState) {
    foreach(GameObject enableObject in newState.enableObjects) {

      if(enableObject != null) {

        enableObject.SetActive(true);
      }
    }

    foreach(GameObject disableObject in newState.disableObjects) {

      if(disableObject != null) {

        disableObject.SetActive(false);
      }
    }

    foreach(ObjSpriteChange spriteChange in newState.spriteChanges) {

      if(spriteChange.objToChange != null) {

        spriteChange.objToChange.sprite = spriteChange.newSprite;
      }
    }
  }

  public RequiredTaskState GetSolveStateOfTask(TaskDataSO solvedTask, TaskHistoryData _playerTaskData) {
    RequiredTaskState currentState = RequiredTaskState.defaultState;
    FinishedTaskData bestTaskData = _playerTaskData.GetNewestTaskData(solvedTask);
    currentState = GetSolveStateOfTaskSave(bestTaskData);
    return currentState;
  }

  public RequiredTaskState GetSolveStateFromCourseData(TaskDataSO solvedTask, TaskHistoryData _playerTaskData)
  {
    RequiredTaskState currentState = RequiredTaskState.defaultState;
    FinishedTaskData bestTaskData = _playerTaskData.GetNewestCourseTaskData(solvedTask);
    currentState = GetSolveStateOfTaskSave(bestTaskData);
    return currentState;
  }

  public RequiredTaskState GetSolveStateOfTaskSave(FinishedTaskData finTaskData) {
    GameController controller = GameController.GetInstance();
    VariableInfoSO info = controller.gameAssets.variableInfo;
    TaskDataSO taskData = finTaskData.solvedTask;
    float maxExp = MaxExpHelper.GetMaxExp(info, taskData);
    //Debug.Log("maxExp " + maxExp);
    int minPartially = 60;
    int maxPartially = 99;

    RequiredTaskState currentState = RequiredTaskState.defaultState;
    if(finTaskData != null) {
      if(finTaskData.achievedPoints > ((maxPartially * maxExp) / 100))    //(finTaskData.achievedPoints == maxExp)
      {
        currentState = RequiredTaskState.taskSolvedCorrect;
        //Debug.Log("taskSolvedCorrect " + ((maxPartially * maxExp) / 100));
      } else if(finTaskData.achievedPoints < ((minPartially * maxExp) / 100)) {
        currentState = RequiredTaskState.taskSolvedIncorrect;
        //Debug.Log("taskSolvedIncorrect " + ((minPartially * maxExp) / 100));
      } else {
        currentState = RequiredTaskState.taskSolvedPartiallyCorrect;
        //Debug.Log("taskSolvedPartiallyCorrect " + finTaskData.achievedPoints);
      }
    } else {
      currentState = RequiredTaskState.defaultState;
    }
    return currentState;
  }
}

[Serializable]
public class WorldObjectState
{
  public TaskDataSO correspondingTask;
  public RequiredTaskState taskState;
  public List<GameObject> enableObjects = new List<GameObject>();
  public List<GameObject> disableObjects = new List<GameObject>();
  public List<ObjSpriteChange> spriteChanges = new List<ObjSpriteChange>();
  public List<string> feedbackTexts = new List<string>();
}

[Serializable]
public enum RequiredTaskState
{
  taskSolvedCorrect, taskSolvedIncorrect, taskActive, taskSolvedPartiallyCorrect, defaultState
}

[Serializable]
public class ObjSpriteChange
{
  public SpriteRenderer objToChange;
  public Sprite newSprite;
}

[Serializable]
public class ObjColorChange
{
  public SpriteRenderer objToChange;
  public Color newColor;
}