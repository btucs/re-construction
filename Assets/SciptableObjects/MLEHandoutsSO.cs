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
using Sirenix.OdinInspector;

//[CreateAssetMenu(menuName = "Handouts")]
public class MLEHandoutsSO : ScriptableObject
{
  [LabelText("MLE Name")]
  [ValueDropdown("GetAllMLE")]
  public string mleName;
  //public MLEDataSO mleName;

  [LabelText("Einträge")]
  public MLEHandoutEntry[] entries;

  [Serializable]
  public class MLEHandoutEntry
  {
    [LabelText("Titel")]
    public string title;

    [MultiLineProperty(5)]
    [LabelText("Beschreibung")]
    public string description;

    [PreviewField]
    [LabelText("Bild")]
    public Sprite picture;

  }

  #if UNITY_EDITOR
  private static IEnumerable GetAllMLE()
  {
    string root = "Assets/DataObjects/MLE/";

    return UnityEditor.AssetDatabase.FindAssets("t:ScriptableObject")
      .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
      .Where(x => x.StartsWith(root))
      .Select(x => UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(x) as MLEDataSO)
      .Select((MLEDataSO x) => x.mleName)
    ;
  }
  #endif
}