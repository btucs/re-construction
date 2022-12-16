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

public class OriginSceneLoader : MonoBehaviour
{
    public MLEDataTransporter prevSceneDataContainer;
    public GameObject confirmExitPopUp;
    private GameObject dataObject;

    void Start()
    {
        dataObject = GameObject.FindWithTag("MLEDataObj");
    	if(dataObject)
    	{
    		prevSceneDataContainer = dataObject.GetComponent<MLEDataTransporter>();
    		Debug.Log("Pref Scene name: " + prevSceneDataContainer.originSceneName);
    	}
    }

    public void ReturnToPrevScene()
    {
        SceneManager.LoadScene(prevSceneDataContainer.originSceneName);
    }

    public void HardExitMLE()
    {
        //confirmExitPopUp.SetActive(true);
        prevSceneDataContainer.achievedPoints = 0;
        SceneManager.LoadScene(prevSceneDataContainer.originSceneName);
    }
}
