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

public enum AssessmentType {
  None,
  ValuesIdentified,
  Converter,
  Result,
  DrawVector,
  DrawForce,
  DrawEquilibrium,
  DrawLineVolatility,
  DrawInteraction,
}

public static class AssessmentFactory {

  public static AssessmentAbstract Get(AssessmentType type, KonstruktorSceneData data, VariableInfoSO info) {

    switch(type) {

      case AssessmentType.ValuesIdentified:
        return new ValuesIdentifiedAssessment(data, info);

      case AssessmentType.Converter:
        return new ConverterAssessment(data, info);

      case AssessmentType.Result:
        return new ResultAssessment(data, info);
      
      case AssessmentType.DrawVector:
        return new DrawVectorAssessment(data, info);

      case AssessmentType.DrawForce:
        return new DrawForceAssessment(data, info);

      case AssessmentType.DrawEquilibrium:
        return new DrawEquilibriumAssessment(data, info);

      case AssessmentType.DrawLineVolatility:
        return new DrawLineVolatilityAssessment(data, info);

      case AssessmentType.DrawInteraction:
        return new DrawInteractionAssessment(data, info);

      default:
        return null;
    }
  }

  public static AssessmentType[] GetResponsibleAssessmentTypes() {

    AssessmentType[] always = { AssessmentType.ValuesIdentified, AssessmentType.Converter };

    return always;
  }

  public static AssessmentType[] GetResponsibleAssessmentTypes(TaskOutputVariableUnit targetUnit) {

    AssessmentType[] always = { AssessmentType.ValuesIdentified, AssessmentType.Converter };
    AssessmentType[] drawVector = { AssessmentType.DrawVector };
    AssessmentType[] drawForce = { AssessmentType.DrawForce };
    AssessmentType[] drawEquilibrium = { AssessmentType.DrawEquilibrium };
    AssessmentType[] drawLineVolatility = { AssessmentType.DrawLineVolatility };
    AssessmentType[] drawInteraction = { AssessmentType.DrawInteraction };
    AssessmentType[] calc = { AssessmentType.Result };

    List<AssessmentType> types = new List<AssessmentType>(always);

    switch(targetUnit) {
      case TaskOutputVariableUnit.Force:
      case TaskOutputVariableUnit.Hypotenuse:
      case TaskOutputVariableUnit.SeiteL:
      case TaskOutputVariableUnit.WeightForce:
        types.AddRange(calc);
        break;

      case TaskOutputVariableUnit.Vector:
        types.AddRange(drawVector);
        break;

      case TaskOutputVariableUnit.ForceVector:
        types.AddRange(drawForce);
        break;

      case TaskOutputVariableUnit.ReplacementModel:
        types.AddRange(calc);
        break;

      case TaskOutputVariableUnit.ForceVectorGG:
        types.AddRange(drawEquilibrium);
        break;

      case TaskOutputVariableUnit.ForceVectorFL:
        types.AddRange(drawLineVolatility);
        break;

      case TaskOutputVariableUnit.ForceVectorWW:
        types.AddRange(drawInteraction);
        break;

      default:
        throw new Exception("Not handled TaskOutputVariable " + targetUnit);
    }

    return types.ToArray();
  }
}
