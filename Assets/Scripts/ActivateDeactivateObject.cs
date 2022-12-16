#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDeactivateObject : MonoBehaviour
{
	public GameObject[] objectsToHandle;
  
  public void ActivateTarget(int index)
  {
    objectsToHandle[index].SetActive(true);
  }

  public void ActivateTarget(string name) {

    GameObject target = FindByName(name);
    if(target != null) {

      target.SetActive(true);
    }
  }

  public void DeactivateTarget(int index)
  {
    objectsToHandle[index].SetActive(false);
  }

  public void DeactivateTarget(string name) {

    GameObject target = FindByName(name);
    if(target != null) {

      target.SetActive(false);
    }
  }

  public void ActivateAll() {

    ActivateDeactivateAll(true);
  }

  public void DeactivateAll() {

    ActivateDeactivateAll(false);
  }

  private void ActivateDeactivateAll(bool activate) {

    foreach(GameObject obj in objectsToHandle) {

      obj.SetActive(activate);
    }
  }

  private GameObject FindByName(string name) {

    return objectsToHandle.FirstOrDefault((GameObject go) => go.name == name);
  }
}
