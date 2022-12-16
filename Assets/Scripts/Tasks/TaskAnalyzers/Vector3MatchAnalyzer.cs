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

public class Vector3MatchAnalyzer: TaskAnalyzerAbstract {

  private Vector3[] matches;

  public Vector3MatchAnalyzer(Vector3[] matches) {

    this.matches = matches;
  }

  protected override bool Analyze() {

    KonstruktorData data = task.GetKonstruktorData();
    Line[] lines = HelperFunctions.FindObjectsOfType<Line>(data.lineContainers.freiebene.transform);

    if (lines.Length == 0) {

      currentMessage = FindMessage("NoVectors");

      return false;
    }

    if (lines.Length < matches.Length) {

      currentMessage = FindMessage("TooLessVectors");

      return false;
    }

    if (lines.Length > matches.Length) {

      currentMessage = FindMessage("TooManyVectors");

      return false;
    }

    IEnumerable<Line> matchingLines = lines.Where((Line line) => {

      Vector2 direction = (Vector2)(line.transform.GetChild(2).position - line.transform.GetChild(1).position);
      Vector2 found = Array.Find(matches, (Vector3 value) => (Vector2) value == direction);

      return found != Vector2.zero;
    });

    bool result = matchingLines.Count() == lines.Length;
    if (result == false) {

      currentMessage = FindMessage("AllVectorsButSomeWrong");
    }

    return result;
  }
}
