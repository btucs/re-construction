#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Task {

  private string[] description;
  private string[] successMessage;
  
  private List<PrerequisiteAbstract> prerequisites = new List<PrerequisiteAbstract>();
  private List<TaskAnalyzerAbstract> analyzers = new List<TaskAnalyzerAbstract>();
  private List<ImpactAbstract> successImpacts = new List<ImpactAbstract>();
  private List<ImpactAbstract> failureImpacts = new List<ImpactAbstract>();
  private KonstruktorData konstruktorData;

  public void SetDescription(string[] description) {

    this.description = description;
  }

  public string[] GetDescription() {

    return description;
  }

  public void SetSuccessMessage(string[] message) {

    successMessage = message;
  }

  public string[] GetSuccessMessage() {

    return successMessage;
  }

  public void AddPrerequisite(PrerequisiteAbstract prerequisite) {

    prerequisites.Add(prerequisite);
  }

  public void AddTaskAnalyzer(TaskAnalyzerAbstract analyzer) {

    analyzer.SetTask(this);
    analyzers.Add(analyzer);
  }

  public void AddSuccessImpact(ImpactAbstract impact) {

    successImpacts.Add(impact);
  }

  public void AddFailureImpact(ImpactAbstract impact) {

    failureImpacts.Add(impact);
  }

  public T[] GetPrerequisites<T>() {

    return prerequisites.OfType<T>().ToArray();
  }

  public TaskAnalyzerResultAbstract Validate() {

    foreach (TaskAnalyzerAbstract analyzer in analyzers) {

      TaskAnalyzerResultAbstract result = analyzer.GetResult();
      if (typeof(NegativeTaskAnalyzerResult) == result.GetType()) {

        return result;
      }
    }

    return new PositiveTaskAnalyzerResult(successMessage);
  }

  public void SetKonstruktorData(KonstruktorData data) {

    konstruktorData = data;
  }

  public KonstruktorData GetKonstruktorData() {

    return konstruktorData;
  }
}
