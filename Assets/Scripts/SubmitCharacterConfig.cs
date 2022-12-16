#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitCharacterConfig : MonoBehaviour
{
    public CharacterChoiceSaver choiceController;
    public CameraCapture imgCapture;
    public SceneManagement sceneLoader;
    public GameObject confirmButtonPanel;

    public void TrySubmit()
    {
    	bool isNotEqual = choiceController.ChoiceHasChanged();
    	Debug.Log("Comparing dictionaries " + isNotEqual);
    	if(isNotEqual == true)
    	{
    		imgCapture.GrabImage();
    		choiceController.SaveCustomPlayerPreferences();
    		sceneLoader.LoadOnboardingSecondPart();
    	} else 
    	{
    		confirmButtonPanel.SetActive(true);
    	}
    }

    public void ForceSubmit()
    {
    	imgCapture.GrabImage();
    	choiceController.SaveCustomPlayerPreferences();
    	sceneLoader.LoadOnboardingSecondPart();
    }
}
