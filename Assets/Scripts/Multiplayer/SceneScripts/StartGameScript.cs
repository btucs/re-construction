#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using Assets.Scripts.Multiplayer.API;
using Assets.Scripts.Multiplayer.API.Helper;
using Assets.Scripts.Multiplayer.API.Models;
using Assets.Scripts.Multiplayer.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Multiplayer.SceneScripts
{
    public class StartGameScript : MonoBehaviour
    {
        public Text P1Name;
        public Text P2Name;
        private APIClientGame _apiClientGame;
        private APIClientRound _apiClientRound;
        public characterGraphicsUpdater Player1;
        public characterGraphicsUpdater Player2;
        private QuizGame game;
        CreatePlayerCharacters createPlayerCharacters;
        // Start is called before the first frame update
        async void Start()
        {
            _apiClientGame = new APIClientGame();
            _apiClientRound = new APIClientRound();
            await InitStartGame();
        }

        private async System.Threading.Tasks.Task InitStartGame()
        {
            try
            {
                game = (await _apiClientGame.GetGame(MultiplayerGameManager.GameId)).FirstOrDefault();
                createPlayerCharacters = new CreatePlayerCharacters(Player1, Player2, game, P1Name, P2Name);
                await createPlayerCharacters.SetPlayersCharacters();
            }
            
            catch (Exception e)
            {
               UnityLogger.GetLogger().Error(e, nameof(StartGameScript) + " InitStartGame");
            }

        }  

        public async void LoadNextScene()
        {
            var currentRound = (await _apiClientRound.GetOpenRoundsByGameId(game.ID)).OrderByDescending(x=> x.RoundNumber).FirstOrDefault();
            if(currentRound == null)
            {
                await new APIClientRound().InsertNewRound(MultiplayerGameManager.GameId, 1);
                currentRound = (await _apiClientRound.GetOpenRoundsByGameId(game.ID)).OrderByDescending(x => x.RoundNumber).FirstOrDefault();

            }
            if (string.IsNullOrEmpty(currentRound.P2AnswerIds) &&
                game.Player2ID == MultiplayerGameManager.getPlayerId() && currentRound.RoundNumber % 2 == 1)
            {
                SceneManager.LoadScene("ChooseCategory");
            }
            else if (string.IsNullOrEmpty(currentRound.P1AnswerIds) &&
                     game.Player1ID == MultiplayerGameManager.getPlayerId() && currentRound.RoundNumber == 2)
            {
                SceneManager.LoadScene("ChooseCategory");
            }
            else
            {
                SceneManager.LoadScene("ResultOverview");
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
