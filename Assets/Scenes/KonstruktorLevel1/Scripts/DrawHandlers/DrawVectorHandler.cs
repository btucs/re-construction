#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MathUnits.Physics.Values;

public class DrawVectorHandler : AbstractDrawHandler {

  [Required, FoldoutGroup("Individual Configuration", expanded: false)]
  public KonstructorDrawPointController pointTemplate;

  private GameController controller;

  private Dictionary<InputOutputDrawController, KonstructorDrawPointController> drawnItems = new Dictionary<InputOutputDrawController, KonstructorDrawPointController>();

  private KonstructorDrawPointController currentlyActiveDrawnItem;
  private InputOutputDrawController currentlyActiveInput;
  //private InputOutputDrawController currentlyActiveOutput;
  private LineControllerUI currentLine;

  public override DrawVariableData[] GetInputs() {

    return new DrawVariableData[2] {
      new DrawVariableData() 
      { 
        name = "P<sub>1</sub>", 
        expectedType = CalculatorParameterType.Point,
        valHeadline = "Punkt 1", 
        valDescription = "Lege hier die Koordinaten von einem Start- oder Endpunkt P1 fest, die du dann einstellen möchtest." 
      },
      new DrawVariableData() 
      { 
        name = "P<sub>2</sub>", 
        expectedType = CalculatorParameterType.Point,
        valHeadline = "Punkt 2", 
        valDescription = "Lege hier die Koordinaten von einem Start- oder Endpunkt P2 fest, die du dann einstellen möchtest."  
      },
    };
  }

  [Obsolete]
  public override DrawResultData[] GetOutputs() {

    return new DrawResultData[1] {
      new DrawResultData() {name = "V", expectedType = TaskOutputVariableUnit.Vector }
    };
  }

  public override void Terminate() {

    base.Terminate();
    inputs.ForEach((InputOutputDrawController item) => {

      item.onItemDropped.RemoveListener(HandleInputItemDrop);
    });

    /*if(currentlyActiveOutput != null) {

      currentlyActiveOutput.onItemDropped.RemoveListener(HandleOutputItemDrop);
    }*/
  }

  public override void TryActivateDrawButton() {

    drawButton.interactable = drawnItems.Count >= 2;
  }

  public override bool AllInputsDropped() {

    return drawnItems.Count >= 2;
  }

  public override void HandleInputItemDrop(InputOutputDrawController item) {

    if(drawnItems.ContainsKey(item) == false) {

      KonstructorDrawPointController pointInstance = GameObject.Instantiate(pointTemplate, drawnItemsContainer);
      pointInstance.SetPosition(Vector3.zero);
      pointInstance.SetPointName(item.droppedItem.magnitude.Value.textMeshProName);
      pointInstance.SetMultiplier(new Vector2(stepMultiplier, stepMultiplier));
      pointInstance.RenderPosition();
      pointInstance.name = "Point " + drawnItems.Count();

      drawnItems.Add(item, pointInstance);
    } else {

      drawnItems.TryGetValue(item, out KonstructorDrawPointController pointInstance);
      pointInstance.SetPointName(item.droppedItem.magnitude.Value.textMeshProName);
      pointInstance.RenderPosition();
    }

    TryActivateDrawButton();
  }

  [Obsolete]
  public override void HandleOutputItemDrop(InputOutputDrawController item) {

  /*  currentlyActiveOutput = item;
    VariableInfoSO.VariableInfoEntry infoEntry = controller.gameAssets.variableInfo.GetInfoFor(item.expectedOutputUnit);
    drawLine.lineSO = infoEntry.line;

    TryActivateDrawButton();*/
  }

  public override Vector3 GetActiveItemPos() {
    Vector3 returnVector = Vector3.zero;
    if(currentlyActiveDrawnItem != null)
      returnVector = currentlyActiveDrawnItem.transform.localPosition;

    return returnVector;
  }

  public override Vector3 GetActiveItemWorldPos() {
    Vector3 returnVector = Vector3.zero;
    if(currentlyActiveDrawnItem != null)
      returnVector = currentlyActiveDrawnItem.transform.position;

    return returnVector;
  }

