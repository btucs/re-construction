#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parralax : MonoBehaviour
{
	public ParallaxElement[] parallaxLayers;
	private Transform cameraTransform;
	private Vector3 previousCamPos;
	public float smoothing = 10f;
	private float parallaxX;
	private float parallaxY;
	private float backgroundTargetPosX;
	private float backgroundTargetPosY;

	void Awake()
	{
		cameraTransform = Camera.main.transform;
	}

	void Start()
	{
		previousCamPos = cameraTransform.position;
	}

	void Update()
	{
		Vector3 backgroundTargetPos;
		foreach (ParallaxElement parallaxLayer in parallaxLayers)
		{
			parallaxX = (previousCamPos.x - cameraTransform.position.x) * parallaxLayer.parallaxXScale;
			parallaxY = (previousCamPos.y - cameraTransform.position.y) * parallaxLayer.parallaxYScale;
			backgroundTargetPosX = parallaxLayer.parallaxObject.position.x - parallaxX;
			backgroundTargetPosY = parallaxLayer.parallaxObject.position.y - parallaxY;
			backgroundTargetPos = new Vector3 (backgroundTargetPosX, backgroundTargetPosY, parallaxLayer.parallaxObject.position.z);
			
			parallaxLayer.parallaxObject.position = Vector3.Lerp(parallaxLayer.parallaxObject.position, backgroundTargetPos, smoothing * Time.deltaTime);
		}

		previousCamPos = cameraTransform.position;
	}

	[Serializable]
    public class ParallaxElement
    {
    	public Transform parallaxObject;
    	public float parallaxXScale = 1f;
    	public float parallaxYScale = 1f;
    }
}
