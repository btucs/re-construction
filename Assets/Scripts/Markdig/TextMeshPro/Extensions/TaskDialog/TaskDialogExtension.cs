#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using Markdig.Renderers;
using UnityEngine;

namespace Markdig.TextMeshPro.Extensions.TaskDialog {

  public class TaskDialogExtension : IMarkdownExtension {

    private TaskDialogOptions options;

    public TaskDialogExtension(ref TaskDialogOptions options) {

      this.options = options;
    }

    public void Setup(MarkdownPipelineBuilder pipeline) {

      if(pipeline.InlineParsers.Contains<TaskInputVariableParser>() == false) {

        pipeline.InlineParsers.Insert(0, new TaskInputVariableParser());
      }

      if(pipeline.InlineParsers.Contains<TaskOutputVariableParser>() == false) {

        pipeline.InlineParsers.Insert(0, new TaskOutputVariableParser());
      }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) {

      if(renderer is TextMeshProRenderer tmpRenderer) {

        if(tmpRenderer.ObjectRenderers.Contains<TaskInputVariableRenderer>() == false) {

          tmpRenderer.ObjectRenderers.Insert(0, new TaskInputVariableRenderer(ref options));
        }

        if(tmpRenderer.ObjectRenderers.Contains<TaskOutputVariableRenderer>() == false) {

          tmpRenderer.ObjectRenderers.Insert(0, new TaskOutputVariableRenderer(ref options));
        }
      }
    }
  }
}
