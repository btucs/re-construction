#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax.Inlines;
using Markdig.Syntax;
using UnityEngine;

namespace Markdig.TextMeshPro.Extensions.TaskDialog {

  public abstract class TaskVariableBaseParserAbstract<T> : InlineParser where T : LeafInline, ITaskVariable
  {

    protected abstract string GetIdentifierString();

    public override bool Match(InlineProcessor processor, ref StringSlice slice) {

      bool matchFound = false;
      char previous = slice.PeekCharExtra(-1);

      // make sure there is a space before
      if(previous.IsWhiteSpaceOrZero() == false) {

        return false;
      }

      //next two characters should be "i:"
      foreach(char c in GetIdentifierString().ToCharArray()) {

        if(c != slice.NextChar()) {

          return false;
        }
      }

      char current;
      int start;
      int end;

      current = slice.NextChar();
      start = slice.Start;
      end = start;

      while(current.IsAlphaNumeric() || current.Equals('_')) {

        end = slice.Start;
        current = slice.NextChar();
      }

      int startDisplayText = -1;
      int endDisplayText = -1;

      current = SkipWhiteSpace(ref slice);

      if(current == '|') {

        current = slice.NextChar();
        startDisplayText = slice.Start;
        endDisplayText = startDisplayText;
        
        while(current != '~' && current != '\0') {

          endDisplayText = slice.Start;
          current = slice.NextChar();
        }
      }

      if(current == '~' || current == '\0') {

        // remove closing char
        slice.NextChar();
        int inlineStart = processor.GetSourcePosition(slice.Start, out int line, out int column);

        StringSlice parameterSlice = new StringSlice(slice.Text, start, end);
        StringSlice displayTextSlice = StringSlice.Empty;
        if(startDisplayText != -1 && endDisplayText != -1) {

          displayTextSlice = new StringSlice(slice.Text, startDisplayText, endDisplayText);
          displayTextSlice.Trim();
        }

        processor.Inline = CreateInstance(
          new SourceSpan {
            Start = inlineStart,
            End = inlineStart + (end - start)
          },
          line,
          column,
          parameterSlice.ToString(),
          displayTextSlice.IsEmptyOrWhitespace() ? string.Empty : displayTextSlice.ToString()
        );

        matchFound = true;
      }

      return matchFound;
    }

    protected abstract T CreateInstance(SourceSpan Span, int Line, int Column, string parameterName, string displayText);

    protected char SkipWhiteSpace(ref StringSlice slice) {

      char current = slice.CurrentChar;
      while(current.IsWhitespace()) {

        current = slice.NextChar();
      }

      return current;
    }
  }
}
