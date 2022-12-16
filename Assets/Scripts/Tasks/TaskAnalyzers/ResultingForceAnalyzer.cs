#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ResultingForceAnalyzer: TaskAnalyzerAbstract {

  protected override bool Analyze() {

    Vector3Prerequisite[] prerequisites = task.GetPrerequisites<Vector3Prerequisite>();

    Vector2 expectedDirection = prerequisites.Aggregate(
      Vector2.zero,
      (Vector2 aggregate, Vector3Prerequisite prerequisite) => {

        return aggregate + (Vector2) prerequisite.Value;
      }
    );

    KonstruktorData data = task.GetKonstruktorData();
    Line line = HelperFunctions.GetLastChildOfType<Line>(data.lineContainers.freiebene.transform);

    Vector2 worldStartPos = line.transform.GetChild(1).position;
    Vector2 worldEndPos = line.transform.GetChild(2).position;

    bool result = expectedDirection == (worldEndPos - worldStartPos) &&
      worldStartPos == Vector2.zero
    ;

    if (result == false) {

      currentMessage = FindMessage("DirectionNotCorrect");
    }

    return result;
  }
}
