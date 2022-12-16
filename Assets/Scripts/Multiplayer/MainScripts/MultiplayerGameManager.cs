#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Multiplayer.API;
using Assets.Scripts.Multiplayer.API.Helper;
using Assets.Scripts.Multiplayer.API.Models;
using UnityEngine;

public class MultiplayerGameManager : MonoBehaviour
{
  public FirebaseHandler firebaseHandler;

  public static int GameId { get => _gameId; }
  public static int RequestId { get => _requestId; }
  public static int CategoryId { get => _categoryId; }
  public static int RoundNumber { get => _roundNumber; }
  public static int PointsOfCurrentGame { get; set; }
  public static string Topic { get => _topic; }
  public static QuizPlayer Player2
  {
    get
    {
      if (_player2 != null) return _player2;
      return GetCurrentPlayer2().Result;
    }
  }

  public static bool[] Answers = new bool[9];
  public static string AnswerIdsAsString;
  
  public static readonly Color32 cGewonnen = new Color32(81, 112, 94, 255);
  public static readonly Color32 cVerloren = new Color32(190, 102, 101, 255);
  public static readonly Color32 cUnentschieden = new Color32(188, 188, 164, 255);
  public static readonly Color32 cHighlight = new Color32(40, 40, 61, 255);
  public static readonly Color32 cWhite = new Color32(255, 255, 255, 255);

  private static int _roundNumber;
  private static MultiplayerGameManager _instance;
  private static int _gameId;
  private static int _requestId;
  private static int _categoryId;
  private static QuizPlayer _player2;
  private static string _topic = "quizgame";
    
  void Awake()
  {
    if (_instance == null)
    {
      DontDestroyOnLoad(gameObject);
      _instance = this;
    } else if (_instance != this)
    {
      Destroy(gameObject);
    }
  }

  public static void ResetCompleteValues()
  {
    _gameId = -1;
    _requestId = -1;
    _categoryId = -1;
    _roundNumber = -1;
    AnswerIdsAsString = null;
    PointsOfCurrentGame = 0;
    Answers = new bool[9];
  }

  public static void ResetRoundValues()
  {
    AnswerIdsAsString = null;
    PointsOfCurrentGame = 0;
  }

  public static string gameapi => GameController.GetInstance().gameAssets.gameConfig.apiConfig.apiBaseUrl;
  public static string bearerToken => GameController.GetInstance().gameAssets.gameConfig.apiConfig.accessToken;

  public static void AddAnswerEntry(int id, bool value)
  {
    Answers[id] = value;
  }

  public static string getPlayerName()
  {
    return GameController.GetInstance().gameState.multiplayer.playerName;
  }

  public static string getPlayerId()
  {
    return GameController.GetInstance().gameState.multiplayer.playerId;
  }

  public static CharacterSO getPlayerCharacter()
  {
    return GameController.GetInstance().gameState.characterData.player;
  }

  public static async Task<bool> SendCharacterDataToDb()
  {
    var character = CharacterAndBodyColorHelper.GetCharacterAsString();
    var colors = CharacterAndBodyColorHelper.GetCharacterColorsAsString();
    return await new APIClientPlayer().UpdatePlayer(getPlayerId(), getPlayerName(), character, colors);

  }

  public static void SavePlayerIdToPlayer(string playerId)
  {
    GameController.GetInstance().gameState.multiplayer.playerId = playerId;
    GameController.GetInstance().SaveGame();

    if(_instance.firebaseHandler != null)
    {
      _instance.firebaseHandler.InitializeFirebase();
    }
  }

  public static void SavePlayerName(string playerName)
  {
    GameController.GetInstance().gameState.multiplayer.playerName = playerName;
    GameController.GetInstance().SaveGame();
  }

  public static void SavePointsToPlayer(int points)
  {
    GameController.GetInstance().gameState.profileData.inventory.currencyAmount += points;
    GameController.GetInstance().SaveGame();
  }

  public static void SetRoundNumber(int number)
  {
    _roundNumber = number;
  }

  public static void SetGameId(int id)
  {
    _gameId = id;
  }

  public static void SetRequestId(int id)
  {
    _requestId = id;
  }

  public static void SetCategoryId(int id)
  {
    _categoryId = id;
  }

  public static void SetPlayer2(QuizPlayer player2)
  {
    _player2 = player2;
  }

  private async static Task<QuizPlayer> GetCurrentPlayer2()
  {
    var game = (await new APIClientGame().GetGame(GameId)).FirstOrDefault();
    if (game.Player1ID == getPlayerId())
    {
      _player2 = new APIClientPlayer().GetPlayerById(game.Player2ID).Result;
    }
    else
    {
      _player2 = new APIClientPlayer().GetPlayerById(game.Player1ID).Result;
    }
    return _player2;
  }
}
