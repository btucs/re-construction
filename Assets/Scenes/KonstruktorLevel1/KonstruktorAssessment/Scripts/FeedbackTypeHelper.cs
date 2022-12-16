#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FeedbackTypeHelper {

  public static Dictionary<FeedbackType, string> FeedbackTypeData = new Dictionary<FeedbackType, string>() {
    { FeedbackType.CalculateResultCorrect, "Berechnung richtig"},
    { FeedbackType.CalculateResultIncorrect, "Berechnung falsch"},

    { FeedbackType.DrawForceNoForce, "keine Kraft gez."},
    { FeedbackType.DrawForceCorrect, "Kraft richtig gez., Punkt richtig, Winkel richtig"},
    { FeedbackType.DrawForceDirectionWrong, "Kraft Vektor falsche Richtung" },
    { FeedbackType.DrawForceIncorrect, "Kraft falsch gez., Punkt falsch, Winkel falsch"},
    { FeedbackType.DrawForcePointCorrectAngleIncorrect, "Kraft richtig gez., Punkt richtig, Winkel falsch"},
    { FeedbackType.DrawForcePointIncorrectAngleCorrect, "Kraft richtig gez., Punkt falsch, Winkel richtig"},
    { FeedbackType.DrawForcePointIncorrectAngleIncorrect, "Kraft richtig gez., Punkt falsch, Winkel falsch"},
    { FeedbackType.DrawForceIncorrectPointCorrectAngleIncorrect, "Kraft falsch gez., Punkt richtig, Winkel falsch" },
    { FeedbackType.DrawForceIncorrectPointIncorrectAngleCorrect, "Kraft falsch gez., Punkt falsch, Winkel richtig" },
    { FeedbackType.DrawForceIncorrectPointCorrectAngleCorrect, "Kraft falsch gez, Punkt richtig, Winkel richtig" },

    { FeedbackType.DrawVectorNoVector, "Kein Vektor gez" },
    { FeedbackType.DrawVectorCorrect, "Vektor richtig gez."},
    { FeedbackType.DrawVectorVectorCorrectPointsIncorrect, "Vektor richtig, aber Punkte falsch" },
    { FeedbackType.DrawVectorPointsIncorrect, "Vektor falsch gez."},
    { FeedbackType.DrawVectorDirectionIncorrect, "Vektor falsche Richtung" },

    { FeedbackType.DrawEquilibriumNoForce, "Gleichgewicht, keine Kraft gezeichnet" },
    { FeedbackType.DrawEquilibriumNotOppositeOnActionLine, "Gleichgewicht, falsche Richtung, aber auf Wirkungslinie" }, // 01 falsche Richtung, aber auf Wirkungslinie
    { FeedbackType.DrawEquilibriumOppositeNotOnActionLine, "Gleichgewicht, richtige Richtung, aber nicht auf Wirkungslinie" }, // 10 richtige Richtung , aber nicht auf der Wirkungslinie
    { FeedbackType.DrawEquilibriumNotOppositeNotOnActionLine, "Gleichgewicht, falsche Richtung und nicht auf Wirkungslinie" }, // 00 falsche Richtung, nicht auf Wirkungslinie
    { FeedbackType.DrawEquilibriumOppositeOnActionLine, "Gleichgewicht, richtige Richtung und auf Wirkungslinie" },

    { FeedbackType.DrawInteractionNoForce, "Wechselwirkung, keine Kraft gezeichnet" },
    { FeedbackType.DrawInteractionNotOppositeOnActionLine, "Wechselwirkung, falsche Richtung, aber auf Wirkungslinie" }, // 01 falsche Richtung, aber auf Wirkungslinie
    { FeedbackType.DrawInteractionOppositeNotOnActionLine, "Wechselwirkung, richtige Richtung, aber nicht auf Wirkungslinie" }, // 10 richtige Richtung , aber nicht auf der Wirkungslinie
    { FeedbackType.DrawInteractionNotOppositeNotOnActionLine, "Wechselwirkung, falsche Richtung und nicht auf Wirkungslinie" }, // 00 falsche Richtung, nicht auf Wirkungslinie
    { FeedbackType.DrawInteractionOppositeOnActionLine, "Wechselwirkung, richtige Richtung und auf Wirkungslinie" },

    { FeedbackType.DrawLineVolatilityNoForce, "Linienflüchtigkeit, keine Kraft gezeichnet"},
    { FeedbackType.DrawLineVolatilityPositionCorrect, "Linienflüchtigkeit, Angriffspunkt richtig" },
    { FeedbackType.DrawLineVolatilityPositionIncorrect, "Linenflüchtigkeit, Angriffspunkt falsch" },

    { FeedbackType.ModuleCorrect, "Modul richtig"},
    { FeedbackType.ModuleIncorrect, "Modul falsch"},
    { FeedbackType.ModuleMissing, "Kein Modul ausgewählt" },

    { FeedbackType.ValuesIdentifiedCorrect, "Werte richtig identifiziert"},
    { FeedbackType.ValuesIdentifiedTooMany, "Werte zuviel"},
    { FeedbackType.ValuesIdentifiedWrongValues, "Werte falsche Werte"},

    { FeedbackType.NoResult, "Kein Ergebnis zugeordnet" }
  };

  public static string Translate(FeedbackType type) {

    if(FeedbackTypeData.TryGetValue(type, out string translation)) {

      return translation;
    }

    return "not found";
  }
}