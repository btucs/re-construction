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
using MathUnits.Physics.Values;

public class ResultAssessment : AssessmentAbstract {

  private KonstruktorSceneData data;
  private VariableInfoSO variableInfo;
  private CalculatorFactory factory;
  private FeedbackType currentFeedback = FeedbackType.None;

  private TaskDataSO.SolutionStep currentStep;
  private ConverterResultData currentResultData;

  public ResultAssessment(KonstruktorSceneData data, VariableInfoSO info) {

    this.data = data;
    variableInfo = info;
    factory = new CalculatorFactory();
  }

  public override int MaxPoints => 1;
  public override int CurrentPoints => CalculatePoints();

  public override FeedbackType GetFeedback() {

    return currentFeedback;
  }

  public override void Configure(TaskDataSO.SolutionStep step, ConverterResultData resultData) {

    currentStep = step;
    currentResultData = resultData;
  }

  public override int CalculatePoints() {

    if(currentResultData == null) {

      return 0;
    }

    bool isCorrect = false;

    try {

      VariableInfoSO.VariableInfoEntry info = variableInfo.GetInfoFor(currentStep.output.unit);
      
      switch(VariableHelper.GetUnitResultType(currentStep.output.unit)) {
        case UnitDataResultType.Scalar:
          isCorrect = HandleScalarCalculator(info, currentResultData, currentStep);

          break;

        case UnitDataResultType.Vector:

          isCorrect = HandleVectorCalculator(info, currentResultData, currentStep);

          break;

        case UnitDataResultType.Other:

          isCorrect = HandleOtherCalculator(info, currentResultData, currentStep);

          break;
      }
    } catch(Exception) {
      //do nothing
    }

    if(isCorrect == true) {

      currentFeedback = FeedbackType.CalculateResultCorrect;

      return 1;
    }

    currentFeedback = FeedbackType.CalculateResultIncorrect;

    return 0;
  }

  private bool HandleOtherCalculator(
    VariableInfoSO.VariableInfoEntry info,
    ConverterResultData converterResultData,
    TaskDataSO.SolutionStep step
  ) {

    AbstractNonCalculatableCalculator calculator = factory.GetNonCalculatableCalculator(info.calculator);

    bool isCorrect = calculator.IsCorrect(converterResultData.calculatorResult);

    return isCorrect;
  }

  private bool HandleScalarCalculator(
    VariableInfoSO.VariableInfoEntry info,
    ConverterResultData resultData,
    TaskDataSO.SolutionStep step
  ) {

    CalculatorAbstract<ScalarValue> calculator = factory.GetScalarCalculator(info.calculator);
    Tuple<string, CalculatorParameterType>[] parameters = calculator.GetExpectedParameterTypes();
    Dictionary<string, MathMagnitude> calculationParams = parameters
      .Select((Tuple<string, CalculatorParameterType> parameter, int index) => Convert((TaskInputVariable)step.inputs[index], parameter.Item1))
      .ToDictionary((MathMagnitude parameter) => parameter.Value.name)
    ;

    calculator.SetParameters(calculationParams);
    ScalarValue result = calculator.Calculate();
    result.Value = Math.Round(result.Value, 2, MidpointRounding.AwayFromZero);
    TaskInputVariable calculationResult = (TaskInputVariable)resultData.calculatorResult.Value;
    ScalarValue resultScalar = calculationResult.GetScalarValue();
    resultScalar.Value = Math.Round(resultScalar.Value, 2, MidpointRounding.AwayFromZero);

    bool isCorrect = result.ToString(CultureInfo.InvariantCulture) == resultScalar.ToString(CultureInfo.InvariantCulture);

    return isCorrect;
  }

  protected bool HandleVectorCalculator(
    VariableInfoSO.VariableInfoEntry info,
    ConverterResultData resultData,
    TaskDataSO.SolutionStep step
  ) {

    CalculatorAbstract<VectorValue> calculator = factory.GetVectorCalculator(info.calculator, resultData.scale);
    Tuple<string, CalculatorParameterType>[] parameters = calculator.GetExpectedParameterTypes();
    Dictionary<string, MathMagnitude> calculationParams = parameters
      .Select((Tuple<string, CalculatorParameterType> parameter, int index) => Convert((TaskInputVariable)step.inputs[index], parameter.Item1))
      .ToDictionary((MathMagnitude parameter) => parameter.Value.name)
    ;

    calculator.SetParameters(calculationParams);
    VectorValue result = calculator.Calculate();
    TaskInputVariable calculationResult = (TaskInputVariable)resultData.calculatorResult.Value;

    bool isCorrect = result.ToString(CultureInfo.InvariantCulture) == calculationResult.GetVectorValue().ToString(CultureInfo.InvariantCulture);

    return isCorrect;
  }

  private MathMagnitude Convert(TaskInputVariable variable, string name) {

    TaskInputVariable clone = variable.Clone();
    clone.name = name;

    return new MathMagnitude() {

      Value = clone,
    };
  }

  private MathMagnitude Convert(ScalarValue variable) {

    return new MathMagnitude() {
      Value = new TaskInputVariable() {
        textValue = variable.ToString(CultureInfo.InvariantCulture),
      },
    };
  }
}
