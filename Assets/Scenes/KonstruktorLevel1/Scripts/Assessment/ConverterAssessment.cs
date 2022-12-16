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

public class ConverterAssessment : AssessmentAbstract {

  private KonstruktorSceneData data;
  private VariableInfoSO variableInfo;
  private FeedbackType currentFeedback = FeedbackType.None;

  TaskDataSO.SolutionStep currentStep;
  ConverterResultData currentResultData;

  public ConverterAssessment(KonstruktorSceneData data, VariableInfoSO info) {

    this.data = data;
    variableInfo = info;
  }

  public override int MaxPoints => 1;
  public override int CurrentPoints => CalculatePoints();

  public override void Configure(TaskDataSO.SolutionStep step, ConverterResultData result) {

    currentStep = step;
    currentResultData = result;
  }

  public override FeedbackType GetFeedback() {

    return currentFeedback;
  }

  public override int CalculatePoints() {

    if(currentResultData == null) {

      currentFeedback = FeedbackType.ModuleMissing;

      return 0;
    }

    VariableInfoSO.VariableInfoEntry info = variableInfo.GetInfoFor(currentStep.output.unit);

    bool isCorrect = currentResultData.calculatorType == info.calculator;

    if(isCorrect) {

      currentFeedback = FeedbackType.ModuleCorrect;

      return 1;
    }

    currentFeedback = FeedbackType.ModuleIncorrect;

    return 0;    
  }
}
