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

public class DrawForceAssessment : ResultAssessment {

  private KonstruktorSceneData data;
  private VariableInfoSO variableInfo;
  private CalculatorFactory factory;

  private FeedbackType currentFeedback = FeedbackType.None;

  private TaskDataSO.SolutionStep currentStep;
  private ConverterResultData currentResultData;

  public DrawForceAssessment(KonstruktorSceneData data, VariableInfoSO info): base(data, info) {

    this.data = data;
    variableInfo = info;
    factory = new CalculatorFactory();
  }

  public override int MaxPoints => 4;
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
    if(currentResultData.calculatorType != CalculatorEnum.DrawForceCalculator) {

      currentFeedback = FeedbackType.DrawForceNoForce;

      return 0;
    }


    bool isForceVectorCorrect = false;

    try {

      TaskOutputVariableUnit unit = currentStep.output.unit;
      VariableInfoSO.VariableInfoEntry info = variableInfo.GetInfoFor(unit);
      isForceVectorCorrect = HandleVectorCalculator(info, currentResultData, currentStep);

    } catch(Exception) {

      // do nothing
    }

    currentResultData.calculatorParams.TryGetValue("alpha", out MathMagnitude angleValue);
    currentResultData.calculatorParams.TryGetValue("P", out MathMagnitude pointValue);
    currentResultData.calculatorParams.TryGetValue("F", out MathMagnitude forceValue);

    ScalarValue expectedAngle = FindScalarVariable("alpha", currentStep.inputs);
    ScalarValue expectedForce = FindScalarVariable("F", currentStep.inputs);
    VectorValue expectedPoint = FindVectorVariable("P", currentStep.inputs);

    TaskInputVariable currentAngle = angleValue.Value as TaskInputVariable;
    TaskInputVariable currentForce = forceValue.Value as TaskInputVariable;
    TaskInputVariable currentPoint = pointValue.Value as TaskInputVariable;

    bool isAngleCorrect = currentAngle.GetScalarValue() == expectedAngle;
    bool isForceCorrect = currentForce.GetScalarValue() == expectedForce;
    bool isPointCorrect = currentPoint.GetVectorValue() == expectedPoint;

    switch(isForceCorrect) {

      case true when isAngleCorrect == true && isPointCorrect == true:

        if(isForceVectorCorrect == true) {

          currentFeedback = FeedbackType.DrawForceCorrect;

          return 4;
        }

        currentFeedback = FeedbackType.DrawForceDirectionWrong;

        return 3;

      case true when isPointCorrect == false && isAngleCorrect == true:
        currentFeedback = FeedbackType.DrawForcePointIncorrectAngleCorrect;

        return 2;

      case true when isPointCorrect == true && isAngleCorrect == false:
        currentFeedback = FeedbackType.DrawForcePointCorrectAngleIncorrect;

        return 2;

      case false when isPointCorrect == true && isAngleCorrect == true:
        currentFeedback = FeedbackType.DrawForceIncorrectPointCorrectAngleCorrect;

        return 2;

      case true when isPointCorrect == false && isAngleCorrect == false:
        currentFeedback = FeedbackType.DrawForcePointIncorrectAngleIncorrect;

        return 1;

      case false when isPointCorrect == true && isAngleCorrect == false:
        currentFeedback = FeedbackType.DrawForceIncorrectPointCorrectAngleIncorrect;

        return 1;

      case false when isPointCorrect == false && isAngleCorrect == true:
        currentFeedback = FeedbackType.DrawForceIncorrectPointIncorrectAngleCorrect;

        return 1;

      case false when isPointCorrect == false && isAngleCorrect == false:
        currentFeedback = FeedbackType.DrawForceIncorrect;

        return 0;
    }

    return 0;
  }

  private ScalarValue FindScalarVariable(string name, TaskInputVariable[] taskInputVariables) {

    TaskInputVariable found = taskInputVariables.FirstOrDefault((TaskInputVariable input) => input.name.StartsWith(name));

    return found.GetScalarValue();
  }

  private VectorValue FindVectorVariable(string name, TaskInputVariable[] taskInputVariables) {

    TaskInputVariable found = taskInputVariables.FirstOrDefault((TaskInputVariable input) => input.name.StartsWith(name));

    return found.GetVectorValue();
  }
}
