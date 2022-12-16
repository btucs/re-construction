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
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public abstract class KonstructorDrawEntityAbstract : MonoBehaviour {

  protected Vector2 multiplier;

  private Canvas parentCanvas;

  public void SetMultiplier(Vector2 multiplier) {

    this.multiplier = multiplier;
  }

  public abstract void SetEditable(bool isEditable);

  public abstract void RenderPosition();

  protected virtual void Start() {

    parentCanvas = GetComponentInParent<Canvas>();
  }

  protected Vector2 ConvertScreenPointToLocalPoint(Vector2 pos) {

    RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, pos, parentCanvas.worldCamera, out Vector2 outPos);

    return outPos;
  }

  protected Vector3 CalculatePosition(Vector3 currentPos, DirectionEnum direction, float xyPos) {

    switch(direction) {

      case DirectionEnum.X:
        return new Vector3(xyPos, currentPos.y, 0);
      case DirectionEnum.Y:
        return new Vector3(currentPos.x, xyPos, 0);
    }

    return Vector3.zero;
  }
}
