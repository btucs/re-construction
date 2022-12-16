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
using UnityEngine.Events;
using TMPro;

public enum PointerEventType
{
  Up,
  Down,
  Click,
  Drag,
  BeginDrag,
  EndDrag,
}

[Serializable]
public class UnityPointerEvent : UnityEvent<PointerEventData> {}

public class TMP_LinkHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler {

  public UnityPointerEvent ForwardedOnBeginDrag = new UnityPointerEvent();
  public UnityPointerEvent ForwardedOnEndDrag = new UnityPointerEvent();
  public UnityPointerEvent ForwardedOnDrag = new UnityPointerEvent();

  private TextMeshProUGUI tmp;
  private Canvas canvas;
  private Camera canvasCamera;

  private Dictionary<string, Action<PointerEventData, string, PointerEventType>> prefixes = new Dictionary<string, Action<PointerEventData, string, PointerEventType>>();

  private string currentLinkId;
  private Action<PointerEventData, string, PointerEventType> currentAction;
  // Dragging has to be handled since the endDrag event fires after pointer up
  private bool isDragging = false;

  private void Awake() {

    tmp = GetComponent<TextMeshProUGUI>();
    canvas = GetComponentInParent<Canvas>();

    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {

      canvasCamera = null;
    } else {

      canvasCamera = canvas.worldCamera;
    }
  }

  public void RegisterPrefix(string prefix, Action<PointerEventData, string, PointerEventType> action) {

    prefixes.Add(prefix, action);
  }

  public void RemovePrefix(string prefix, PointerEventType pointerEventType) {

    prefixes.Remove(prefix);
  }

  public void OnPointerDown(PointerEventData data) {
    int linkIndex = TMP_TextUtilities.FindIntersectingLink(
      tmp, Input.mousePosition, canvasCamera
    );
    
    if(linkIndex != -1) {

      TMP_LinkInfo linkInfo = tmp.textInfo.linkInfo[linkIndex];
      currentLinkId = linkInfo.GetLinkID();
      string prefix = GetPrefixString(currentLinkId);

      prefixes.TryGetValue(prefix, out currentAction);

      currentAction?.Invoke(data, currentLinkId, PointerEventType.Down);
    } else {

      currentLinkId = null;
      currentAction = null;
    }
  }

  public void OnBeginDrag(PointerEventData data) {

    ForwardedOnBeginDrag?.Invoke(data);

    isDragging = true;
    if(currentLinkId != null && currentAction != null) {

      currentAction?.Invoke(data, currentLinkId, PointerEventType.BeginDrag);
    }
  }

  public void OnDrag(PointerEventData data) {

    ForwardedOnDrag?.Invoke(data);

    if(currentLinkId != null && currentAction != null) {

      currentAction?.Invoke(data, currentLinkId, PointerEventType.Drag);
    }
  }

  /// <summary>
  /// either OnPointerClick or OnEndDrag fire last an have to cleanup
  /// </summary>
  /// <param name="data"></param>
  public void OnEndDrag(PointerEventData data) {

    ForwardedOnEndDrag?.Invoke(data);

    if(currentLinkId != null && currentAction != null) {

      currentAction?.Invoke(data, currentLinkId, PointerEventType.EndDrag);
    }

    currentLinkId = null;
    currentAction = null;
    isDragging = false;
  }

  /// <summary>
  /// either OnPointerClick or OnEndDrag fire last an have to cleanup
  /// </summary>
  public void OnPointerClick(PointerEventData data) {

    if(currentLinkId != null && currentAction != null) {

      currentAction?.Invoke(data, currentLinkId, PointerEventType.Click);
    }

    // when dragging and letting go while inside the tmp are endDrag fires last
    if(isDragging == false) {

      currentLinkId = null;
      currentAction = null;
    }
  }

  public void OnPointerUp(PointerEventData data) {

    if(currentLinkId != null && currentAction != null) {

      currentAction?.Invoke(data, currentLinkId, PointerEventType.Up);
    }
  }

  private string GetPrefixString(string fullString) {

    return fullString.Split(':')[0];
  }

  private void FindAndInvokeAction(PointerEventData data, PointerEventType eventType) {

    if(currentLinkId != null) {

      string prefix = GetPrefixString(currentLinkId);
      prefixes.TryGetValue(prefix, out Action<PointerEventData, string, PointerEventType> action);

      action?.Invoke(data, currentLinkId, PointerEventType.BeginDrag);
    }
  }
}
