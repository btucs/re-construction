#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "HandleTypes/Simple")]
public class HandleTypeSO : ScriptableObject {

  public Sprite icon;
  public Color color;

  public virtual void SetSpriteOnHandle(GameObject handle) {

    handle.SetActive(true);
    Transform sprite = handle.transform.GetChild(0);
    SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
    spriteRenderer.sprite = icon;
    spriteRenderer.color = color;
  }
}
