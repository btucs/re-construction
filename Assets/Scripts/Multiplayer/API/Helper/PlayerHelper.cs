#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Threading.Tasks;
using Assets.Scripts.Multiplayer.API.Models;

namespace Assets.Scripts.Multiplayer.API.Helper
{
    public class PlayerHelper
    {
        private APIClientPlayer apiClientPlayer;
        public PlayerHelper()
        {
            apiClientPlayer = new APIClientPlayer();
        }
       
        public async Task<string> GetPlayerNameByGame(QuizGame quizGame)
        {
            return quizGame.Player1ID == MultiplayerGameManager.getPlayerId() 
                ? (await apiClientPlayer.GetPlayerById(quizGame.Player2ID)).PlayerName 
                : (await apiClientPlayer.GetPlayerById(quizGame.Player1ID)).PlayerName;
        }
        
        public async Task<QuizPlayer> GetPlayerByGame(QuizGame quizGame)
        {
            return quizGame.Player1ID ==  MultiplayerGameManager.getPlayerId() 
                ? await apiClientPlayer.GetPlayerById(quizGame.Player2ID) 
                : await apiClientPlayer.GetPlayerById(quizGame.Player1ID);
        }
        
        public async Task<string> GetPlayerNameByRequest(QuizOpenRequest request)
        {
            if(request.Player1ID == MultiplayerGameManager.getPlayerId())
            {
                return (await apiClientPlayer.GetPlayerById(request.Player2ID)).PlayerName;
            }

            return (await apiClientPlayer.GetPlayerById(request.Player1ID)).PlayerName;
        }
        
        public async Task<string> GetPlayerNameById(string id)
        {
            return (await apiClientPlayer.GetPlayerById(id)).PlayerName;
        }
    }
}