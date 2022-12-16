#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Multiplayer.API.Models;
using UnityEngine;

namespace Assets.Scripts.Multiplayer.API
{
    public class APIClientGame
    {
        private static string mainapi = MultiplayerGameManager.gameapi + "/QuizGameMain";
        private static string requestapi = MultiplayerGameManager.gameapi + "/QuizGameRequest";
        private static string opengame = mainapi + "/OpenNewGame";
        private static string updategame = mainapi + "/UpdateGame";
        private static string updaterequest= requestapi + "/UpdateRequest";
        private static string deleterequest = requestapi + "/DeleteOpenRequest";
        private static string opennewrequest = requestapi + "/OpenNewRequest";
        private static string getopenrequests = requestapi + "/GetOpenRequests";
        private static string getgame = mainapi + "/GetGameByGameId";
        private static string searchopengamefriend = requestapi + "/LookupOpenRequestWithPlayer2";
        private static string searchopengamerandom = requestapi + "/LookupOpenRequestWithRandomPlayer";
        
        public async Task<IList<QuizOpenRequest>> GetOpenRequests(string playerId = null)
        {
            var urlRequest = getopenrequests;
            if(!string.IsNullOrEmpty(playerId))
            {
                urlRequest += "?playerId=" + playerId;
            }

            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<QuizOpenRequest>>(urlRequest);
            }           
        }

        public async Task<IList<QuizGame>> GetGame(int gameId)
        {
            var urlRequest = getgame + "?gameId=" + gameId;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<QuizGame>>(urlRequest);
            }
        }

        public async Task<int> OpenNewGame(string playerId, string playerId2)
        {
            WWWForm form = new WWWForm();
            form.AddField("player1Id", playerId);
            form.AddField("player2Id", playerId2);
            var urlRequest = opengame + "?player1Id=" + playerId + "&player2Id=" + playerId2;

            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<int>(urlRequest, form);
            }
        }

        public async Task<int> UpdateGame(string player1Id, string player2Id, string wonBy, bool isFinished, int id)
        {
            WWWForm form = new WWWForm();
            form.AddField("player1Id", player1Id);
            form.AddField("player2Id", player2Id);
            form.AddField("isFinished", isFinished.ToString());
            form.AddField("gameId", id);

            var urlRequest = updategame
                + "?player1Id=" + player1Id
                + "&player2Id=" + player2Id
                + "&isFinished=" + isFinished
                + "&gameId=" + id;

            if (!string.IsNullOrEmpty(wonBy))
            {
                form.AddField("wonBy", wonBy);
                urlRequest += "&wonBy=" + wonBy;
            }


            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<int>(urlRequest, form);
            }
        }

        public async Task<int> SearchOpenGameOfFriend(string playerId, string playerId2)
        {
            WWWForm form = new WWWForm();
            form.AddField("player1Id", playerId);
            form.AddField("player2Id", playerId2);
            var urlRequest = searchopengamefriend + "?player1Id=" + playerId + "&player2Id=" + playerId2;

            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<int>(urlRequest, form);
            }
        }
        
        public async Task<int> SearchOpenGameRandom(string playerId)
        {
            WWWForm form = new WWWForm();
            form.AddField("player1Id", playerId);
            var urlRequest = searchopengamerandom + "?player1Id=" + playerId;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<int>(urlRequest, form);
            }
        }
        
        public async Task<int> OpenNewSearch(string playerId, string playerId2)
        {
            WWWForm form = new WWWForm();
            form.AddField("playerId1", playerId);
            var urlRequest = opennewrequest + "?playerId1=" + playerId;
            if (!string.IsNullOrEmpty(playerId2))
            {
                form.AddField("playerId2", playerId2);
                urlRequest += "&playerId2=" + playerId2;
            }

            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<int>(urlRequest, form);
            }
        }
        
        public async Task<int> UpdateSearch(int Id, string playerId, string playerId2, bool accepted)
        {
            WWWForm form = new WWWForm();
            form.AddField("ID", Id);
            form.AddField("player1Id", playerId);
            form.AddField("player2Id", playerId2);
            form.AddField("accepted", accepted.ToString());
            var urlRequest = updaterequest + "?ID="+ Id +"&player1Id=" + playerId + "&player2Id=" + playerId2+"&accepted="+accepted;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<int>(urlRequest, form);
            }
        }
        
        public async Task<bool> DeleteSearch(int Id)
        {
            WWWForm form = new WWWForm();
            form.AddField("rowId", Id.ToString());
            var urlRequest = deleterequest + "?rowId=" + Id;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<bool>(urlRequest, form);
            }
        }
    }
}