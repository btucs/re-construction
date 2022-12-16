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
using Sirenix.OdinInspector;
using MathUnits.Physics;
using MathUnits.Physics.Units;
using MathUnits.Physics.Values;
using TMPro;
using UniRx;

public class DrawForceHandler : AbstractDrawHandler {

  [Required, FoldoutGroup("Individual Configuration", expanded: false)]
  public KonstructorDrawAngleController angleTemplate;
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

  private GameController controller;

  private CalculatorParameterType currentlyActiveDrawnItem;
  private InputOutputDrawController currentlyActiveInput;
  //private InputOutputDrawController currentlyActiveOutput;

  private LineControllerUI currentDrawnLine;
  private KonstructorDrawAngleController angleController;

  private IDisposable amountDisposable;
  private IDisposable angleDisposable;

  private bool isPointDropped;
  private bool isAngleDropped;
  private bool isForceDropped;

  public override Vector3 GetActiveItemPos() {

    switch(currentlyActiveDrawnItem) {
      case CalculatorParameterType.Point:
        return angleController.Position;

      case CalculatorParameterType.Angle:
      case CalculatorParameterType.Force:
      default:
        return Vector3.zero;
    }
  }

  public override Vector3 GetActiveItemWorldPos() {

    if(angleController == null) {

      return Vector3.zero;
    }

    return angleController.transform.position;
  }

  public override KonstructorDrawPointController[] GetDrawPoints() {

    return new KonstructorDrawPointController[2] {
      angleController.attackPoint, angleController.amountPoint
    };
  }

  public override DrawVariableData[] GetInputs() {

    return new DrawVariableData[3] {
      new DrawVariableData() 
      { 
        name = "P", 
        expectedType =  CalculatorParameterType.Point, 
        valHeadline = "Angriffspunkt", 
        valDescription = "Lege hier die Koordinaten von dem Punkt ab, an dem die Kraft wirkt." 
      },
      new DrawVariableData() 
      { 
        name = Symbols.GreekAlpha.ToString(), 
        expectedType = CalculatorParameterType.Angle, 
        valHeadline = "Winkel", 
        valDescription = "Lege hier den Winkel ab, der die Wirkungslinie der Kraft definiert." 
      },
      new DrawVariableData() 
      { 
        name = "|F|", 
        expectedType = CalculatorParameterType.Force, 
        valHeadline = "Kraftbetrag", 
        valDescription = "Lege hier einen Wert ab, der die Stärke der Kraft beschreibt." 
      },
    };
  }

  [Obsolete]
  public override DrawResultData[] GetOutputs() {

    return new DrawResultData[1] {
      new DrawResultData() { name = "V", expectedType = TaskOutputVariableUnit.ForceVector },
    };
  }

  public override void HandleInputItemDrop(InputOutputDrawController input) {

    switch(input.expectedParameterType) {

      case CalculatorParameterType.Point: HandlePointDrop(input); break;
      case CalculatorParameterType.Angle: HandleAngleDrop(input); break;
      case CalculatorParameterType.Force: HandleForceDrop(input); break;
    }
  }

  [Obsolete]
  public override void HandleOutputItemDrop(InputOutputDrawController item) {

    /*currentlyActiveOutput = item;
    VariableInfoSO.VariableInfoEntry infoEntry = controller.gameAssets.variableInfo.GetInfoFor(item.expectedOutputUnit);
    drawLine.lineSO = infoEntry.line;

    TryActivateDrawButton();*/
  }

  public override void MoveActiveItem(DirectionEnum direction, float value) {

    if(currentlyActiveDrawnItem != CalculatorParameterType.Any) {

      switch(currentlyActiveDrawnItem) {
        case CalculatorParameterType.Point:
          angleController.SetPosition(direction, value);
          angleController.RenderPosition();
          PositionMarkers(angleController.Position);          
          break;
      }
    }
  }

  public override void TryActivateDrawButton() {

    drawButton.interactable = isPointDropped == true &&
      isForceDropped == true &&
      isAngleDropped == true
    ;
  }

