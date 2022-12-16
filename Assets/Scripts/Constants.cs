#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;

namespace Constants {

  public static class Layers {
    public static readonly int interactable = LayerMask.NameToLayer("Interactable");
    public static readonly int background = LayerMask.NameToLayer("Background");
    public static readonly int coordinateSystem = LayerMask.NameToLayer("CoordinateSystem");
  }
}