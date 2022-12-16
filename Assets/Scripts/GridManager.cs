#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;

public class GridManager: MonoBehaviour {

  public static GridManager instance;

  public Camera mainCamera;
  public GameObject grid;

  private void Awake() {

    if (instance == null) {

      instance = this;
    }
  }

  public Vector3 GetCameraPosition(Vector3 position, GameObject target) {

    return GetCameraPosition(position, target, true);
  }

  public Vector3 GetCameraPosition(
    Vector3 position,
    GameObject target,
    bool placeOnGrid
  ) {
  
    Ray ray = mainCamera.ScreenPointToRay(position);
    float cameraDistance = Vector3.Distance(
      mainCamera.transform.position,
      target.transform.position
    );

    Physics.Raycast(ray, out RaycastHit hit, 100000);

    if (placeOnGrid == true) {

      return PlacePointOnGrid(hit.point);
    }

    return hit.point;
  }

  private Vector3 PlacePointOnGrid(Vector3 point) {

    GenerateGridExtended gridScript = grid.GetComponent<GenerateGridExtended>();
    float resolution = gridScript.resol;
    float divResolution = gridScript.divResol;
    float spacing = resolution / divResolution;

    return new Vector3(
      CalculateClosest(point.x, spacing),
      CalculateClosest(point.y, spacing),
      point.z
    );
  }

  private float CalculateClosest(float value, float spacing) {

    return Mathf.Round(value / spacing) * spacing;
  }
}
