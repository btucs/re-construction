#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cutscene/UnlockMenu")]
public class UnlockMenuEvent : EventAction
{
	public MenuContentType whichMenu;

    public override void Invoke(ScriptedEventManager eventManager)
    {
        Debug.Log("Unlock Menu Event invoked");
    	manager = eventManager;

    	switch(whichMenu)
    	{
    		case MenuContentType.bibliothek :
    			manager.onboardingManager.UnlockBibMenu();
    			break;
    		case MenuContentType.profile :
    			manager.onboardingManager.UnlockProfileMenu();
    			break;
    		case MenuContentType.worldmap :
    			manager.onboardingManager.UnlockMapMenu();
    			break;
    		case MenuContentType.questoverview :
    			manager.onboardingManager.UnlockQuestMenu();
    			break;
            case MenuContentType.analyzer :
                manager.onboardingManager.UnlockAnalyzeTool();
                break;
	
    	}
        SetupContinueCondition();
    }
}

[System.Serializable]
public enum MenuContentType
{
	bibliothek, worldmap, profile, questoverview, analyzer
}