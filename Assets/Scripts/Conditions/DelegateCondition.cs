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

public class DelegateCondition<T> : AbstractCondition {

  private T compareTo;
  private DelegateConditionDelegate callback;

  public override bool IsFullfilled() {

    return callback(compareTo);
  }

  public override void SetCompareToValue(object compareTo) {

    this.compareTo = (T)compareTo;
  }

  public override void SetReferenceValue(object reference) {

    callback = (DelegateConditionDelegate)reference;
  }

  public delegate bool DelegateConditionDelegate(T item);
}
