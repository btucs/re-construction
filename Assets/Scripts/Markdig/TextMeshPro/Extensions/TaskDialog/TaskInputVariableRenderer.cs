#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Markdig.Renderers;
using Markdig.Renderers.TextMeshPro;
using Markdig.Syntax;

namespace Markdig.TextMeshPro.Extensions.TaskDialog {

  public class TaskInputVariableRenderer : TextMeshProObjectRenderer<TaskInputVariable> {

    private TaskDialogOptions options;

    public TaskInputVariableRenderer(ref TaskDialogOptions options) : base() {
      this.options = options;
    }

    protected override void Write(TextMeshProRenderer renderer, TaskInputVariable obj) {

      Predicate<global::TaskInputVariable> FindVariable = (global::TaskInputVariable variable) => variable.name == obj.parameterName;
      global::TaskInputVariable inputVariable = Array.Find<global::TaskInputVariable>(options.taskInputs, FindVariable);

      if(inputVariable == null)
      {
      	inputVariable = Array.Find<global::TaskInputVariable>(options.taskDummyInputs, FindVariable);
      }

      string displayValue = string.IsNullOrEmpty(obj.displayText) ? inputVariable.textMeshProValue.Replace('.', ',') : obj.displayText;

      renderer.Write("<style=\"Variable\"><nobr><link=\"input:" + inputVariable.name + "\">").Write(displayValue).Write("<sprite=0></link></nobr></style>");
    }
  }
}
