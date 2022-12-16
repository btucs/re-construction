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

public class InventoryCategory : MonoBehaviour
{
    public CosmeticCategory category;
    public PlayerInventoryController inventoryController;
    public Color selectedBGColor;
    public Color defaultBGColor;
    public Image bgIMG;
    public CosmeticItem defaultItem;

    private InventorySlot equippedItem;

    private bool isActive = false;

    public void OnSelectCategory()
    {

        if(!isActive)
        {
            inventoryController.DeselectAllCategories();
            inventoryController.FilterInventory(category);
            bgIMG.color = selectedBGColor;
            isActive = true;
        } else {
            inventoryController.ResetFilter();
            bgIMG.color = defaultBGColor;
            isActive = false;
        }
    }

    public void Deselect()
    {
        bgIMG.color = defaultBGColor;
        isActive = false;
    }


    public void OnEquipItem(InventorySlot newEquipped)
    {
    	if(equippedItem != null && equippedItem != newEquipped)
    	{
    		equippedItem.SetSelected(false);
    	}

    	equippedItem = newEquipped;
        if(newEquipped != null)
        	newEquipped.SetSelected(true);
    }
}
