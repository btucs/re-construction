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

public class FinishedTaskDataConverter : fsDirectConverter<FinishedTaskData> {

  private GameController controller;

  public FinishedTaskDataConverter() {

    controller = GameController.GetInstance();
  }

  public override object CreateInstance(fsData data, Type storageType) {

    return new FinishedTaskData();
  }

  protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref FinishedTaskData model) {

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

    model.solvedTask = controller.gameAssets.FindTask(taskUid.AsString);
    if(model.solvedTask == null) {

      return result += fsResult.Fail("Task with UID " + taskUid.AsString + " not found");
    }

    model.taskObject = controller.gameAssets.FindTaskObject(taskObjectUid.AsString);
    if(model.taskObject == null) {

      return result+= fsResult.Fail("TaskObject with UID " + taskObjectUid.AsString + " not found");
    }

    result += DeserializeMember<int>(data, null, "achievedPoints", out model.achievedPoints);

    return result;
  }

  protected override fsResult DoSerialize(FinishedTaskData model, Dictionary<string, fsData> serialized) {

    SerializeMember<int>(serialized, null, "achievedPoints", model.achievedPoints);
    SerializeMember<string>(serialized, null, "taskUid", model.solvedTask.UID);
    SerializeMember<string>(serialized, null, "taskObjectUid", model.taskObject.UID);

    return fsResult.Success;
  }
}