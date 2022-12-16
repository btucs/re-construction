#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Multiplayer.API.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Multiplayer.API.Helper
{
    public class CheckReqGamePopUp : MonoBehaviour
    {
        public GameObject popUpBox;
        private APIClientGame _apiClientGame = new APIClientGame();
        private APIClientPlayer _apiClientPlayer = new APIClientPlayer();
        private APIClientRound _apiClientRound = new APIClientRound();
        private HashSet<int> excludedReqIds;
        private HashSet<int> excludedGameIds;
        private const string reqText = "Es ist eine neue Anfrage eingegangen";
        private const string roundText = "Du bist an der Reihe";
        private const string gameText = "Es gibt ein neues offenes Spiel";

        private void Awake()
        {
            InitLists();
        }

        IEnumerator Start() {
           
            while (true) {
                //Alle 5 Minuten
                yield return new WaitForSeconds(300f);
                CheckApi();
            }
        }

        private async void InitLists()
        {
            var oldRequests = (await _apiClientGame.GetOpenRequests( MultiplayerGameManager.getPlayerId())).ToList();
            excludedReqIds = new HashSet<int>(oldRequests.Select(p => p.ID));
            var oldGames = (await _apiClientPlayer.GetGamesByPlayerId( MultiplayerGameManager.getPlayerId())).ToList();
            excludedGameIds = new HashSet<int>(oldGames.Select(p => p.ID));
        }

        private async void CheckApi()
        {
           var requests = (await _apiClientGame.GetOpenRequests( MultiplayerGameManager.getPlayerId())).ToList();
           if (requests.Where(p => !excludedReqIds.Contains(p.ID)).Any())
           {
                excludedReqIds = new HashSet<int>(requests.Select(p => p.ID));
                CallPopUp(reqText);
           }
           else
           {
               var games = (await _apiClientPlayer.GetGamesByPlayerId( MultiplayerGameManager.getPlayerId())).ToList();
               if (games.Where(p => !excludedGameIds.Contains(p.ID)).Any())
               {
                    excludedGameIds = new HashSet<int>(games.Select(p => p.ID));
                    CallPopUp(gameText);
               }
               else if(games.Count > 0)
               {
                   foreach (var game in games)
                   {
                       var round = (await _apiClientRound.GetOpenRoundsByGameId(game.ID))
                           ?.OrderByDescending(x => x.RoundNumber).FirstOrDefault();
                       if (round == null) continue;
                       if (game.Player2ID == MultiplayerGameManager.getPlayerId() &&
                           string.IsNullOrEmpty(round.P2AnswerIds) && round.RoundNumber % 2 == 1  || game.Player2ID == MultiplayerGameManager.getPlayerId() && string.IsNullOrEmpty(round.P2AnswerIds) && !string.IsNullOrEmpty(round.P1AnswerIds))
                       {
                           CallPopUp(roundText);
                           break;
                       }

                       if (game.Player1ID == MultiplayerGameManager.getPlayerId() &&
                           string.IsNullOrEmpty(round.P1AnswerIds) && round.RoundNumber == 2 ||
                           game.Player1ID == MultiplayerGameManager.getPlayerId() &&
                           !string.IsNullOrEmpty(round.P2AnswerIds) && string.IsNullOrEmpty(round.P1AnswerIds))
                       {
                           CallPopUp(roundText);
                           break;
                       }
                   }
               }
           }
        }

        public void GoToMultiplayer()
        {
            popUpBox.SetActive(false);
            SceneManager.LoadScene("Overview");
        }

        public void Decline()
        {
            popUpBox.SetActive(false);
        }
        
        private void CallPopUp(string text)
        {
            popUpBox?.SetActive(true);
            var headerText = popUpBox.transform.Find("HeaderText").gameObject.GetComponent<Text>();
            headerText.text = text;
        }
    }
}