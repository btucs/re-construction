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

public class TeachAndPlayUIController : MonoBehaviour
{
    //public Text deviceDescriptionText;
    public TaskListManager taskListManager;
    public MainQuestSO teachAndPlayQuest;
    public GameObject errorMSG;
    public Text errorText;
    public Text errorTaskName;
    public GameObject taskContent;
    public GameObject skipTaskButton;
    private GameController controller;
    private Course courseData;

    private TaskDataSO currentTaskData;

    private void Initialize()
    {
        controller = GameController.GetInstance();  
        courseData = controller.gameState.course;
    }

    // public void OpenVRDeviceUI()
    // {
    //     deviceDescriptionText.text = "Eine VR Brille, über die Problemsituationen virtuell nachgestellt werden können.";
    // }

    public void LoadNextTask()
    {
        Initialize();
        skipTaskButton.SetActive(false);
        errorMSG.SetActive(false);

        int questStep = controller.gameState.profileData.GetStepOfActiveQuest(teachAndPlayQuest);

        if(controller.gameState.profileData.IsQuestActive(teachAndPlayQuest) == false)
        {
            errorText.text = "Zurzeit ist in dem System keine Aufgabe geladen. Sprich mit Ausbilder Jörg und frage ihn nach neuen Aufgaben.";
            errorMSG.SetActive(true);
            this.gameObject.SetActive(true);
            errorTaskName.text = "Keine Aufgabe";
            foreach(Transform child in errorTaskName.GetComponent<Transform>())
            {
                child.gameObject.SetActive(false);
            }
            taskContent.SetActive(false);
            Debug.Log("The T&P-Quest is not active.");
            return;
        }

        if(teachAndPlayQuest.objectives.ElementAt(questStep).type != QuestType.courseProgress)
        {
            errorText.text = "Zurzeit ist in dem System keine Aufgabe geladen.";
            errorMSG.SetActive(true);
            this.gameObject.SetActive(true);
            errorTaskName.text = "Keine Aufgabe";
            foreach(Transform child in errorTaskName.GetComponent<Transform>())
            {
                child.gameObject.SetActive(false);
            }
            taskContent.SetActive(false);
            Debug.Log("Current taskstep of T&P-Quest is not related to constructor task.");
            return;
        }

        CourseTask nextTask = courseData.GetNextTask(controller.gameState);
        Debug.Log(nextTask.GetAsString());
        

        //Get Task Data
        List<TaskDataSO> topicTasks = controller.gameAssets.GetTasksOfTopic(nextTask.topic);

        TaskDataSO taskToLoad = null; 
        foreach(TaskDataSO topicTask in topicTasks)
        {
            if(controller.gameState.taskHistoryData.IsCourseTaskDone(topicTask) == false)
            {
                taskToLoad = topicTask;
                break;
            }
        }
        currentTaskData = taskToLoad;

        //Check Date
        if(nextTask.IsDateValid() == false)
        {
            if(DateTime.Compare(nextTask.accessibleUntil, DateTime.Now) < 0) {
                errorText.text = "Du hast den Zeitraum für diese Kursaufgabe verpassst. Diese Aufgabe war nur bis " + nextTask.accessibleUntil.ToString() + " verfügbar.";
                skipTaskButton.SetActive(true);
            } else {
                errorText.text = "Diese Aufgabe ist erst ab dem " + nextTask.accessibleFrom.ToString() + " verfügbar.";
            }
            errorMSG.SetActive(true);
            Debug.Log("Der vorgegebene Zeitraum für die Aufgabe abgelaufen.");
            return;
        }


        if(taskToLoad!=null)
        {
            Debug.Log("Found a task for T&P course.");
            
            //taskContent.SetActive(true);

            taskListManager.UpdateTaskData(taskToLoad, TaskState.unsolved);
            GameWorldObject worldObj = taskToLoad.connectedObject.objectPrefab.GetComponentInChildren<GameWorldObject>();
            taskListManager.SetKonstruktorObject(worldObj);
            taskListManager.SetKonstruktorQuestionData(nextTask.singleChoiceQuestions);
            taskListManager.EnableTaskPreview();
        } else {
            Debug.LogError("Found no task to load.");
            errorText.text = "Aktuell stehen keine Aufgaben zur Verfügung.";
            errorMSG.SetActive(true);
        }
    }



    public void SkipCourseTask()
    {
        if(currentTaskData == null)
        {
            Debug.LogError("No Task Data to skip");
            return;
        } 

        FinishedTaskData defaultTaskData = new FinishedTaskData(currentTaskData, currentTaskData.connectedObject, 0);
        controller.gameState.taskHistoryData.skippedCourseTaskData.Add(defaultTaskData);
        controller.SaveGame();

        OnboardingController.Instance.sceneIntroController.CheckForQuestProgress();
    }
}
