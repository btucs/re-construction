#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gma.DataStructures;

public class ModuleConnectionsIn : MonoBehaviour {

  public new Collider collider;
  
  private OrderedSet<ModuleConnection> inConnections = new OrderedSet<ModuleConnection>();

  private void Start() {

    if(collider == null) {

      collider = GetComponent<Collider>();
    }
  }

  public Collider GetInCollider() {

    return collider;
  }

  public void AddInConnection(ModuleConnection connection) {

    inConnections.Add(connection);
  }

  public void RemoveInConnection(ModuleConnection connection) {

    inConnections.Remove(connection);
  }

  public int CountInConnections() {

    return inConnections.Count;
  }

  public void RedrawInConnections() {

    foreach(ModuleConnection connection in inConnections) {

      connection.UpdateConnection();
    }
  }

  public OrderedSet<ModuleConnection> GetInConnections() {

    return inConnections;
  }
}
