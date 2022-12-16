#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.IO;
using Markdig.Renderers.PlainText;
using Markdig.Renderers.PlainText.Inline;

namespace Markdig.Renderers {

  public class PlainTextRenderer: TextRendererBase<PlainTextRenderer> {

    public PlainTextRenderer(TextWriter writer) : base(writer) {

      // Block renderers
      ObjectRenderers.Add(new HeadingRenderer());

      // inline renderers
      ObjectRenderers.Add(new LiteralInlineRenderer());      
    }
  }
}
