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

public class MainMenuOnboardingController : MonoBehaviour
{
	public bool showProfile = false;
	public bool showBib = false;
	public bool showMap = false;

    public OnboardingController onboardingManager;
    
    public GameObject menuPanel;
	public GameObject profileButton;
    public GameObject questlogButton;
	public GameObject mapButton;
	public GameObject bibButton;
	private List<GameObject> activeButtons = new List<GameObject>();

    public void UpdateSideMenuUI(OnboardingData myOnboardingData)
    {
        onboardingManager = OnboardingController.Instance;

    	CheckButtonStates(myOnboardingData);

    	UpdateMenuPanel();
    }

    public void CheckButtonStates(OnboardingData _onboardingData)
    {

        activeButtons.Clear();
    	if(_onboardingData.EventEntryExists(onboardingManager.unlockProfileEvent.UID))
        {
            activeButtons.Add(profileButton);
        } else {
            profileButton.SetActive(false);
        }

        if(_onboardingData.EventEntryExists(onboardingManager.unlockProfileEvent.UID))
        {
            activeButtons.Add(questlogButton);
        } else {
            questlogButton.SetActive(false);
        }

        if(_onboardingData.EventEntryExists(onboardingManager.unlockBibEvent.UID))
        {
            activeButtons.Add(bibButton);
        } else {
            bibButton.SetActive(false);
        }

        if(_onboardingData.EventEntryExists(onboardingManager.unlockMapEvent.UID))
    	{
    		activeButtons.Add(mapButton);
    	} else {
    		mapButton.SetActive(false);
    	}
    	
    }

    public void AddProfileButton()
    {
        activeButtons.Add(profileButton);
        profileButton.SetActive(true);
        UpdateMenuPanel();
    }

    public void AddQuestlogButton()
    {
        activeButtons.Add(questlogButton);
        questlogButton.SetActive(true);
        UpdateMenuPanel();
    }

    public void AddBibButton()
    {
        activeButtons.Add(bibButton);
        bibButton.SetActive(true);
        UpdateMenuPanel();
    }

    public void AddMapButton()
    {
        activeButtons.Add(mapButton);
        mapButton.SetActive(true);
        UpdateMenuPanel();
    }

    private void UpdateMenuPanel()
    {
    	RectTransform menuRT = menuPanel.GetComponent<RectTransform>();
    	menuPanel.SetActive(true);

    	if(activeButtons.Count == 0)
    	{
    		menuPanel.SetActive(false);
    	}
    	else if (activeButtons.Count == 1)
    	{
    		menuRT.sizeDelta = new Vector2(menuRT.sizeDelta.x, 164);
    	}
    	else if (activeButtons.Count == 2)
    	{
    		menuRT.sizeDelta = new Vector2(menuRT.sizeDelta.x, 240);
    	}
    	else
    	{
            float ySize = (float)(activeButtons.Count + 1)*80;
    		menuRT.sizeDelta = new Vector2(menuRT.sizeDelta.x, ySize);
    	}

        UpdateButtonPositions();
    }

    private void UpdateButtonPositions()
    {
    	for(int i = 0; i < activeButtons.Count; i++)
    	{
            int inverseIndex = activeButtons.Count - i;
    		activeButtons[i].SetActive(true);
    		activeButtons[i].GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, ((float)inverseIndex)*(1f/((float)activeButtons.Count + 1f)));
    		activeButtons[i].GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, ((float)inverseIndex)*(1f/((float)activeButtons.Count + 1f)));
    		activeButtons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-5f, 4f);
    	}
    }

}
