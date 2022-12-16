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
using UnityEngine.Experimental.U2D.Animation;

public class CharacterCreationLabelButton : MonoBehaviour
{
	public Image buttonBG;
	private string labelString;
	private CosmeticItem connectedItemData;
	public Color defaultBGColor;
	private string categoryNameString;
	private Image buttonImageComponent;
	private characterGraphicsUpdater labelButtonsGraphicsUpdater;
	private Dictionary <string, string> graphicObjectsWithLabels;
	private CharacterCreationCategoryToggle categoryController;

	public void SetLabelButtonSettings(CharacterCreationCategoryToggle _categoryController, SpriteLibrary _spriteLibrary, CosmeticItem newItemData, string[] _relatedObjectNames)
	{
		categoryController = _categoryController;
		//update the data of this button
		connectedItemData = newItemData;
		labelString = newItemData.label; 
		labelButtonsGraphicsUpdater = _spriteLibrary.gameObject.GetComponent<characterGraphicsUpdater>();

		//save names of gameobjects with the corresponding labelnames
		graphicObjectsWithLabels = new Dictionary <string, string>();
		for(int i = 0; i < _relatedObjectNames.Length; i++)
		{
			string labelNameOfObject = GetObjectsLabelName(_spriteLibrary, _relatedObjectNames[i]);
			graphicObjectsWithLabels.Add(_relatedObjectNames[i], labelNameOfObject);
		}

		//Set the Image of the Button to the image of that label
    	GetIMGComponent();
    	buttonImageComponent.sprite = connectedItemData.image;

    	IsSelectedItemCheck();
	}

	public CosmeticItem GetItemData()
	{
		return connectedItemData;
	}

	public void IsSelectedItemCheck()
	{
		Debug.Log("Checking if it is equipped: " + labelButtonsGraphicsUpdater.IsItemEquipped(connectedItemData.GetCategoryNames()[0], connectedItemData.label));
		if(labelButtonsGraphicsUpdater.IsItemEquipped(connectedItemData.GetCategoryNames()[0], connectedItemData.label))
			SetAsSelectedInCategory();
		else
			SetSelected(false);
	}

	private string GetObjectsLabelName(SpriteLibrary _spriteLibrary, string objectNameString)
	{
		//Get all labels of Category
		IEnumerable <string> labelNamesOfObject = _spriteLibrary.spriteLibraryAsset.GetCategoryLabelNames(objectNameString);
		//Check which complete label contains the shortened labelstring and return that one
		foreach(string singleLabelName in labelNamesOfObject)
		{
			if(singleLabelName.Contains(labelString))
			{
				return singleLabelName;
			}
		}
		Debug.Log("No label found containing: " + labelString);
		return "none";
	}

	private void GetIMGComponent()
	{
		Image[] buttonImageComponents = GetComponentsInChildren<Image>();
    	foreach(Image uiImg in buttonImageComponents)
    	{
    		if(uiImg.gameObject.transform.parent == this.gameObject.transform)
    		{
    			buttonImageComponent = uiImg;
    		}
    	}
	}

	public void SetAsSelectedInCategory()
	{
		categoryController.SelectInCategory(this);
		SetSelected(true);
	}

	public void SetSelected(bool isSelected)
	{
		buttonBG.color = isSelected ? Color.white : defaultBGColor;
	}

	public void UpdateCharacterSprite()
	{
		if(labelButtonsGraphicsUpdater != null)
		{
			foreach (KeyValuePair<string, string> graphicObjectName in graphicObjectsWithLabels)
			{
				//Debug.Log("Trying to update: " + graphicObjectName.Key + " with the sprite of: " + graphicObjectName.Value);

				labelButtonsGraphicsUpdater.UpdateImageOfCategory(graphicObjectName.Key, graphicObjectName.Value);
				labelButtonsGraphicsUpdater.UpdateSingleData(graphicObjectName.Key, graphicObjectName.Value);	
			}
		}
	}
}
