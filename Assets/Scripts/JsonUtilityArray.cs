#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using UnityEngine;

public class JsonUtilityArray {

  public static T[] FromJsonArray<T>(string json) {

    string newJson = "{ \"array\": " + json + "}";
    Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);

    return wrapper.array;
  }

  [Serializable]
  private class Wrapper<T> {
    #pragma warning disable CS0649
    public T[] array;
    #pragma warning restore CS0649
  }
}