  public override bool AllInputsDropped() {

    return isPointDropped && isForceDropped && isAngleDropped;
  }

  public override void ClearDrawnLine() {

    if(currentDrawnLine != null) {

      Destroy(currentDrawnLine.gameObject);
      currentDrawnLine = null;
    }

    TryActivateDrawButton();
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

  public bool EditingParameter(CalculatorParameterType paramType)
  {
    return (currentlyActiveDrawnItem == paramType);
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
      { "P", CreateMathMagnitude(angleController.Position * stepMultiplier , "P")},
      { "alpha", CreateMathMagnitude(angleController.Angle, "alpha", new Degree())},
      { "F", CreateMathMagnitude(angleController.Force , "F", new Newton())}
    };

    MathMagnitude calculatorResult = CreateMathMagnitude(result, "V_F");
    TaskInputVariable resultInput = calculatorResult.Value as TaskInputVariable;
    resultInput.shortDescription = "Ein gezeichneter Vektor, erstellt im Modul: Kraft einzeichnen.";
    resultInput.textMeshProName = "V<sub>F</sub>";
    resultInput.startPointText = VectorValueHelper.ToString(lineController.startPos * stepMultiplier);

    return new ConverterResultData() {
      //step = currentlyActiveOutput.StepIndex,
      calculatorType = CalculatorEnum.DrawForceCalculator,
      calculatorParams = usedParameters,
      calculatorResult = calculatorResult,
      moduleType = KonstruktorModuleType.ForceGraphical,
      scale = scaleAmount,
    };
  }

