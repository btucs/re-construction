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
using MathUnits.Physics.Values;

public class DrawLineVolatilityAssessment : ResultAssessment {

  private KonstruktorSceneData data;
  private VariableInfoSO variableInfo;
  private CalculatorFactory factory;

  private FeedbackType currentFeedback = FeedbackType.None;

  private TaskDataSO.SolutionStep currentStep;
  private ConverterResultData currentResultData;

  public DrawLineVolatilityAssessment(KonstruktorSceneData data, VariableInfoSO info): base(data, info) {

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

      currentFeedback = FeedbackType.NoResult;

      return 0;
    }

    // if the wrong module was used there can be no vector drawn
    if(currentResultData.calculatorType != CalculatorEnum.LineVolatilityCalculator) {

      currentFeedback = FeedbackType.DrawLineVolatilityNoForce;

      return 0;
    }

    currentResultData.calculatorParams.TryGetValue("F1", out MathMagnitude F1);

    TaskInputVariable F1Vector = F1.Value as TaskInputVariable;
    VectorValue F1VectorValue = F1Vector.GetVectorValue();
    VectorValue startPoint = F1Vector.GetStartPoint();

    TaskInputVariable resultVector = currentResultData.calculatorResult.Value as TaskInputVariable;
    VectorValue resultVectorValue = resultVector.GetVectorValue();
    VectorValue resultStartPoint = resultVector.GetStartPoint();

    VectorValue expectedStartingPoint = currentStep.output.GetExpectedStartingPoint();

    bool isCorrectTargetPosition = resultStartPoint == expectedStartingPoint;

    if(isCorrectTargetPosition == true) {

      currentFeedback = FeedbackType.DrawLineVolatilityPositionCorrect;

      return 1;
    }

    currentFeedback = FeedbackType.DrawLineVolatilityPositionIncorrect;

    return 0;
  }
}
