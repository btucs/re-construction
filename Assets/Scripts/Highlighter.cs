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
using Sirenix.OdinInspector;

public class Highlighter : MonoBehaviour {

  private Color originalColor;
  private bool isHightlighted = false;

  [ColorPalette("Learn&Play")]
  public Color highlightColor; 

  private new Renderer renderer;
  private Type rendererType;

  private void Start() {

    renderer = gameObject.GetComponent(typeof(Renderer)) as Renderer;
    rendererType = renderer.GetType();

    if (rendererType == typeof(SpriteRenderer)) {

      SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
      originalColor = spriteRenderer.color;
    }
  }

  public void Highlight() {

    if (rendererType == typeof(SpriteRenderer)) {

      SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
      spriteRenderer.color = highlightColor;
      isHightlighted = true;

      return;
    }
  }

  public void UnHighlight() {

    if (rendererType == typeof(SpriteRenderer) && isHightlighted == true) {

      (renderer as SpriteRenderer).color = originalColor;

      return;
    }
  }
}
