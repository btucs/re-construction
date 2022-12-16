#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using TMPro;
using Sirenix.OdinInspector;
using Constants;
using System;

public class InventoryItem : MonoBehaviour, IEquatable<InventoryItem>, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
  public MathMagnitude magnitude;

  [ChildGameObjectsOnly]
  [Required]
  public Image objectImage;
  [ChildGameObjectsOnly]
  [Required]
  public TextMeshProUGUI variableName;
  [Required]
  public CanvasGroup canvasGroup;

  public InventoryItemEvent OnMoveEvent = new InventoryItemEvent();
  public InventoryItemEvent OnReleaseEvent = new InventoryItemEvent();
  public InventoryItemEvent OnTapEvent = new InventoryItemEvent();
  public InventoryItemEvent OnDestroyEvent = new InventoryItemEvent();

  public bool enableDrag = true;

  // if magnitude Value is TaskInputVariable
  [ShowIf("@this.magnitude.Value is TaskInputVariable"), ReadOnly]
  public bool isResult = false;
  // if magnitude value is TaskOutputVariable
  [ShowIf("@this.magnitude.Value is TaskOutputVariable"), ReadOnly]
  public bool hasResult = false;

  public Transform ParentTransform { get; private set; }

  private Vector3 dragStartPos;
  private GameObject target;

  private Canvas parentCanvas;

  private Subject<object> cancelDestroyTimeoutSubject = new Subject<object>();
  private GameObject currentlyDroppedAt;

  public void ResetPosition() {

    if(target != null) {

      target.transform.SetParent(ParentTransform);
      target.transform.localPosition = Vector3.zero;
      target.transform.localScale = Vector3.one;
    }
  }

  public void HasBeenDropped(DropAreaUI target) {

    currentlyDroppedAt = target.gameObject;
    cancelDestroyTimeoutSubject.OnNext(target);
  }

  public void HasBeenDropped(KonstruktorVarDropHandler target) {

    currentlyDroppedAt = target.gameObject;
    cancelDestroyTimeoutSubject.OnNext(target);
  }

  private void Start() {

    OnReleaseEvent.AsObservable()
      .SelectMany((InventoryItem item) => Observable.Timer(TimeSpan.FromMilliseconds(100))
        .TakeUntil(cancelDestroyTimeoutSubject)
        .Do((_) => {

          if(currentlyDroppedAt != null) {

            ResetPosition();
          } else {

            Destroy(item.gameObject);
          }            
        })
        .Select((_) => item)
      )
      .TakeUntilDestroy(this)
      .Subscribe()
    ;
  }

  private void Awake() {

    parentCanvas = GetComponentInParent<Canvas>();
  }

  private void MoveTo(Vector2 position) {

    RectTransformUtility.ScreenPointToLocalPointInRectangle(
      parentCanvas.transform as RectTransform,
      position,
      parentCanvas.worldCamera,
      out Vector2 worldPos
    );

    target.transform.position = parentCanvas.transform.TransformPoint(worldPos);
  }

  private void OnSingleFingerTap(Vector2 position) {
    
    OnTapEvent?.Invoke(this);
  }

  public Sprite GetSprite() {

    return magnitude.taskObject.objectThumbnail;
  }

  public string GetText() {

    return variableName.text;
  }

  public bool Equals(InventoryItem other) {

    bool equals = other != null &&
      magnitude.Value.name == other.magnitude.Value.name
    ;

    return equals;
  }

  public override bool Equals(object other) {

    if(other == null) {

      return false;
    }

    InventoryItem otherInventoryItem = other as InventoryItem;
    if(otherInventoryItem == null) {

      return false;
    }

    return Equals(otherInventoryItem);
  }

  public override int GetHashCode() {

    return variableName.text.GetHashCode() | objectImage.sprite.GetHashCode();
  }

  public void OnPointerDown(PointerEventData eventData) {
    
  }

  public void OnPointerUp(PointerEventData eventData) {

    canvasGroup.alpha = 1f;
    canvasGroup.blocksRaycasts = true;

    if(enableDrag == true) {

      OnReleaseEvent?.Invoke(this);
    }
  }

  public void OnBeginDrag(PointerEventData eventData) {

    if(enableDrag == true) {

      dragStartPos = transform.position;
      canvasGroup.alpha = 0.60f;
      canvasGroup.blocksRaycasts = false;

      target = gameObject;
      ParentTransform = transform.parent;
      transform.SetParent(parentCanvas.transform);
    }
  }

  public void OnDrag(PointerEventData eventData) {

    if(enableDrag == true) {

      MoveTo(eventData.position);
      OnMoveEvent?.Invoke(this);
    }
  }

  public void OnEndDrag(PointerEventData eventData) {

    if(enableDrag == true) {

      canvasGroup.alpha = 1f;
      canvasGroup.blocksRaycasts = true;
    }
  }

  public void OnPointerClick(PointerEventData eventData) {
    cancelDestroyTimeoutSubject.OnNext(eventData);
    OnTapEvent?.Invoke(this);
  }
}
