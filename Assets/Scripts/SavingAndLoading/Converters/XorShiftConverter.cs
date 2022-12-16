#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using UnityEngine;
using FullSerializer;

public class XorShiftConverter : fsDirectConverter
{
  public override Type ModelType => typeof(XorShift);

  public override object CreateInstance(fsData data, Type storageType) {

    return new XorShift(Guid.NewGuid().GetHashCode());
  }

  public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {

    if(data.IsInt64 == false || data.AsInt64 < 0) {

      return fsResult.Fail("Expected uint in " + data);
    }

    instance = new XorShift((int)data.AsInt64);

    return fsResult.Success;
  }

  public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType) {

    serialized = new fsData(((XorShift)instance).Next());

    return fsResult.Success;
  }
}
