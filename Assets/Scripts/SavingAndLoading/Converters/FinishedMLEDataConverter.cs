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

public class FinishedMLEDataConverter : fsDirectConverter<FinishedMLEData> {

  private GameController controller;

  public FinishedMLEDataConverter() {

    controller = GameController.GetInstance();
  }

  public override object CreateInstance(fsData data, Type storageType) {

    return new FinishedMLEData();
  }

  protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref FinishedMLEData model) {

    fsResult result = fsResult.Success;

    // check for new savegame with uid
    if(
      (result += CheckKey(data, "mleUid", out fsData mleUid)).Failed ||
      (result += CheckType(mleUid, fsDataType.String)).Failed
    ) {

      // if it failed check vor older savegame data
      result = fsResult.Success;
      if(
        (result += CheckKey(data, "mleName", out fsData mleName)).Failed ||
        (result += CheckType(mleName, fsDataType.String)).Failed
      ) {

        return result;
      }

      mleUid = mleName;
    }

    model.solvedMLE = controller.gameAssets.FindMLE(mleUid.AsString);
    if(model.solvedMLE == null) {

      return result += fsResult.Fail("MLE with UID " + mleUid.AsString + " not found");
    }

    result += DeserializeMember<int>(data, null, "achievedPoints", out model.achievedPoints);

    return result;
  }

  protected override fsResult DoSerialize(FinishedMLEData model, Dictionary<string, fsData> serialized) {

    SerializeMember<int>(serialized, null, "achievedPoints", model.achievedPoints);
    SerializeMember<string>(serialized, null, "mleUid", model.solvedMLE.UID);

    return fsResult.Success;
  }
}