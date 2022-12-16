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

[Serializable]
public class PlayerDataSO : ScriptableObject {

  public string playerName;
  public PlayerCharacter avatar;

  public void SetPlayerName(string name) {

    playerName = name;
  }

  public void SetAvatar(PlayerCharacter avatar) {

    this.avatar = avatar;
  }
}
