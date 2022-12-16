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

public class PlayerRankSO : ScriptableObject {

  public List<PlayerRank> ranks = new List<PlayerRank>();

  public RankInfo GetRankInfo(int currentExp) {

    int rankIndex = GetRankIndex(currentExp);

    return new RankInfo {
      currentRank = ranks[rankIndex],
      nextRank = ranks.Count > rankIndex + 1 ? ranks[rankIndex + 1]: null,
    };
  }

  public string GetCurrentRankAsString(int currentExp)
  {
    return GetRankInfo(currentExp).currentRank.title;
  }

  public string GetRankTitleByIndex(int rankIndex)
  {
    return ranks[rankIndex].title;
  }

  public int GetCurrentRankAsInt(int currentExp)
  {
    return GetRankIndex(currentExp);
  }

  private int GetRankIndex(int currentExp)
  {
    // find the index of the rank that is larger than the exp and take the previous one
    int rankIndex = ranks.FindIndex((PlayerRank rank) => currentExp < rank.expRequirement);

    // -1 if no rank found that is larger
    if(rankIndex < 0) {

      rankIndex = ranks.Count - 1;
    } else {

      rankIndex = Math.Max(0, rankIndex - 1);
    }

    return rankIndex;
  }
}

public struct RankInfo {
  public PlayerRank currentRank;
  public PlayerRank nextRank;
}