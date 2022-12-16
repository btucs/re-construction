#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum TaskTypeEnum {
  Calculation,
  DrawVector,
  DrawForce,
  DrawReplacementModel,
  DrawFreeCut,
  DrawEquilibrium,
  DrawLineVolatility,
  DrawInteraction,
}

public static class TaskTypeEnumHelper {

  public static Dictionary<TaskTypeEnum, string> TaskTypeEnumData = new Dictionary<TaskTypeEnum, string> {
    {TaskTypeEnum.Calculation, "Ergebins berechnen" },
    {TaskTypeEnum.DrawVector, "Vektor zeichnen" },
    {TaskTypeEnum.DrawForce, "Kraft zeichnen" },
    {TaskTypeEnum.DrawReplacementModel, "Ersatzmodell zeichnen" },
    {TaskTypeEnum.DrawFreeCut, "Freischnitt zeichnen" },
    {TaskTypeEnum.DrawEquilibrium, "Gleichgewicht zeichnen" },
    {TaskTypeEnum.DrawLineVolatility, "Linienflüchtigkeit zeichnen" },
    {TaskTypeEnum.DrawInteraction, "Wechselwirkung zeichnen" },
  };

  private static ValueDropdownList<TaskTypeEnum> valueDropdownList = new ValueDropdownList<TaskTypeEnum>();

  public static IEnumerable GetTaskTypeEnumTranslation() {

    if(valueDropdownList.Count > 0) {

      return valueDropdownList;
    }

    foreach(KeyValuePair<TaskTypeEnum, string> pair in TaskTypeEnumData) {

      valueDropdownList.Add(pair.Value, pair.Key);
    }

    return valueDropdownList;
  }
}
