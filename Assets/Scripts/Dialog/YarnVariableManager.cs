#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;

public class YarnVariableManager : VariableStorageBehaviour
{
	private GameController saveController;
    public override void SetValue(string variableName, Yarn.Value value) {
        // 'variableName' is the name of the variable that 'value' 
        // should be stored in.
    }

    // Return a value, given a variable name
    public override Yarn.Value GetValue(string variableName) {

    	Debug.Log("Looking up YarnData: " + variableName);

    	if(saveController==null)
    		saveController = GameController.GetInstance();

        Yarn.Value returnVal = new Yarn.Value();
        int currentExp;
        
        if(variableName.Contains("$isTaskSolvedCorrect"))
        {
            string[] splitresult = variableName.Split('_');
            TaskDataSO task = saveController.gameAssets.FindTask(splitresult[1]);
            return new Yarn.Value((saveController.gameState.taskHistoryData.GetStateOfTask(task) == TaskState.solvedCorrect));
        }
        if(variableName.Contains("$isTaskUnlocked"))
        {
            string[] splitresult = variableName.Split('_');
            TaskDataSO task = saveController.gameAssets.FindTask(splitresult[1]);
            return new Yarn.Value(task.GetMLE().IsUnlocked());
        }

        if(variableName.Contains("$isQuestActive"))
        {
            string[] splitresult = variableName.Split('_');
            MainQuestSO quest = saveController.gameAssets.FindQuestByShortID(splitresult[1]);
            return new Yarn.Value(saveController.gameState.profileData.IsQuestActive(quest));
        }

        if(variableName.Contains("$isQuestCompleted"))
        {
            string[] splitresult = variableName.Split('_');
            MainQuestSO quest = saveController.gameAssets.FindQuestByShortID(splitresult[1]);
            return new Yarn.Value(saveController.gameState.profileData.IsQuestCompleted(quest));
        }

        if(variableName.Contains("$questIndex"))
        {
            string[] splitresult = variableName.Split('_');
            MainQuestSO quest = saveController.gameAssets.FindQuestByShortID(splitresult[1]);
            
            if(RoomState.Instance != null)
                RoomState.Instance.sceneIntroController.CheckForQuestProgress();

            int index = saveController.gameState.profileData.GetStepOfActiveQuest(quest); //returns -1 if quest is not active
            return new Yarn.Value(index);
        }

        switch(variableName)
        {
        	case "$playername" : 
        		returnVal = new Yarn.Value(saveController.gameState.profileData.playerName);
        		break;
            case "$mentorname" : 
                returnVal = new Yarn.Value(saveController.gameState.characterData.mentor.characterName);
                break;
        	case "$isinprofile" : 
	        	returnVal = new Yarn.Value(GameStateIdentifier.IsGameState(GameSceneState.ProfileUI));
	        	break;
	        case "$isinbib" :
	        	returnVal = new Yarn.Value(GameStateIdentifier.IsGameState(GameSceneState.BibUI));
	        	break;
	        case "$isingameworld" :
	        	returnVal = new Yarn.Value(GameStateIdentifier.IsGameState(GameSceneState.GameWorld));
	        	break;
	        case "$isinkonstruktor" :
	        	returnVal = new Yarn.Value(GameStateIdentifier.IsGameState(GameSceneState.KonstruktorStart));
	        	break;
            case "$isinquestlog" : 
                returnVal = new Yarn.Value(GameStateIdentifier.IsGameState(GameSceneState.QuestUI));
                break;
            case "$currentRankText" : 
                currentExp = saveController.gameState.taskHistoryData.CalculateCurrentEXP();
                returnVal = new Yarn.Value(saveController.gameAssets.playerRanks.GetCurrentRankAsString(currentExp));
                break;
            case "$currentRankNumber" :
                currentExp = saveController.gameState.taskHistoryData.CalculateCurrentEXP();
                returnVal = new Yarn.Value(saveController.gameAssets.playerRanks.GetCurrentRankAsInt(currentExp));
                break;
            case "$konstruktorFeedbackText" :
                string helpText = CreateKonstruktorFeedbackText();
                returnVal = new Yarn.Value(helpText);
                break;
        }

        return returnVal;
    }

    private string CreateKonstruktorFeedbackText()
    {
        string helpText = "Entschuldige aber bei deiner aktuellen Aufgabe kann ich dir leider nicht helfen.";
        KonstruktorStateIdentifier konstruktorState = KonstruktorStateIdentifier.Instance;
        if(konstruktorState != null)
        {
            if(konstruktorState.SearchedVarIdentified() == false) {
                helpText = "Zuerst solltest du die gesuchte Information identifizieren. Du findest sie indem du mit anderen sprichst. Lege sie dann in dem Feld oben rechts ab.";
            } else if(konstruktorState.TaskStepAdded() == false) {
                helpText = "Um den gesuchten Wert zu berechnen, musst du einen neuen Rechenschritt hinzufügen. Tippe dafür auf das Plus neben dem Dropdown-Menu am oberen Bildschirmrand.";
            } else if(konstruktorState.ResultCreated() == true && konstruktorState.ResultAssigned() == false) {
                helpText = "Scheinbar hast du schon ein Ergebnis erstellt! Um es als Lösung abzugeben musst du es dem gesuchten Wert zuordnen, indem du das Ergebnis auf das entsprechende Feld ziehst.";
            }
        }
        StepByStepExplanationController tutorialController = StepByStepExplanationController.Instance;
        if(tutorialController != null)
        {
            helpText = tutorialController.GetTextOfActiveModule();
        }

        return helpText;
    }

    // Return to the original state
    public override void ResetToDefaults () {

    }
}


