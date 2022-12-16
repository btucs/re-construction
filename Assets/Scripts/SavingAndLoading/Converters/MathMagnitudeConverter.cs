#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using Markdig;
using Markdig.TextMeshPro;
using Markdig.Renderers;

public class MathMagnitudeConverter : fsDirectConverter<MathMagnitude> {

  private GameController controller;
  private MarkdownPipeline pipeline;
  private TextMeshProRenderer textRenderer;
  private StringWriter writer = new StringWriter();


  public MathMagnitudeConverter() {

    controller = GameController.GetInstance();

    MarkdownPipelineBuilder builder = new MarkdownPipelineBuilder();
    pipeline = builder.UseSubSuperScript().Build();
    textRenderer = new TextMeshProRenderer(writer);
  }

  public override object CreateInstance(fsData data, Type storageType) {

    return new MathMagnitude();
  }

  protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref MathMagnitude model) {

    fsResult result = fsResult.Success;
    // check for new Savegames with uid
    if(
      (result += CheckKey(data, "taskUid", out fsData taskUid)).Failed ||
      (result += CheckType(taskUid, fsDataType.String)).Failed ||
      (result += CheckKey(data, "taskObjectUid", out fsData taskObjectUid)).Failed ||
      (result += CheckType(taskObjectUid, fsDataType.String)).Failed
    ) {
      
      // if it failed check for older savegame data
      result = fsResult.Success;
      if(
        (result += CheckKey(data, "taskName", out fsData taskName)).Failed ||
        (result += CheckType(taskName, fsDataType.String)).Failed ||
        (result += CheckKey(data, "taskObjectName", out fsData taskObjectName)).Failed ||
        (result += CheckType(taskObjectName, fsDataType.String)).Failed
      ) {

        return result;
      }

      taskUid = taskName;
      taskObjectUid = taskObjectName;
    }

    model.taskData = controller.gameAssets.FindTask(taskUid.AsString);
    if(model.taskData == null) {

      return result += fsResult.Fail("Task with UID " + taskUid.AsString + " not found");
    }

    model.taskObject = controller.gameAssets.FindTaskObject(taskObjectUid.AsString);
    if(model.taskObject == null) {

      return result += fsResult.Fail("TaskObject with UID " + taskObjectUid.AsString + " not found");
    }

    result += DeserializeMember<TaskVariable>(data, null, "value", out model.Value);
    result += DeserializeMember<DirectionEnum>(data, null, "direction", out model.Direction);
    result += DeserializeMember<ReplacementModelMapping[]>(data, null, "replacementModelMapping", out model.replacementModelMapping);

    model.Value.textMeshProName = RenderStringToTextMeshPro(model.Value.name).TrimEnd('\n');

    TaskVariable found = null;
    if(model.Value is TaskInputVariable input) {

      if(input.textValue != null) {

        input.textMeshProValue = RenderStringToTextMeshPro(input.textValue).TrimEnd('\n');
      }

      found = model.taskData.FindInputVariable(input.name);
    } else {
      // taskOuput variable
      found = model.taskData.FindOutputVariable(model.Value.name);
    }

    if(found != null) {

      model.Value.icon = found.icon;
    }


    return result;
  }

  protected override fsResult DoSerialize(MathMagnitude model, Dictionary<string, fsData> serialized) {

    SerializeMember<TaskVariable>(serialized, null, "value", model.Value);
    SerializeMember<DirectionEnum>(serialized, null, "direction", model.Direction);
    SerializeMember<string>(serialized, null, "taskUid", model.taskData?.UID);
    SerializeMember<string>(serialized, null, "taskObjectUid", model.taskObject?.UID);
    SerializeMember<ReplacementModelMapping[]>(serialized, null, "replacementModelMapping", model.replacementModelMapping);

    return fsResult.Success;
  }
  
  private string RenderStringToTextMeshPro(string text) {

    writer.GetStringBuilder().Clear();
    Markdown.Convert(text, textRenderer, pipeline);

    return writer.ToString();
  }
}