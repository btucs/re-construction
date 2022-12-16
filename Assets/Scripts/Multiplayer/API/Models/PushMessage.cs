#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;

namespace Assets.Scripts.Multiplayer.API.Models
{
  public class PushMessage
  {
    public string title;
    public string body;
    public string token;
    public Dictionary<string, string> data;
  }
}