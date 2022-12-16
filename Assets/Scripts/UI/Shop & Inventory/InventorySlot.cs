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

public class InventorySlot : MonoBehaviour
{
    public Image iconIMG;
    public GameObject equippedIndicator;
    public GameObject newIndicator;

    [HideInInspector]
    public PermanentItem item;
    private CosmeticItem cosmetic;
    private bool isEquipped = false;
    private PlayerInventoryController controller;

    public void Setup(PlayerInventoryController inventoryController, PermanentItem itemData)
    {
    	item = itemData;
    	controller = inventoryController;

    	iconIMG.sprite = itemData.image;
    	if(item is CosmeticItem)
    	{
    		cosmetic = item as CosmeticItem;
    		isEquipped = inventoryController.IsItemEquipped(cosmetic);
            if(isEquipped)
            {
                controller.EquipCosmetic(cosmetic, this);
            }
	    	equippedIndicator.SetActive(isEquipped);
    	}
    }

    public void OnSelect()
    {
    	if(cosmetic != null && !isEquipped)
    	{
	    	controller.EquipCosmetic(cosmetic, this);
	    	SetSelected(true);
    	} else if(isEquipped){
            controller.TryEquipDefaultItem(cosmetic.category);
        }
    }

    public void SetSelected(bool selected)
    {
        isEquipped = selected;
    	equippedIndicator.SetActive(selected);
    }

    public bool IsOfCategory(CosmeticCategory _category)
    {
    	if(item != null && item is CosmeticItem)
    	{
    		CosmeticItem cosmetic = item as CosmeticItem;
    		return (_category == cosmetic.category);	
    	}
    	return false;
    }
}
