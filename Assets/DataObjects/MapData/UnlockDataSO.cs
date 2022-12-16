#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="WorldMap/UnlockData")]
public class UnlockDataSO : ScriptableObject
{
    public List<UnlockSpecification> unlockRules = new List<UnlockSpecification>();

    public List<UnlockSpecification> GetRulesBySceneName(string searchSceneName)
    {
    	List<UnlockSpecification> rulesFound = new List<UnlockSpecification>();
    	
    	foreach(UnlockSpecification rule in unlockRules)
    	{
    		if(rule.area.sceneName == searchSceneName)
    			rulesFound.Add(rule);
    	}
    	return rulesFound;
    }
}

[System.Serializable]
public class UnlockSpecification
{
	public AreaDataSO area;
	public AreaDirection direction;
	public int index;
	public ProgressRequirement condition;
}

public enum AreaDirection
{
	right, left
}