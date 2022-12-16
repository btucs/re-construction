#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using MathUnits.Physics;
using MathUnits.Physics.Values;
using MathUnits.Physics.Units;
using TMPro;
using UniRx;

public class DrawEquilibriumHandler : AbstractDrawHandler
{
  [Required, FoldoutGroup("Individual Configuration", expanded: false)]
  public KonstructorDrawAngleController angleTemplate;
  [Required, FoldoutGroup("Individual Configuration")]
  public KonstructorDrawVectorController vectorTemplate;

  [Required, FoldoutGroup("Individual Configuration")]
  public TMP_Text scale;
  [Required, FoldoutGroup("Individual Configuration"), SuffixLabel("N")]
  public float scaleAmount = 100;
  [Required, FoldoutGroup("Individual Configuration")]
  public float amountStep = 1;
  [Required, FoldoutGroup("Individual Configuration")]
  public float amountStepSize = 50;
  [Required, FoldoutGroup("Individual Configuration")]
  public float angleStep = 1;
  [Required, FoldoutGroup("Individual Configuration")]
  public Toggle editAngleButton;
  [Required, FoldoutGroup("Individual Configuration")]
  public Toggle editPointButton;
  [Required, FoldoutGroup("Individual Configuration")]
  public SpriteTransparencyController spriteTransparencyController;

  private GameController controller;

  private CalculatorParameterType currentlyActiveDrawnItem;
  private InputOutputDrawController currentlyActiveInput;

  private LineControllerUI currentDrawnLine;
  private KonstructorDrawAngleController angleController;
  private KonstructorDrawVectorController vectorController;

  private IDisposable amountDisposable;
  private IDisposable angleDisposable;

  private bool isPointDropped;
  private bool isAngleDropped;
  private bool isForceDropped;
  private bool isVectorDropped;

  private bool wasAngleEdited = false;

  private float dragShiftDistance = 0;

  public override bool AllAdjustmentsMade()
  {
    return wasAngleEdited;
  }

  public override bool AllInputsDropped() {

    return isPointDropped && isForceDropped && isAngleDropped && isVectorDropped;
  }

  public override void ClearDrawnLine() {

    if(currentDrawnLine != null) {

      Destroy(currentDrawnLine.gameObject);
      currentDrawnLine = null;
    }

    TryActivateDrawButton();
  }

  public override Vector3 GetActiveItemPos() {

    switch(currentlyActiveDrawnItem) {
      case CalculatorParameterType.Point:
        return angleController.Position;

      case CalculatorParameterType.Vector:
        return vectorController.Position;

      case CalculatorParameterType.Angle:
      case CalculatorParameterType.Force:
      default:
        return Vector3.zero;
    }
  }

  public override Vector3 GetActiveItemWorldPos() {

    switch(currentlyActiveDrawnItem) {
      case CalculatorParameterType.Point:
        return angleController.transform.position;
      case CalculatorParameterType.Vector:
        return vectorController.transform.position;

      case CalculatorParameterType.Angle:
      case CalculatorParameterType.Force:
      default:
        return Vector3.zero;
    }
  }

  public override KonstructorDrawPointController[] GetDrawPoints() {
    
    return new KonstructorDrawPointController[2] {
      angleController.attackPoint, angleController.amountPoint
    };
  }

  public override DrawVariableData[] GetInputs() {

    return new DrawVariableData[1] {
      new DrawVariableData() 
      { 
        name = "F_1", 
        expectedType = CalculatorParameterType.Vector, 
        disableTap = true,
        valHeadline = "Kraft", 
        valDescription = "Lege hier die Kraft ab, die mit der zu ermittelnden Kraft im Gleichgewicht steht." 
      },
    };
  }

  [Obsolete]
  public override DrawResultData[] GetOutputs() {

    return new DrawResultData[1] {
      new DrawResultData() { name = "F_g", expectedType = TaskOutputVariableUnit.ForceVectorGG },
    };
  }

  public override void HandleInputItemDrop(InputOutputDrawController input) {

    switch(input.expectedParameterType) {

      /*case CalculatorParameterType.Point: HandlePointDrop(input); break;
      case CalculatorParameterType.Angle: HandleAngleDrop(input); break;
      case CalculatorParameterType.Force: HandleForceDrop(input); break;*/
      case CalculatorParameterType.Vector: HandleVectorDrop(input); break;
    }
  }

