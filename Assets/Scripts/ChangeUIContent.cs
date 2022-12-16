#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUIContent : MonoBehaviour
{
	public int activeContent = 0;
	public ProgressBar progressBarReference;
	public ButtonController continueButtonScript;
	public ButtonController backButtonScript;
	public TextManager textManagerScript;

	public GameObject ButtonPanel;

	int numberOfPages;
	int toBeActivated;

	void Start()
	{
		numberOfPages = this.transform.childCount;
		continueButtonScript.disableOn = numberOfPages - 1;
		progressBarReference.numberOfPages = numberOfPages;
	}

    public void ActivateNextContent ()
    {
    	Debug.Log("change UI called and childcount: " + numberOfPages);
	    toBeActivated = activeContent + 1;
    	if(toBeActivated < numberOfPages)
    	{
    		textManagerScript.NextTextInstant();
	    	ActivateChildByIndex(toBeActivated);
	    	if(toBeActivated == transform.childCount-1)
	    	{
	    		ActivateButtons();
	    	}
	    	continueButtonScript.UpdateButtonState();
	    	backButtonScript.UpdateButtonState();
    	}	
    }

        public void ActivateAfterNextContent ()
    {
    	if(textManagerScript.DisplayNextText() == true)
    	{
	    	toBeActivated = activeContent + 2;
	    	if(toBeActivated < transform.childCount)
	    	{
	    		ActivateChildByIndex(toBeActivated);
	    	}
	    	if(toBeActivated == transform.childCount-1)
	    	{
	    		ActivateButtons();
	    	}
	    	continueButtonScript.UpdateButtonState();
	    	backButtonScript.UpdateButtonState();
    	}	
    }

    public void ActivatePrevContent ()
    {
    	if(textManagerScript.DisplayPreviousText() == true)
    	{
	    	toBeActivated = activeContent - 1;
	    	if(toBeActivated >= 0)
	    	{
	    		ActivateChildByIndex(toBeActivated);
	    	}
	    	continueButtonScript.UpdateButtonState();
	    	backButtonScript.UpdateButtonState();

	    	DeactivateButtons();
	    }
    }

    public void ActivateBeforePrevContent ()
    {
    	if(textManagerScript.DisplayPreviousText() == true)
    	{
	    	toBeActivated = activeContent - 2;
	    	if(toBeActivated >= 0)
	    	{
	    		ActivateChildByIndex(toBeActivated);
	    	}
	    	continueButtonScript.UpdateButtonState();
	    	backButtonScript.UpdateButtonState();

	    	DeactivateButtons();
	    }
    }

    public void ActivateChildByIndex(int childIndex)
    {
    	transform.GetChild(activeContent).gameObject.SetActive(false);
    	transform.GetChild(childIndex).gameObject.SetActive(true);
    	activeContent = childIndex;
    }

    void ActivateButtons()
    {
    	if(ButtonPanel != null)
    	ButtonPanel.SetActive(true);

    	textManagerScript.textContainer.SetActive(false);
    }

    void DeactivateButtons()
    {
    	if(ButtonPanel != null)
    	ButtonPanel.SetActive(false);
    }
}
