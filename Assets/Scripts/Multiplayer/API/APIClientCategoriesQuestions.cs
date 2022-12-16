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

namespace Assets.Scripts.Multiplayer.API
{
    public class APIClientCategoriesQuestions
    {
        private static string categoryquestionapi = MultiplayerGameManager.gameapi + "/QuizGameCategoriesQuestions";
        private static string validcategoriesapi = categoryquestionapi + "/GetAllCategoriesWithEnoughQuestions";
        private static string questionsapi = categoryquestionapi + "/GetQuestionsByCatId";
        private static string questionapi = categoryquestionapi + "/GetQuestionByID";

        public async Task<IList<QuizCategory>> GetValidCategories()
        {
            var urlRequest = validcategoriesapi;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<QuizCategory>>(urlRequest);
            }
        }
        
        public async Task<IList<QuizQuestion>> GetQuestionsByCategory(int catId)
        {
            var urlRequest = questionsapi+"?ID="+catId;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<QuizQuestion>>(urlRequest);
            }
        }
        
        public async Task<IList<QuizQuestion>> GetQuestionsById(int qId)
        {
            var urlRequest = questionapi+"?ID="+qId;
            using (AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption()))
            {
                return await httpClient.Get<IList<QuizQuestion>>(urlRequest);
            }
        }

    }
}