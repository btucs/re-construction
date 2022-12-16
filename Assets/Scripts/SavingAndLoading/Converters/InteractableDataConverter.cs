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
using System;

public class InteractableDataConverter : fsDirectConverter<KonstruktorSceneData.InteractableData> {

  private GameController controller;

  public InteractableDataConverter() {

    controller = GameController.GetInstance();
  }

  public override object CreateInstance(fsData data, Type storageType) {

    return new KonstruktorSceneData.InteractableData();
  }

  protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref KonstruktorSceneData.InteractableData model) {

    fsResult result = fsResult.Success;
    // check for new Savegames with uid
    if(
      (result += CheckKey(data, "taskObjectUid", out fsData taskObjectUid)).Failed ||
      (result += CheckType(taskObjectUid, fsDataType.String)).Failed
    ) {

      // if it failed check for older savegame data
      result = fsResult.Success;
      if(
        (result += CheckKey(data, "taskObjectName", out fsData taskObjectName)).Failed ||
        (result += CheckType(taskObjectName, fsDataType.String)).Failed
      ) {

        return result;
      }

      taskObjectUid = taskObjectName;
    }

    model.taskObject = controller.gameAssets.FindTaskObject(taskObjectUid.AsString);
    if(model.taskObject == null) {

      return result += fsResult.Fail("TaskObject with UID " + taskObjectUid.AsString + " not found");
    }

    result += DeserializeMember<Vector3>(data, null, "position", out model.position);

    return result;
  }

  protected override fsResult DoSerialize(KonstruktorSceneData.InteractableData model, Dictionary<string, fsData> serialized) {

    SerializeMember<Vector3>(serialized, null, "position", model.position);
    SerializeMember<string>(serialized, null, "taskObjectUid", model.taskObject.UID);

    return fsResult.Success;
  }
}
