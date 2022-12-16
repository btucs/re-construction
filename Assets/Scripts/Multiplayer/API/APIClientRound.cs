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
    public class APIClientRound
    {
        private static string roundapi = MultiplayerGameManager.gameapi + "/QuizGameRound";
        private static string openroundsapi = roundapi + "/GetRounds";
        private static string newroundapi = roundapi + "/InsertNewRound";
        private static string updateroundapi = roundapi + "/UpdateRound";

        public async Task<IList<QuizRound>> GetOpenRoundsByGameId(int gameId = -1, bool isFinished = false)
        {
            string urlRequest = openroundsapi + "?gameId=" + gameId + "&isFinished="+isFinished;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<QuizRound>>(urlRequest);
            }
        }
        
        public async Task<IList<QuizRound>> GetAllRoundsByGameId(int gameId = -1)
        {
            string mainurl = openroundsapi + "?gameId=" + gameId + "&isFinished=";
            string closedUrl = mainurl + "true";

            List<QuizRound> rounds = new List<QuizRound>();
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                var closedRounds = await httpClient.Get<IList<QuizRound>>(closedUrl);
                if (closedRounds?.Count > 0)
                {
                    rounds.AddRange(closedRounds);
                }
            }

            string openUrl = mainurl + "false";

            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                var openRounds = await httpClient.Get<IList<QuizRound>>(openUrl);
                if (openRounds?.Count > 0)
                {
                    rounds.AddRange(openRounds);
                }
            }
  
            return rounds;
        }

        public async Task<int> InsertNewRound(int gameId, int currentRound)
        {
            WWWForm form = new WWWForm();
            form.AddField("gameId", gameId);
            form.AddField("roundNumber", currentRound);
            var urlRequest = newroundapi + "?gameId=" + gameId +"&roundNumber=" + currentRound;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<int>(urlRequest, form);
            }
        }

        public async Task<int> UpdateRound(QuizRound quizRound)
        {
            WWWForm form = new WWWForm();
            form.AddField("Id", quizRound.ID);
            form.AddField("gameId", quizRound.QuizGameId);
            form.AddField("catId", quizRound.QuizCatId);
            form.AddField("P1Points", quizRound.S1Points);
            form.AddField("P2Points", quizRound.S2Points);
            form.AddField("roundNumber", quizRound.RoundNumber);
            form.AddField("isFinished", quizRound.IsFinished.ToString());

            var urlRequest = updateroundapi
                + "?Id="
                + quizRound.ID
                + "&gameId="
                + quizRound.QuizGameId
                + "&catId="
                + quizRound.QuizCatId
                + "&P1Points="
                + quizRound.S1Points
                + "&P2Points="
                + quizRound.S2Points
                + "&isFinished="
                + quizRound.IsFinished
                + "&roundNumber="
                + quizRound.RoundNumber;

            if (!string.IsNullOrEmpty(quizRound.QuestionIDs))
            {
                form.AddField("questionIds", quizRound?.QuestionIDs);
                urlRequest += "&questionIds=" + quizRound.QuestionIDs;
            }
            if(!string.IsNullOrEmpty(quizRound.P1AnswerIds))
            {
                form.AddField("P1Answers", quizRound.P1AnswerIds);
                urlRequest += "&P1Answers=" + quizRound.P1AnswerIds;
            }
            if (!string.IsNullOrEmpty(quizRound.P2AnswerIds))
            {
                urlRequest += "&P2Answers=" + quizRound.P2AnswerIds;
                form.AddField("P2Answers", quizRound.P2AnswerIds);
            }

            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Post<int>(urlRequest, form);
            }
        }
    }
}