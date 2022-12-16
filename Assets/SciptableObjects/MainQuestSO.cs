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
using UnityEditor;
using Sirenix.OdinInspector;
using FullSerializer;

[CreateAssetMenu(menuName="GameProgress/Quest")]
public class MainQuestSO : ScriptableObject, UIDSearchableInterface 
{
	[LabelText("Titel")]
    public string title;

	[MultiLineProperty(5)]
	[LabelText("Beschreibung")]
    public string description;

    public List<QuestTask> objectives;
    public MainQuestSO followUpQuest;
    public bool isMainQuest = true;

    [SerializeField, ReadOnly]
    private string uid;

    public string UID {
        get {
            return uid;
        }
    }  

    private void OnValidate()
    {
    #if UNITY_EDITOR
        if(uid == "" || uid == null)
        {
            uid = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    #endif
    }

    public int GetCurrentStep(GameController controller, int startAt = 0)
    {
    	TaskHistoryData playerProgress = controller.gameState.taskHistoryData;
    	string activeSceneName = controller.gameState.gameworldData.activeScene;
    	VariableInfoSO varDataHelper = controller.gameAssets.variableInfo;
    	for(int i = startAt; i < objectives.Count; i++)
    	{
    		if(objectives.ElementAt(i).ConditionMet(controller.gameState, varDataHelper, activeSceneName) == false)
    			return i;
    	}
    	return objectives.Count;
    }

    public bool SolvedAtIndex(int index, GameController controller)
    {
    	TaskHistoryData playerProgress = controller.gameState.taskHistoryData;
    	string activeSceneName = controller.gameState.gameworldData.activeScene;
    	VariableInfoSO varDataHelper = controller.gameAssets.variableInfo;
    	if(objectives.Count > index)
    	{
    		return objectives.ElementAt(index).ConditionMet(controller.gameState, varDataHelper, activeSceneName);
    	} else {
    		return false;
    	}
    }

    public bool Solved(GameController controller)
    {
    	return (GetCurrentStep(controller) > objectives.Count - 1);
    }

    public bool Solved(int currentStep)
    {
    	return (currentStep > objectives.Count - 1);
    }

    public string GetProgressText(GameController controller)
    {
    	int index = GetCurrentStep(controller);

        int savedIndex = controller.gameState.profileData.GetStepOfActiveQuest(this);
        if(savedIndex > index)
            index = savedIndex;

        if(index >= objectives.Count)
            index= objectives.Count - 1;

    	return objectives.ElementAt(index).ProgressText(controller.gameState, controller.gameAssets.variableInfo);
    }
}

[Serializable]
public class QuestTask
{
	public QuestType type;
	public string progressText;
	public int requiredAmount = 1;
	public MLEDataSO specificTopic;
    public List<TaskDataSO> specificTasks = new List<TaskDataSO>();
	public string targetAreaName;
    public TopicSO topic;

	public bool ConditionMet(GameState playerProgress, VariableInfoSO varDataHelper, string sceneName)
	{
		switch(type)
		{
			case QuestType.tasksByTopic : 
				return (playerProgress.taskHistoryData.GetFinishedTasksByMLE(specificTopic, varDataHelper) >= requiredAmount);

			case QuestType.tasksGeneral :
				return (playerProgress.taskHistoryData.CountDistinctTasks() >= requiredAmount);

			case QuestType.goTo :
				return (sceneName == targetAreaName);

            case QuestType.specificTasks : 
                foreach(TaskDataSO task in specificTasks)
                {
                    if(playerProgress.taskHistoryData.ExistsEntry(task) == false)
                    return false;
                }
                return true;

            case QuestType.talkTo : 
                return false;

            case QuestType.courseProgress :
                if(playerProgress.course == null)
                    return false;
                int tasksTotal = playerProgress.course.GetNumberOfTasks();
                int tasksDone = playerProgress.taskHistoryData.courseHistoryData.Count + playerProgress.taskHistoryData.skippedCourseTaskData.Count;
                return (Mathf.CeilToInt(((float)tasksDone / (float)tasksTotal) * 100f) >= requiredAmount);

            case QuestType.finishTopic :      
                return (playerProgress.taskHistoryData.GetTopicProgressIndex(topic) >= 1f);

			default :
				Debug.LogError("No condition behaviour defined.");
				return false;
		}
	}

	public string ProgressText(GameState playerProgress, VariableInfoSO varDataHelper)
	{
		string returnString = "Hier sollte ein Aufgabentext stehen.";
		int count = 0;
		switch(type)
		{
			case QuestType.tasksByTopic : 
				count = playerProgress.taskHistoryData.GetFinishedTasksByMLE(specificTopic, varDataHelper);
				returnString = progressText + " (" + count + "/" + requiredAmount + ')';
				break;

			case QuestType.tasksGeneral :
				count = playerProgress.taskHistoryData.CountDistinctTasks();
    			returnString = progressText + " (" + count + "/" + requiredAmount + ')';
    			break;

            case QuestType.specificTasks :
                foreach(TaskDataSO task in specificTasks)
                {
                    if(playerProgress.taskHistoryData.ExistsEntry(task))
                        count ++;
                }
                returnString = count + "/" + specificTasks.Count + " " + progressText;
                break;

    		case QuestType.goTo :
    			returnString = progressText;
    			break;

            case QuestType.talkTo :
                returnString = progressText;
                break;

            case QuestType.courseProgress :
                int tasksRequired = Mathf.RoundToInt(((float)requiredAmount / 100f) * (float)playerProgress.course.GetNumberOfTasks());
                int tasksSolvedOrSkipped = playerProgress.taskHistoryData.courseHistoryData.Count + playerProgress.taskHistoryData.skippedCourseTaskData.Count;
                returnString = "Erledige Aufgaben mit dem VR-Ausbildungssystem. (" + tasksSolvedOrSkipped + "/" + tasksRequired + ")";
                break;

            case QuestType.finishTopic :
                if(progressText.Length > 2)
                {
                    returnString = progressText + "(" + playerProgress.taskHistoryData.GetTopicProgressIndex(topic)*100 + "%)";;
                } else {
                    returnString = "Schließe den Inhalt " + topic.name + " ab. (" + playerProgress.taskHistoryData.GetTopicProgressIndex(topic)*100 + "% abgeschlossen)";
                }
                break;

			default :
				Debug.LogError("No condition behaviour defined.");
				break;
		}
		return returnString;
	}
}

[Serializable]
public enum QuestType
{
  tasksGeneral, tasksByTopic, specificTasks, goTo, talkTo, finishTopic, courseProgress
}