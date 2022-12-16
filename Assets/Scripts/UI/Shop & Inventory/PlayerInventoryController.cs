#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    public List<InventoryCategory> categories = new List<InventoryCategory>();
    public GameObject slotPrefab;
    public GameObject portfolioUI;
    public Transform slotContainer;
    public RectTransform playerFrame;
    public string headline = "Gardrobe";
    private List<InventorySlot> inventorySlots = new List<InventorySlot>();
    private GameController gameController;
    private InventoryData playerInventoryData;
    private characterGraphicsUpdater playerGraphics;
    private bool initialized = false;
    private WorldSceneCameraController camController;

    public void OpenInventory()
    {
    	if(!initialized)
    	{
    		Setup();
    	} else {
    		UpdatePlayerItemList();
    	}

    	this.gameObject.SetActive(true);
    	MenuUIController.Instance.breadcrumController.openSecondLayer(this.gameObject, headline, portfolioUI);
    	FocusPlayerGraphics();
    }

    public void Setup()
    {
    	gameController = GameController.GetInstance();
    	playerInventoryData = gameController.gameState.profileData.inventory;
    	playerGraphics = playerScript.Instance.GetPlayerGraphics();
    	camController = MenuUIController.Instance.cameraController;

    	CreatePlayerItemList();
    	initialized = true;
    }

    public void DeselectAllCategories()
    {
        foreach(InventoryCategory cat in categories)
        {
            cat.Deselect();
        }
    }

    private void CreatePlayerItemList()
    {
    	List<PermanentItem> playerItems = playerInventoryData.GetItemListFromSaveData(gameController.gameAssets);

    	foreach(PermanentItem playerItem in playerItems)
    	{
            if(playerItem is CosmeticItem)
            {
                CosmeticItem _item = playerItem as CosmeticItem;
                if(DisplayCheck(_item))
                {
            		InventorySlot newSlot = Instantiate(slotPrefab, slotContainer).GetComponent<InventorySlot>();
            		newSlot.Setup(this, playerItem);
            		inventorySlots.Add(newSlot);
                }
            }
    	}
    }

    private void UpdatePlayerItemList()
    {
    	List<PermanentItem> playerItems = playerInventoryData.GetItemListFromSaveData(gameController.gameAssets);

    	foreach(PermanentItem playerItem in playerItems)
    	{
    		if(playerItem is CosmeticItem)
            {
                CosmeticItem _item = playerItem as CosmeticItem;
	    		if(ContainsPermanentItem(playerItem) == false && DisplayCheck(_item))
	    		{
	    			InventorySlot newSlot = Instantiate(slotPrefab, slotContainer).GetComponent<InventorySlot>();
	    			newSlot.Setup(this, playerItem);
	    			inventorySlots.Add(newSlot);
	    		}
            }
    	}
    }

    private bool DisplayCheck(CosmeticItem _item)
    {
    	return (_item.category != CosmeticCategory.hair && _item.category != CosmeticCategory.mouth && _item.category != CosmeticCategory.nose);
    }

    private bool ContainsPermanentItem(PermanentItem checkItem)
    {
    	foreach(InventorySlot existingSlot in inventorySlots)
    	{
    		if(existingSlot.item == checkItem)
    		{
    			return true;
    		}
    	}
    	return false;
    }

    public void FilterInventory(CosmeticCategory filter)
    {
    	foreach(InventorySlot slot in inventorySlots)
    	{
    		slot.gameObject.SetActive(slot.IsOfCategory(filter));
    	}
    }

    public void ResetFilter()
    {
        foreach(InventorySlot slot in inventorySlots)
        {
            slot.gameObject.SetActive(true);
        }
    }


    public void EquipCosmetic(CosmeticItem itemToEquip, InventorySlot itemSlot)
    {
    	playerGraphics.EquipItem(itemToEquip);

    	foreach(InventoryCategory itemGroup in categories)
    	{
    		if(itemGroup.category == itemToEquip.category)
    		{
    			itemGroup.OnEquipItem(itemSlot);
    		}
    	}
    	//update player graphics;
    }

    public void TryEquipDefaultItem(CosmeticCategory _category)
    {
    	foreach(InventoryCategory itemGroup in categories)
    	{
	    	if(itemGroup.category == _category && itemGroup.defaultItem != null)
	    	{
	    		playerGraphics.EquipItem(itemGroup.defaultItem);
	    		itemGroup.OnEquipItem(null);
	    	}
	    }
    }

    public bool IsItemEquipped(CosmeticItem checkItem)
    {
    	return playerGraphics.IsItemEquipped(checkItem.GetCategoryNames()[0], checkItem.label);
    }

    public void FocusPlayerGraphics()
    {
    	//get offset of playerFrame from scene center
    	Vector3 camGoalPosition = playerGraphics.transform.position;
    	Vector3 frameOffset = playerFrame.position - camController.transform.position;
    	camGoalPosition -= frameOffset;
    	camGoalPosition.y += 1.5f;
    	camController.ForceMoveTowards(camGoalPosition);
    }

}

