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

public class ModuleConnectionsOut : MonoBehaviour {

  public new Collider collider;

  private OrderedSet<ModuleConnection> outConnections = new OrderedSet<ModuleConnection>();

  private void Start() {

    if(collider == null) {

      collider = GetComponent<Collider>();
    }
  }

  public Collider GetOutCollider() {

    return collider;
  }

  public void AddOutConnection(ModuleConnection connection) {

    outConnections.Add(connection);
  }

  public void RemoveOutConnection(ModuleConnection connection) {

    outConnections.Remove(connection);
  }

  public int CountOutConnections() {

    return outConnections.Count;
  }

  public void RedrawOutConnections() {

    foreach(ModuleConnection connection in outConnections) {

      connection.UpdateConnection();
    }
  }

  public OrderedSet<ModuleConnection> GetOutConnections() {

    return outConnections;
  }
}
