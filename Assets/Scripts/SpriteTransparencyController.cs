#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpriteTransparencyController : MonoBehaviour {

  [Range(0, 1f)]
  public float opacity = 1f;
  public SpriteRenderer[] spriteRenderers;

  [Button]
  public void ApplyOpacity() {

    SetOpacity(opacity);
  }

  [Button]
  public void ResetOpacity() {

    SetOpacity(1);
  }

  private void SetOpacity(float opacity) {

    foreach(SpriteRenderer renderer in spriteRenderers) {

      Color currentColor = renderer.color;
      currentColor.a = opacity;
      renderer.color = currentColor;
    }
  }
}
