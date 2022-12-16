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

public class UIExpandAnimator : MonoBehaviour
{
    public RectTransform uiContainer;


    public bool hideOnStart = false;
	public float animationTime = 0.5f;
	public float hideUIAfter = 3f;
	
	private bool showUI = true;
	private bool isAnimating = false;
	private bool willAnimate = false;

	private float currentTime = 0f;
	private Vector2 startPos, endPos;
    private Vector2 defaultUIPos;

    private void Start()
    {
        defaultUIPos = uiContainer.anchoredPosition;

        if(hideOnStart)
        {
        	HideUIInstant();
        }
    }

    private void Update()
    {
        if(willAnimate == false && showUI == true)
        {
        	Invoke("HideUI", hideUIAfter);
        	willAnimate = true;
        }

        if(Input.touchCount > 0 || Input.GetMouseButton(0))
        {
        	CancelInvoke("HideUI");
        	willAnimate = false;
        }

        if(isAnimating)
        {
 			if(currentTime <= animationTime)
 				currentTime += Time.deltaTime;
 			if(currentTime >= animationTime)
 			{
 				currentTime = animationTime;
 				isAnimating = false;
 			}

 			uiContainer.anchoredPosition = Vector2.Lerp(startPos, endPos, currentTime/animationTime);
        }

        if(isAnimating == false && showUI == false && (Input.touchCount > 0 || Input.GetMouseButton(0)))
        {
        	ShowUI();
        }
    }

    public void HideUIInstant()
    {
    	if(showUI)
    	{
	    	showUI = false;
	    	uiContainer.anchoredPosition = new Vector2(uiContainer.anchoredPosition.x, uiContainer.anchoredPosition.y - uiContainer.rect.height);    		
    	}
    }

    public void HideUI()
    { 
    	currentTime = 0f;

    	//Debug.Log("Calculating yPos by Height: " + topContainer.rect.height + " and CanvasScale: " + topCanvas.localScale.y);
    	startPos = uiContainer.anchoredPosition;
    	endPos = startPos;
    	endPos.y = endPos.y - uiContainer.rect.height;
    	showUI = false;
    	isAnimating = true;
    	willAnimate = false;
    }

    public void DisplayUIInstant()
    {
        uiContainer.anchoredPosition = defaultUIPos;
        showUI = true;
        isAnimating = false;
        willAnimate = false;
        CancelInvoke("HideUI");
    }

    public void ShowUI()
    { 
    	if(showUI == false)
    	{
	    	currentTime = 0f;
	    	
	    	startPos = uiContainer.anchoredPosition;
	    	endPos = startPos;
	    	endPos.y = endPos.y + uiContainer.rect.height;

	    	showUI = true;
	    	isAnimating = true;
    	}

    }

}
