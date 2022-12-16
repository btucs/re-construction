#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;
using MathUnits.Physics.Values;

public class IntermediateResultHandler
{
  public ScalarValue Value;
  public List<MathVarHandler> linkedVars = new List<MathVarHandler>();

  public void UpdateAllLinkedVars() {

    foreach(MathVarHandler linkedVar in linkedVars) {

      linkedVar.linkedResult = this;
    }
  }

  public void MergeWith(IntermediateResultHandler secondResult) {

    foreach(MathVarHandler link in secondResult.linkedVars) {

      linkedVars.Add(link);
      link.linkedResult = this;
    }
  }
}
