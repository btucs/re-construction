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

public class DrawInteractionHandler : AbstractDrawHandler
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

  private GameController controller;

  private LineControllerUI currentDrawnLine;
  private KonstructorDrawAngleController angleController;
  private KonstructorDrawVectorController vectorController;

  private IDisposable amountDisposable;
  private IDisposable angleDisposable;

  private bool angleEdited = false;
  private bool pointEdited = false;

  private float dragShiftDistance = 0;

  public override bool AllInputsDropped() {

    return vectorController != null;
  }

  public override bool AllAdjustmentsMade()
  {
    return (angleEdited == true);
  }

  public override void ClearDrawnLine() {

    if(currentDrawnLine != null) {

      Destroy(currentDrawnLine.gameObject);
      currentDrawnLine = null;
    }

    TryActivateDrawButton();
  }

  public override Vector3 GetActiveItemPos() {

    return Vector3.zero;
  }

  public override Vector3 GetActiveItemWorldPos() {

    return Vector3.zero;
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
        valDescription = "Lege hier die Kraft ab, zu der du die Gegenkraft bestimmen möchtest."  
      },
    };
  }

  [Obsolete]
  public override DrawResultData[] GetOutputs() {
    throw new System.NotImplementedException();
  }

  public override void HandleInputItemDrop(InputOutputDrawController input) {

    if(input.expectedParameterType == CalculatorParameterType.Vector) {

      HandleVectorDrop(input);
    }
  }

  [Obsolete]
  public override void HandleOutputItemDrop(InputOutputDrawController output) {
    throw new System.NotImplementedException();
  }

  public override void MoveActiveItem(DirectionEnum direction, float value) {

    throw new NotImplementedException();
  }

  public override void TryActivateDrawButton() {

    if(vectorController != null) {

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

    Vector3 vectorPos = vectorController.Position;

    Dictionary<string, MathMagnitude> usedParameters = new Dictionary<string, MathMagnitude>() {
      { "F1", CreateMathMagnitude((vectorPos + vectorController.Vector) * stepMultiplier, vectorController.Position * stepMultiplier, "F1") },
    };

    MathMagnitude calculatorResult = CreateMathMagnitude(result, lineController.startPos * stepMultiplier, "F_g");
    TaskInputVariable resultInput = calculatorResult.Value as TaskInputVariable;
    resultInput.shortDescription = "Gegenkraft erstellt im Modul: Wechselwirkung";
    resultInput.textMeshProName = "F<sub>w</sub>";

    return new ConverterResultData() {
      calculatorType = CalculatorEnum.DrawInteractionCalculator,
      calculatorParams = usedParameters,
      calculatorResult = calculatorResult,
      moduleType = KonstruktorModuleType.Interaction,
    };
  }

  protected override void HandleDrawButton(bool isActive) {

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

  }

  private void HandlePointDrop(InputOutputDrawController item) {

    if(angleController == null) {

      angleController = InstantiateAngle();

    }

    angleController.SetPointName("P");
    angleController.RenderPosition();
  }

  private void HandleAngleDrop(InputOutputDrawController item) {

    if(angleController == null) {

      angleController = InstantiateAngle();
    }

    angleController.SetAngleName(SymbolHelper.GreekAlpha.ToString());
    angleController.RenderPosition();
  }

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
    vectorController.ShowValueText(false);
    vectorController.RenderPosition();

    angleController.SetPosition(startPos / stepMultiplier);
    angleController.SetForcePosition(dragShiftDistance);
    angleController.SetAnglePosition(vector);
    angleController.RenderPosition();

    TryActivateDrawButton();
  }

  private void HandleAngleTap(Toggle toggle) {

    angleEdited = true;
    angleController.SetAngleEditable(toggle.isOn);

    confirmPointPositionButton.gameObject.SetActive(toggle.isOn);
    confirmPointPositionButton.onClick.RemoveAllListeners();
    confirmPointPositionButton.onClick.AddListener(() => toggle.isOn = !toggle.isOn);

    if(toggle.isOn == true) {

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

      amountDisposable.Dispose();
      amountDisposable = null;
    }
  }

  private void HandleForceTap(Toggle toggle) {

    angleController.SetEditable(toggle.isOn);

    confirmPointPositionButton.gameObject.SetActive(toggle.isOn);
    confirmPointPositionButton.onClick.RemoveAllListeners();
    confirmPointPositionButton.onClick.AddListener(() => toggle.isOn = !toggle.isOn);

    if(toggle.isOn == true) {

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

      amountDisposable.Dispose();
      amountDisposable = null;
    }
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
  }
}
