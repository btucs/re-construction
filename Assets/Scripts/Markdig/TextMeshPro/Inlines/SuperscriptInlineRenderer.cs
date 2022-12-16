#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using Markdig.Syntax.TextMeshPro.Inline;

namespace Markdig.Renderers.TextMeshPro.Inline {

  public class SuperscriptInlineRenderer : TextMeshProObjectRenderer<SuperscriptInline> {

    protected override void Write(TextMeshProRenderer renderer, SuperscriptInline obj) {

      renderer
        .Write("<sup>")
        .Write(ref obj.content)
        .Write("</sup>")
      ;
    }
  }
}
