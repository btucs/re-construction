﻿#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Globalization;
using Markdig.Syntax;

namespace Markdig.Renderers.TextMeshPro {

  public class ListRenderer: TextMeshProObjectRenderer<ListBlock> {

    private char? customBulletType;

    public void OverrideBulletType(char? bulletType) {

      customBulletType = bulletType;
    }

    protected override void Write(TextMeshProRenderer renderer, ListBlock listBlock) {

      renderer.EnsureLine();
      bool compact = renderer.CompactParagraph;
      renderer.CompactParagraph = !listBlock.IsLoose;

      if(listBlock.IsOrdered) {

        int index = 0;
        if(listBlock.OrderedStart != null) {

          switch(listBlock.BulletType) {

            case '1':
              int.TryParse(listBlock.OrderedStart, out index);
              break;
          }
        }

        for(int i = 0; i < listBlock.Count; i++) {

          Block item = listBlock[i];
          ListItemBlock listItem = item as ListItemBlock;
          renderer.EnsureLine();
          renderer.Write(index.ToString(CultureInfo.InvariantCulture));
          renderer.Write(listBlock.OrderedDelimiter);
          renderer.Write(' ');
          renderer.PushIndent(new string(' ', IntLog10Fast(index) + 3));
          renderer.WriteChildren(listItem);
          renderer.PopIndent();

          switch(listBlock.BulletType) {

            case '1':
              index++;
              break;
          }

          if(i + 1 < listBlock.Count && listBlock.IsLoose) {

            renderer.EnsureLine();
            renderer.WriteLine();
          }
        }
      } else {

        for(int i = 0; i < listBlock.Count; i++) {

          Block item = listBlock[i];
          ListItemBlock listItem = item as ListItemBlock;
          renderer.EnsureLine();
          renderer.Write(customBulletType != null ? (char) customBulletType : listBlock.BulletType);
          renderer.Write(' ');
          renderer.PushIndent("  ");
          renderer.WriteChildren(listItem);
          renderer.PopIndent();

          if(i + 1 < listBlock.Count && listBlock.IsLoose) {

            renderer.EnsureLine();
            renderer.WriteLine();
          }
        }
      }

      renderer.CompactParagraph = compact;
      renderer.FinishBlock(true);
    }

    private static int IntLog10Fast(int input) =>
      (input < 10) ? 0 :
      (input < 100) ? 1 :
      (input < 1000) ? 2 :
      (input < 10000) ? 3 :
      (input < 100000) ? 4 :
      (input < 1000000) ? 5 :
      (input < 10000000) ? 6 :
      (input < 100000000) ? 7 :
      (input < 1000000000) ? 8 : 9
    ;
  }
}