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
using UnityEngine.SceneManagement;

public class WorldMapUI : MonoBehaviour
{
	public Text AreaNameText;
	public Text DescriptionText;
	public Image previewIMGHolder;
	public AreaButtonController enterAreaButton;
	[HideInInspector]
	public List<AreaSelection> areaSelectionButtons = new List<AreaSelection>();
	public GameObject areaPreviewContainer;
	public WorldMapController worldMapRef;

	public void UpdateAreaPreviewData(AreaDataSO selectedAreaData)
	{
	    if(DescriptionText != null) {
	    	DescriptionText.text = selectedAreaData.areaDescription;
	    	AreaNameText.text = selectedAreaData.areaName;
	    	if(selectedAreaData.sceneName == SceneManager.GetActiveScene().name) {
	    		enterAreaButton.gameObject.SetActive(false);
	    	} else {
	    		enterAreaButton.gameObject.SetActive(true);
	    		enterAreaButton.SetSelectedArea(selectedAreaData);
	    	}
	    	previewIMGHolder.sprite = selectedAreaData.areaPreviewIMG;
	    }
	}

	public void SelectAreaButton(AreaSelection selectedButton)
	{
		DeselectAllAreaButtons();
		worldMapRef.ScrollToMapPosition(selectedButton.mapPosition, Vector2.zero);
		selectedButton.SetSelectionState(true);
	}

	public void DeselectAllAreaButtons()
	{
		foreach (AreaSelection areaButton in areaSelectionButtons)
		{
			areaButton.SetSelectionState(false);
		}
	}

	public void OpenAreaDescription()
	{
		areaPreviewContainer.SetActive(true);
	}

	public void CloseAreaDescription()
	{
		areaPreviewContainer.SetActive(false);
		DeselectAllAreaButtons();
	}


}
