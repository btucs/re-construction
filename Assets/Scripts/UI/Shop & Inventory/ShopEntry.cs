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
using TMPro;

public class ShopEntry : MonoBehaviour
{
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI itemNameText;
    public Image itemIMG;
    private PermanentItem itemData;
    private ShopController shopManager;

    public void Setup(PermanentItem data, ShopController shopController)
    {
        itemData = data;
        shopManager = shopController;

        itemIMG.sprite = data.image;
        priceText.text = itemData.price.ToString();
        itemNameText.text = itemData.itemName;
    }

    public bool IsItem(PermanentItem itemCheck)
    {
        if(itemData.itemName == itemCheck.itemName)
            return true;
        else
            return false;
    }

    public bool IsOfCategory(CosmeticCategory category)
    {
        if(itemData is CosmeticItem)
        {
            CosmeticItem cosmetic = itemData as CosmeticItem;
            return (cosmetic.category == category);
        }

        return false;
    }

    public void OnClickItem()
    {
        shopManager.RequestPurchase(this);
    }

    public PermanentItem GetItemData()
    {
        return itemData;
    }

}
