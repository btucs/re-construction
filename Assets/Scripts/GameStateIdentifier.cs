#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameStateIdentifier 
{
    private static string konstruktorSceneName = "KonstruktorMultistep";
	
    public static bool IsGameState(GameSceneState checkState)
    {
    	GameSceneState currentState = GetCurrentGameState();
    	return (currentState == checkState);
    }

    public static GameSceneState GetCurrentGameState()
    {
    	GameSceneState returnState = GameSceneState.GameWorld;
    	string sceneName = SceneManager.GetActiveScene().name;
    	if(sceneName == konstruktorSceneName)
    	{
    		returnState = GameSceneState.KonstruktorStart;
    	} else {
    		MenuUIState uiState = GetUIState();
    		switch(uiState)
    		{
    			case MenuUIState.profile : 
    				returnState = GameSceneState.ProfileUI;
    				break;
    			case MenuUIState.bib : 
    				returnState = GameSceneState.BibUI;
    				break;
    			case MenuUIState.worldmap : 
    				returnState = GameSceneState.WorldMapUI;
    				break;
    			case MenuUIState.objectSelection :
    				returnState = GameSceneState.ObjectSelectionUI;
    				break;
                case MenuUIState.questlog :
                    returnState = GameSceneState.QuestUI;
                    break;
    		}
    	}
    	return returnState;
    }

    private static MenuUIState GetUIState()
    {
    	MenuUIController menuController = MenuUIController.Instance;
    	if(menuController == null)
    	{
    		return MenuUIState.closed;
    	} else {
    		return menuController.State();
    	}
    }

}


public enum GameSceneState
{
	KonstruktorStart, //when user is in constructor base layer
	KonstruktorModuleSelection, //when user is in constructor module selection
	KonstruktorCalcModule, //when user is inside a calculation module
	ObjectSelectionUI,
	ProfileUI,
	BibUI,
	WorldMapUI,
    QuestUI,
	GameWorld
}