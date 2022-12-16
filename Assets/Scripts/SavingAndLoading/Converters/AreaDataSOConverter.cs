#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;

public class AreaDataSOConverter : fsDirectConverter
{

  private GameController controller;

  public AreaDataSOConverter() {

    controller = GameController.GetInstance();
  }

  public override object CreateInstance(fsData data, Type storageType) {

    return ScriptableObject.CreateInstance<AreaDataSO>();
  }

  public override Type ModelType { get { return typeof(AreaDataSO); } }

  public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {

    if(data.IsString == false) {

      return fsResult.Fail("Expected string in " + data);
    }

    instance = controller.gameAssets.FindAreaData(data.AsString);
    if(instance == null) {

      return fsResult.Fail("Area " + data.AsString + " not found");
    }

    return fsResult.Success;
  }

  public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType) {

    serialized = new fsData(((AreaDataSO)instance).UID);

    return fsResult.Success;
  }
}
