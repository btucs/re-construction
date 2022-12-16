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

public class AreaSelection : MonoBehaviour
{
    public Image areaIcon;
    public GameObject newContentIcon;
    public GameObject activeAreaIMG;
    public GameObject highlight;

    private WorldMapController worldMapScriptRef;
    private AreaDataSO buttonData;
    [HideInInspector]
    public Vector2Int mapPosition;

    public void Setup(AreaDataSO newData, WorldMapController controller, Vector2Int mapPos, bool isActiveArea = false)
    {
        buttonData = newData;
        areaIcon.sprite = buttonData.areaIconIMG;
        worldMapScriptRef = controller;
        mapPosition = mapPos;
        activeAreaIMG.SetActive(isActiveArea);
    }

    public void DisplayNewContentMarker(bool display = true)
    {
        newContentIcon.SetActive(display);
    }

    public void HandleClick()
    {
        if(worldMapScriptRef.interactable)
        {
            DisplayAreaData();
            worldMapScriptRef.uiController.SelectAreaButton(this);
        }
    }

    public void SetSelectionState(bool selected)
    {
        highlight.SetActive(selected);
    }

    public void DisplayAreaData()
    {
        if(buttonData != null)
            worldMapScriptRef.uiController.UpdateAreaPreviewData(buttonData);

        worldMapScriptRef.OpenAreaDescription();
    }
}
