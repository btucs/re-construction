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

public class UIPanelShowHideSortOrderComparer : IComparer, IComparer<UIPanelManager.ShowHideItem> {

  private List<GameObject> matchTo;

  public UIPanelShowHideSortOrderComparer(List<GameObject> matchTo) {

    this.matchTo = matchTo;
  }

  public int Compare(object x, object y) {

    UIPanelManager.ShowHideItem xTyped = x as UIPanelManager.ShowHideItem;
    UIPanelManager.ShowHideItem yTyped = y as UIPanelManager.ShowHideItem;

    if(xTyped == null || yTyped == null) {

      throw new ArgumentException();
    }

    return Compare(xTyped, yTyped);
  }

  public int Compare(UIPanelManager.ShowHideItem x, UIPanelManager.ShowHideItem y) {

    int xIndex = matchTo.IndexOf(x.targetPanel);
    int yIndex = matchTo.IndexOf(y.targetPanel);

    return xIndex.CompareTo(yIndex);
  }
}