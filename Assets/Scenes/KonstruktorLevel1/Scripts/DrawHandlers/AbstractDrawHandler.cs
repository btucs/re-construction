#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using MathUnits.Physics.Values;
using MathUnits.Physics.Units;

public abstract class AbstractDrawHandler: MonoBehaviour, IDrawHandler {

  [Required, FoldoutGroup("General Configuration", expanded: false)]
  public RectTransform leftSidebarContent;
  [Required, FoldoutGroup("General Configuration")]
  public RectTransform rightSidebarContent;
  [Required, FoldoutGroup("General Configuration")]
  public RectTransform drawnItemsContainer;

  [Required, FoldoutGroup("General Configuration")]
  public RectTransform xMarker;
  [Required, FoldoutGroup("General Configuration")]
  public RectTransform yMarker;

  [Required, FoldoutGroup("General Configuration")]
  public InputOutputDrawController inputOutputPlaceholderTemplate;

  [Required, FoldoutGroup("General Configuration")]
  public DrawLineUI drawLine;
  [Required, FoldoutGroup("General Configuration")]
  public Toggle drawButton;
  [Required, FoldoutGroup("General Configuration")]
  public Button confirmPointPositionButton;

  [Required, FoldoutGroup("General Configuration")]
  public TaskDataSO.CoordinateSystemData coordinateSystemData;

  [Required, FoldoutGroup("General Configuration")]
  public InputMenuController inputMenuController;

  [FoldoutGroup("General Configuration")]
  public TaskDataSO currentTask;
  [FoldoutGroup("General Configuration")]
  public TaskObjectSO currentTaskObject;

  public abstract DrawVariableData[] GetInputs();
  [Obsolete()]
  public abstract DrawResultData[] GetOutputs();

  public InputOutputDrawControllerEvent onTap = new InputOutputDrawControllerEvent();
  public DrawLineEvent onEndDraw = new DrawLineEvent();

  protected List<InputOutputDrawController> inputs = new List<InputOutputDrawController>();
  
  protected float stepMultiplier = 1;

  public abstract void HandleInputItemDrop(InputOutputDrawController input);
  [Obsolete()]
  public abstract void HandleOutputItemDrop(InputOutputDrawController output);
  public abstract void TryActivateDrawButton();
  public abstract void MoveActiveItem(DirectionEnum direction, float value);
  public abstract Vector3 GetActiveItemPos();
  public abstract Vector3 GetActiveItemWorldPos();
  public abstract KonstructorDrawPointController[] GetDrawPoints();
  public abstract bool AllInputsDropped();
  public abstract void ClearDrawnLine();
  public abstract void CreateAndSaveResult();

  protected abstract void HandleTaskVariableTap(InputOutputDrawController item);
  protected abstract void HandleDrawButton(bool isActive);
  protected abstract void HandleEndDraw(LineControllerUI lineController);
  protected abstract ConverterResultData CreateResultData(LineControllerUI lineController);

  public virtual void Initialize() {

    GenerateInputs();
    //GenerateOutputs();

    drawButton.onValueChanged.AddListener(HandleDrawButton);
  }

  public virtual void Terminate() {

    inputs.ForEach((InputOutputDrawController item) => {

      item.onTap.RemoveListener(HandleTaskVariableTap);
      item.onItemDropped.RemoveListener(HandleTaskVariableTap);
    });
  }

  public virtual bool AllAdjustmentsMade()
  {
    return false;
  }

  public void SetStepMultiplier(float stepMultiplier) {

    this.stepMultiplier = stepMultiplier;
  }

  private void GenerateInputs() {

    DrawVariableData[] variableNames = GetInputs();
    HelperFunctions.DestroyChildren<InputOutputDrawController>(leftSidebarContent);

    for(int i = 0; i < variableNames.Length; i++) {

      TextMeshProRendererFactory tmpFactory = new TextMeshProRendererFactory();
      DrawVariableData data = variableNames[i];
      InputOutputDrawController obj = Instantiate(inputOutputPlaceholderTemplate, leftSidebarContent);
      obj.Initialize();
      obj.expectedParameterType = data.expectedType;
      obj.gameObject.SetActive(true);
      obj.onTap.AddListener(HandleTaskVariableTap);
      obj.onItemDropped.AddListener(HandleInputItemDrop);
      // when item is dropped it is also selected, so every other selected item has to deselected
      obj.onItemDropped.AddListener(HandleTaskVariableTap);
      obj.GetComponentInChildren<TMP_Text>().text = tmpFactory.RenderMarkdownStringToTextMeshProString(data.name);
      obj.disableTap = data.disableTap;
      obj.varData = data;
      
      inputs.Add(obj);
    }
  }

