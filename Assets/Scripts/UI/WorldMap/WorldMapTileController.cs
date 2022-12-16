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

public class WorldMapTileController : MonoBehaviour
{
    [HideInInspector]
    public WorldMapTile tileData;

    private Color32 goodColor1 = new Color32(159, 171, 169, 255);
    private Color32 averageColor1 = new Color32(196, 195, 183, 255);
    private Color32 badColor1 = new Color32(189, 160, 172, 255);
    public Color32 lastColor;
    public Color32 targetColor;

    private float currentTime = 0;
    private float animationDuration = 0.3f;
    private Image thisImg;

    private bool shouldChange = false; 

    void Update()
    {
    	SetImageComponent();

    	if(shouldChange)
    	{
    		if(targetColor != thisImg.color)
    		{
    			currentTime = currentTime + Time.deltaTime;
	    		ColorUpdate(currentTime/animationDuration);
    		} 
    		else 
    		{
    			shouldChange = false;
    		}
    	}
    }

    public void ImproveTileState()
    {
        if (tileData.tileStatus == tileState.average)
        {
            tileData.tileStatus = tileState.good;
            UpdateTileDisplay();
        } 
        else if (tileData.tileStatus == tileState.bad)
        {
           tileData.tileStatus = tileState.average;
            UpdateTileDisplay();
        }
    }

    public void UpdateTileDisplay()
    {
        SetImageComponent();
    	if(tileData.tileStatus == tileState.good && thisImg.color != goodColor1)
    	{
    		StartColorChange(goodColor1);	
    	} 
    	else if (tileData.tileStatus == tileState.average && thisImg.color != averageColor1)
    	{
    		StartColorChange(averageColor1);
    	} 
    	else if (tileData.tileStatus == tileState.bad && thisImg.color !=badColor1)
    	{
    		StartColorChange(badColor1);
    	}
    }

    public void StartColorChange(Color newColor)
    {
    	SetImageComponent();

    	shouldChange = true;
    	currentTime = 0f;
    	lastColor = thisImg.color;
    	targetColor = newColor;
    }

    private void ColorUpdate(float _time)
    {
    	thisImg.color = Color.Lerp(lastColor, targetColor, _time);
    }


    private void SetImageComponent()
    {
    	if(thisImg == null)
    	{
    		thisImg = GetComponent<Image>();
    	}
    }

    public void UpdateColorInstant()
    {
		SetImageComponent();
		Color32 colorHolder = new Color32(100, 100, 100, 100);

    	if(tileData.tileStatus == tileState.good)
    	{
    		colorHolder = goodColor1;
    	}
    	else if (tileData.tileStatus == tileState.average)
    	{
    		colorHolder = averageColor1;
    	}
    	else if (tileData.tileStatus == tileState.bad)
    	{
    		colorHolder = badColor1;
    	}
    	
    	thisImg.color = colorHolder;
    }

}
