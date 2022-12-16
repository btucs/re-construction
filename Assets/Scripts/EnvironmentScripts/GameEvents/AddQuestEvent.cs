#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cutscene/AddQuest")]
public class AddQuestEvent : EventAction
{
	public MainQuestSO questToAdd;

    public override void Invoke(ScriptedEventManager eventManager)
    {
    	manager = eventManager;
    	
    	GameController controller = GameController.GetInstance();
		controller.gameState.profileData.AddActiveQuest(questToAdd);

		SetupContinueCondition();
    }
}
