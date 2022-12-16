#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Multiplayer.API.Models;
using UnityEngine;

namespace Assets.Scripts.Multiplayer.API
{
    public class APIClientLeaderboard
    {
        private static string leaderboardapi = MultiplayerGameManager.gameapi + "/QuizGameLeaderboard";
        private static string seasonapi = MultiplayerGameManager.gameapi + "/QuizGameSeason";
        private static string getleaderboardapi = leaderboardapi + "/GetLeaderboard";
        private static string postleaderboardapi = leaderboardapi + "/InsertUpdateEntry";
        private static string getleaderboardreset= leaderboardapi + "/GetResetDateTime";
        private static string getcurrentseasonid = seasonapi + "/GetCurrentSeasonId";

        private static string getleaderboardbyplayerandseasonid =
            leaderboardapi + "/GetLeaderboardByPlayerIDAndSeasonId";

        private static string getleaderboardbyplayeridnotseenswitch = leaderboardapi+
            "/GetLeaderboardByPlayerIDWhereNotSeenSeasonSwitch";
        public async Task<IList<QuizLeaderboard>> GetLeaderboardOfCurrentSeason()
        {
            var seasonId = await GetSeasonId();
            string urlRequest = getleaderboardapi + "?seasonId="+seasonId;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<QuizLeaderboard>>(urlRequest);
            }

        }
        
        public async Task<IList<QuizLeaderboard>> GetLeaderboardBySeason(int seasonId)
        {
            string urlRequest = getleaderboardapi + "?seasonId="+seasonId;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<QuizLeaderboard>>(urlRequest);
            }
        }
        
        public async Task<IList<QuizLeaderboard>> GetLeaderboardByPlayerAndSeasonId(string playerId)
        {
            var seasonId = await GetSeasonId();
            string urlRequest = getleaderboardbyplayerandseasonid + "?playerId="+ playerId +"&seasonId="+seasonId;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<QuizLeaderboard>>(urlRequest);
            }
        }
        
        public async Task<IList<QuizLeaderboard>> GetLeaderboardByPlayerIdNotSeenSwitch(string playerId)
        {
            var seasonId = await GetSeasonId();
            string urlRequest = getleaderboardbyplayeridnotseenswitch + "?playerId="+ playerId + "&seasonId=" + seasonId;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<QuizLeaderboard>>(urlRequest);
            }
        }
        
        private async Task<int> GetSeasonId()
        {
            string urlRequest = getcurrentseasonid;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<int>(urlRequest);
            }
        }
        
        public async Task<IList<string>> GetResetDateTime()
        {
            string urlRequest = getleaderboardreset;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<string>>(urlRequest);
            }
        }

        public async Task<bool> InsertOrUpdateEntry(string playerId, int points, int seasonId = -1, bool seenSeasonSwitch = false)
        {
            if (seasonId == -1)
            {
                seasonId = await GetSeasonId();
            }

            WWWForm form = new WWWForm();
            form.AddField("playerId", playerId);
            form.AddField("points", points);
            form.AddField("seasonId", seasonId);
            var urlRequest = postleaderboardapi + "?playerId=" + playerId + "&points=" + points + "&seasonId=" + seasonId;
            if (seenSeasonSwitch)
            {
                form.AddField("seenSeasonSwitch", seenSeasonSwitch.ToString());
                urlRequest += "&seenSeasonSwitch=" + seenSeasonSwitch;
            }

            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<bool>(urlRequest, form);
            }
        }
    }
}