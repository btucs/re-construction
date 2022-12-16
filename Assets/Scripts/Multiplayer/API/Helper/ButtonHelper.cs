#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universit�t Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Multiplayer.API.Helper
{
    public static class ButtonHelper
    {
        public static void SetButtonColor(Button button, Color32 color)
        {
            if (button == null) return;
            var colorBlock = button.colors;
            colorBlock.normalColor = color;
            colorBlock.highlightedColor = color;
            colorBlock.disabledColor = color;
            colorBlock.pressedColor = color;
            colorBlock.selectedColor = color;
            button.colors = colorBlock;
        }
    }
}