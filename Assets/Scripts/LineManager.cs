#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete()]
public class LineManager: MonoBehaviour {

  public static LineManager instance;

  private List<Line> lines = new List<Line>();
  private List<GameObject> handles = new List<GameObject>();
  
  private void Awake() {

    if (instance == null) {

      instance = this;
    }
  }

  public void AddLine(Line line) {

    lines.Add(line);

    for (int i = 0; i < line.transform.childCount; i++) {

      Transform child = line.transform.GetChild(i);
      handles.Add(child.gameObject);
    }
  }


}
