#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.TextMeshPro.Inline {

  public class LineBreakInlineRenderer: TextMeshProObjectRenderer<LineBreakInline> {

    protected override void Write(TextMeshProRenderer renderer, LineBreakInline obj) {

      if (obj.IsHard) {

        renderer.WriteLine("\n\n");
      } else {

        renderer.Write(" ");
      }
    }
  }
}