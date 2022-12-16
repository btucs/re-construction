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

public class VectorsFormLineAnalyzer: TaskAnalyzerAbstract {

  protected override bool Analyze() {

    IEnumerable<IGrouping<string, GameObject>> notTwoHandles = GameManager.instance.handlePool
      .GroupBy((GameObject gameObject) => ((Vector2)gameObject.transform.position).ToString())
      .Where((IGrouping<string, GameObject> group) => {

        return group.Count() != 2;
      })
    ;

    return notTwoHandles.Count() == 0;
  }
}
