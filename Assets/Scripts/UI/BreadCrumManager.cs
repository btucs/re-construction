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

public class BreadCrumManager : MonoBehaviour
{
	public GameObject secondLayerObject; 
	public GameObject secondBreadCrumObj;
	public GameObject pointer;
	public GameObject returnButton;
    public Text secondBreadCrumText;
    //public bool secondLayerOpened = false;
    private GameObject firstLayerObj;

	public void ReturnToFirstLayer()
	{
		if(secondLayerObject)
		{
			secondLayerObject.SetActive(false);
		}
		secondBreadCrumObj.SetActive(false);
		returnButton.SetActive(false);
		pointer.SetActive(false);

		if(firstLayerObj != null)
		{
			firstLayerObj.SetActive(true);
			firstLayerObj = null;
		}
	}

	public void openSecondLayer(GameObject newLayerObj, string layerName, GameObject _firstLayerObj = null)
	{
		secondBreadCrumText.text = layerName;
		secondBreadCrumObj.SetActive(true);
		secondLayerObject = newLayerObj;
		secondLayerObject.SetActive(true);
		pointer.SetActive(true);
		returnButton.SetActive(true);
		
		firstLayerObj = _firstLayerObj;
		if(firstLayerObj != null)
			firstLayerObj.SetActive(false);
	}

}