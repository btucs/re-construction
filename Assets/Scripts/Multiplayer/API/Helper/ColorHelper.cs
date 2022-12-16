#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Globalization;
using UnityEngine;

namespace Assets.Scripts.Multiplayer.API.Helper
{
    public static class ColorHelper
    {
        public static Color ParseColor(string cssColor)
        {
            cssColor = cssColor.Trim();

            if (cssColor.ToLowerInvariant().StartsWith("rgb")) //rgb or argb
            {
                int left = cssColor.IndexOf('(');
                int right = cssColor.IndexOf(')');

                if (left < 0 || right < 0)
                    throw new FormatException("rgba format error");
                string noBrackets = cssColor.Substring(left + 1, right - left - 1);

                string[] parts = noBrackets.Split(',');

                float r = (float.Parse(parts[0], CultureInfo.InvariantCulture));
                float g = (float.Parse(parts[1], CultureInfo.InvariantCulture));
                float b = (float.Parse(parts[2], CultureInfo.InvariantCulture));
                
                if (parts.Length == 3)
                {
                    return new Color(r, g, b);
                }

                if (parts.Length == 4)
                {
                    float a = float.Parse(parts[3], CultureInfo.InvariantCulture);
                    return new Color( r, g, b,a);
                }
            }
            throw new FormatException("Not rgb, rgba or hexa color string");
        }
    }

}