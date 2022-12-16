#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;

namespace Assets.Scripts.Multiplayer.API.Models
{
    public class QuizLeaderboard
    {
        public int ID { get; set; }
        
        public string PlayerID { get; set; }
        
        public int Points { get; set; }
        
        public DateTime CreateDateTime { get; set; }
        
        public int SeasonID { get; set; }
        
        public bool SeenSeasonSwitch { get; set; }
    }
}