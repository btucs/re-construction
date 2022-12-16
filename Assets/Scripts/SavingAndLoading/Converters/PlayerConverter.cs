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

public class PlayerConverter : fsDirectConverter<CharacterSO> {
  
  public override object CreateInstance(fsData data, Type storageType) {

    return ScriptableObject.CreateInstance<CharacterSO>();
  }

  protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref CharacterSO model) {

    fsResult result = new fsResult();

    result += DeserializeMember<string>(data, null, "characterName", out model.characterName);
    result += DeserializeMember<CharacterType>(data, null, "type", out model.type);
    //result += DeserializeMember<Sprite>(data, null, "thumbnail", out model.thumbnail);
    result += DeserializeMember<Dictionary<string, string>>(data, null, "spriteRenderDictionary", out model.spriteRenderDictionary);
    result += DeserializeMember<Color>(data, null, "bodyColor", out model.bodyColor);
    result += DeserializeMember<Color>(data, null, "hairColor", out model.hairColor);

    string playerThumbnailPath = Application.persistentDataPath + "/images/player.png";
    Sprite playerSprite = LoadSprite(playerThumbnailPath);

    if(playerSprite != null) {

      model.thumbnail = playerSprite;
    }

    return result;
  }

  protected override fsResult DoSerialize(CharacterSO model, Dictionary<string, fsData> serialized) {

    SerializeMember<string>(serialized, null, "characterName", model.characterName);
    SerializeMember<CharacterType>(serialized, null, "type", model.type);
    //SerializeMember<Sprite>(serialized, null, "thumbnail", model.thumbnail);
    SerializeMember<Dictionary<string, string>>(serialized, null, "spriteRenderDictionary", model.spriteRenderDictionary);
    SerializeMember<Color>(serialized, null, "bodyColor", model.bodyColor);
    SerializeMember<Color>(serialized, null, "hairColor", model.hairColor);
   
    return fsResult.Success;
  }

  private Sprite LoadSprite(string path) {
    if(string.IsNullOrEmpty(path)) {

      return null;
    }

    if(System.IO.File.Exists(path)) {
      byte[] bytes = System.IO.File.ReadAllBytes(path);
      Texture2D texture = new Texture2D(1, 1);
      texture.LoadImage(bytes);
      Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

      return sprite;
    }

    return null;
  }
}
