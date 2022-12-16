#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;

[Serializable]
public static class MaxExpHelper {

  public static int GetMaxExp(VariableInfoSO info, TaskDataSO taskData) {

    TaskDataSO.SolutionStep[] steps = taskData.steps;

    int sum = steps.Sum((TaskDataSO.SolutionStep step) => {

      TaskOutputVariableUnit expectedUnit = step.output.unit;
      AssessmentType[] responsibleTypes = AssessmentFactory.GetResponsibleAssessmentTypes(expectedUnit);

      int maxExp = responsibleTypes.Sum((AssessmentType assessmentType) => {

        if(assessmentType == AssessmentType.None) {

          return 0;
        }

        // create fake KonstruktorSceneData to get max Exp from Assessment
        KonstruktorSceneData tmp = new KonstruktorSceneData() {
          taskData = taskData
        };

        AssessmentAbstract assessment = AssessmentFactory.Get(assessmentType, tmp, info);
        if(assessment == null) {

          return 0;
        }

        return assessment.MaxPoints;
      });

      return maxExp;
    });

    return sum;
  }  
}