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

public class CharacterCreationCategoryToggle : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject labelButtonPrefab;
    public bool openOnStart = false;
    public CosmeticCategory category;
    private CharacterCreationLabelButton currentlySelectedButton; 
    private List<CosmeticItem> items;
    private string[] categoryNames;

    Image buttonImageComponent;
    SpriteLibrary buttonAssetLibrary;
    IEnumerable <string> labelNames;
    Transform labelContainerTransform;
    List<string> connectedGraphicContainerNames;
    private CosmeticItem selectedItem = null;
    private characterGraphicsUpdater graphicsUpdater;

    void Start()
    {
        graphicsUpdater = buttonAssetLibrary.gameObject.GetComponent<characterGraphicsUpdater>();
        if(openOnStart)
        {
            CreateSelectionList();
        } else {
            IdentifySelected();
        }
    }

    public bool IsItemEquipped(CosmeticItem itemData)
    {
        return graphicsUpdater.IsItemEquipped(itemData.GetCategoryNames()[0], itemData.label);
    }

    private List<CosmeticItem> GetDefaultCosmetics(CosmeticCategory _category)
    {
        return GameController.GetInstance().gameAssets.GetCosmeticsOfCategory(_category, true);
    } 

    public void SelectInCategory(CharacterCreationLabelButton newSelection)
    {
        if(currentlySelectedButton != null)
            currentlySelectedButton.SetSelected(false);

        currentlySelectedButton = newSelection;
        selectedItem = currentlySelectedButton.GetItemData();
    }

    public CosmeticItem GetSelected()
    {
        return selectedItem;
    }

    public void SetButtonContent(string[] categories, SpriteLibrary _SpriteLibrary, GameObject _labelButtonContainer)
    {
    	//update Button data
    	categoryNames = categories;
    	buttonAssetLibrary = _SpriteLibrary;
        labelContainerTransform = _labelButtonContainer.transform;
    	//labelNames = buttonAssetLibrary.spriteLibraryAsset.GetCategoryLabelNames(categories[0]);
    	items = GetDefaultCosmetics(category);

    	//Set the Text of the Button to the category name
    	//this.GetComponentInChildren<Text>().text = categoryName;
    	
    	//Set the Image of the Button to the first Image in that Category
    	//Image[] buttonImageComponents = GetComponentsInChildren<Image>();
    	//foreach(Image uiImg in buttonImageComponents)
    	//{
    	//	if(uiImg.gameObject.transform.parent == this.gameObject.transform)
    	//	{
    	//		buttonImageComponent = uiImg;
    	//	}
    	//}
    	//string labelNameHolder;
    	//labelNameHolder = buttonAssetLibrary.spriteLibraryAsset.GetCategoryLabelNames(categoryName).FirstOrDefault();
    	//buttonImageComponent.sprite = buttonAssetLibrary.spriteLibraryAsset.GetSprite(categoryName, labelNameHolder);
    }

    public void SetButtonContentWithList(string _mainCategory, string[] _subCategoryList, SpriteLibrary _SpriteLibrary, GameObject _labelButtonContainer)
    {
    	//update Button data
    	buttonAssetLibrary = _SpriteLibrary;
    	//string primarySubCategory = ReturnPrimarySubCategory(_subCategoryList); !!!
    	//categoryName = _mainCategory + '_' + primarySubCategory; !!!
    	//labelNames = buttonAssetLibrary.spriteLibraryAsset.GetCategoryLabelNames(categoryName); !!!
    	labelContainerTransform = _labelButtonContainer.transform;
    	//Set the Text of the Button to the category name
    	//this.GetComponentInChildren<Text>().text = categoryName;
    	
    	//Set the Image of the Button to the first Image in that Category
    	//Image[] buttonImageComponents = GetComponentsInChildren<Image>();
    	//foreach(Image uiImg in buttonImageComponents)
    	//{
    	//	if(uiImg.gameObject.transform.parent == this.gameObject.transform)
    	//	{
    	//		buttonImageComponent = uiImg;
    	//	}
    	//}
    	
    	//string labelNameHolder;
    	//labelNameHolder = buttonAssetLibrary.spriteLibraryAsset.GetCategoryLabelNames(categoryName).FirstOrDefault();
    	//buttonImageComponent.sprite = buttonAssetLibrary.spriteLibraryAsset.GetSprite(categoryName, labelNameHolder);
    }

    public void CreateSelectionList()
    {
	    	int numberOfExistingButtons = labelContainerTransform.childCount;
	    	int entryNumber = 0;
	    	CharacterCreationLabelButton currentLabelButton;

	    	foreach(CosmeticItem item in items)
	    	{
	    		entryNumber++;
	    		if(numberOfExistingButtons >= entryNumber)
	    		{
	    			currentLabelButton = labelContainerTransform.GetChild(entryNumber-1).GetComponent<CharacterCreationLabelButton>();
	    		} else {
	    			currentLabelButton = Instantiate(labelButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, labelContainerTransform).GetComponent<CharacterCreationLabelButton>();
	    		}

	    		currentLabelButton.SetLabelButtonSettings(this, buttonAssetLibrary, item, categoryNames);
	    	}

			//if there are more childobjects than labelNames destroy the childobjects, that we do not need
	    	for(int i = entryNumber; i < numberOfExistingButtons; i++)
	    	{
	    		Destroy(labelContainerTransform.transform.GetChild(i).gameObject);
	    	}

    }

    private void IdentifySelected()
    {
        foreach(CosmeticItem item in items)
        {
            if(graphicsUpdater.IsItemEquipped(item.GetCategoryNames()[0], item.label)){
                selectedItem = item;
            }
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