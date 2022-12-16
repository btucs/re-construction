#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllVectorsStartZeroAnalyzer: TaskAnalyzerAbstract {

  protected override bool Analyze() {

    KonstruktorData data = task.GetKonstruktorData();
    Line[] lines = HelperFunctions.FindObjectsOfType<Line>(data.lineContainers.freiebene.transform);

    IEnumerable<Line> matchingLines = lines.Where((Line line) => {

      Vector2 startPos = line.transform.GetChild(1).position;
      
      return startPos == Vector2.zero;
    });

    return matchingLines.Count() == lines.Length;
  }
}
