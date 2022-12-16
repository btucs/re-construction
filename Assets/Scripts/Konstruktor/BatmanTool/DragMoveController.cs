#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMoveController : MonoBehaviour
{
	private Vector2 startPos;
	private Vector2 mousePosition;

	private float deltaX, deltaY;
	private Camera camRef;
	private bool unlockInteraction = false;


	public bool locked = false;
	public playerScript playerSceneControlls;
    public UIHoverController uiListener;

    private bool raycastBlocked = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    private void OnMouseDown()
    {
    	camRef = Camera.main;
        raycastBlocked = uiListener.CheckUIHover();
        
    	if(!locked)
    	{
    		if(playerSceneControlls != null)
		    {
		    	playerSceneControlls.DisableInteraction();
		    	unlockInteraction = true;
		    }

    		deltaX = camRef.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
    		deltaY = camRef.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
    	}
    }

    private void OnMouseDrag()
    {
        
    	if(!locked && !raycastBlocked)
    	{
    		mousePosition = camRef.ScreenToWorldPoint(Input.mousePosition);
    		transform.position = new Vector3(mousePosition.x - deltaX, mousePosition.y - deltaY, transform.position.z);
    	}
    }

    private void OnMouseUp()
    {
    	if(playerSceneControlls != null && unlockInteraction)
    	{
    		playerSceneControlls.EnableInteraction();
    		unlockInteraction = false;
    	}
    }

}
