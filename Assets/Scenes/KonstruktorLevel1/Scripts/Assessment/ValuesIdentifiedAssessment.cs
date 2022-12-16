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
using MathUnits.Physics.Values;

public class ValuesIdentifiedAssessment : AssessmentAbstract {

  private KonstruktorSceneData data;
  private VariableInfoSO variableInfo;
  private FeedbackType currentFeedback = FeedbackType.None;

  private TaskDataSO.SolutionStep currentStep;
  private ConverterResultData currentResultData;

  public ValuesIdentifiedAssessment(KonstruktorSceneData data, VariableInfoSO info) {

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

    MathMagnitude[] inputVariables = data.inputs;
    MathMagnitude[] outputVariables = data.outputs;

    int identifiedInputCount = CheckInputs(currentStep.inputs, inputVariables);
    int identifiedOutputCount = CheckOutputs(new TaskOutputVariable[] { currentStep.output } , outputVariables);

    int totalCount = identifiedInputCount + identifiedOutputCount;
    int totalExpectedCount = 1 /*output variables*/ + currentStep.inputs.Length;

    int diff = totalExpectedCount - totalCount;

    switch(diff) {

      case 0:
        currentFeedback = FeedbackType.ValuesIdentifiedCorrect;

        return 1;

      default:
        currentFeedback = FeedbackType.ValuesIdentifiedWrongValues;

        return 0;
    }
  }

  private int CheckInputs(TaskInputVariable[] taskInputs, MathMagnitude[] identifiedInputs) {

    HashSet<TaskInputVariable> identified = identifiedInputs.Aggregate(
      new HashSet<TaskInputVariable>(),
      (HashSet<TaskInputVariable> agg, MathMagnitude identifiedInput) => {

        agg.Add((TaskInputVariable)identifiedInput.Value);

        return agg;
      }
    );

    int identifiedInputCount = taskInputs.Count((TaskInputVariable taskInput) => {

      bool contains = identified.Contains(taskInput);

      return contains;
    });

    return identifiedInputCount;
  }

  private int CheckOutputs(TaskOutputVariable[] taskOutputs, MathMagnitude[] identifiedOutputs) {

    HashSet<TaskOutputVariable> identified = identifiedOutputs.Aggregate(
      new HashSet<TaskOutputVariable>(),
      (HashSet<TaskOutputVariable> agg, MathMagnitude identifiedOutput) => {

        agg.Add((TaskOutputVariable)identifiedOutput.Value);

        return agg;
      }
    );

    int identifiedOutputCount= taskOutputs.Count((TaskOutputVariable taskOutput) => identified.Contains(taskOutput));

    return identifiedOutputCount;
  }
}
