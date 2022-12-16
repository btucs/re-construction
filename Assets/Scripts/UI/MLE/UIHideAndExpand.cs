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

public class UIHideAndExpand : MonoBehaviour
{
	public RectTransform topContainer;
	public Transform topCanvas;
	public RectTransform bottomContainer;
	public CustomVCR videoController;
	private bool showUI = true;
	private bool isAnimating = false;
	private bool willAnimate = false;
	public float animationTime = 0.5f;
	public float hideUIAfter = 3f;
	private float currentTime = 0f;
	private Vector2 topStartPos, topEndPos, bottomStartPos, bottomEndPos;
    private Vector2 defaultTopUIPos;
    private Vector2 defaultBottomUIPos;

    // Start is called before the first frame update
    void Start()
    {
        defaultTopUIPos = topContainer.anchoredPosition;
        defaultBottomUIPos = bottomContainer.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(willAnimate == false && videoController.videoIsPlaying == true && showUI == true)
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

 			topContainer.anchoredPosition = Vector2.Lerp(topStartPos, topEndPos, currentTime/animationTime);
 			bottomContainer.anchoredPosition = Vector2.Lerp(bottomStartPos, bottomEndPos, currentTime/animationTime);
        }

        if(isAnimating == false && showUI == false && (Input.touchCount > 0 || Input.GetMouseButton(0)))
        {
        	ShowUI();
        }
    }

    public void HideUI()
    { 
    	currentTime = 0f;

    	topStartPos = topContainer.anchoredPosition;
    	topEndPos = topStartPos;
    	topEndPos.y = topEndPos.y + topContainer.rect.height;
    	Debug.Log("Calculating yPos by Height: " + topContainer.rect.height + " and CanvasScale: " + topCanvas.localScale.y);
    	bottomStartPos = bottomContainer.anchoredPosition;
    	bottomEndPos = bottomStartPos;
    	bottomEndPos.y = bottomEndPos.y - bottomContainer.rect.height;
    	showUI = false;
    	isAnimating = true;
    	willAnimate = false;
    }

    public void DisplayUIInstant()
    {
        topContainer.anchoredPosition = defaultTopUIPos;
        bottomContainer.anchoredPosition = defaultBottomUIPos;
        showUI = true;
        isAnimating = false;
        willAnimate = false;
        CancelInvoke("HideUI");
    }

    public void ShowUI()
    { 
    	Debug.Log("Show UI is called");
    	currentTime = 0f;

    	topStartPos = topContainer.anchoredPosition;
    	topEndPos = topStartPos;
    	topEndPos.y = topEndPos.y - topContainer.rect.height;
    	
    	bottomStartPos = bottomContainer.anchoredPosition;
    	bottomEndPos = bottomStartPos;
    	bottomEndPos.y = bottomEndPos.y + bottomContainer.rect.height;
    	showUI = true;
    	isAnimating = true;
    }
}
