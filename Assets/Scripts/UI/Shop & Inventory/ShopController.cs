#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ShopController : MonoBehaviour
{
    public GameObject portfolioUI;

    public GameObject shopEntryPrefab;
    public List<TextMeshProUGUI> playerCashTexts = new List<TextMeshProUGUI>();

    public Transform shopContainer;
    public string headline = "Shop";

    public Dropdown filterUI;

    public UnityEvent onItemPurchase = new UnityEvent();

    private List<ShopEntry> shopEntries = new List<ShopEntry>();
    private GameController gameController;
    private PermanentItem purchaseItem;
    private InventoryData playerInventoryData;
    private PopUpManager popUpManager;
    private UnityAction itemPurchaseCommand;

    public void Open()
    {
        Setup();
        UpdateCurrencyDisplay();
        InitializeItemList();
        MenuUIController.Instance.breadcrumController.openSecondLayer(this.gameObject, headline, portfolioUI);
    }

    public void Setup()
    {
        gameController = GameController.GetInstance();
        playerInventoryData = gameController.gameState.profileData.inventory;
        popUpManager = PopUpManager.Instance;

    }

    public void FilterShopEntries()
    {
        int filterID = filterUI.value;

        if(filterID == 0)
        {
            ResetFilter();
        } else if(filterID == 1)
        {
            FilterShopEntries(CosmeticCategory.torso);
        } else if(filterID == 2)
        {
            FilterShopEntries(CosmeticCategory.legs);
        } else if(filterID == 3)
        {
            FilterShopEntries(CosmeticCategory.decoration);
        }

    }

    public void FilterShopEntries(CosmeticCategory filter)
    {
        foreach(ShopEntry shopEntry in shopEntries)
        {
            shopEntry.gameObject.SetActive(shopEntry.IsOfCategory(filter));
        }
    }

    public void ResetFilter()
    {
        foreach(ShopEntry shopEntry in shopEntries)
        {
            shopEntry.gameObject.SetActive(true);
        }
    }

    public void UpdateCurrencyDisplay()
    {
        Setup();
        foreach(TextMeshProUGUI playerCashText in playerCashTexts)
        {
            playerCashText.text = playerInventoryData.currencyAmount.ToString();
        }
    }

    public void InitializeItemList()
    {
        PermanentItem[] items = gameController.gameAssets.GetItems();
        for(int i = 0; i < items.Length; i++)
        {
            if(items[i].canBeBought && items[i].requirement.ConditionMet(gameController))
            {
                if(ShopListContains(items[i]) == false)
                    shopEntries.Add(CreateShopEntry(items[i]));
            }
        }
    }

    public void RequestPurchase(ShopEntry requestEntry)
    {
        purchaseItem = requestEntry.GetItemData();

        bool canPurchase = (purchaseItem.price <= playerInventoryData.currencyAmount);
        if(!canPurchase)
        {
            string feedbackText = "Du kannst dir den ausgewählten Gegenstand nicht leisten.";
            popUpManager.DisplayFeedbackPopUp(feedbackText);
        } else {
            string confirmText = "Bist du sicher, dass du " + requestEntry.GetItemData().itemName + " kaufen willst?";
            itemPurchaseCommand = null;
            itemPurchaseCommand += ConfirmPurchase;
            popUpManager.DisplayConfirmPopUp(confirmText, itemPurchaseCommand);
        }
    }

    public void ConfirmPurchase()
    {
        playerInventoryData.currencyAmount -= purchaseItem.price;
        UpdateCurrencyDisplay();
        playerInventoryData.AddItem(purchaseItem);

        if(onItemPurchase != null)
          onItemPurchase.Invoke();

        gameController.gameState.profileData.inventory = playerInventoryData;
        gameController.SaveGame();
    }

    private ShopEntry CreateShopEntry(PermanentItem item)
    {
        ShopEntry itemEntryScript = Instantiate(shopEntryPrefab, shopContainer).GetComponent<ShopEntry>();
        itemEntryScript.Setup(item, this);
        return itemEntryScript;
    }

    private bool ShopListContains(PermanentItem item)
    {
        foreach(ShopEntry listEntry in shopEntries)
        {
            if(listEntry.IsItem(item))
                return true;
        }
        return false;
    }
}
