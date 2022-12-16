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
using MathUnits.Physics.Values;

public class DrawInteractionAssessment : ResultAssessment {

  private KonstruktorSceneData data;
  private VariableInfoSO variableInfo;
  private CalculatorFactory factory;

  private FeedbackType currentFeedback = FeedbackType.None;

  private TaskDataSO.SolutionStep currentStep;

  private ConverterResultData currentResultData;

  public DrawInteractionAssessment(KonstruktorSceneData data, VariableInfoSO info): base(data, info) {

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
    if(currentResultData.calculatorType != CalculatorEnum.DrawEquilibriumCalculator) {

      currentFeedback = FeedbackType.DrawEquilibriumNoForce;

      return 0;
    }

    currentResultData.calculatorParams.TryGetValue("F1", out MathMagnitude F1);

    TaskInputVariable F1Vector = F1.Value as TaskInputVariable;
    VectorValue F1VectorValue = F1Vector.GetVectorValue();
    VectorValue startPoint = F1Vector.GetStartPoint();

    TaskInputVariable resultVector = currentResultData.calculatorResult.Value as TaskInputVariable;
    VectorValue resultVectorValue = resultVector.GetVectorValue();
    VectorValue resultStartPoint = resultVector.GetStartPoint();
    
    bool isOnActionLine = IsOnActionLine(startPoint, startPoint + F1VectorValue, resultStartPoint, resultStartPoint + resultVectorValue);
    bool isOppositeDirection = F1VectorValue * -1 == resultVectorValue;

    switch(isOnActionLine) {

      case true when isOppositeDirection == true:

        currentFeedback = FeedbackType.DrawInteractionOppositeOnActionLine;

        return 1;

      case true when isOppositeDirection == false:

        currentFeedback = FeedbackType.DrawInteractionOppositeNotOnActionLine;

        return 0;

      case false when isOppositeDirection == true:

        currentFeedback = FeedbackType.DrawInteractionNotOppositeOnActionLine;

        return 0;

      case false when isOppositeDirection == false:

        currentFeedback = FeedbackType.DrawInteractionNotOppositeNotOnActionLine;

        return 0;
    }

    return 0;
  }

  private bool IsOnActionLine(VectorValue ref1, VectorValue ref2, VectorValue target1, VectorValue target2) {

    double ref1X = ref1.Value.X1;
    double ref1Y = ref1.Value.X2;
    
    double ref2X = ref2.Value.X1;
    double ref2Y = ref2.Value.X2;

    double target1X = target1.Value.X1;
    double target1Y = target1.Value.X2;

    double target2X = target2.Value.X1;
    double target2Y = target2.Value.X2;

    Func<double, double, bool> IsOnLine = (x, y) => (y - ref1Y) * (ref2X - ref1X) == (x - ref1X) * (ref2Y - ref1Y);

    bool isTarget1OnLine = IsOnLine(target1X, target1Y);
    bool isTarget2OnLine = IsOnLine(target2X, target2Y);

    return isTarget1OnLine && isTarget2OnLine;
  }
}
