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
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName="WorldMap/TweetMessage")]
public class TweetData : ScriptableObject, UIDSearchableInterface
{
	[SerializeField, ReadOnly]
	private string uid;

    public CharacterSO author;
    public string messageText;
    public TaskDataSO relatedTask;
    public RequiredTaskState taskCondition; //RequiredTaskState is defined in WorldObjectResolver

    public string UID
    {
		get {
			return uid;
		}
	}

	public bool IsTaskFeedback(TaskDataSO identTask, RequiredTaskState identCondition)
	{
		if(relatedTask != null && relatedTask.UID == identTask.UID)
		{
			if(identCondition == taskCondition || (identCondition == RequiredTaskState.taskSolvedPartiallyCorrect && taskCondition == RequiredTaskState.taskSolvedCorrect))
				return true;
		} 
		return false;
	}

	private void OnValidate()
	{
	#if UNITY_EDITOR
		if(uid == "" || uid == null) {
			uid = Guid.NewGuid().ToString();
			UnityEditor.EditorUtility.SetDirty(this);
    	}
	#endif
	}
}

[Serializable]
public class TweetDataInstance
{
	public string contentID;
	public DateTime publishingDate;
	public int likeAmount;

	public TweetDataInstance(TweetData _Content)
	{
		contentID = _Content.UID;
		DateTime currentDT = DateTime.UtcNow.ToLocalTime();
		publishingDate = currentDT.AddYears(19);
		likeAmount = UnityEngine.Random.Range(7, 400);
	}
}