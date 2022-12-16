#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILineRenderer : MonoBehaviour
{
	public List<Transform> checkpoints = new List<Transform>();
	private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        this.DrawLine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DrawLine()
    {
    	GameObject lineObject = new GameObject();
    	this.lineRenderer = lineObject.AddComponent<LineRenderer>();
    	this.lineRenderer.startWidth = 0.2f;
    	this.lineRenderer.endWidth = 0.2f;
    	this.lineRenderer.positionCount = checkpoints.Count;

    	Vector3[] checkPointArray = new Vector3[this.checkpoints.Count];

    	for(int i = 0; i< this.checkpoints.Count; i++)
    	{
    		Vector3 checkpointpos = this.checkpoints[i].position;
    		checkPointArray[i] = new Vector3(checkpointpos.x, checkpointpos.y, 0f);
    	}

    	this.lineRenderer.SetPositions(checkPointArray);

    }

}
