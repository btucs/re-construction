#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Threading.Tasks;
using System.Collections.Generic;
using Assets.Scripts.Multiplayer.API.Models;
using UnityEngine;

namespace Assets.Scripts.Multiplayer.API
{
    public class APIClientPlayer
    {
        private static string playerapi = MultiplayerGameManager.gameapi + "/QuizGamePlayer";
        private static string mainapi = MultiplayerGameManager.gameapi + "/QuizGameMain";
        private static string createplayer = playerapi + "/CreateNewPlayer";
        private static string getplayer = playerapi + "/GetPlayerById";
        private static string getGames = mainapi + "/GetGamesByPlayerId";
        private static string updatePlayer = playerapi + "/UpdatePlayer";

        public async Task<IList<QuizGame>> GetGamesByPlayerId(string playerId)
        {
            string urlRequest = getGames + "?playerId=" + playerId;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<QuizGame>>(urlRequest);
            }
        }

        public async Task<QuizPlayer> GetPlayerById(string playerId)
        {
            string urlRequest = getplayer + "?playerId=" + playerId;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<QuizPlayer>(urlRequest);
            }
        }

        public async Task<string> GetOrCreatePlayerId(string playerName = null)
        {
            string urlRequest = createplayer;
            if (!string.IsNullOrEmpty(playerName))
            {
                urlRequest += "?playerName=" + playerName;
            }

            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<string>(urlRequest);
            }

        }

        public async Task<bool> UpdatePlayer(string playerId, string playerName, string characterData = null, string colors = null)
        {
            WWWForm form = new WWWForm();
            form.AddField("playerId", playerId);
            form.AddField("player", playerName);
            string urlRequest = updatePlayer + "?playerId=" + playerId + "&playerName=" + playerName;

            if (!string.IsNullOrEmpty(colors))
            {
                form.AddField("colors", colors);
                urlRequest += "&colors=" + colors;
            }

            if (!string.IsNullOrEmpty(characterData))
            {
                form.AddField("characterData", characterData);
                urlRequest += "&characterData=" + characterData;
            }

            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<bool>(urlRequest, form);
            }
        }
    }
}