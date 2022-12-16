#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using Markdig.Renderers;
using Markdig.Parsers.Inlines;
using UnityEngine;

using Markdig.Parsers.TextMeshPro.Inline;
using Markdig.Renderers.TextMeshPro.Inline;

namespace Markdig.TextMeshPro {

  public class TextMeshProExtension : IMarkdownExtension {

    public void Setup(MarkdownPipelineBuilder pipeline) {

      if(pipeline.InlineParsers.Contains<SubscriptInlineParser>() == false) {

        pipeline.InlineParsers.Insert(0, new SubscriptInlineParser());
      }

      if(pipeline.InlineParsers.Contains<SuperscriptInlineParser>() == false) {

        pipeline.InlineParsers.Insert(0, new SuperscriptInlineParser());
      }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) {

      if(renderer is TextMeshProRenderer tmpRenderer) {

        if(tmpRenderer.ObjectRenderers.Contains<SubscriptInlineRenderer>() == false) {

          tmpRenderer.ObjectRenderers.Insert(0, new SubscriptInlineRenderer());
        }

        if(tmpRenderer.ObjectRenderers.Contains<SuperscriptInlineRenderer>() == false) {

          tmpRenderer.ObjectRenderers.Insert(0, new SuperscriptInlineRenderer());
        }
      }

    }
  }
}