  [Obsolete]
  public override void HandleOutputItemDrop(InputOutputDrawController output) {

    throw new NotImplementedException();
  }

  public override void MoveActiveItem(DirectionEnum direction, float value) {

    switch(currentlyActiveDrawnItem) {
      case CalculatorParameterType.Point:
        angleController.SetPosition(direction, value);
        angleController.RenderPosition();
        PositionMarkers(angleController.Position);
        break;

      case CalculatorParameterType.Vector:
        vectorController.SetPosition(direction, value);
        vectorController.RenderPosition();
        PositionMarkers(vectorController.Position);
        break;
    }
  }

  public override void TryActivateDrawButton() {

    if(isVectorDropped == true) {

      drawButton.interactable = true;
    }
  }

  public override void CreateAndSaveResult() {

    if(currentDrawnLine != null) {

      ConverterResultData result = CreateResultData(currentDrawnLine);

      controller.gameState.konstruktorSceneData.AddResult(result);

      inputMenuController.AddInput(result.calculatorResult, true);
      inputMenuController.Persist(false);

      controller.SaveGame();
    }
  }

  protected override ConverterResultData CreateResultData(LineControllerUI lineController) {

    Dictionary<string, MathMagnitude> parameters = new Dictionary<string, MathMagnitude>() {
      { "p1", CreateMathMagnitude(lineController.startPos * stepMultiplier, "p1") },
      { "p2", CreateMathMagnitude(lineController.endPos * stepMultiplier, "p2", 2) },
    };

    VectorCalculator calculator = new VectorCalculator();
    calculator.SetParameters(parameters);
    VectorValue result = calculator.Calculate();

    Dictionary<string, MathMagnitude> usedParameters = new Dictionary<string, MathMagnitude>() {
      { "F1", CreateMathMagnitude(vectorController.Vector * stepMultiplier, vectorController.Position * stepMultiplier, "F1") },
    };

    MathMagnitude calculatorResult = CreateMathMagnitude(result, lineController.startPos * stepMultiplier, "F_g");
    TaskInputVariable resultInput = calculatorResult.Value as TaskInputVariable;
    resultInput.shortDescription = "Gegenkraft erstellt im Modul: Gleichgewicht";
    resultInput.textMeshProName = "F<sub>g</sub>";

    return new ConverterResultData() {
      calculatorType = CalculatorEnum.DrawEquilibriumCalculator,
      calculatorParams = usedParameters,
      calculatorResult = calculatorResult,
      moduleType = KonstruktorModuleType.Equilibrium,
    };
  }

  protected override void HandleDrawButton(bool isActive) {

    angleController.ShowHighlights(isActive);

    if(isActive == true) {

      leftSidebarContent.gameObject.SetActive(false);
      editAngleButton.gameObject.SetActive(false);
      editPointButton.gameObject.SetActive(false);
      
    } else {

      leftSidebarContent.gameObject.SetActive(true);
      editAngleButton.gameObject.SetActive(true);
      editPointButton.gameObject.SetActive(true);
    }
  }

  protected override void HandleEndDraw(LineControllerUI lineController) {

    if(lineController != null) {

      drawButton.isOn = false; 
    }

    currentDrawnLine = lineController;

    onEndDraw?.Invoke(lineController);
  }

  protected override void HandleTaskVariableTap(InputOutputDrawController input) {

    // if a different variable is tapped then the current active one then tap the active one to deactivate it
    /*if(currentlyActiveInput != null && currentlyActiveInput != input) {

      currentlyActiveInput.ShowHighlight(false);
      switch(currentlyActiveInput.expectedParameterType) {
        case CalculatorParameterType.Point: HandlePointTap(currentlyActiveInput); break;
        case CalculatorParameterType.Angle: HandleAngleTap(currentlyActiveInput); break;
        case CalculatorParameterType.Force: HandleForceTap(currentlyActiveInput); break;
        case CalculatorParameterType.Vector: HandleVectorTap(currentlyActiveInput); break;
      }
    }

    switch(input.expectedParameterType) {
      case CalculatorParameterType.Point: HandlePointTap(input); break;
      case CalculatorParameterType.Angle: HandleAngleTap(input); break;
      case CalculatorParameterType.Force: HandleForceTap(input); break;
      case CalculatorParameterType.Vector: HandleVectorTap(input); break;
    }*/
  }

  private void HandlePointDrop(InputOutputDrawController item) {

    if(angleController == null) {

      angleController = InstantiateAngle();

    }

    angleController.SetPointName("P");
    angleController.RenderPosition();

    isPointDropped = true;

    TryActivateDrawButton();
  }

