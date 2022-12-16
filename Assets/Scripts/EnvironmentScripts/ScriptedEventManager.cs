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
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Yarn.Unity;

public class ScriptedEventManager : MonoBehaviour
{
    public ScriptedEventDataSO eventData;
    public GameEventType type = GameEventType.onSceneStart;
	public List<EventPart> eventParts = new List<EventPart>();

    public BoxCollider2D triggerArea;
    public GameObject dialogTextBox;
    //public DialogController currentDialogController;
    private MenuUIController uiController;
    private DialogueRunner yarnRunner;
    public OnboardingController onboardingManager;

    private playerScript interactionScript;
    [HideInInspector]
    public List<bool> moveFin = new List<bool>();
    [HideInInspector]
    public bool eventActive = false;
    [HideInInspector]
    public int currentPartOfEvent = 0;

    private void Start()
    {
        uiController = MenuUIController.Instance;
        yarnRunner = uiController.dialogueManager.yarnRunner;
    }

    public void Continue()
    {
        if(eventActive)
        {
            Debug.Log("Doing next eventstep with index " + currentPartOfEvent);
            if(currentPartOfEvent + 1 < eventParts.Count)
            {
                //next part of event
                ExecuteNextEventStep();
            } 
            else
            {
                //Event over
                interactionScript.EnableInteraction();
                onboardingManager.OnEventFinished(this);
                eventActive = false;
            }
        }
    }

    public bool RequirementsMet(GameController controller)
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        return(currentSceneName == eventData.eventSceneName && eventData.requirement.ConditionMet(controller));
    }

    public void StartEvent()
    {
        eventActive = true;
        interactionScript = playerScript.Instance;
    	interactionScript.DisableInteraction();
        onboardingManager.EnterEventMode();
        onboardingManager.SetActiveEvent(this);
    	EventPart currentEvent = eventParts[0];
        for(int i = 0; i < currentEvent.actions.Count; i++)
        {
            currentEvent.actions[i].Invoke(this);
        }
    }

    public void ExecuteNextEventStep()
    {
    	currentPartOfEvent++;
        Debug.Log("Doing next eventstep with index " + currentPartOfEvent);
    	EventPart currentEvent = eventParts[currentPartOfEvent];

        for(int i = 0; i <= currentEvent.actions.Count - 1; i++)
    	{

            currentEvent.actions[i].Invoke(this);
    	}

/*        if(currentEvent.delayTime > 0.1f)
        {
            StartCoroutine("ContinueEvent", currentEvent.delayTime);
        }*/

    }

    public bool CheckForContinue()
    {
        Debug.Log("Check for continue event");
        foreach(EventAction action in eventParts[currentPartOfEvent].actions)
        {
            if(action.IsFinished() == false)
            {
                Debug.Log("Event step is not yet finished at index " + currentPartOfEvent);
                return false;
            }
        }
        Continue();
        return true;
    }

    /*public void UpdateDialogContent(DialogEvent _dialog)
    {
    	//currentDialogController = dialogTextBox.GetComponentInChildren<DialogController>();
    	currentDialogController.DialogEventManager = this;
    	currentDialogController.UpdateDialogContent(_dialog);
    	dialogTextBox.SetActive(true);
    }*/

    private void OnTriggerEnter2D(Collider2D otherCol)
    {
        if(otherCol.gameObject.CompareTag("Player"))
        {
            onboardingManager.CheckForEvent(this);
        }
    }

    IEnumerator ContinueEvent(float Count){
        yield return new WaitForSeconds(Count);
        ExecuteNextEventStep();
        yield return null;
    }

}

[Serializable]
public enum GameEventType
{
    onSceneStart, onEnterArea
}