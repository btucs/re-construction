#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Multiplayer.API;
using Assets.Scripts.Multiplayer.API.Models;
using Assets.Scripts.Multiplayer.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Multiplayer.SceneScripts
{
    public class GameFinishedScript : MonoBehaviour
    {
        public Text TxtGameStatus;

        public Text TxtInfos;

        public Text TxtPoints;
        
        public Text TxtLeistung;
        
        private APIClientGame _apiClientGame;

        private APIClientRound _apiClientRound;

        private APIClientLeaderboard _apiClientLeaderboard;
       
        // Start is called before the first frame update
        void Start()
        {
            _apiClientGame = new APIClientGame();
            _apiClientRound = new APIClientRound();
            _apiClientLeaderboard = new APIClientLeaderboard();
            CloseGame();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public async void Revanche()
        {
            await _apiClientGame.OpenNewSearch(MultiplayerGameManager.getPlayerId(), MultiplayerGameManager.Player2.PlayerID);
            SceneManager.LoadScene("RequestStatus");
        }
        
        private async void CloseGame()
        {
            try
            {
                var game = (await _apiClientGame.GetGame(MultiplayerGameManager.GameId)).FirstOrDefault();
                var rounds = await _apiClientRound.GetAllRoundsByGameId(MultiplayerGameManager.GameId);
                var points = RoundHelper.CalculatePoints(rounds, 10);
                var currentPoints = GetCurrentPointsOfPlayer(points, game);
                
                await UpdateLeaderboard(game, points);
                await UpdateGame(points, game);

                SetTxtView(currentPoints, game.WonBy, MultiplayerGameManager.Player2.PlayerName);
                MultiplayerGameManager.ResetCompleteValues();
            }
            catch (Exception e)
            {
                UnityLogger.GetLogger().Error(e, nameof(GameFinishedScript) + " CloseGame");
            }

        }

        private async System.Threading.Tasks.Task UpdateLeaderboard(QuizGame game, int[] points)
        {
            await _apiClientLeaderboard.InsertOrUpdateEntry(game.Player1ID, points[0]);
            await _apiClientLeaderboard.InsertOrUpdateEntry(game.Player2ID, points[1]);
        }

        private int GetCurrentPointsOfPlayer(int[] points, QuizGame game)
        {
            var currentPoints = points[1];
            if (game.Player1ID == MultiplayerGameManager.getPlayerId())
            {
                currentPoints = points[0];
            }
            return currentPoints;
        }

        private async System.Threading.Tasks.Task UpdateGame(int[] points, QuizGame game)
        {
            game.IsFinished = true;
            
            if (points[0] > points[1])
            {
                game.WonBy = game.Player1ID;
            }
            else if (points[0] == points[1])
            {
                game.WonBy = "None";
            }
            else
            {
                game.WonBy = game.Player2ID;
            }

            await _apiClientGame.UpdateGame(game.Player1ID, game.Player2ID, game.WonBy, game.IsFinished, game.ID);
        }

        void SetTxtView(int points, string wonBy, string opponent)
        {
           
            if (wonBy == MultiplayerGameManager.getPlayerId())
            {
                TxtGameStatus.text = "Gewonnen!";
                TxtGameStatus.color = MultiplayerGameManager.cGewonnen;
                TxtInfos.text = "Du hast das Spiel gegen " + opponent + " gewonnen";
            }
            else if (wonBy == "None")
            {
                TxtGameStatus.text = "Unentschieden!";
                TxtGameStatus.color = MultiplayerGameManager.cUnentschieden;
                TxtInfos.text = "Das Spiel gegen " + opponent + " endete unentschieden";
            }
            else
            {
                TxtGameStatus.text = "Verloren!";
                TxtGameStatus.color = MultiplayerGameManager.cVerloren;
                TxtInfos.text = "Du hast das Spiel gegen " + opponent + " verloren";
            }
            TxtPoints.text = "+ " + points + " Ranglistenpunkte";
            TxtLeistung.text = "Für deine Leistung erhälst du: " + " + " + points; 
            MultiplayerGameManager.SavePointsToPlayer(points);
            TxtPoints.color = TxtGameStatus.color;
        }

    }
}
