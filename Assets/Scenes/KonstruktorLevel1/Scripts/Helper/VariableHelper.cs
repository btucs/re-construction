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

public enum UnitDataResultType {
  Scalar,
  Vector,
  Other,
};

public struct UnitData {

  public string translation;
  public UnitDataResultType resultType;

  public UnitData(string translation, UnitDataResultType resultType) {

    this.translation = translation;
    this.resultType = resultType;
  }
}

public static class VariableHelper {

  public static Dictionary<TaskOutputVariableUnit, UnitData> VariableUnitData = new Dictionary<TaskOutputVariableUnit, UnitData>() {
    { TaskOutputVariableUnit.Force, new UnitData("Kraft", UnitDataResultType.Scalar) },
    { TaskOutputVariableUnit.WeightForce, new UnitData("Gewichtskraft", UnitDataResultType.Scalar) },
    { TaskOutputVariableUnit.ForceVector, new UnitData("Kraftpfeil angeben", UnitDataResultType.Vector)},
    { TaskOutputVariableUnit.Hypotenuse, new UnitData("Hypotenuse", UnitDataResultType.Scalar) },
    { TaskOutputVariableUnit.SeiteL, new UnitData("Seite l", UnitDataResultType.Scalar) },
    { TaskOutputVariableUnit.Vector, new UnitData("Vektor", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.ForceVectorGG, new UnitData("Kraftpfeil Geleichgewicht", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.ForceVectorWW, new UnitData("Kraftpfeil Wechselwirkung", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.ForceVectorFL, new UnitData("Kraftpfeile Linienfl.", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.ForceComponentXA, new UnitData("Kraftkomponente in X analytisch", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.ForceComponentXG, new UnitData("Kraftkomponente in X grafisch", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.ForceComponentYA, new UnitData("Kraftkomponente in Y analytisch", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.ForceComponentYG, new UnitData("Kraftkomponente in Y grafisch", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.SubstituteForceA, new UnitData("Ersatzkraft analytisch", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.SubstituteForceG, new UnitData("Ersatzkraft grafisch", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.SubstituteForceAngle, new UnitData("Winkel der Ersatzkraft", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.ResultingForceA, new UnitData("Resultierende analytisch", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.ResultingForceG, new UnitData("Resultierende grafisch", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.ResultingForceAngle, new UnitData("Winkel der Resultierenden", UnitDataResultType.Vector) },
    { TaskOutputVariableUnit.ReplacementModel, new UnitData("Ersatzmodell", UnitDataResultType.Other) }
  };

  private static ValueDropdownList<TaskOutputVariableUnit> valueDropdownList = new ValueDropdownList<TaskOutputVariableUnit>();

  public static IEnumerable GetTaskOutVariableUnitTranslation() {

    if(valueDropdownList.Count > 0) {

      return valueDropdownList;
    }

    foreach(KeyValuePair<TaskOutputVariableUnit, UnitData> pair in VariableUnitData) {

      valueDropdownList.Add(pair.Value.translation, pair.Key);
    }

    return valueDropdownList;
  }

  public static string Translate(TaskOutputVariableUnit unit) {

    if(VariableHelper.VariableUnitData.TryGetValue(unit, out UnitData unitData)) {

      return unitData.translation;
    }

    return "not found";
  }

  public static UnitDataResultType GetUnitResultType(TaskOutputVariableUnit unit) {

    VariableHelper.VariableUnitData.TryGetValue(unit, out UnitData unitData);

    return unitData.resultType;
  }

  public static IEnumerable GetReplacementModelTypeTranslation() {

    return new ValueDropdownList<ReplacementModelType>() {
      { "keiner", ReplacementModelType.None },
      { "Balken", ReplacementModelType.Beam },
      { "Stab", ReplacementModelType.Rod },
      { "Seil", ReplacementModelType.Rope },
      { "Masse", ReplacementModelType.Mass },
    };
  }
}
