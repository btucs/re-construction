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

public class DropAreaHighlightManager : MonoBehaviour
{
    public GameObject inventoryHighlightObj;
    public GameObject targetHighlightObj;
    private UnityAction<InventoryItem> enableHighlightUA;
    private UnityAction<InventoryItem> disableHighlightUA;

    void Start()
    {
    	enableHighlightUA += EnableHighlight;
    	disableHighlightUA += DisableHighlight;
    }

    public void SetupInventoryItem(InventoryItem _itemScript)
    {
      _itemScript.OnMoveEvent.AddListener(enableHighlightUA);
    	_itemScript.OnReleaseEvent.AddListener(disableHighlightUA);
    }

    public void EnableHighlight(InventoryItem _itemScript)
    {
    	inventoryHighlightObj.SetActive(true);
    	targetHighlightObj.SetActive(true);
    }

    public void DisableHighlight(InventoryItem _itemScript)
    {
    	inventoryHighlightObj.SetActive(false);
    	targetHighlightObj.SetActive(false);
    	_itemScript.OnMoveEvent.RemoveListener(enableHighlightUA);
    	_itemScript.OnReleaseEvent.RemoveListener(disableHighlightUA);
    }
}
