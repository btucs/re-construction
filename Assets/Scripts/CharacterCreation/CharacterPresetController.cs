#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPresetController : MonoBehaviour
{
  public characterGraphicsUpdater characterPresetObj;
	public characterGraphicsUpdater targetCharacter;
  public CharacterType type;

  void Start()
  {
  }
    
  public void SetTargetCharacter()
  {
    CharacterSO character = characterPresetObj.characterSOData.Value;

    if(character.type == CharacterType.Player) {

      targetCharacter.characterSOData.Value = MergeWithPlayer(
       targetCharacter.characterSOData.Value,
       character
      );
    } else {

      targetCharacter.characterSOData.Value = character;
    }
  }

  /**
   * Copy values from another character, excluding name and type
   */
  private CharacterSO MergeWithPlayer(CharacterSO a, CharacterSO b) {

    CharacterSO tmp = ScriptableObject.CreateInstance<CharacterSO>();

    foreach(KeyValuePair<string, string> pair in b.spriteRenderDictionary) {

      tmp.spriteRenderDictionary.Add(pair.Key, pair.Value);
    }

    tmp.hairColor = b.hairColor;
    tmp.bodyColor = b.bodyColor;

    tmp.characterName = a.characterName;
    tmp.type = a.type;
    
    return tmp;
  }
}
