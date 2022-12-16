#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using System;

public class CharacterDataConverter : fsDirectConverter<CharacterData> {

  private GameController controller;

  public CharacterDataConverter() {

    controller = GameController.GetInstance();
  }

  protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref CharacterData model) {

    fsResult result = fsResult.Success;

    // check for new savegame where mentor has a uid
    if(
      (result += CheckKey(data, "mentorUid", out fsData mentorUid)).Failed ||
      (result += CheckType(mentorUid, fsDataType.String)).Failed
    ) {

      // if failed check for older Savegame where mentor 
      result = fsResult.Success;
      if(
        (result += CheckKey(data, "mentorName", out fsData mentorName)).Failed ||
        (result += CheckType(mentorName, fsDataType.String)).Failed
      ) {

        return result;
      }

      mentorUid = mentorName;
    }


    model.mentor = controller.gameAssets.FindCharacter(mentorUid.AsString);
    if(model.mentor == null) {

      return result += fsResult.Fail("Mentor with UID " + mentorUid.AsString + " not found");
    }

    result += DeserializeMember<CharacterSO>(data, typeof(PlayerConverter), "playerData", out model.player);

    return result;
  }

  protected override fsResult DoSerialize(CharacterData model, Dictionary<string, fsData> serialized) {

    if(model.player == null) {

      model.player = controller.gameAssets.FindCharacter("DefaultPlayer");
    }

    SerializeMember<CharacterSO>(serialized, typeof(PlayerConverter), "playerData", model.player);
    SerializeMember<string>(serialized, null, "mentorUid", model.mentor?.UID);

    return fsResult.Success;
  }
}
