#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion

// file Touchable.cs
// Correctly backfills the missing Touchable concept in Unity.UI's OO chain.
/** @link https://stackoverflow.com/a/36892803/1244727 */
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(Touchable))]
public class Touchable_Editor: Editor {
  public override void OnInspectorGUI() {
  }
}
#endif
public class Touchable: Text {
  protected override void Awake() {
    base.Awake();
  }
}