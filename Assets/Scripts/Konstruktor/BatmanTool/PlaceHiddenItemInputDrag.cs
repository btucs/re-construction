#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHiddenItemInputDrag : MonoBehaviour
{
	private Vector2 startPos;
	private Vector2 mousePosition;

	private float deltaX, deltaY;
	private Camera camRef;

    void Start()
    {
        startPos = transform.position;
        camRef = Camera.main;
    }

    private void OnMouseDown()
    {
    	deltaX = camRef.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
    	deltaY = camRef.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
    }

    private void OnMouseDrag()
    {
    	mousePosition = camRef.ScreenToWorldPoint(Input.mousePosition);
    	transform.position = new Vector3(mousePosition.x - deltaX, mousePosition.y - deltaY, transform.position.z);
    }

    private void OnMouseUp()
    {
    	//Update Position?
    }

}
