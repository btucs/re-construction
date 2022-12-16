#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;

[Serializable]
public class PlayerRank
{
  public string title;
  public int expRequirement;

  public PlayerRank(string title = "no title", int reqExp = 0) {
    this.title = title;
    this.expRequirement = reqExp;
  }
}