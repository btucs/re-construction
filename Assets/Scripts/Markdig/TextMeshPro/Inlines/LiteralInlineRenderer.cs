#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.TextMeshPro.Inline {

  public class LiteralInlineRenderer: TextMeshProObjectRenderer<LiteralInline> {

    protected override void Write(TextMeshProRenderer renderer, LiteralInline obj) {

      renderer.Write(ref obj.Content);
    }
  }
}