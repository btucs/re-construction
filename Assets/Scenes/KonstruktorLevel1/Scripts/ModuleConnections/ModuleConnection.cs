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
using UniRx;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ModuleConnection : MonoBehaviour {

  public ModuleConnectionsOut from;
  public ModuleConnectionsIn to;
  
  [Range(.05f, 1.5f)]
  public float spacing = 1;
  public float width = 1;
  public BoolReactiveProperty autoUpdate = new BoolReactiveProperty(false);
  public float tiling = 1;

  // for debugging
  private ModuleConnectorPath temp;

  private Vector2[] connectionPoints;

  private struct FromTo {

    public ModuleConnectionsOut from;
    public ModuleConnectionsIn to;
  }

  private void Start() {

    UpdateConnection(from, to);
    from.AddOutConnection(this);
    to.AddInConnection(this);

    Func<Collider, Vector3> ObserveCenter = (Collider collider) => collider.bounds.center;
    IObservable<Vector3> fromObservable = from.GetOutCollider().ObserveEveryValueChanged(ObserveCenter);
    IObservable<Vector3> toObservable = to.GetInCollider().ObserveEveryValueChanged(ObserveCenter);

    Observable.CombineLatest(fromObservable, toObservable)
      .Where((IList<Vector3> fromTo) => fromTo[0] != null && fromTo[1] != null)
      .Do(_ => UpdateConnection(from, to))
      .Subscribe()
      .AddTo(this)
    ;

    autoUpdate
      .Where((bool value) => value == true && from != null && to != null)
      .SelectMany(
        (_) => Observable.EveryUpdate().Do((__) => UpdateConnection())
      )
      .Subscribe()
      .AddTo(this)
    ;
  }

  public void UpdateConnection() {

    UpdateConnection(from, to);
  }

  public void UpdateConnection(ModuleConnectionsOut from, ModuleConnectionsIn to) {

    Vector2[] anchorPoints = GetAnchorPoints(new FromTo() { from = from, to = to});
    ModuleConnectorPath path = new ModuleConnectorPath(anchorPoints[0], anchorPoints[1]);
    temp = path;
    connectionPoints = path.CalculateEvenlySpacedPoints(spacing);
    
    GetComponent<MeshFilter>().mesh = CreateMesh(connectionPoints);

    int textureRepeat = Mathf.RoundToInt(tiling * connectionPoints.Length * spacing * 0.4f);
    GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, textureRepeat);
  }

  public Vector2[] GetConnectionPoints() {

    return connectionPoints;
  }

  private void OnDrawGizmos() {

    if(temp != null && temp.points.Count > 0) {

      Gizmos.DrawLine(temp.points[0], temp.points[1]);
      Gizmos.DrawLine(temp.points[2], temp.points[3]);
      Gizmos.DrawLine(temp.points[0], temp.points[3]);
    }
  }

  private Mesh CreateMesh(Vector2[] points) {

    Vector3[] verts = new Vector3[points.Length * 2];
    Vector2[] uvs = new Vector2[verts.Length];
    int numTris = 2 * (points.Length - 1) + 2;
    int[] tris = new int[numTris * 3];
    int vertIndex = 0;
    int triIndex = 0;

    for(int i = 0; i < points.Length; i++) {

      Vector2 forward = Vector2.zero;
      if(i < points.Length - 1) {

        forward += points[(i + 1) % points.Length] - points[i];
      }

      if(i > 0) {

        forward += points[i] - points[(i - 1 + points.Length) % points.Length];
      }

      forward.Normalize();
      Vector2 left = new Vector2(-forward.y, forward.x);

      verts[vertIndex] = points[i] + left * width * .5f;
      verts[vertIndex + 1] = points[i] - left * width * .5f;

      float completionPercent = i / (float)(points.Length - 1);
      float v = 1 - Mathf.Abs(2 * completionPercent - 1);
      uvs[vertIndex] = new Vector2(0, v);
      uvs[vertIndex + 1] = new Vector2(1, v);

      if(i < points.Length - 1) {
        tris[triIndex] = vertIndex;
        tris[triIndex + 1] = (vertIndex + 2) % verts.Length;
        tris[triIndex + 2] = vertIndex + 1;

        tris[triIndex + 3] = vertIndex + 1;
        tris[triIndex + 4] = (vertIndex + 2) % verts.Length;
        tris[triIndex + 5] = (vertIndex + 3) % verts.Length;
      }

      vertIndex += 2;
      triIndex += 6;
    }

    Mesh mesh = new Mesh();
    mesh.vertices = verts;
    mesh.triangles = tris;
    mesh.uv = uvs;

    return mesh;
  }

  private Vector2[] GetAnchorPoints(FromTo modules) {

    if(modules.from == null || modules.to == null) {

      throw new Exception("From and To Modules have to be set");
    }

    Vector2 fromPoint = GetAnchorPoint(modules.from);
    Vector2 toPoint = GetAnchorPoint(modules.to);

    return new Vector2[2] {
      fromPoint,
      toPoint
    };
  }

  private Vector2 GetAnchorPoint(ModuleConnectionsIn module) {

    Collider collider = module.GetInCollider();

    return GetAnchorPoint(collider, true);
  }

  private Vector2 GetAnchorPoint(ModuleConnectionsOut module) {

    Collider collider = module.GetOutCollider();

    return GetAnchorPoint(collider, false);
  }

  private Vector2 GetAnchorPoint(Collider collider, bool isLeft) {

    Vector3 anchor = collider.bounds.center;
    float halfX = collider.bounds.size.x / 2;
    anchor.x = (isLeft ? anchor.x - halfX + 0.1f : anchor.x + halfX - 0.1f);

    return (Vector2) anchor;
  }

  private void OnDestroy() {

    if(from != null) {

      from.RemoveOutConnection(this);
    }

    if(to != null) {

      to.RemoveInConnection(this);
    }
  }
}