  private void HandleAngleDrop(InputOutputDrawController item) {

    if(angleController == null) {

      angleController = InstantiateAngle();
    }

    angleController.SetAngleName(SymbolHelper.GreekAlpha.ToString());
    angleController.RenderPosition();

    isAngleDropped = true;

    TryActivateDrawButton();
  }

/*  private void HandleForceDrop(InputOutputDrawController item) {

    if(angleController == null) {

      angleController = InstantiateAngle();
    }

    angleController.SetForceName(item.droppedItem.magnitude.Value.textMeshProName);
    angleController.RenderPosition();

    isForceDropped = true;

    TryActivateDrawButton();
  }*/

  private void HandleVectorDrop(InputOutputDrawController item) {

    if(vectorController == null) {

      vectorController = InstantiateVector();

      // Initialize Point and Angle
      HandlePointDrop(item);
      HandleAngleDrop(item);

      editAngleButton.interactable = true;
      editPointButton.interactable = true;
    }

    TaskInputVariable input = item.droppedItem.magnitude.Value as TaskInputVariable;
    vectorController.SetVectorName(input.textMeshProName);

    VectorValue value = input.GetVectorValue();

    Vector2 vector = VectorValueHelper.Convert(value);
    Vector2 startPos = VectorValueHelper.Convert(input.GetStartPoint());
    dragShiftDistance = vector.magnitude / stepMultiplier;

    vectorController.SetVectorLine(vector, startPos);
    vectorController.RenderPosition();
    vectorController.ShowValueText(false);

    angleController.SetPosition(startPos / stepMultiplier);
    angleController.SetForcePosition(dragShiftDistance);
    angleController.SetAnglePosition(vector);
    angleController.RenderPosition();

    isVectorDropped = true;

    TryActivateDrawButton();
  }

  private void HandlePointTap(Toggle toggle) {

    angleController.SetEditable(toggle.isOn);

    rightSidebarContent.gameObject.SetActive(!toggle.isOn);
    xMarker.gameObject.SetActive(toggle.isOn);
    yMarker.gameObject.SetActive(toggle.isOn);

    //BestätigungsButton aktivieren
    //Listener adden (diese funktion mit aktuellen item)
    confirmPointPositionButton.gameObject.SetActive(toggle.isOn);
    confirmPointPositionButton.onClick.RemoveAllListeners();
    confirmPointPositionButton.onClick.AddListener(() => toggle.isOn = !toggle.isOn);

    if(toggle.isOn == true) {

      currentlyActiveDrawnItem = CalculatorParameterType.Point;
      //currentlyActiveInput = item;

      PositionMarkers(angleController.Position);
    } else {

      currentlyActiveInput = null;
      currentlyActiveDrawnItem = CalculatorParameterType.Any;
    }

    //onTap?.Invoke(item);
  }

  private void HandleAngleTap(Toggle toggle) {

    wasAngleEdited = true;

    angleController.SetAngleEditable(toggle.isOn);

    confirmPointPositionButton.gameObject.SetActive(toggle.isOn);
    confirmPointPositionButton.onClick.RemoveAllListeners();
    confirmPointPositionButton.onClick.AddListener(() => toggle.isOn = !toggle.isOn);

    if(toggle.isOn == true) {

      currentlyActiveDrawnItem = CalculatorParameterType.Angle;
      //currentlyActiveInput = item;

      amountDisposable = angleController.amountPoint.OnDrag
        .Do((Vector2 position) => {
          angleController.SetAnglePosition(position);
          angleController.RenderPosition();
          UpdateDrawnLineIfExists();
        })
        .SelectMany((_) => angleController.amountPoint.OnFingerUp)
        .Do((Vector2 position) => {
          angleController.SetAnglePosition(position);
          angleController.RenderPosition();
          UpdateDrawnLineIfExists();
        })
        .Subscribe()
      ;

    } else {

      currentlyActiveInput = null;
      currentlyActiveDrawnItem = CalculatorParameterType.Any;
      amountDisposable.Dispose();
      amountDisposable = null;
    }

    //onTap?.Invoke(item);
  }

