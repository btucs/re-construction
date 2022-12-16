#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

  // limit to x items, 0 = unlimited
  public int limitTo = 0;

  private List<InventoryItem> items = new List<InventoryItem>();

  public InventoryItem GetItem(int index) {

    return items.ElementAtOrDefault(index);
  }

  public void InsertItem(int index, InventoryItem item) {

    if(CanInsert()) {

      items.Insert(index, item);
    }
  }

  public bool CanInsert() {

    return limitTo == 0 || limitTo < items.Count();
  }

  public void AppendItem(InventoryItem item) {

    if(CanInsert()) {

      items.Add(item);
    }
  }

  public int Count {

    get => items.Count();
  }

  public InventoryItem this[int index] {

    get => items[index];
    set => AppendItem(value);
  }
}
