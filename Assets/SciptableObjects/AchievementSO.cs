#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using FullSerializer;

[CreateAssetMenu(menuName = "Achivement")]
public class AchievementSO : ScriptableObject, UIDSearchableInterface 
{
	[SerializeField, ReadOnly]
	private string uid;
	
	public string title;
	public string progressText;
	public string description;
	public Sprite icon;
	public AchievementCategory category;

	public LevelData[] levels;
	public Color tint;

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

	public int GetProgressAmount(GameState saveData)
	{
		int progressInt = 0;
		if(category == AchievementCategory.dialogues)
		{
			progressInt = saveData.gameworldData.visitedDialogueNodes.Count;
		} 
		else if(category == AchievementCategory.glossary)
		{
			progressInt = saveData.profileData.achievements.glossaryEntries;
		}
		else if(category == AchievementCategory.forcesDrawn)
		{
			progressInt = saveData.profileData.achievements.forcesDrawn;
		}
		else if(category == AchievementCategory.calculations)
		{
			progressInt = saveData.profileData.achievements.calculationsMade;
		}
		else if(category == AchievementCategory.items)
		{
			progressInt = saveData.profileData.inventory.items.Length;
		}

		return progressInt;
	}

	public int GetNextLevelThreshold(int progressAmount)
	{
		int returnInt = 9999;
		for(int i = 0; i < levels.Length; i++)
		{
			if(levels[i].threshold < returnInt && (levels[i].threshold > progressAmount || i == levels.Length - 1))
				returnInt = levels[i].threshold;
		}
		return returnInt;
	}

	public int GetCurrentLevel(GameState saveData)
	{
		int progress = GetProgressAmount(saveData);

		int returnInt = 0;
		for(int i = 0; i < levels.Length; i++)
		{
			if(progress >= levels[i].threshold)
			{
				returnInt = i;
			} else {
				break;
			}
		}
		return returnInt;
	}
}

[Serializable]
public enum AchievementCategory
{
	dialogues, MLEs, items, glossary, forcesDrawn, calculations
}

[Serializable]
public class LevelData
{
	public int threshold = 1;
	public string levelName = "Stufe 1";
}