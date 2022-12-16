#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Constants;
using UniRx.Triggers;

public class Line : MonoBehaviour {

  public GameObject linePrefab;
  public GameObject handlePrefab;
  public HandleTypeSO defaultType;
  public CommandInvoker invoker;
  
  public float lineWidth = 0;

  public GameObject AddPosition(Vector3 pos, int sortingOrder) {

    return AddPosition(pos, sortingOrder, defaultType, false);
  }

  public GameObject AddPosition(Vector3 pos, int sortingOrder, HandleTypeSO type, bool select) {

    pos.z = 0;
    if (transform.childCount == 0) {

      GameObject lineSprite = Instantiate<GameObject>(linePrefab, pos, Quaternion.identity, transform);
      lineSprite.transform.localScale = new Vector3(0, lineWidth, 1);
    }

    GameObject handle = Instantiate<GameObject>(handlePrefab, pos, Quaternion.identity, transform);

    handle.GetComponent<Handle>().currentType.Value = type;
    SpriteRenderer spriteRenderer = handle.GetComponentInChildren<SpriteRenderer>();
    spriteRenderer.sortingOrder = sortingOrder;
    spriteRenderer.color = type.color;

    GameManager manager = GameManager.instance;
    manager.AddToHandlePool(handle);

    MoveHandle script = handle.GetComponent<MoveHandle>();
    if (script != null) {

      script.invoker = invoker;
    }

    if (select == true) {

      manager.selectedObjectSource.OnNext(handle);
    }

    if (transform.childCount >= 3) {

      Transform linePos = transform.GetChild(0);
      Transform handle1Pos = transform.GetChild(1);
      // take the last child, there may be a dummy handle still existing before it is destroyed
      Transform handle2Pos = transform.GetChild(transform.childCount - 1);

      TransformExtensions.LookAtOrAway(handle1Pos, handle2Pos);
      TransformExtensions.LookAtOrAway(handle2Pos, handle1Pos);

      PlaceLineBetweenHandles(linePos, handle1Pos, handle2Pos);
    }

    return handle;
  }

  private Vector3 GetLocalPos(Vector3 point) {

    Vector3 localPos = transform.InverseTransformPoint(point);
    localPos.z = 0;

    return localPos;
  }

  private void PlaceLineBetweenHandles(Transform linePos, Transform handle1Pos, Transform handle2Pos) {

    Vector3 center = new Vector3(
        handle1Pos.position.x + handle2Pos.position.x,
        handle1Pos.position.y + handle2Pos.position.y
      ) / 2
    ;

    linePos.position = center;

    float scaleX = Vector3.Distance(handle2Pos.position, handle1Pos.position);
    linePos.localScale = new Vector3(scaleX, lineWidth, 1);

    Vector3 direction = Vector3.Normalize(handle2Pos.position - handle1Pos.position);
    linePos.rotation = Quaternion.FromToRotation(Vector3.right, direction);

    // make sure the collider is large enough to be pressed
    if (lineWidth < 0.2f) {

      BoxCollider collider = linePos.GetComponentInChildren<BoxCollider>();
      collider.size = new Vector3(collider.size.x, 0.2f / lineWidth, collider.size.z);
    }
  }

  public void UpdateLinePosAndRotation() {

    if (transform.childCount >= 3) {

      Transform linePos = transform.GetChild(0);
      Transform handle1Pos = transform.GetChild(1);
      Transform handle2Pos = transform.GetChild(2);

      PlaceLineBetweenHandles(linePos, handle1Pos, handle2Pos);
    }
  }
}