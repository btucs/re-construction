#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Markdig.Renderers;

namespace Markdig.TextMeshPro.Extensions.TaskDialog {

  public class TaskDialogMarkdigFactory {

    private StringWriter writer = new StringWriter();
    private MarkdownPipeline pipeline;
    private TextMeshProRenderer textRenderer;

    public TaskDialogMarkdigFactory(ref TaskDialogOptions options) {

      MarkdownPipelineBuilder builder = new MarkdownPipelineBuilder();
      builder = builder.UseTaskDialog(ref options).UseSubSuperScript();

      pipeline = builder.Build();
      textRenderer = new TextMeshProRenderer(writer);
    }

    public string[] RenderStringToTextMeshPro(string[] texts) {

      string[] rendered = texts.Select((string text) => RenderStringToTextMeshPro(text)).ToArray();

      return rendered;
    }

    public string RenderStringToTextMeshPro(string text) {

      writer.GetStringBuilder().Clear();

      Markdown.Convert(text, textRenderer, pipeline);

      return writer.ToString();
    }
  }
}