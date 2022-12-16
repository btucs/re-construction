#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorldObject : MonoBehaviour {

  public TaskObjectSO objectData;
  public GameObject areaBackground;
  private WorldObjectResolver stateResolver;
	private MenuUIController connectedUI;
  private bool isExamined = false;
  private GameController gameController;

	public void OpenObjectMenu() {

    WorldSceneCameraController camController = connectedUI.cameraController;
    camController.ForceMoveTowards(transform.position + objectData.cameraOffset);
    camController.ZoomByFactor(objectData.cameraZoom);

    connectedUI.ToggleMenuPanelVisibility(false);
    connectedUI.ActivateObjectSelectionUI(this);
	}

  public void Setup(TaskHistoryData taskHistory, WorldObjectData saveData)
  {
    SetReferences();
    stateResolver = this.gameObject.GetComponent<WorldObjectResolver>() ?? this.gameObject.AddComponent<WorldObjectResolver>();
    stateResolver.ResolveObjectStateFromTaskHistory(taskHistory);

    CreateObjectMarker();

    if(saveData != null)
    {
      isExamined = saveData.isExamined;
      if(isExamined)
      {
        CreateTaskMarkers();
      }
    }
  }

  public bool GetExaminedState()
  {
    return isExamined;
  }

  private void CreateObjectMarker()
  {
    if(connectedUI)
      connectedUI.worldSpaceUI.CreateObjectInteractionMarker(this);
  }

  private void CreateTaskMarkers(bool unlockAnim = false)
  {
    if(isExamined && connectedUI != null)
      connectedUI.objectUIManager.CreateTaskUIMarkers(this, unlockAnim);
  }

  private void SetReferences()
  {
    if(gameController == null)
    {
      gameController = GameController.GetInstance();
    }

    connectedUI = MenuUIController.Instance;
  }

  public Vector2 GetMarkerPosition()
  {
    Vector2 pos = new Vector2(this.transform.position.x + objectData.markerOffset.x, this.transform.position.y + objectData.markerOffset.y);
    //Debug.Log("Marker position for " + objectData.objectName + " is " + pos.x + "; " + pos.y);
    return pos;
  }

  public void Examine()
  {
    if(!isExamined)
    {
      isExamined = true;
      CreateTaskMarkers(true);
      if(RoomState.Instance != null)
        RoomState.Instance.SaveSceneState();   
    }
  }

}