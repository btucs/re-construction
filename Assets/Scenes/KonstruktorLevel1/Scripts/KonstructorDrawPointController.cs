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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UniRx.Triggers;

public class PositionEvent : UnityEvent<Vector2>
{
}

[RequireComponent(typeof(SingleFingerInputTrigger), typeof(ObservablePointerEnterTrigger), typeof(ObservablePointerExitTrigger))]
public class KonstructorDrawPointController : KonstructorDrawEntityAbstract {

  [Required]
  public TMP_Text text;
  [Required]
  public Image image;
  [Required]
  public Color defaultColor = Color.black;
  [Required]
  public Color editableColor = Color.black;

  public GameObject interactionHighlight;

  public Subject<Vector2> OnDragStart = new Subject<Vector2>();
  public Subject<Vector2> OnDrag = new Subject<Vector2>();
  public Subject<Vector2> OnDragEnd = new Subject<Vector2>();
  public Subject<Vector2> OnFingerDown = new Subject<Vector2>();
  public Subject<Vector2> OnFingerUp = new Subject<Vector2>();
  public Subject<Vector2> OnFingerEnter = new Subject<Vector2>();
  public Subject<Vector2> OnFingerExit = new Subject<Vector2>();

  public PositionEvent onMove = new PositionEvent();

  private SingleFingerInputTrigger trigger;
  private ObservablePointerEnterTrigger enterTrigger;
  private ObservablePointerExitTrigger exitTrigger;

  private string variableName;

  public Vector3 Position {
    get {
      return transform.localPosition;
    }
  }

  public void ShowHighlight(bool toShow)
  {
    if(interactionHighlight)
      interactionHighlight.SetActive(toShow);
  }

  public void SetPointName(string name) {

    variableName = name;
  }

  public override void RenderPosition() {

    float xPos = transform.localPosition.x * multiplier.x;
    float yPos = transform.localPosition.y * multiplier.y;

    text.text = SymbolHelper.GetSymbol(variableName) + " (" + Math.Round(xPos, 2) + " | " + Math.Round(yPos, 2) + " )";
  }

  public override void SetEditable(bool isEditable) {

    image.color = isEditable == false ? defaultColor : editableColor;   
  }

  public void SetPosition(DirectionEnum direction, float xyPos) {

    transform.localPosition = CalculatePosition(transform.localPosition, direction, xyPos);
    onMove?.Invoke(transform.localPosition);
  }

  public void SetPosition(Vector3 pos) {

    transform.localPosition = pos;
    onMove?.Invoke(transform.localPosition);
  }

  protected override void Start() {

    base.Start();
    text.color = defaultColor;
    InitializeObservables();
  }

  private void InitializeObservables() {

    trigger = GetComponent<SingleFingerInputTrigger>();
    trigger
      .OnSingleFingerBeginDragAsObservable()
      // map position to center of gameobject
      .Subscribe((Vector2 pos) => OnDragStart.OnNext((Vector2)transform.localPosition))
      .AddTo(this)
    ;

    trigger
      .OnSingleFingerDragAsObservable()
      .Subscribe((Vector2 pos) => OnDrag.OnNext(ConvertScreenPointToLocalPoint(pos)))
      .AddTo(this)
    ;

    trigger
      .OnSingleFingerEndDragAsObservable()
      .Subscribe((Vector2 pos) => OnDragEnd.OnNext(ConvertScreenPointToLocalPoint(pos)))
      .AddTo(this)
    ;

    trigger
      .OnSingleFingerDownAsObservable()
      // map position to center of gameobject
      .Subscribe((Vector2 pos) => OnFingerDown.OnNext((Vector2)transform.localPosition))
      .AddTo(this)
    ;

    trigger
      .OnSingleFingerUpAsObservable()
      .Subscribe((Vector2 pos) => OnFingerUp.OnNext(ConvertScreenPointToLocalPoint(pos)))
      .AddTo(this)
    ;

    enterTrigger = GetComponent<ObservablePointerEnterTrigger>();
    enterTrigger
      .OnPointerEnterAsObservable()
      // map position to center of gameobject
      .Subscribe((PointerEventData data) => OnFingerEnter.OnNext((Vector2)transform.localPosition))
      .AddTo(this)
    ;

    exitTrigger = GetComponent<ObservablePointerExitTrigger>();
    exitTrigger
      .OnPointerExitAsObservable()
      .Subscribe((PointerEventData data) => OnFingerExit.OnNext(ConvertScreenPointToLocalPoint(data.position)))
      .AddTo(this)
    ;
  }
}
