#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universit�t Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;

[System.Serializable]
[fsObject(Converter = typeof(CharacterDataConverter))]
public class CharacterData
{
  public CharacterSO player;
  public CharacterSO mentor;
}
