#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using MathUnits.Physics.Values;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class DrawLineVolatilityHandler : AbstractDrawHandler {

  [Required, FoldoutGroup("Individual Configuration", expanded: false)]
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
  public Toggle editPointButton;
  [Required, FoldoutGroup("Individual Configuration")]
  public Button continueButton;

  private GameController controller;

  private Vector3 startPos;

  private IDisposable vectorDisposable;

  private bool wasForceMoved = false;

  private KonstructorDrawVectorController vectorController;

  public override bool AllInputsDropped() {

    return vectorController != null;
  }

  public override bool AllAdjustmentsMade()
  {
    return wasForceMoved;
  }

  public override void ClearDrawnLine() {
        
  }

  public override void CreateAndSaveResult() {

    ConverterResultData result = CreateResultData(vectorController.vectorLine);
    controller.gameState.konstruktorSceneData.AddResult(result);

    inputMenuController.AddInput(result.calculatorResult, true);
    inputMenuController.Persist(false);

    controller.SaveGame();
  }

  public override Vector3 GetActiveItemPos() {

    if(vectorController != null) {

      return vectorController.Position;
    }

    return Vector3.zero;
  }

  public override Vector3 GetActiveItemWorldPos() {

    if(vectorController != null) {

      return vectorController.transform.position;
    }

    return Vector3.zero;
  }

  public override KonstructorDrawPointController[] GetDrawPoints() {
    throw new System.NotImplementedException();
  }

  public override DrawVariableData[] GetInputs() {

    return new DrawVariableData[1] {
      new DrawVariableData() 
      { 
        name = "F_1", 
        expectedType = CalculatorParameterType.Vector, 
        disableTap = true,
        valHeadline = "Kraft", 
        valDescription = "Lege hier eine Kraft ab, die du entlang ihrer Wirkungslinie verschieben willst." 
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

    vectorController.SetPosition(direction, value);
    vectorController.RenderPosition();
    PositionMarkers(vectorController.Position);
  }

  public override void TryActivateDrawButton() {
    throw new System.NotImplementedException();
  }

  protected override ConverterResultData CreateResultData(LineControllerUI lineController) {

    Dictionary<string, MathMagnitude> usedParameters = new Dictionary<string, MathMagnitude>() {
      { "F1", CreateMathMagnitude(vectorController.Vector * stepMultiplier, startPos * stepMultiplier, "F1") },
    };

    MathMagnitude result = CreateMathMagnitude(vectorController.Vector * stepMultiplier, vectorController.Position * stepMultiplier, "F_l");
    TaskInputVariable resultInput = result.Value as TaskInputVariable;
    resultInput.shortDescription = "Verschobener Vektor, erstellt im Module: Linienflüchtigkeit";
    resultInput.textMeshProName = "F<sub>l</sub>";

    return new ConverterResultData() {
      calculatorType = CalculatorEnum.LineVolatilityCalculator,
      calculatorParams = usedParameters,
      calculatorResult = result,
      moduleType = KonstruktorModuleType.LineVolatility,
    };
  }

  protected override void HandleDrawButton(bool isActive) {
    throw new System.NotImplementedException();
  }

  protected override void HandleEndDraw(LineControllerUI lineController) {
    throw new System.NotImplementedException();
  }

  protected override void HandleTaskVariableTap(InputOutputDrawController item) {
    
  }

  private void HandleVectorDrop(InputOutputDrawController item) {

    if(vectorController == null) {

      vectorController = InstantiateVector();

      editPointButton.interactable = true;
    }

    TaskInputVariable input = item.droppedItem.magnitude.Value as TaskInputVariable;
    vectorController.SetVectorName(input.textMeshProName);

    VectorValue value = input.GetVectorValue();

    Vector2 vector = VectorValueHelper.Convert(value);
    startPos = VectorValueHelper.Convert(input.GetStartPoint());

    vectorController.SetVectorLine(vector, startPos);
    vectorController.RenderPosition();

    continueButton.interactable = true;
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

  private void ActivateVectorMove(Toggle toggle) {

    if(vectorController != null) {

      wasForceMoved = true;

      vectorController.SetEditable(toggle.isOn);

      confirmPointPositionButton.gameObject.SetActive(toggle.isOn);
      confirmPointPositionButton.onClick.RemoveAllListeners();
      confirmPointPositionButton.onClick.AddListener(() => toggle.isOn = !toggle.isOn);

      if(toggle.isOn == true) {

        vectorDisposable = vectorController.attackPoint.OnDrag
          .Do((Vector2 position) => {

            Vector3 worldPos = vectorController.attackPoint.transform.parent.TransformPoint(new Vector3(position.x, 0, 0));
            Vector3 vectorControllerLocalPos = vectorController.transform.parent.InverseTransformPoint(worldPos);
            vectorController.SetPosition(vectorControllerLocalPos);
            vectorController.RenderPosition();
          })
          .SelectMany((_) => vectorController.attackPoint.OnFingerUp)
          .Do((Vector2 position) => {
            float factor = amountStepSize / amountStep;
            float roundToClosestMultiple = (int)Math.Round(
               (position.x / (double)factor),
               MidpointRounding.AwayFromZero
            ) * factor;

            Vector3 worldPos = vectorController.attackPoint.transform.parent.TransformPoint(new Vector3(roundToClosestMultiple, 0, 0));
            Vector3 vectorControllerLocalPos = vectorController.transform.parent.InverseTransformPoint(worldPos);

            vectorController.SetPosition(vectorControllerLocalPos.Round(0));
            vectorController.RenderPosition();
          })
          .Subscribe()
        ;

      } else {

        vectorDisposable.Dispose();
        vectorDisposable = null;
      }
    }
  }

  private void Start() {

    controller = GameController.GetInstance();

    scale.transform.parent.gameObject.SetActive(true);
    scale.text = "1 LE = " + scaleAmount + " N";

    editPointButton.transform.parent.gameObject.SetActive(true);
    editPointButton.onValueChanged.AddListener((bool isActive) => ActivateVectorMove(editPointButton));

    drawButton.gameObject.SetActive(false);

    Initialize();
  }

  private void OnDestroy() {

    if(vectorController != null) {

      Destroy(vectorController.gameObject);
    }
  }
}
