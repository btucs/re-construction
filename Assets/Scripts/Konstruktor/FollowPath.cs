#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FollowPath : MonoBehaviour {

  public float lineWidth = 0.1f;
  public float animationSpeed = 1f;
  public int textureTiling = 30; 

  private LineRenderer lineRenderer;
  private ModuleConnection connection;

  private void Awake() {

    lineRenderer = GetComponent<LineRenderer>();
    lineRenderer.useWorldSpace = false;
    lineRenderer.startWidth = lineWidth;
    lineRenderer.endWidth = lineWidth;

    connection = GetComponentInParent<ModuleConnection>();
    if(connection == null) {

      throw new System.Exception("ModuleConnection not found in parent");
    }

    
    RenderLine();
  }

  public void RenderLine() {

    Vector2[] points = connection.GetConnectionPoints();
    lineRenderer.positionCount = points.Length;
    lineRenderer.SetPositions(points.Select((Vector2 point) => (Vector3) point).ToArray());
  }

  private void Update() {

    lineRenderer.startWidth = lineWidth;
    lineRenderer.endWidth = lineWidth;
    lineRenderer.material.mainTextureOffset = new Vector2(Time.time * animationSpeed, 0);
    int textureRepeat = Mathf.RoundToInt(textureTiling * lineRenderer.positionCount * 0.5f);
    lineRenderer.material.mainTextureScale = new Vector2(textureRepeat, 1);

    RenderLine();
  }
}

