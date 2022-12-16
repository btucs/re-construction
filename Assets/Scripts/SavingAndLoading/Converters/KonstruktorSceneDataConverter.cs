#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;

public class KonstruktorSceneDataConverter : fsDirectConverter<KonstruktorSceneData> {

  private GameController controller;

  public KonstruktorSceneDataConverter() {

    controller = GameController.GetInstance();
  }

  protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref KonstruktorSceneData model) {

    fsResult result = fsResult.Success;

    if(
      (result += CheckKey(data, "backgroundPrefabName", out fsData backgroundPrefabName)).Failed ||
      (result += CheckType(backgroundPrefabName, fsDataType.String)).Failed
      
    ) {
      
      return result;
    }

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


    model.backgroundPrefab = controller.gameAssets.FindPlace(backgroundPrefabName.AsString);
    if(model.backgroundPrefab == null) {

      return result += fsResult.Fail("Background " + backgroundPrefabName.AsString + " not found");
    }

    model.taskData = controller.gameAssets.FindTask(taskUid.AsString);
    if(model.taskData == null) {

      return result += fsResult.Fail("Task with UID " + taskUid.AsString + " not found");
    }

    model.taskObject = controller.gameAssets.FindTaskObject(taskObjectUid.AsString);
    if(model.taskObject == null) {

      return result += fsResult.Fail("TaskObject with UID " + taskObjectUid.AsString + " not found");
    }

    result += DeserializeMember<KonstruktorSceneData.InteractableData[]>(data, null, "interactablesPrefabs", out model.interactablesPrefabs);
    result += DeserializeMember<Vector3>(data, null, "cameraPosition", out model.cameraPosition);
    result += DeserializeMember<float>(data, null, "cameraZoomFactor", out model.cameraZoomFactor);
    result += DeserializeMember<MathMagnitude[]>(data, null, "inputs", out model.inputs);
    result += DeserializeMember<MathMagnitude[]>(data, null, "outputs", out model.outputs);
    result += DeserializeMember<string>(data, null, "returnSceneName", out model.returnSceneName);
    result += DeserializeMember<ConverterResultData[]>(data, null, "converterResults", out model.converterResults);
    result += DeserializeMember<int>(data, null, "currentStep", out model.currentStep);
    result += DeserializeMember<float>(data, null, "npcScale", out model.npcScale);

    if(model.npcScale == 0) {

      model.npcScale = 1;
    }

    if(model.cameraZoomFactor == 0) {

      model.cameraZoomFactor = 1;
    }

    return result;
  }

  protected override fsResult DoSerialize(KonstruktorSceneData model, Dictionary<string, fsData> serialized) {

    SerializeMember<string>(serialized, null, "backgroundPrefabName", model.backgroundPrefab != null ? model.backgroundPrefab.name : "");
    SerializeMember<string>(serialized, null, "taskUid", model.taskData?.UID);
    SerializeMember<string>(serialized, null, "taskObjectUid", model.taskObject?.UID);
    SerializeMember<KonstruktorSceneData.InteractableData[]>(serialized, null, "interactablesPrefabs", model.interactablesPrefabs);
    SerializeMember<Vector3>(serialized, null, "cameraPosition", model.cameraPosition);
    SerializeMember<float>(serialized, null, "cameraZoomFactor", model.cameraZoomFactor);
    SerializeMember<MathMagnitude[]>(serialized, null, "inputs", model.inputs);
    SerializeMember<MathMagnitude[]>(serialized, null, "outputs", model.outputs);
    SerializeMember<string>(serialized, null, "returnSceneName", model.returnSceneName);
    SerializeMember<ConverterResultData[]>(serialized, null, "converterResults", model.converterResults);
    SerializeMember<int>(serialized, null, "currentStep", model.currentStep);
    SerializeMember<float>(serialized, null, "npcScale", model.npcScale);

    return fsResult.Success;
  }
}
