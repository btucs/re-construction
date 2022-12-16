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
using UnityEngine.UI;
using TMPro;
using Markdig;
using Markdig.Syntax;
using Markdig.Renderers;
using Markdig.TextMeshPro;
using System.IO;

public class GlossaryEntryController : MonoBehaviour {

  public GlossaryEntrySO entry;
  public GlossaryType type;

  public Text headlineText;
  public TextMeshProUGUI content;
  public Image image;

  public void UpdateEntryContent() {

    GlossaryEntrySO.GlossaryEntry entryData = entry.GetContent(type);
    content.SetText(entryData.tmpContent);
    image.sprite = entryData.image;
  }

  public void SetContent(GlossaryEntrySO entry, GlossaryType type) {

    this.entry = entry;
    this.type = type;
    Start();
  }

  private void Start() {

    headlineText.text = entry.headline;
    UpdateEntryContent();
    
    gameObject.name = "GlossaryEntry - " + entry.headline;
  }
}
