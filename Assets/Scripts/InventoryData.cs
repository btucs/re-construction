#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
	public int currencyAmount = 600;
    public string[] items = new string[0];

    public void SetItemSaveData(List<PermanentItem> _items)
    {
    	items = new string[_items.Count];
    	for(int i = 0; i < _items.Count; i++)
    	{
    		items[i] = _items.ElementAt(i).UID;
    	}
    }

    public bool Contains(CosmeticItem _item)
    {
        for(int i=0; i<items.Length; i++)
        {
            if(items[i] == _item.UID)
                return true;
        }

        return false;
    }

    public void AddItem(CosmeticItem newItem)
    {
        Debug.Log("Item Added: " + newItem.itemName);
        if(items.Length > 0)
        {
            string[] itemID = new string[] {newItem.UID};
            string[] result = items.Union(itemID).ToArray();
            items = result;
        } else {
            items = new string[] {newItem.UID};
        }
    }

    public void AddItem(PermanentItem newItem)
    {
        Debug.Log("Perma item added");
        string[] itemID = new string[] {newItem.UID};
        string[] result = items.Union(itemID).ToArray();
        items = result;
    }

    public void AddItems(List<CosmeticItem> newItems)
    {
        foreach(CosmeticItem newItem in newItems)
        {
            AddItem(newItem);
            Debug.Log("Item Added!");
        }
    }

    /*public string[] GetItemIDs()
    {
    	string[] returnIDs = new string [items.Count];
    	for(int i = 0; i < items.Count; i++)
    	{
    		returnIDs[i] = items.ElementAt(i).UID;
    	}

    	return returnIDs;
    }*/

    public List<PermanentItem> GetItemListFromSaveData(GameAssetsSO assetData)
    {
		List<PermanentItem> itemData = new List<PermanentItem>();

    	for(int i = 0; i < items.Length; i++)
    	{
    		PermanentItem itemOfID = assetData.FindItem(items[i]);
    		itemData.Add(itemOfID);
    	}

    	return itemData;
    }
}