  [Obsolete]
  private void GenerateOutputs() {

    DrawResultData[] variableNames = GetOutputs();
  }

  protected MathMagnitude CreateMathMagnitude(Vector2 value, Vector2 startPoint, string name) {

    MathMagnitude magnitude = CreateMathMagnitude(value, name);
    TaskInputVariable input = magnitude.Value as TaskInputVariable;
    input.startPointText = VectorValueHelper.ToString(startPoint);

    return magnitude;
  }

  protected MathMagnitude CreateMathMagnitude(Vector2 value, string name) {

    value = RoundVector(value);

    return CreateMathMagnitude(VectorValueHelper.Convert(value), name);
  }

  protected MathMagnitude CreateMathMagnitude(Vector2 value, string name, int decimals) {

    value = value.Round(decimals);

    return CreateMathMagnitude(VectorValueHelper.Convert(value), name);
  }

  protected Vector2 RoundVector(Vector2 vector) {

    int decimals = coordinateSystemData.intermediateSteps < 2 ? 0 : 2;

    return vector.Round(decimals);
  }

  protected MathMagnitude CreateMathMagnitude(VectorValue value, VectorValue startPoint, string name) {

    TaskInputVariable input = new TaskInputVariable();
    input.SetVectorValue(value);
    input.name = name;
    input.startPointText = VectorValueHelper.ToString(startPoint);

    return new MathMagnitude() {
      Value = input,
      taskData = currentTask,
      taskObject = currentTaskObject,
    };
  }

  protected MathMagnitude CreateMathMagnitude(VectorValue value, Vector2 startPoint, string name) {

    TaskInputVariable input = new TaskInputVariable();
    input.SetVectorValue(value);
    input.name = name;
    input.startPointText = VectorValueHelper.ToString(startPoint);

    return new MathMagnitude() {
      Value = input,
      taskData = currentTask,
      taskObject = currentTaskObject,
    };
  }

  protected MathMagnitude CreateMathMagnitude(VectorValue value, string name) {

    TaskInputVariable input = new TaskInputVariable();
    input.SetVectorValue(value);
    input.name = name;

    return new MathMagnitude() {
      Value = input,
      taskData = currentTask,
      taskObject = currentTaskObject,
    };
  }

  protected MathMagnitude CreateMathMagnitude(float value, string name, Unit unit) {

    ScalarValue scalarValue = new ScalarValue(unit, (double)value);

    return CreateMathMagnitude(scalarValue, name);
  }

  protected MathMagnitude CreateMathMagnitude(double value, string name, Unit unit) {

    ScalarValue scalarValue = new ScalarValue(unit, value);

    return CreateMathMagnitude(scalarValue, name);
  }

  protected MathMagnitude CreateMathMagnitude(ScalarValue value, string name) {

    TaskInputVariable input = new TaskInputVariable();
    input.SetScalarValue(value);
    input.name = name;

    return new MathMagnitude() {
      Value = input,
      taskData = currentTask,
      taskObject = currentTaskObject,
    };
  }

  protected void PositionMarkers(Vector3 currentPosition) {

    float xDistance = Mathf.Abs(currentPosition.x);
    float yDistance = Mathf.Abs(currentPosition.y);

    float xCenter = currentPosition.x / 2;
    float yCenter = currentPosition.y / 2;

    xMarker.localPosition = new Vector3(xCenter, currentPosition.y, 0);
    xMarker.sizeDelta = new Vector2(xDistance, xMarker.sizeDelta.y);

    yMarker.localPosition = new Vector3(currentPosition.x, yCenter, 0);
    yMarker.sizeDelta = new Vector2(yDistance, yMarker.sizeDelta.y);
  }
}