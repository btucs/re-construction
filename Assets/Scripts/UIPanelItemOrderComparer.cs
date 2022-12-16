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

public class UIPanelItemSortOrderComparer : IComparer, IComparer<UIPanelManager.UIPanelItem> {

  private List<GameObject> matchTo;

  public UIPanelItemSortOrderComparer(List<GameObject> matchTo) {

    this.matchTo = matchTo;
  }

  public int Compare(object x, object y) {

    UIPanelManager.UIPanelItem xTyped = x as UIPanelManager.UIPanelItem;
    UIPanelManager.UIPanelItem yTyped = y as UIPanelManager.UIPanelItem;

    if(xTyped == null || yTyped == null) {

      throw new ArgumentException();
    }

    return Compare(xTyped, yTyped);
  }

  public int Compare(UIPanelManager.UIPanelItem x, UIPanelManager.UIPanelItem y) {

    int xIndex = matchTo.IndexOf(x.targetPanel);
    int yIndex = matchTo.IndexOf(y.targetPanel);

    return xIndex.CompareTo(yIndex);
  }
}