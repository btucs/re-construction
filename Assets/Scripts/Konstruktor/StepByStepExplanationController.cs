#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class StepByStepExplanationController : MonoBehaviour
{
  public List<KonstruktorExplanation> tutorials = new List<KonstruktorExplanation>();

  private KonstruktorExplanation activeExplanation;

  private static StepByStepExplanationController instance;
  public static StepByStepExplanationController Instance
  {
    get { return instance; }
  }

  private void Awake ()
  {
    if (instance == null || instance != this) { instance = this; }
    //else if (instance != this) { Destroy(this.gameObject); }
  }

  private void LateUpdate() {
    UpdateTutorial();
  }

  public string GetTextOfActiveModule()
  {
  	string returnText = "Entschuldige, aber zu deinem aktuellen Arbeitsschritt kann ich dir keine Infos geben.";
  	foreach(KonstruktorExplanation tutorial in tutorials) {
      if(tutorial.relatedModule.name == activeExplanation.relatedModule.name) {
      	returnText = tutorial.GetCurrentHelpText();
      }
	}
	return returnText;
  }

  public void ActivateTutorialOfModule(KonstructorModuleSO activeModule) {
    foreach(KonstruktorExplanation tutorial in tutorials) {
      if(tutorial.relatedModule.name == activeModule.name) {
        activeExplanation = tutorial;
        activeExplanation.StartTutorial();
        return;
      }
    }
    activeExplanation = null;
  }

  public void ResetTutorial() {
    activeExplanation = null;
  }

  public void UpdateTutorial() {
    if(activeExplanation != null) {
      activeExplanation.CheckForCurrentStep();
    }
  }
}

[Serializable]
public class KonstruktorExplanation
{
  public KonstructorModuleSO relatedModule;
  public Text tutorialTextelement;
  public List<ExplanationPrompt> singlePrompts = new List<ExplanationPrompt>();
  private int tutorialIndex;

  public void DisplayExplanation(int indexToDisplay) {
    if(singlePrompts.Count >= indexToDisplay)
    {
      tutorialTextelement.text = singlePrompts[indexToDisplay].messageText;
    }
  }

  public void StartTutorial() {
    //tutorialTextelement.transform.parent.gameObject.SetActive(true);
    tutorialIndex = 0;
    DisplayExplanation(tutorialIndex);
  }

  public string GetCurrentHelpText()
  {
    string content = "Tut mir leid aber zu dem aktuellen Arbeitsschritt liegen mir keine Informationen vor.";
    int index = 0;
    for(int i = 1; i < singlePrompts.Count; i++)
    {
      if(singlePrompts[i].CheckAllConditions())
        index = i;
    }

    if(index >= 0 && index < singlePrompts.Count)
      content = singlePrompts[index].messageText;
    return content;
  }

  public void CheckForCurrentStep() {
    tutorialIndex = 0;
    for(int i = 1; i < singlePrompts.Count; i++) {
      bool conditionMet = singlePrompts[i].CheckAllConditions();
      if(conditionMet) {
        tutorialIndex = i;
      } else {
        //break;
      }
    }

    try {

      DisplayExplanation(tutorialIndex);
    } catch(ArgumentOutOfRangeException) {
      // catch index does not exist
    }
  }
}

[Serializable]
public class ExplanationPrompt
{
  //public DisplayCondition condition;
  public List<DisplayCondition> conditions = new List<DisplayCondition>();
  public string messageText;

  public bool CheckAllConditions() {
    foreach(DisplayCondition condition in conditions) {
      if(condition.CheckCondition() == false) {
        return false;
      }
    }

    return true;
  }

}

[Serializable]
public class DisplayCondition
{
  public DisplayConditionType type;

  [HideIf("@this.type == DisplayConditionType.intCallback || this.type == DisplayConditionType.boolCallback")]
  public GameObject conditionObject;

  [ShowIf("type", DisplayConditionType.intCallback)]
  public IntCallback intCallback;
  [ShowIf("type", DisplayConditionType.boolCallback)]
  public BoolCallback boolCallback;

  [ShowIf("@this.type == DisplayConditionType.childAdded || this.type == DisplayConditionType.intCallback")]
  public int compareValue = 1;
  [ShowIf("@this.type == DisplayConditionType.isObjectActive || this.type == DisplayConditionType.buttonIsInteractable || this.type == DisplayConditionType.boolCallback")]
  public bool compareBool = true;
  

  private int referenceValue;

  public void GetReferenceValue() {
    if(type == DisplayConditionType.childAdded && conditionObject != null) {
      referenceValue = conditionObject.transform.childCount;
    }
  }

  [Serializable]
  public class BoolCallback : SerializableCallback<bool>  {}
  [Serializable]
  public class IntCallback : SerializableCallback<int> {}

  public bool CheckCondition() {
    if(type == DisplayConditionType.isObjectActive) {
      if(conditionObject != null && conditionObject.activeSelf == compareBool) {
        return true;
      }
    } else if(type == DisplayConditionType.childAdded) {
      if(conditionObject != null && conditionObject.transform.childCount >= compareValue) {
        return true;
      }
    } else if(type == DisplayConditionType.buttonIsInteractable) {
      if(conditionObject != null && conditionObject.GetComponent<Selectable>().interactable == compareBool) {
        return true;
      }
    } else if(type == DisplayConditionType.intCallback) {

      return intCallback.Invoke() == compareValue;
    } else if(type == DisplayConditionType.boolCallback) {

      return boolCallback.Invoke() == compareBool;
    }
    return false;
  }
}

[Serializable]
public enum DisplayConditionType
{
  isObjectActive, childAdded, buttonIsInteractable, intCallback, boolCallback
}