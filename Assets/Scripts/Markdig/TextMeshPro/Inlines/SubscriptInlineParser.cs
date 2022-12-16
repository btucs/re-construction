#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using Markdig.Helpers;
using Markdig.Syntax.TextMeshPro.Inline;

namespace Markdig.Parsers.TextMeshPro.Inline {

  public class SubscriptInlineParser : InlineParser {

    public SubscriptInlineParser() {

      OpeningCharacters = new[] { '_' };
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice) {

      char previous = slice.PeekCharExtra(-1);
      char next = slice.PeekCharExtra(1);

      if(previous == '_' || next == '_') {

        return false;
      }

      char current = slice.NextChar();
      int start = slice.Start;
      int end = start;

      while(current.IsAlphaNumeric()) {

        end = slice.Start;
        current = slice.NextChar();
      }

      int inlineStart = processor.GetSourcePosition(slice.Start, out int line, out int column);

      StringSlice content = new StringSlice(slice.Text, start, end);

      processor.Inline = new SubscriptInline() {
        Span = {
          Start = inlineStart,
          End = inlineStart + (end - start)
        },
        Line = line,
        Column = column,
        content = content
      };

      return true;
    }
  }
}
