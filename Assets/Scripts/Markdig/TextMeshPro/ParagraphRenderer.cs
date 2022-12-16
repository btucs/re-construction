#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using Markdig.Syntax;

namespace Markdig.Renderers.TextMeshPro {
  public class ParagraphRenderer: TextMeshProObjectRenderer<ParagraphBlock> {

    protected override void Write(TextMeshProRenderer renderer, ParagraphBlock obj) {

      renderer.WriteLeafInline(obj);
      renderer.FinishBlock(!renderer.CompactParagraph);
    }
  }
}