#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Markdig;
using Markdig.Syntax;
using Markdig.Renderers;
using Markdig.TextMeshPro;
using System.IO;

public class TextMeshProRendererFactory
{

  private StringWriter writer = new StringWriter();
  private MarkdownPipeline pipeline;
  private TextMeshProRenderer textRenderer;

  public TextMeshProRendererFactory() {

    MarkdownPipelineBuilder builder = new MarkdownPipelineBuilder().UseSubSuperScript();
    pipeline = builder.Build();
    textRenderer = new TextMeshProRenderer(writer);
    textRenderer.OverrideBulletType('•');
  }

  public string RenderMarkdownStringToTextMeshProString(string text) {

    writer.GetStringBuilder().Clear();
    Markdown.Convert(text, textRenderer, pipeline);

    writer.Flush();

    return writer.ToString();
  }
}
