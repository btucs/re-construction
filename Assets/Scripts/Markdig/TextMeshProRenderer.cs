#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.IO;
using Markdig.Renderers.TextMeshPro;
using Markdig.Renderers.TextMeshPro.Inline;
using Markdig.Syntax;

namespace Markdig.Renderers {

  public class TextMeshProRenderer: TextRendererBase<TextMeshProRenderer> {

    private ListRenderer listRenderer;

    public TextMeshProRenderer(TextWriter writer) : base(writer) {

      // Block renderers
      ObjectRenderers.Add(new HeadingRenderer());
      listRenderer = new ListRenderer();
      ObjectRenderers.Add(listRenderer);
      ObjectRenderers.Add(new ParagraphRenderer());

      // Inline renderers
      ObjectRenderers.Add(new LiteralInlineRenderer());
      ObjectRenderers.Add(new LineBreakInlineRenderer());
      ObjectRenderers.Add(new DelimiterInlineRenderer());
      ObjectRenderers.Add(new LinkInlineRenderer());
      ObjectRenderers.Add(new EmphasisInlineRenderer());
    }

    public bool CompactParagraph {
      get; set;
    }

    public void FinishBlock(bool emptyLine) {
      if(!IsLastInContainer) {
        WriteLine();
        if(emptyLine) {
          WriteLine();
        }
      }
    }

    public TextMeshProRenderer WriteLeafRawLines(LeafBlock leafBlock, bool writeEndOfLines, bool indent = false) {
      if(leafBlock == null)
        throw new ArgumentNullException(nameof(leafBlock));
      if(leafBlock.Lines.Lines != null) {
        var lines = leafBlock.Lines;
        var slices = lines.Lines;
        for(int i = 0; i < lines.Count; i++) {
          if(!writeEndOfLines && i > 0) {
            WriteLine();
          }

          if(indent) {
            Write("    ");
          }

          Write(ref slices[i].Slice);

          if(writeEndOfLines) {
            WriteLine();
          }
        }
      }
      return this;
    }

    public void OverrideBulletType(char? customBulletType) {

      if(listRenderer != null) {

        listRenderer.OverrideBulletType(customBulletType);
      }
    }
  }  
}
