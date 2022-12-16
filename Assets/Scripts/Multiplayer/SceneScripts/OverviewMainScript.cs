#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Globalization;
using Assets.Scripts.Multiplayer.API;
using Assets.Scripts.Multiplayer.API.Helper;
using Assets.Scripts.Multiplayer.API.Models;
using UnityEngine.SceneManagement;
using PullToRefresh;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Multiplayer.Logging;

public class OverviewMainScript : MonoBehaviour
{
    public GameObject txtWon;
    public GameObject txtOpenGames;
    public GameObject ScrollView;
    public GameObject EntryObjectPrefab;
    public GameObject NoInternetConnection;
    public Text NoGames;
    public PopupPlayerName PopUp;
    private static string newGame = "Neues Spiel";
    private static string enemyRequest = "Gegnersuche";
    private static string friendRequest = "Warte auf Spielannahme von ";
    private static string playAgainst = "Spiel gegen ";
    private static string yourTurn = " Du bist an der Reihe!";
    const string opponentTurn = "Gegenspieler ist an der Reihe";
    const string startGame = "Warte auf Spielstart von ";
    const string info = "Info";
    const string status = "Status";
    const string game = "game";
    const string request = "request";
    const string content = "Content";
    private APIClientPlayer _apiPlayerClient;
    private APIClientGame _apiGameClient;
    private APIClientRound _apiClientRound;
    [SerializeField] public UIRefreshControl m_UIRefreshControl;
    // Start is called before the first frame update
    void Start()
    {
        // This registration is possible even from Inspector.
        m_UIRefreshControl.OnRefresh.AddListener(RefreshItems);

        if (string.IsNullOrEmpty(MultiplayerGameManager.getPlayerName()))
        {
            PopUp.Popup();
        }
        gameObject.AddComponent<FirebaseHandler>();
        _apiPlayerClient = new APIClientPlayer();
        _apiGameClient = new APIClientGame();
        _apiClientRound = new APIClientRound();
        SetWonAndCurrentGamesToGui();
    }

    private void RefreshItems()
    {
        StartCoroutine(FetchData());
    }

    private IEnumerator FetchData()
    {
        // Instead of data acquisition.
        SetWonAndCurrentGamesToGui();
        yield return new WaitForSeconds(1.5f);
        // Call EndRefreshing() when refresh is over.
        m_UIRefreshControl.EndRefreshing();
    }

