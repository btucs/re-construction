#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tasks/TaskModulesOverrideData")]
public class TaskModuleResolver : ScriptableObject
{
    public List<AvailableModulesOverride> taskModuleRules = new List<AvailableModulesOverride>();

    public List<KonstructorModuleSO> GetModulesOfTask(TaskDataSO task)
    {
    	List<KonstructorModuleSO> moduleList = null;
    	foreach(AvailableModulesOverride moduleOverride in taskModuleRules)
    	{
    		if(moduleOverride.task.UID == task.UID)
    		{
    			moduleList = moduleOverride.availableModules;
    		}
    	}
    	return moduleList;
    }
}

[System.Serializable]
public class AvailableModulesOverride
{
	public TaskDataSO task;
	public List<KonstructorModuleSO> availableModules;
}