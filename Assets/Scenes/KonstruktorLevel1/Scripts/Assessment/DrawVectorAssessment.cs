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

public class DrawVectorAssessment : ResultAssessment {
  private KonstruktorSceneData data;
  private VariableInfoSO variableInfo;
  private CalculatorFactory factory;

  private FeedbackType currentFeedback = FeedbackType.None;

  private TaskDataSO.SolutionStep currentStep;
  private ConverterResultData currentResultData;

  public DrawVectorAssessment(KonstruktorSceneData data, VariableInfoSO info) : base(data, info) {

    this.data = data;
    variableInfo = info;
    factory = new CalculatorFactory();
  }

  public override int MaxPoints => 3;
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
    if(currentResultData.calculatorType != CalculatorEnum.VectorCalculator) {

      currentFeedback = FeedbackType.DrawVectorNoVector;

      return 0;
    }

    bool pointsCorrect = false;
    if(currentResultData.calculatorParams != null) {

      pointsCorrect = currentResultData.calculatorParams.All((KeyValuePair<string, MathMagnitude> param) => {

        TaskInputVariable paramValue = param.Value.Value as TaskInputVariable;

        return currentStep.inputs.Any((TaskInputVariable variable) => variable.GetVectorValue() == paramValue.GetVectorValue());
      });
    }


    bool vectorCorrect = false;
    try {

      TaskOutputVariableUnit unit = currentStep.output.unit;
      VariableInfoSO.VariableInfoEntry info = variableInfo.GetInfoFor(unit);
      vectorCorrect = HandleVectorCalculator(info, currentResultData, currentStep);
    } catch(Exception) {

      // do nothing
    }

    if(pointsCorrect == true && vectorCorrect == true) {

      currentFeedback = FeedbackType.DrawVectorCorrect;

      return 3;
    }

    if(pointsCorrect == true && vectorCorrect == false) {

      currentFeedback = FeedbackType.DrawVectorDirectionIncorrect;

      return 2;
    }

    if(pointsCorrect == false && vectorCorrect == true) {

      currentFeedback = FeedbackType.DrawVectorVectorCorrectPointsIncorrect;

      return 1;
    }

    currentFeedback = FeedbackType.DrawVectorPointsIncorrect;

    return 0;
  }
}
