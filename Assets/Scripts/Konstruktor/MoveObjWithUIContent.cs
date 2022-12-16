#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;
using UnityEngine;

public class MoveObjWithUIContent : MonoBehaviour
{
  public Transform objectToMove;
  public RectTransform movingUIElement;
  public bool isActive = false;
  public KonstructorSetup konstruktorManager;
  
  private List<characterMovement> npcMoveControllers = new List<characterMovement>();
  
  private Vector3 offset;

  public void SetActive() {
    isActive = true;
    npcMoveControllers = konstruktorManager.GetNPCReferences();
    foreach(characterMovement moveController in npcMoveControllers) {
      moveController.enabled = false;
    }

    Vector3 objectStart = objectToMove.position;
    Vector3 movingElementPosition = movingUIElement.transform.position;
    offset = objectStart - movingElementPosition;
  }

  public void SetInactive() {

    foreach(characterMovement moveController in npcMoveControllers) {
      moveController.enabled = true;
    }

    isActive = false;
  }

  private void Update() {
    if(isActive) {

      objectToMove.position = movingUIElement.transform.position + offset;
    }
  }
}
