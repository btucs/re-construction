#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.IO;
using UnityEngine;
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.TextMeshPro.Inline {

  public class LinkInlineRenderer: TextMeshProObjectRenderer<LinkInline> {

    protected override void Write(TextMeshProRenderer renderer, LinkInline link) {

      /*string url = Path.GetFileNameWithoutExtension(link.Url);
      string[] parts = url.Split('#');
      string index = "0";

      if (parts.Length == 2 && link.IsImage) {

        index = parts[1];
        url = parts[0];
      }

      renderer.Write(link.IsImage ? "<size=\"250px\"><sprite=\"" : "<color=\"blue\"><u><link=\"");
      renderer.Write(link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? url : url);

      if (link.IsImage) {
      
        renderer.Write("\" index=\"" + index + "\"></size>");
      } else {

        renderer.Write("\">");
        renderer.WriteChildren(link);
        renderer.Write("</link></u></color>");
      }*/
      string url = link.Url;
      renderer.Write("<color=#131313><u><link=\"");
      renderer.Write(link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? url : url);
      renderer.Write("\">");
      renderer.WriteChildren(link);
      renderer.Write("</link></u></color>");
    }
  }
}