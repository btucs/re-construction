#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.TextMeshPro.Inline {

  public class EmphasisInlineRenderer: TextMeshProObjectRenderer<EmphasisInline> {

    public delegate string GetTagDelegate(EmphasisInline obj);

    public EmphasisInlineRenderer() {

      GetTag = GetDefaultTag;
    }

    public GetTagDelegate GetTag {get; set; }

    protected override void Write(TextMeshProRenderer renderer, EmphasisInline obj) {
      
      string tag = GetTag(obj);
      renderer.Write("<").Write(tag).Write(">");
      renderer.WriteChildren(obj);
      renderer.Write("</").Write(tag).Write(">");
    }

    public string GetDefaultTag(EmphasisInline obj) {

      if (obj.DelimiterChar == '*' || obj.DelimiterChar == '_') {

        return obj.DelimiterCount == 2 ? "b" : "i";
      }

      return null;
    }
  }
}