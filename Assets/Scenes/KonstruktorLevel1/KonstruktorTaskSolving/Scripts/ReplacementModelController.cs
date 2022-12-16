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

public enum ReplacementModelType {
  // used to have a default value
  None = -1,
  Mass,
  Rope,
  Beam,
  Rod, 
}

public class ReplacementModelController : MonoBehaviour {

  [Serializable]
  public class ReplacementModelItem
  {
    public ReplacementModelType replacementType;
    public GameObject replacementTarget;
  }

  public ReplacementModelItem[] replacementTargets = new ReplacementModelItem[0];
}
