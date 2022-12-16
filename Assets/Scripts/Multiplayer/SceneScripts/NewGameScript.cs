#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using Assets.Scripts.Multiplayer.API;
using Assets.Scripts.Multiplayer.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameScript : MonoBehaviour
{
  public Text friendId;
  public GameObject txtError;
  public GameObject txtSameId;
  private APIClientGame _apiClientGame;
  private APIClientRound _apiClientRound;
  private APIClientPushNotificationClient _apiClientPushNotificationClient;
  // Start is called before the first frame update
  void Start()
  {
    GameController controller = GameController.GetInstance();
    string bearerToken = controller.gameAssets.gameConfig.apiConfig.accessToken;

    _apiClientGame = new APIClientGame();
    _apiClientRound = new APIClientRound();
    _apiClientPushNotificationClient = new APIClientPushNotificationClient(bearerToken);
    
    DisableErrorText();
    DisableSameId();
  }

  private void DisableSameId()
  {
    txtSameId.SetActive(false);
  }

  void DisableErrorText()
  {
    txtError.SetActive(false);
  }

  public async void FindFriend()
  {
    try
    {
      DisableErrorText();
      DisableSameId();
      if (!string.IsNullOrEmpty(friendId.text) && friendId.text != MultiplayerGameManager.getPlayerId())
      {
        var id = await _apiClientGame.SearchOpenGameOfFriend(MultiplayerGameManager.getPlayerId(), friendId.text);
        if (id > -1)
        {
          //Der andere Spieler hat das Spiel zuerst eröffnet
          await OpenNewGame(friendId.text);
          await _apiClientPushNotificationClient.SendNotification(
            new ApiNotification(
              friendId.text,
              "Das Spiel wurde von " + MultiplayerGameManager.getPlayerName() + " akzeptiert",
              "Das Spiel beginnt",
              new ApiNotificationData(NotificationType.RequestAccepted)
            )
          );
        }
        else
        {
          await OpenNewRequest();
          var result = await _apiClientPushNotificationClient.SendNotification(
            new ApiNotification(
              friendId.text,
              "Du hast eine Anfrage von " + MultiplayerGameManager.getPlayerName() + " erhalten",
              "Schnell, nehme es an",
              new ApiNotificationData(NotificationType.RequestReceived)
            )
          );
        }
      }
      else if (friendId.text == MultiplayerGameManager.getPlayerId())
      {
        txtSameId.SetActive(true);
      }
    }
    catch (Exception e)
    {
      UnityLogger.GetLogger().Error(e, nameof(NewGameScript) + " FindFriend");
    }

  }

  private async System.Threading.Tasks.Task OpenNewRequest()
  {
    try
    {
      await MultiplayerGameManager.SendCharacterDataToDb();
      var searchId = await _apiClientGame.OpenNewSearch(MultiplayerGameManager.getPlayerId(), friendId.text);
      if (searchId > -1)
      {
        SceneManager.LoadScene("RequestStatus");
      }
      else
      {
        txtError.SetActive(true);
      }
    }
    catch (Exception e)
    {
      UnityLogger.GetLogger().Error(e, nameof(NewGameScript) + " OpenNewRequest");
    }

  }

  private async System.Threading.Tasks.Task OpenNewGame(string player1)
  {
    await MultiplayerGameManager.SendCharacterDataToDb();
    var gameID = await _apiClientGame.OpenNewGame(player1, MultiplayerGameManager.getPlayerId());

    if (gameID > -1)
    {
      MultiplayerGameManager.SetGameId(gameID);
      MultiplayerGameManager.SetRoundNumber(1);
      SceneManager.LoadScene("StartGame");
    }
    else
    {
      txtError.SetActive(true);
    }
  }

  public async void RandomGame()
  {
    try
    {
      var id = await _apiClientGame.SearchOpenGameRandom(MultiplayerGameManager.getPlayerId());
      if (id > -1)
      {
        var request = (await _apiClientGame.GetOpenRequests(MultiplayerGameManager.getPlayerId())).FirstOrDefault(x => x.ID == id);
        await _apiClientGame.UpdateSearch(request.ID, request.Player1ID, request.Player2ID, true);
        //Der andere Spieler hat das Spiel zuerst eröffnet
        await OpenNewGame(request.Player1ID);
      }
      else
      {
        await OpenNewRequest();
      }
    }
    catch (Exception e)
    {
      UnityLogger.GetLogger().Error(e, nameof(NewGameScript) + " RandomGame");
    }

  }

  // Update is called once per frame
  void Update()
  {

  }

}
