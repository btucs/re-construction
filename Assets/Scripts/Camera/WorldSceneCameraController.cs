#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSceneCameraController : MonoBehaviour
{
  public float cameraSpeed = 4f;
  public float zoomSpeed = 2f;
  public Vector3 targetPosition;
  public bool isMovable = true;
  public bool ignoreBounds = false;
  private float sceneScale = 1f;
  private float targetScale = 1f;
  private float sceneDefaultCamSpeed;

  private Camera sceneCamera;
  #pragma warning disable 0649
  [SerializeField] CameraBounds2D bounds;
  #pragma warning restore 0649
  Vector2 maxXPositions,maxYPositions;

  void Awake()
  {
    bounds.Initialize(Camera.main);
    maxXPositions = bounds.maxXlimit;
    maxYPositions = bounds.maxYlimit;
  }

  // Start is called before the first frame update
  void Start()
  {
    sceneCamera = Camera.main;
    sceneScale = sceneCamera.orthographicSize;
    targetScale = sceneScale;
    targetPosition = transform.position;
    sceneDefaultCamSpeed = cameraSpeed;
  }

  public void SetGoalPosition(Vector3 newPosVector)
  {
    if(isMovable)
    {
      targetPosition = new Vector3 (newPosVector.x, newPosVector.y, transform.position.z);
    }
  }

  public void ForceGoalPositionUpdate(Vector3 newPosVector)
  {
    targetPosition = new Vector3 (newPosVector.x, newPosVector.y, transform.position.z);
  }

  public void ZoomByFactor(float zoomFactor)
  {
     targetScale = sceneScale/zoomFactor;
  }

  public void ZoomTo(float newCameraSize)
  {
     targetScale = newCameraSize;
  }

  public void SetToWorldMode()
  {
    cameraSpeed = sceneDefaultCamSpeed;
    ignoreBounds = false;
    ResetZoom();
    isMovable = true;
  }

  public void ForceMoveTowards(Vector3 goalPos, float moveSpeed = 1.5f)
  {
    ignoreBounds = true;
    cameraSpeed = moveSpeed;
    ForceGoalPositionUpdate(goalPos);
    isMovable = false;
  }

  public void ResetZoom()
  {
     targetScale = sceneScale;
  }

  public void UpdateCameraBoundsX(Vector2 newXBounds)
  {
    maxXPositions = newXBounds;
  }

  void LateUpdate()
  {
    if(!ignoreBounds)
      {
        targetPosition = new Vector3(Mathf.Clamp(targetPosition.x, maxXPositions.x, maxXPositions.y), Mathf.Clamp(targetPosition.y, maxYPositions.x, maxYPositions.y), targetPosition.z);
      }

    if(transform.position != targetPosition)
    {
      sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position, targetPosition, Time.deltaTime * cameraSpeed);
    }

    if(sceneCamera.orthographicSize != targetScale)
    {
       sceneCamera.orthographicSize = Mathf.Lerp(sceneCamera.orthographicSize, targetScale, Time.deltaTime*zoomSpeed);
    }
  }
}
