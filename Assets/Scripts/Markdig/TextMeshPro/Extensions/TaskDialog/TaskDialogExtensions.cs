#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using Markdig.Helpers;
using UnityEngine;

namespace Markdig.TextMeshPro.Extensions.TaskDialog {

  public static class TaskDialogExtensions {

    public static MarkdownPipelineBuilder UseTaskDialog(this MarkdownPipelineBuilder pipeline, ref TaskDialogOptions options) {

      OrderedList<IMarkdownExtension> extensions;

      extensions = pipeline.Extensions;

      if(!extensions.Contains<TaskDialogExtension>()) {
        extensions.Add(new TaskDialogExtension(ref options));
      }

      return pipeline;
    }
  }
}