  public override void MoveActiveItem(DirectionEnum direction, float pos) {

    if(currentlyActiveDrawnItem != null) {

      Vector3 currentPos = currentlyActiveDrawnItem.Position;
      currentlyActiveDrawnItem.SetPosition(direction, pos);
      Vector3 newPos = currentlyActiveDrawnItem.Position;
      currentlyActiveDrawnItem.RenderPosition();
      PositionMarkers(currentlyActiveDrawnItem.Position);

      if(currentLine != null) {

        Vector3 startPos = currentLine.startPos;
        Vector3 endPos = currentLine.endPos;

        if(startPos == currentPos) {

          currentLine.PositionLine(newPos, endPos);
        } else {

          currentLine.PositionLine(startPos, newPos);
        }
      }
    }
  }

  public override KonstructorDrawPointController[] GetDrawPoints() {

    return drawnItems.Values.ToArray();
  }

  public override void ClearDrawnLine() {

    if(currentLine != null) {

      Destroy(currentLine.gameObject);
      currentLine = null;
    }

    TryActivateDrawButton();
  }

  public override void CreateAndSaveResult() {

    if(currentLine != null) {

      ConverterResultData result = CreateResultData(currentLine);

      controller.gameState.konstruktorSceneData.AddResult(result);

      inputMenuController.AddInput(result.calculatorResult, true);
      inputMenuController.Persist(false);

      controller.SaveGame();
    }
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

  protected override void HandleTaskVariableTap(InputOutputDrawController item) {

    inputs.ForEach((InputOutputDrawController input) => {

      if(input != item) {

        input.ShowHighlight(false);
      }
    });

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

      drawnItems.TryGetValue(item, out KonstructorDrawPointController activeItem);

      Vector3 currentPos = activeItem.transform.localPosition;

      currentlyActiveDrawnItem = activeItem;
      currentlyActiveInput = item;
      PositionMarkers(activeItem.transform.localPosition);
    } else {

      currentlyActiveDrawnItem = null;
      currentlyActiveInput = null;
    }

    onTap?.Invoke(item);
  }

  protected override void HandleEndDraw(LineControllerUI lineController) {

    if(lineController != null) {

      drawButton.isOn = false;
    }

    currentLine = lineController;

    onEndDraw?.Invoke(lineController);
  }

  protected override ConverterResultData CreateResultData(LineControllerUI lineController) {

    Dictionary<string, MathMagnitude> parameters = new Dictionary<string, MathMagnitude>() {
      { "p1", CreateMathMagnitude(lineController.startPos * stepMultiplier, "p1") },
      { "p2", CreateMathMagnitude(lineController.endPos * stepMultiplier, "p2") },
    };

    VectorCalculator calculator = new VectorCalculator();
    calculator.SetParameters(parameters);
    VectorValue result = calculator.Calculate();

    MathMagnitude calculatorResult = CreateMathMagnitude(result, "V");
    TaskInputVariable resultInput = calculatorResult.Value as TaskInputVariable;
    resultInput.shortDescription = "Ein gezeichneter Vektor, erstellt im Vektormodul";
    resultInput.textMeshProName = "V";
    resultInput.startPointText = VectorValueHelper.ToString(lineController.startPos * stepMultiplier);

    return new ConverterResultData() {
      calculatorType = CalculatorEnum.VectorCalculator,
      calculatorParams = parameters,
      calculatorResult = calculatorResult,
      moduleType = KonstruktorModuleType.Vector,
    };
  }

  private void Start() {

    controller = GameController.GetInstance();
    drawLine.onEndDraw.AddListener(HandleEndDraw);
    Initialize();
  }

  private void OnDestroy() {

    foreach(KonstructorDrawPointController drawnItem in drawnItems.Values) {

      Destroy(drawnItem.gameObject);
    }

    if(currentLine != null) {

      Destroy(currentLine.gameObject);
    }
  }
}
