#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
public abstract class AssessmentAbstract {

  public abstract int MaxPoints {
    get;
  }

  public abstract int CurrentPoints {
    get;
  }

  public abstract int CalculatePoints();
  public abstract FeedbackType GetFeedback();
  public abstract void Configure(TaskDataSO.SolutionStep step, ConverterResultData result);
}
