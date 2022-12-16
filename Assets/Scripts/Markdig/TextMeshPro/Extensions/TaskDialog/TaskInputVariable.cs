#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Syntax.Inlines;
using UnityEngine;

namespace Markdig.TextMeshPro.Extensions.TaskDialog {

  public class TaskInputVariable : LeafInline, ITaskVariable {

    public string parameterName;
    public string displayText;

  }
}