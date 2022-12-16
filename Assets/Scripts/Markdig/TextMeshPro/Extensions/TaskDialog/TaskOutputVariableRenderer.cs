#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using Markdig.Renderers;
using Markdig.Renderers.TextMeshPro;
using UnityEngine;

namespace Markdig.TextMeshPro.Extensions.TaskDialog {

  public class TaskOutputVariableRenderer : TextMeshProObjectRenderer<TaskOutputVariable> {

    private TaskDialogOptions options;

    public TaskOutputVariableRenderer(ref TaskDialogOptions options) : base() {

      this.options = options;
    }

    protected override void Write(TextMeshProRenderer renderer, TaskOutputVariable obj) {

      Predicate<global::TaskOutputVariable> FindVariable = (global::TaskOutputVariable variable) => variable.name == obj.parameterName;
      global::TaskOutputVariable outputVariable = Array.Find<global::TaskOutputVariable>(options.taskOutputs, FindVariable);

      string displayValue = string.IsNullOrEmpty(obj.displayText) ? outputVariable.textMeshProName : obj.displayText;

      renderer.Write("<style=\"Variable\"><nobr><link=\"output:" + obj.parameterName +"\">").Write(displayValue).Write("<sprite=0></link></nobr></style>");
    }
  }
}