    // Register the callback you want to call to OnRefresh when refresh starts.
    public void OnRefreshCallback()
    {
        UnityLogger.GetLogger().Information("OnRefresh called.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private async void SetWonAndCurrentGamesToGui()
    {
        try
        {
            var playerId =  MultiplayerGameManager.getPlayerId();
            if (!string.IsNullOrEmpty(playerId))
            {
                var listOfGames = await _apiPlayerClient.GetGamesByPlayerId(playerId);
                var listOfRequest = await _apiGameClient.GetOpenRequests(playerId);
                if (!(listOfGames?.Count > 0) && !(listOfRequest?.Count > 0)) return;
                var countOpenGamesRequests = listOfGames.Count() + listOfRequest.Count();
                SetTextInformations(listOfGames, countOpenGamesRequests);
                await InitScrollView(countOpenGamesRequests, listOfGames, listOfRequest);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            UnityLogger.GetLogger().Error(e, nameof(OverviewMainScript) + " SetWonAndCurrentGamesToGUI");
        }
    }

    private async System.Threading.Tasks.Task InitScrollView(int countOpenGamesRequests, IList<QuizGame> listOfGames, IList<QuizOpenRequest> listOfRequest)
    {
        if (ScrollView != null && countOpenGamesRequests > 0)
        {
            ScrollView.SetActive(true);
            var Temp = ScrollView.GetComponentsInChildren<Transform>();
            foreach (var Child in Temp)
            {
                if (Child.name != content) continue;
                RemoveOldChildren(Child);
                await AddQuizGamesToScrollView(listOfGames, Child);
                AddRequestsToScrollView(listOfRequest, Child);
                break;
            }
        }
        else if (ScrollView != null)
        {
            ScrollView.SetActive(false);
            NoGames.gameObject.SetActive(true);
        }
    }

    private void SetTextInformations(IEnumerable<QuizGame> listOfGames, int countOpenGamesRequests)
    {
        NoGames.gameObject.SetActive(false);
        txtWon.GetComponent<Text>().text = listOfGames
            ?.Where(x => x.WonBy == MultiplayerGameManager.getPlayerId()).Count().ToString();
        txtOpenGames.GetComponent<Text>().text = countOpenGamesRequests.ToString();
    }

    private void AddRequestsToScrollView(IEnumerable<QuizOpenRequest> listOfRequest, Component Child)
    {
        foreach (QuizOpenRequest requestObject in listOfRequest)
        {
            var listObject = InitListObject();
            ObjectInfoHelper.AddObjectInfo(listObject, requestObject.ID, request);

            SetItemRequestInfo(listObject, requestObject);
            SetItemRequestStatus(listObject, requestObject);

            listObject.transform.SetParent(Child.transform, false);
        }
    }

    private static void SetItemRequestStatus(GameObject listObject, QuizOpenRequest requestObject)
    {
        var textStatus = listObject.transform.Find(status).gameObject.GetComponent<Text>();
        if (!string.IsNullOrEmpty(requestObject.Player1ID) &&
            !string.IsNullOrEmpty(requestObject.Player2ID))
        {
            textStatus.text = friendRequest;
            if (requestObject.Player1ID !=  MultiplayerGameManager.getPlayerId())
                textStatus.text += "dir!";
            else
                textStatus.text += requestObject.Player2ID;
        }
        else
        {
            textStatus.text = enemyRequest;
        }
    }

    private static void SetItemRequestInfo(GameObject listObject, QuizOpenRequest requestObject)
    {
        var textInfo = listObject.transform.Find(info).gameObject.GetComponent<Text>();

        textInfo.text = newGame + " (" +
                        requestObject.CreateDateTime.ToString("dd.MM.yyyy",
                            CultureInfo.InvariantCulture) + ")";
    }

    private async System.Threading.Tasks.Task AddQuizGamesToScrollView(IList<QuizGame> listOfGames, Transform Child)
    {
        foreach (QuizGame quizgame in listOfGames.Where(x => x.IsFinished == false))
        {
            var listObject = InitListObject();
            ObjectInfoHelper.AddObjectInfo(listObject, quizgame.ID, game);

            await SetItemGameInfo(listObject, quizgame);
            await SetItemGameStatus(listObject, quizgame);

            listObject.transform.SetParent(Child.transform, false);
        }
    }

    private async System.Threading.Tasks.Task SetItemGameInfo(GameObject listObject, QuizGame quizgame)
    {
        var textInfo = listObject.transform.Find(info).gameObject.GetComponent<Text>();
        textInfo.text = playAgainst;
        textInfo.text += quizgame.Player1ID ==  MultiplayerGameManager.getPlayerId()
            ? (await _apiPlayerClient.GetPlayerById(quizgame.Player2ID)).PlayerName
            : (await _apiPlayerClient.GetPlayerById(quizgame.Player1ID)).PlayerName;
    }

    private async System.Threading.Tasks.Task SetItemGameStatus(GameObject listObject, QuizGame quizgame)
    {
        var textStatus = listObject.transform.Find(status).gameObject.GetComponent<Text>();
        var currentRound = (await _apiClientRound.GetOpenRoundsByGameId(quizgame.ID))
            ?.OrderByDescending(x => x.RoundNumber)?.FirstOrDefault();

        if (currentRound != null)
        {
            SetRoundInfosToItem(textStatus, currentRound, quizgame);
        }
        else
        {
            textStatus.text = startGame;
            if (quizgame.Player2ID ==  MultiplayerGameManager.getPlayerId())
                textStatus.text += "dir!";
            else
                textStatus.text += quizgame.Player1ID;
        }
    }

    private static void SetRoundInfosToItem(Text textStatus, QuizRound currentRound, QuizGame quizgame)
    {
        textStatus.text = "Runde " + currentRound.RoundNumber + " ";

        textStatus.text += CheckTurn(currentRound, quizgame);
    }

    private static string CheckTurn(QuizRound currentRound, QuizGame quizgame)
    {
        if (string.IsNullOrEmpty(currentRound.P2AnswerIds) &&
            quizgame.Player2ID ==  MultiplayerGameManager.getPlayerId() && currentRound.RoundNumber % 2 == 1
            || !string.IsNullOrEmpty(currentRound.P2AnswerIds) &&
            string.IsNullOrEmpty(currentRound.P1AnswerIds) &&
            currentRound.RoundNumber % 2 == 1 &&
            quizgame.Player1ID ==  MultiplayerGameManager.getPlayerId()
            || currentRound.RoundNumber == 2 && string.IsNullOrEmpty(currentRound.P1AnswerIds) &&
            quizgame.Player1ID ==  MultiplayerGameManager.getPlayerId()
            || currentRound.RoundNumber == 2 && !string.IsNullOrEmpty(currentRound.P1AnswerIds) &&
            string.IsNullOrEmpty(currentRound.P2AnswerIds) &&
            quizgame.Player2ID ==  MultiplayerGameManager.getPlayerId())
        {
            return yourTurn;
        }

        return !currentRound.IsFinished ? opponentTurn : null;
    }

    private static void RemoveOldChildren(Transform Child)
    {
        if (Child.transform.childCount > 0)
        {
            int count = Child.transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                if (Child.transform.GetChild(i).gameObject.name == "LoadingDots")
                    continue;
                GameObject.DestroyImmediate(Child.transform.GetChild(i).gameObject);
            }
        }
    }

    private GameObject InitListObject()
    {
        var game = Instantiate(EntryObjectPrefab);
        var btn = game.GetComponent<Button>();
        btn.onClick.AddListener(delegate { SetInfosToGameManager(game); });
        return game;
    }

    private async void SetInfosToGameManager(GameObject listObject)
    {
        var infos = listObject.gameObject.GetComponent<ObjectInfo>();

        switch (infos?.name)
        {
            case request:
            {
                MultiplayerGameManager.SetRequestId(infos.Id);
                var request =
                    (await _apiGameClient.GetOpenRequests( MultiplayerGameManager.getPlayerId())).FirstOrDefault(x =>
                        x.ID == infos.Id);
                SceneManager.LoadScene(request.Player1ID ==  MultiplayerGameManager.getPlayerId()
                    ? "RequestStatus"
                    : "AcceptRequest");

                break;
            }
            case game:
            {
                MultiplayerGameManager.SetGameId(infos.Id);
                var currentRound = (await _apiClientRound.GetOpenRoundsByGameId(infos.Id))
                    .OrderByDescending(x => x.RoundNumber).FirstOrDefault();
                var currentGame = (await _apiGameClient.GetGame(infos.Id)).FirstOrDefault();
                    MultiplayerGameManager.SetPlayer2(await new PlayerHelper().GetPlayerByGame(currentGame));
                if (currentRound != null)
                {
                    LoadSceneWithRound(currentRound, currentGame);
                }
                else if (currentGame.Player2ID ==  MultiplayerGameManager.getPlayerId())
                {
                    await PlayerStartsGame(-1);
                }

                break;
            }
        }
    }

    private async System.Threading.Tasks.Task PlayerStartsGame(int roundId)
    {
        if (roundId == -1)
        {
           await _apiClientRound.InsertNewRound(MultiplayerGameManager.GameId, 1);
        }
        MultiplayerGameManager.SetRoundNumber(1);
        SceneManager.LoadScene("StartGame");
    }

    private async void LoadSceneWithRound(QuizRound currentRound, QuizGame currentGame)
    {
        if (string.IsNullOrEmpty(currentRound.P2AnswerIds) &&
            currentGame.Player2ID ==  MultiplayerGameManager.getPlayerId() &&
            currentRound.RoundNumber == 1)
        {
            await PlayerStartsGame(currentRound.ID);
        }
        else if (currentRound.QuizCatId <= 0 && currentRound.RoundNumber == 2 &&
          string.IsNullOrEmpty(currentRound.P1AnswerIds) &&
          currentGame.Player1ID ==  MultiplayerGameManager.getPlayerId())
        {
            SceneManager.LoadScene("ChooseCategory");
        }
        else if (currentRound.QuizCatId <= 0 && currentRound.RoundNumber == 3 &&
          string.IsNullOrEmpty(currentRound.P2AnswerIds) &&
          currentGame.Player2ID ==  MultiplayerGameManager.getPlayerId())
        {
            SceneManager.LoadScene("ChooseCategory");
        }
        else
        {
            SceneManager.LoadScene("ResultOverview");
        }

    }
}
