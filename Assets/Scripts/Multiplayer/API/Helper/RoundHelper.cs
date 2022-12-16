#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;
using Assets.Scripts.Multiplayer.API.Models;

public static class RoundHelper
{
    public static int[] CalculatePoints(IList<QuizRound> rounds, int Multiplikator = 1)
    {
        int maxPoints = 0;
        int p1Points = 0;
        int p2Points = 0;
        for (var i = 0; i < rounds.Count; i++)
        {
            p1Points += rounds[i].S1Points * Multiplikator;
            p2Points += rounds[i].S2Points * Multiplikator;
            maxPoints += 3;
        }

        return new[]{p1Points, p2Points, maxPoints};


    }
}

