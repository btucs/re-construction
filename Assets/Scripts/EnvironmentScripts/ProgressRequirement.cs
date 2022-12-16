#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProgressRequirement
{
	public List<TaskDataSO> requiredTasks = new List<TaskDataSO>();
	public TopicSO finishedTopic;
	public TopicSO unlockedTopic;
	public ScriptedEventDataSO finishedEvent;
	public MainQuestSO activeQuest;
	public MainQuestSO finishedQuest;
	public int taskAmount = 0;

	public bool ConditionMet(GameController controller)
	{
		TaskHistoryData solvedTasks = controller.gameState.taskHistoryData;
		VariableInfoSO taskMLEResolveData = controller.gameAssets.variableInfo;
		foreach(TaskDataSO requiredTask in requiredTasks)
		{
			if(solvedTasks.ExistsEntry(requiredTask) == false)
				return false;
		}

		if(finishedTopic != null)
		{
			float progressIndex = solvedTasks.GetTopicProgressIndex(finishedTopic);
			if(progressIndex < 1f)
				return false;
		}

		if(unlockedTopic != null)
		{
			if(unlockedTopic.IsUnlocked()==false)
				return false;
		}

		if(finishedEvent != null)
		{
			if(controller.gameState.onboardingData.EventEntryExists(finishedEvent.UID) == false)
				return false;
		}

		if(activeQuest != null)
		{
			if(controller.gameState.profileData.IsQuestActive(activeQuest) == false)
				return false;
		}

		if(finishedQuest != null)
		{
			if(controller.gameState.profileData.IsQuestFinished(finishedQuest) == false)
				return false;
		}
		
		if(solvedTasks.taskHistoryData.Count < taskAmount)
		{
			return false;
		}

		return true;
	}
}
