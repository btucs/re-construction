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

public class AssessmentFeedbackSO : ScriptableObject {

  [ListDrawerSettings(Expanded = true, IsReadOnly = true)]
  public FeedbackEntry[] entries;

  public FeedbackEntry GetEntryFor(FeedbackType type) {

    return entries.FirstOrDefault((FeedbackEntry entry) => entry.feedbackType == type);
  }

  [Button]
  private void Populate() {

    FeedbackType[] values = ((FeedbackType[])Enum.GetValues(typeof(FeedbackType)))
      .Where((FeedbackType value) => value != FeedbackType.None)
      .ToArray();

    List<FeedbackEntry> tmp = new List<FeedbackEntry>();

    foreach(FeedbackType value in values) {

      FeedbackEntry found = GetEntryFor(value);
      if(found != null) {

        tmp.Add(found);

        if(found.feedback.Length == 0) {

          found.feedback = new string[] { FeedbackTypeHelper.Translate(value) };
        }
      } else {

        FeedbackEntry newEntry = new FeedbackEntry() {
          feedbackType = value,
          feedback = new string[] { FeedbackTypeHelper.Translate(value) }
        };

        tmp.Add(newEntry);
      }
    }

    entries = tmp
      .OrderBy((FeedbackEntry entry) => FeedbackTypeHelper.Translate(entry.feedbackType))
      .ToArray()
    ;
  }

  [Serializable]
  public class FeedbackEntry {

    [HideInInspector]
    public FeedbackType feedbackType;

    [Title("@FeedbackTypeHelper.Translate(this.feedbackType)")]
    [LabelText("Feedback")]
    [MultiLineProperty(5)]
    public string[] feedback;
  }
}

public enum FeedbackType {
  ValuesIdentifiedCorrect,
  ValuesIdentifiedTooMany,
  ValuesIdentifiedWrongValues,

  ModuleCorrect,
  ModuleIncorrect,

  DrawVectorNoVector,
  DrawVectorCorrect,
  DrawVectorDirectionIncorrect, // wrong direction
  DrawVectorVectorCorrectPointsIncorrect,
  DrawVectorPointsIncorrect,

  DrawForceNoForce,
  DrawForceCorrect, // 111
  DrawForceIncorrect, // wrong value, 000
  DrawForcePointCorrectAngleIncorrect, // 110
  DrawForcePointIncorrectAngleCorrect, // 101
  DrawForceIncorrectPointIncorrectAngleCorrect, // 001
  DrawForceIncorrectPointCorrectAngleIncorrect, // 010
  DrawForcePointIncorrectAngleIncorrect, // 100
  DrawForceIncorrectPointCorrectAngleCorrect, // 011
  DrawForceDirectionWrong, // 111, wrong direction,

  CalculateResultCorrect,
  CalculateResultIncorrect,

  None, // don't use, default value

  ModuleMissing,

  NoResult,

  DrawEquilibriumNoForce,
  DrawEquilibriumNotOppositeOnActionLine, // 01 falsche Richtung, aber auf Wirkungslinie
  DrawEquilibriumOppositeNotOnActionLine, // 10 richtige Richtung , aber nicht auf der Wirkungslinie
  DrawEquilibriumNotOppositeNotOnActionLine, // 00 falsche Richtung, nicht auf Wirkungslinie
  DrawEquilibriumOppositeOnActionLine, // 11 richtige Richtung, auf Wirkungslinie

  DrawLineVolatilityNoForce,
  DrawLineVolatilityPositionCorrect,
  DrawLineVolatilityPositionIncorrect,

  DrawInteractionNoForce,
  DrawInteractionNotOppositeOnActionLine, // 01 falsche Richtung, aber auf Wirkungslinie
  DrawInteractionOppositeNotOnActionLine, // 10 richtige Richtung , aber nicht auf der Wirkungslinie
  DrawInteractionNotOppositeNotOnActionLine, // 00 falsche Richtung, nicht auf Wirkungslinie
  DrawInteractionOppositeOnActionLine,
}

