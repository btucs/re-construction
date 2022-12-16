#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;

namespace Assets.Scripts.Multiplayer.API.Helper
{
    public static class ObjectInfoHelper
    {
        public static void AddObjectInfo(GameObject gameObject, int id, string name)
        {
            var infos = gameObject.gameObject.GetComponent<ObjectInfo>();
            infos.Id = id;
            infos.name = name;
        }
    }
}