  private void HandleForceTap(Toggle toggle) {

    angleController.SetEditable(toggle.isOn);

    confirmPointPositionButton.gameObject.SetActive(toggle.isOn);
    confirmPointPositionButton.onClick.RemoveAllListeners();
    confirmPointPositionButton.onClick.AddListener(() => toggle.isOn = !toggle.isOn);

    if(toggle.isOn == true) {

      currentlyActiveDrawnItem = CalculatorParameterType.Force;

      amountDisposable = angleController.attackPoint.OnDrag
        .Do((Vector2 position) => {

          Vector3 worldPos = angleController.attackPoint.transform.parent.TransformPoint(new Vector3(position.x, 0, 0));
          Vector3 angleControllerLocalPos = angleController.transform.parent.InverseTransformPoint(worldPos);
          angleController.SetPosition(angleControllerLocalPos);
          angleController.RenderPosition();
          UpdateDrawnLineIfExists();
        })
        .SelectMany((_) => angleController.attackPoint.OnFingerUp)
        .Do((Vector2 position) => {
          float factor = amountStepSize / amountStep;
          float shiftDistance = position.x;
          float roundToClosestMultiple = (int)Math.Round(
             (shiftDistance / (double)factor),
             MidpointRounding.AwayFromZero
          ) * factor;

          Vector3 worldPos = angleController.attackPoint.transform.parent.TransformPoint(new Vector3(roundToClosestMultiple, 0, 0));
          Vector3 angleControllerLocalPos = angleController.transform.parent.InverseTransformPoint(worldPos);

          angleController.SetPosition(angleControllerLocalPos.Round(0));
          angleController.RenderPosition();
          UpdateDrawnLineIfExists();
        })
        .Subscribe()
      ;

    } else {

      currentlyActiveInput = null;
      currentlyActiveDrawnItem = CalculatorParameterType.Any;
      amountDisposable.Dispose();
      amountDisposable = null;
    }

    //onTap?.Invoke(item);
  }

  private KonstructorDrawVectorController InstantiateVector() {

    KonstructorDrawVectorController instance = Instantiate(vectorTemplate, drawnItemsContainer);
    instance.SetPosition(Vector3.zero);
    instance.SetMultiplier(new Vector2(stepMultiplier, stepMultiplier));
    instance.SetScaleMultiplier(scaleAmount / amountStepSize);
    instance.SetMinVectorLength(amountStepSize / amountStep);
    instance.name = "Vector";

    return instance;
  }

  private KonstructorDrawAngleController InstantiateAngle() {

    KonstructorDrawAngleController instance = Instantiate(angleTemplate, drawnItemsContainer);
    instance.SetPosition(Vector3.zero);
    instance.SetMultiplier(new Vector2(stepMultiplier, stepMultiplier));
    instance.SetScaleMultiplier(scaleAmount / amountStepSize);
    instance.SetMinForcePosition(amountStepSize / amountStep);
    instance.name = "Angle";

    return instance;
  }

  private void UpdateDrawnLineIfExists() {

    if(currentDrawnLine != null) {

      currentDrawnLine.PositionLine(
        angleController.Position,
        angleController.Position + (angleController.wirkungslinie.rotation * angleController.amountPoint.transform.localPosition)
      );
    }
  }

  private void Start() {

    controller = GameController.GetInstance();
    drawLine.onEndDraw.AddListener(HandleEndDraw);
    scale.transform.parent.gameObject.SetActive(true);
    scale.text = "1 LE = " + scaleAmount + " N";

    editAngleButton.transform.parent.gameObject.SetActive(true);
    editAngleButton.onValueChanged.AddListener((bool isActive) => HandleAngleTap(editAngleButton));

    editPointButton.transform.parent.gameObject.SetActive(true);
    editPointButton.onValueChanged.AddListener((bool isActive) => HandleForceTap(editPointButton));

    if(spriteTransparencyController != null) {

      spriteTransparencyController.ApplyOpacity();
    }

    Initialize();
  }

  private void OnDestroy() {

    if(angleController != null) {

      Destroy(angleController.gameObject);
    }

    if(currentDrawnLine != null) {

      Destroy(currentDrawnLine.gameObject);
    }

    if(vectorController != null) {

      Destroy(vectorController.gameObject);
    }

    if(spriteTransparencyController != null) {

      spriteTransparencyController.ResetOpacity();
    }
  }

  private void OnEnable() {

    if(spriteTransparencyController != null) {

      spriteTransparencyController.ApplyOpacity();
    }
  }

  private void OnDisable() {

    if(spriteTransparencyController != null) {

      spriteTransparencyController.ResetOpacity();
    }
  }
}
