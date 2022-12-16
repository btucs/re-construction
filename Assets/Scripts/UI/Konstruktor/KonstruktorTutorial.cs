#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class KonstruktorTutorial : MonoBehaviour
{
    public List<TutorialStep> tutorialSteps;
    [HideInInspector]
    public int currentTutorialPart = -1;

    public RectTransform onboardingDialogContainer;
    public TMPro.TextMeshProUGUI onboardingDialogText;
    public Button continueEventButton;

    public GameObject highlightRectPrefab;
    private GameObject highlightRectObj;

    [HideInInspector]
    public Component[] alternativeButtons;

    [HideInInspector]
    public KonstruktorOnboardingController onboardingController; 

    [HideInInspector]
    public UnityAction continueButtonAction;

    public void StartEvent()
    {
    	currentTutorialPart = 0;
    	//tutorialSteps[currentTutorialPart]
    	if(tutorialSteps.Count > currentTutorialPart)
    	{
    		ShowTutorialPart(currentTutorialPart);
    	}
    }

    public void ContinueEvent()
    {
    	if(highlightRectObj)
    		Destroy(highlightRectObj);

    	currentTutorialPart++;
    	if(tutorialSteps.Count > currentTutorialPart)
    	{
    		ShowTutorialPart(currentTutorialPart);
    	}
    	else 
    	{
    		onboardingDialogContainer.gameObject.SetActive(false);
    		FinishEvent();
    	}
    }

    public void FinishEvent()
    {
    	if(onboardingController != null)
    	{
    		onboardingController.ActiveTutorialFinished();
    	} 
    }

    public void ShowTutorialPart(int stepIndex)
    {
    	BlendInOutContinueButton(stepIndex);
    	onboardingDialogText.text = tutorialSteps[stepIndex].dialogBoxMessage;
    	onboardingDialogContainer.gameObject.SetActive(true);
    	onboardingDialogContainer.anchoredPosition = tutorialSteps[stepIndex].dialogBoxPosition;

    	if(tutorialSteps[stepIndex].objectToHighlight != null)
    	{
    		//Debug.Log("Placing Highlight...");
    		highlightRectObj = Instantiate(highlightRectPrefab, tutorialSteps[stepIndex].objectToHighlight);
    		RectTransform highlightTransform = highlightRectObj.GetComponent<RectTransform>();

    		//highlightTransform.anchoredPosition = tutorialSteps[stepIndex].objectToHighlight.anchoredPosition;
    		//highlightRect.sizeDelta = new Vector2 (tutorialSteps[stepIndex].objectToHighlight.sizeDelta.x + 10f,
    		//	tutorialSteps[stepIndex].objectToHighlight.sizeDelta.y + 10f);
    		//highlightRect.gameObject.SetActive(true);
    	}

    	foreach(GameObject enableObject in tutorialSteps[stepIndex].blendInObjects)
    	{
    		enableObject.SetActive(true);	
    	}
    	foreach(GameObject disableObject in tutorialSteps[stepIndex].blendOutObjects)
    	{
    		disableObject.SetActive(false);
    	}

    }

    public void ContinueOnAlternativeButton()
    {
    	ContinueEvent();

    	foreach(Button singleButton in alternativeButtons)
    	{
    		singleButton.onClick.RemoveListener(continueButtonAction);
    	}
    }

    private void BlendInOutContinueButton(int stepIndex)
    {
    	if(tutorialSteps[stepIndex].continueOnButton == true)
    	{
    		continueEventButton.gameObject.SetActive(true);
    	} 
    	else 
    	{
    		if(tutorialSteps[stepIndex].alternativeContinueButton != null)
    		{
    			continueButtonAction = ContinueOnAlternativeButton;

    			alternativeButtons = tutorialSteps[stepIndex].alternativeContinueButton.GetComponentsInChildren(typeof(Button), true);
    			foreach(Button singleButton in alternativeButtons)
    			{
    				singleButton.onClick.AddListener(continueButtonAction);
    			}
    		}
			continueEventButton.gameObject.SetActive(false);
    	}
    }


}

[System.Serializable]
public class TutorialStep
{
	public Vector2 dialogBoxPosition;
  [MultiLineProperty(3)]
	public string dialogBoxMessage;
	public bool continueOnButton = true;
	public List<GameObject> blendInObjects;
	public List<GameObject> blendOutObjects;
	public GameObject alternativeContinueButton;
	public Transform objectToHighlight;
}