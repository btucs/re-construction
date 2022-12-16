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
using System.Linq;

public class CharacterCreationCategoryButton : MonoBehaviour
{
    // Start is called before the first frame update
    public string categoryName;
    public GameObject labelButtonPrefab;

    Image buttonImageComponent;
    SpriteLibrary buttonAssetLibrary;
    IEnumerable <string> labelNames;
    Transform labelContainerTransform;
    List<string> connectedGraphicContainerNames;
    
    public void SetButtonContent(string _category, SpriteLibrary _SpriteLibrary, GameObject _labelButtonContainer)
    {
    	//update Button data
    	buttonAssetLibrary = _SpriteLibrary;
    	categoryName = _category;
    	labelNames = buttonAssetLibrary.spriteLibraryAsset.GetCategoryLabelNames(categoryName);
    	labelContainerTransform = _labelButtonContainer.transform;
    	SetConnectedGraphicContainers(_category, new List<string>{_category});

    	//Set the Text of the Button to the category name
    	this.GetComponentInChildren<Text>().text = categoryName;
    	
    	//Set the Image of the Button to the first Image in that Category
    	Image[] buttonImageComponents = GetComponentsInChildren<Image>();
    	foreach(Image uiImg in buttonImageComponents)
    	{
    		if(uiImg.gameObject.transform.parent == this.gameObject.transform)
    		{
    			buttonImageComponent = uiImg;
    		}
    	}
    	string labelNameHolder;
    	labelNameHolder = buttonAssetLibrary.spriteLibraryAsset.GetCategoryLabelNames(categoryName).FirstOrDefault();
    	buttonImageComponent.sprite = buttonAssetLibrary.spriteLibraryAsset.GetSprite(categoryName, labelNameHolder);
    }

    public void SetButtonContentWithList(string _mainCategory, List<string> _subCategoryList, SpriteLibrary _SpriteLibrary, GameObject _labelButtonContainer)
    {
    	//update Button data
    	buttonAssetLibrary = _SpriteLibrary;
    	string primarySubCategory = ReturnPrimarySubCategory(_subCategoryList);
    	categoryName = _mainCategory + '_' + primarySubCategory;
    	labelNames = buttonAssetLibrary.spriteLibraryAsset.GetCategoryLabelNames(categoryName);
    	labelContainerTransform = _labelButtonContainer.transform;
    	SetConnectedGraphicContainers(_mainCategory, _subCategoryList);

    	//Set the Text of the Button to the category name
    	this.GetComponentInChildren<Text>().text = categoryName;
    	
    	//Set the Image of the Button to the first Image in that Category
    	Image[] buttonImageComponents = GetComponentsInChildren<Image>();
    	foreach(Image uiImg in buttonImageComponents)
    	{
    		if(uiImg.gameObject.transform.parent == this.gameObject.transform)
    		{
    			buttonImageComponent = uiImg;
    		}
    	}
    	
    	string labelNameHolder;
    	labelNameHolder = buttonAssetLibrary.spriteLibraryAsset.GetCategoryLabelNames(categoryName).FirstOrDefault();
    	buttonImageComponent.sprite = buttonAssetLibrary.spriteLibraryAsset.GetSprite(categoryName, labelNameHolder);
    }

    public void CreateSelectionList()
    {

    	int numberOfExistingButtons = labelContainerTransform.childCount;
    	int labelNumber = 0;
    	CharacterCreationLabelButton currentLabelButton;

    	foreach(string labelName in labelNames)
    	{
    		labelNumber++;
    		if(numberOfExistingButtons >= labelNumber)
    		{
    			currentLabelButton = labelContainerTransform.GetChild(labelNumber-1).GetComponent<CharacterCreationLabelButton>();
    			if(currentLabelButton != null)
    			{
    				//currentLabelButton.labelNameString = labelName; !!!
    			}
    		} else 
    		{
    			GameObject labelButtonObject = Instantiate(labelButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, labelContainerTransform);
    			currentLabelButton = labelButtonObject.GetComponent<CharacterCreationLabelButton>();
    		}

    		//currentLabelButton.SetLabelButtonSettings(buttonAssetLibrary, categoryName, labelName, connectedGraphicContainerNames); !!!
    	}

		//if there are more childobjects than labelNames destroy the childobjects, that we do not need
    	for(int i = labelNumber; i < numberOfExistingButtons; i++)
    	{
    		Destroy(labelContainerTransform.transform.GetChild(i).gameObject);
    	}
    }

    public string ReturnPrimarySubCategory(List<string> _subCategoryNames)
    {
    	string firstNameString = _subCategoryNames.FirstOrDefault();
    	foreach(string subCategoryString in _subCategoryNames)
    	{
    		if(subCategoryString.Contains('#'))
    		{
    			return subCategoryString;
    		}
    	}
    	return firstNameString;
    }

    public void SetConnectedGraphicContainers(string mainCategoryString, List<string> subCategoryStrings)
    {
    	connectedGraphicContainerNames = new List<string>();
    	if(subCategoryStrings.FirstOrDefault() == mainCategoryString)
    	{
    		connectedGraphicContainerNames.Add(mainCategoryString);
    	}
    	else 
    	{
			foreach (string singleString in subCategoryStrings)
	    	{
	    		connectedGraphicContainerNames.Add(mainCategoryString + '_' + singleString);
	    		//Debug.Log("connected Graphic Object: " + mainCategoryString + '_' + singleString);
	    	}
    	}
    }
} 