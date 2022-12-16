#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using FullSerializer;

[CreateAssetMenu(menuName = "Character")]
public class CharacterSO : SerializedScriptableObject, UIDSearchableInterface
{
  [SerializeField, ReadOnly]
  private string uid;
  public string characterName;
  [PreviewField]
  public Sprite thumbnail;
  public CharacterType type;

  public Color hairColor;
  public Color bodyColor;

  public VoiceType voice = VoiceType.female;

  [NonSerialized, OdinSerialize]
  public Dictionary<string, string> spriteRenderDictionary = new Dictionary<string, string>();

  public string UID {
    get {
      return uid;
    }
  }

  private void OnValidate() {
#if UNITY_EDITOR
    if(uid == "" || uid == null) {
      uid = Guid.NewGuid().ToString();
      UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
  }
}

[Serializable]
public enum VoiceType {
  male, female
}