#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HelperFunctions {

  public static T GetLastChildOfType<T>(Transform parent) where T : UnityEngine.Object {

    for (int i = parent.childCount - 1; i >= 0; i--) {

      Transform child = parent.GetChild(i);
      T component = child.GetComponent<T>();
      if (component != null) {

        return component;
      }
    }

    return null;
  }

  public static T[] FindObjectsOfType<T>(Transform parent) where T : UnityEngine.Object {

    List<T> results = new List<T>();

    for (int i = 0; i < parent.childCount; i++) {

      Transform child = parent.GetChild(i);
      T component = child.GetComponent<T>();
      if (component != null) {

        results.Add(component);
      }
    }

    return results.ToArray();
  }

  public static void DestroyChildren(Transform parent) {

    DestroyChildren(parent, false);
  }

  public static void DestroyChildren(Transform parent, bool ignoreHidden) {
  
    for(int i = parent.childCount - 1; i >= 0; i--) {

      GameObject child = parent.GetChild(i).gameObject;

      if(ignoreHidden == true) {

        if(child.activeSelf == true) {

          Object.Destroy(child);
        }
      } else {

        Object.Destroy(child);
      }
    }
  }

  public static void DestroyChildren<T>(Transform parent) where T : UnityEngine.Object {

    for(int i = parent.childCount - 1; i >= 0; i--) {

      Transform child = parent.GetChild(i);
      T component = child.GetComponent<T>();
      if(component != null) {

        Object.Destroy(child.gameObject);
      }
    }
  }

  public static void MoveToLayer(Transform root, int layer) {
    root.gameObject.layer = layer;
    foreach(Transform child in root)
      MoveToLayer(child, layer);
  }
}