  protected override void HandleDrawButton(bool isActive) {

    if(isActive == true) {

      if(currentlyActiveInput != null) {

        currentlyActiveInput.ShowHighlight(false);
        HandleTaskVariableTap(currentlyActiveInput);
      }

      leftSidebarContent.gameObject.SetActive(false);
    } else {

      leftSidebarContent.gameObject.SetActive(true);
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

    Debug.Log("Task Variable tapped");

    // if a different variable is tapped then the current active one then tap the active one to deactivate it
    if(currentlyActiveInput != null && currentlyActiveInput != input) {

      currentlyActiveInput.ShowHighlight(false);
      switch(currentlyActiveInput.expectedParameterType) {
        case CalculatorParameterType.Point: HandlePointTap(currentlyActiveInput); break;
        case CalculatorParameterType.Angle: HandleAngleTap(currentlyActiveInput); break;
        case CalculatorParameterType.Force: HandleForceTap(currentlyActiveInput); break;
      }
    }

    switch(input.expectedParameterType) {
      case CalculatorParameterType.Point: HandlePointTap(input); break;
      case CalculatorParameterType.Angle: HandleAngleTap(input); break;
      case CalculatorParameterType.Force: HandleForceTap(input); break;
    }
  }

  private void HandlePointDrop(InputOutputDrawController item) {

    if(angleController == null) {

      angleController = InstantiateAngle();
      
    }

    angleController.SetPointName(item.droppedItem.magnitude.Value.textMeshProName);
    angleController.RenderPosition();

    isPointDropped = true;

    TryActivateDrawButton();
  }

  private void HandleAngleDrop(InputOutputDrawController item) {

    if(angleController == null) {

      angleController = InstantiateAngle();
    }

    angleController.SetAngleName(item.droppedItem.magnitude.Value.textMeshProName);
    angleController.RenderPosition();

    isAngleDropped = true;

    TryActivateDrawButton();
  }

  private void HandleForceDrop(InputOutputDrawController item) {

    if(angleController == null) {

      angleController = InstantiateAngle();
    }

    angleController.SetForceName(item.droppedItem.magnitude.Value.textMeshProName);
    angleController.RenderPosition();

    isForceDropped = true;

    TryActivateDrawButton();
  }

  private void HandlePointTap(InputOutputDrawController item) {

    angleController.SetEditable(item.IsHighlighted);

    rightSidebarContent.gameObject.SetActive(!item.IsHighlighted);
    xMarker.gameObject.SetActive(item.IsHighlighted);
    yMarker.gameObject.SetActive(item.IsHighlighted);

    //BestätigungsButton aktivieren
    //Listener adden (diese funktion mit aktuellen item)
    confirmPointPositionButton.gameObject.SetActive(item.IsHighlighted);
    confirmPointPositionButton.onClick.RemoveAllListeners();
    confirmPointPositionButton.onClick.AddListener(item.ToggleHighlight);
    confirmPointPositionButton.onClick.AddListener(delegate {
      HandleTaskVariableTap(item);
    });

    if(item.IsHighlighted == true) {

      currentlyActiveDrawnItem = CalculatorParameterType.Point;
      currentlyActiveInput = item;

      PositionMarkers(angleController.Position);
    } else {

      currentlyActiveInput = null;
      currentlyActiveDrawnItem = CalculatorParameterType.Any;
    }

    onTap?.Invoke(item);
  }

  private void HandleAngleTap(InputOutputDrawController item) {

    angleController.SetAngleEditable(item.IsHighlighted);

    confirmPointPositionButton.gameObject.SetActive(item.IsHighlighted);
    confirmPointPositionButton.onClick.RemoveAllListeners();
    confirmPointPositionButton.onClick.AddListener(item.ToggleHighlight);
    confirmPointPositionButton.onClick.AddListener(delegate {
      HandleTaskVariableTap(item);
    });

    if(item.IsHighlighted == true) {

      currentlyActiveDrawnItem = CalculatorParameterType.Angle;
      currentlyActiveInput = item;

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

    onTap?.Invoke(item);
  }

  private void HandleForceTap(InputOutputDrawController item) {

    angleController.SetForceEditable(item.IsHighlighted);

    confirmPointPositionButton.gameObject.SetActive(item.IsHighlighted);
    confirmPointPositionButton.onClick.RemoveAllListeners();
    confirmPointPositionButton.onClick.AddListener(item.ToggleHighlight);
    confirmPointPositionButton.onClick.AddListener(delegate {
      HandleTaskVariableTap(item);
    });

    if(item.IsHighlighted == true) {

      currentlyActiveDrawnItem = CalculatorParameterType.Force;
      currentlyActiveInput = item;
      amountDisposable = angleController.amountPoint.OnDrag
        .Do((Vector2 position) => {
          angleController.SetForcePosition(position.x);
          angleController.RenderPosition();
          UpdateDrawnLineIfExists();
        })
        .SelectMany((_) => angleController.amountPoint.OnFingerUp)
        .Do((Vector2 position) => {
          float factor = amountStepSize / amountStep;
          float roundToClosestMultiple = (int)Math.Round(
             (position.x / (double)factor),
             MidpointRounding.AwayFromZero
          ) * factor;
          angleController.SetForcePosition(roundToClosestMultiple);
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

    onTap?.Invoke(item);
  }

  private void UpdateDrawnLineIfExists() {

    if(currentDrawnLine != null) {

      currentDrawnLine.PositionLine(
        angleController.Position,
        angleController.Position + (angleController.wirkungslinie.rotation * angleController.amountPoint.transform.localPosition)
      );
    }
  }

  private KonstructorDrawAngleController InstantiateAngle() {

    KonstructorDrawAngleController instance = Instantiate(angleTemplate, drawnItemsContainer);
    instance.SetPosition(Vector3.zero);
    instance.SetMultiplier(new Vector2(stepMultiplier, stepMultiplier));
    instance.SetScaleMultiplier(scaleAmount / amountStepSize);
    instance.SetMinForcePosition(amountStepSize / amountStep);
    instance.name = "Angle";

    instance.onMove.AddListener((_) => UpdateDrawnLineIfExists());

    return instance;
  }

  private void Start() {

    controller = GameController.GetInstance();
    drawLine.onEndDraw.AddListener(HandleEndDraw);
    scale.transform.parent.gameObject.SetActive(true);
    scale.text = "1 LE = " + scaleAmount + " N";
    Initialize();
  }

  private void OnDestroy() {

    if(angleController != null) {

      Destroy(angleController.gameObject);
    }

    if(currentDrawnLine != null) {

      Destroy(currentDrawnLine.gameObject);
    }
  }
}
