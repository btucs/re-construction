#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawCircle : MonoBehaviour {

  public float lineWidth = 0.1f;
  public float animationSpeed = 1f;
  public int textureTiling = 60;

  private float radius = 3;

  private LineRenderer lineRenderer;
  private BoxCollider parentCollider;

  private void Awake() {

    lineRenderer = GetComponent<LineRenderer>();
    lineRenderer.useWorldSpace = false;
    lineRenderer.startWidth = lineWidth;
    lineRenderer.endWidth = lineWidth;

    parentCollider = GetComponentInParent<BoxCollider>();
    if(parentCollider == null) {

      throw new System.Exception("BoxCollider not found in parent");
    }

    Vector3 leftTopCorner = parentCollider.center + (Vector3.left * parentCollider.size.x / 2) + (Vector3.up * parentCollider.size.y / 2);
    radius = Vector3.Distance(parentCollider.center, leftTopCorner) + 0.5f;

    RenderCircle();
  }

  public void RenderCircle() {

    int segments = 360;
    
    // extra point to close the circle
    lineRenderer.positionCount = segments + 1;
    Vector3[] points = new Vector3[segments + 1];

    for(int i = 0; i < points.Length; i++) {

      float rad = Mathf.Deg2Rad * (i * 360 / segments);
      points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
    }

    lineRenderer.SetPositions(points);
  }

  private void Update() {

    lineRenderer.startWidth = lineWidth;
    lineRenderer.endWidth = lineWidth;
    lineRenderer.material.mainTextureOffset = new Vector2(Time.time * animationSpeed, 0);
    lineRenderer.material.mainTextureScale = new Vector2(textureTiling, 1);
  }
}
