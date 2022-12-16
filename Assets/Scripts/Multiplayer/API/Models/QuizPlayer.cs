#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
namespace Assets.Scripts.Multiplayer.API.Models
{
    [Serializable]
    public class QuizPlayer
    {
        public string PlayerID { get; set; }
        
        public string PlayerName { get; set; }
        
        public DateTime CreateDateTime { get; set; }

        public string PlayerCharacter { get; set; }

        public string Colors { get; set; }
    }
}