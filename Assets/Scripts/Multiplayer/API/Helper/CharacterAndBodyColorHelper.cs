#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Assets.Scripts.Multiplayer.API.Helper
{
    public static class CharacterAndBodyColorHelper
    {
        public static string GetCharacterAsString()
        {
            var character1AsString = JsonConvert.SerializeObject(MultiplayerGameManager.getPlayerCharacter().spriteRenderDictionary);
            return character1AsString.Replace('#','|');
        }

        public static string GetCharacterColorsAsString()
        {
            return MultiplayerGameManager.getPlayerCharacter().bodyColor + ";" + MultiplayerGameManager.getPlayerCharacter().hairColor;
        }

        public static Dictionary<string, string> DeserializeCharacter(string character)
        {
            var currentCharacter = character.Replace('|', '#');
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(currentCharacter);
        }

        public static void SetBodyColors(CharacterSO character, string bodyColors)
        {
            var colors = bodyColors.Split(';');
            character.bodyColor = ColorHelper.ParseColor(colors[0]);
            character.hairColor = ColorHelper.ParseColor(colors[1]);
        }
    }
}