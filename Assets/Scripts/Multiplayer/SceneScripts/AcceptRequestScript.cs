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
  public class AcceptRequestScript : MonoBehaviour
  {
    public Text txtInfo;
    private APIClientGame _apiClientGame;
    private QuizOpenRequest _request;
    private APIClientRound _apiClientRound;
    private APIClientPushNotificationClient _apiClientPushNotificationClient;
    string ErrorAnfrage = "Beim Löschen der Anfrage trat ein Fehler auf";
    // Start is called before the first frame update
    void Start()
    {
      GameController controller = GameController.GetInstance();
      string bearerToken = controller.gameAssets.gameConfig.apiConfig.accessToken;

      _apiClientGame = new APIClientGame();
      _apiClientRound = new APIClientRound();
      _apiClientPushNotificationClient = new APIClientPushNotificationClient(bearerToken);

      InitView();
    }

    private async void InitView()
    {
      try
      {
        var openRequests = await _apiClientGame.GetOpenRequests(MultiplayerGameManager.getPlayerId());
        if (openRequests != null && openRequests.Any())
        {
          _request = openRequests.FirstOrDefault(x => x.ID == MultiplayerGameManager.RequestId);
          var playerName = await new PlayerHelper().GetPlayerNameByRequest(_request);
          txtInfo.text += " " + playerName + " möchte mit dir ein Spiel beginnen.";
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        UnityLogger.GetLogger().Error(e, nameof(AcceptRequestScript) + " InitView");
      }

    }

    public async void DeclineRequest()
    {
      if (!await _apiClientGame.DeleteSearch(MultiplayerGameManager.RequestId))
      {
        Debug.Log(ErrorAnfrage);
        UnityLogger.GetLogger().Error("Beim Löschen der Anfrage " + MultiplayerGameManager.RequestId + " trat ein Fehler auf", "AcceptRequestScript");
      }
      SceneManager.LoadScene("Overview");
    }

    public async void AcceptRequest()
    {
      try
      {
        await _apiClientGame.UpdateSearch(_request.ID, _request.Player1ID, _request.Player2ID, true);
        var gameId = await _apiClientGame.OpenNewGame(_request.Player1ID, _request.Player2ID);

        await _apiClientRound.InsertNewRound(MultiplayerGameManager.GameId, 1);
        await _apiClientPushNotificationClient.SendNotification(
          new ApiNotification(
            _request.Player1ID,
            "Das Spiel wurde von " + MultiplayerGameManager.getPlayerName() + " akzeptiert",
            "Das Spiel beginnt",
            new ApiNotificationData(NotificationType.RequestAccepted)
          )
        );

        MultiplayerGameManager.SetGameId(gameId);
        SceneManager.LoadScene("StartGame");
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        UnityLogger.GetLogger().Error(e, "AcceptRequestScript:AcceptRequest");
      }

    }
  }
}
