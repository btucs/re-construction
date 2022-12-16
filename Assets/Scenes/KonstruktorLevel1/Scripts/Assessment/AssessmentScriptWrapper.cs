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
using Sirenix.OdinInspector;

public class AssessmentScriptWrapper : MonoBehaviour {

  public AssessmentType assessmentType;

  public int MaxPoints => assessment.MaxPoints;
  public int CurrentPoints => assessment.CurrentPoints;
  public int CalculatePoints() {

    return assessment.CalculatePoints();
  }
  public void Configure(TaskDataSO.SolutionStep step, ConverterResultData resultData) {

    if(assessment == null) {

      Start();
    }

    assessment.Configure(step, resultData);
  }

  public string GetFeedback() {

    FeedbackType feedbackType = assessment.GetFeedback();

    AssessmentFeedbackSO.FeedbackEntry feedbackEntry = feedbackSO.GetEntryFor(feedbackType);
    if(feedbackEntry != null) {

      string[] feedbackOptions = feedbackEntry.feedback;

      switch(feedbackOptions.Length) {

        case 0: throw new Exception("no feedback found for " + feedbackType);
        case 1: return feedbackOptions[0];
        default:
          return feedbackOptions[rng.Range(1, feedbackOptions.Length) - 1];
      }
    }

    throw new Exception("no feedback found for " + feedbackType);
  }
  
  private KonstruktorSceneData data;
  private GameController controller;
  private VariableInfoSO variableInfo;
  private RandomNumberGenerator rng;
  private AssessmentFeedbackSO feedbackSO;

  private AssessmentAbstract assessment;

  private void Start() {

    if(assessment == null) {

      controller = GameController.GetInstance();

      rng = controller.gameState.profileData.rng;
      data = controller.gameState.konstruktorSceneData;
      variableInfo = controller.gameAssets.variableInfo;
      feedbackSO = controller.gameAssets.feedbackInfo;
      assessment = AssessmentFactory.Get(assessmentType, data, variableInfo);
    }
  }
}
