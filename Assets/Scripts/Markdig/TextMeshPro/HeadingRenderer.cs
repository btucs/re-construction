#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using Markdig.Syntax;

namespace Markdig.Renderers.TextMeshPro {

  public class HeadingRenderer : TextMeshProObjectRenderer<HeadingBlock> {

    protected override void Write(TextMeshProRenderer renderer, HeadingBlock obj) {

      int sizeIncrease = obj.Level * 2;

      if (renderer.IsFirstInContainer == false) {

        renderer.EnsureLine();
        renderer.Write("\n");
      }

      renderer.Write("<size=+" + sizeIncrease.ToString() + "><u>");
      renderer.WriteLeafInline(obj);
      renderer.Write("</u></size>");

      renderer.EnsureLine();
    }
  }
}
