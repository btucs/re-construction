#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using UnityEngine;

namespace Markdig.TextMeshPro.Extensions.TaskDialog {

  public class TaskOutputVariableParser : TaskVariableBaseParserAbstract<TaskOutputVariable> {

    private static readonly char[] openingChars = {
      '~',
    };

    public TaskOutputVariableParser() {

      OpeningCharacters = openingChars;
    }

    protected override TaskOutputVariable CreateInstance(SourceSpan Span, int Line, int Column, string parameterName, string displayText) {

      return new TaskOutputVariable() {
        Span = Span,
        Line = Line,
        Column = Column,
        parameterName = parameterName,
        displayText = displayText
      };
    }

    protected override string GetIdentifierString() {

      return "o:";
    }
  }
}
