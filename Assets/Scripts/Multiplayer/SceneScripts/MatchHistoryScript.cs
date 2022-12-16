#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Multiplayer.API;
using Assets.Scripts.Multiplayer.API.Helper;
using Assets.Scripts.Multiplayer.API.Models;
using Assets.Scripts.Multiplayer.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchHistoryScript : MonoBehaviour
{
    public GameObject ScrollView;
    public GameObject EntryObjectPrefab;
    public GameObject CanvasScrollView;
    public GameObject CanvasNoEntries;
    private APIClientPlayer _apiClientPlayer;
    private const string _spielGegen = "Spiel gegen ";
    private const string _gewonnen = "gewonnen";
    private readonly string _verloren = "verloren";
    private readonly string _unentschieden = "unentschieden";
    // Start is called before the first frame update
    async void Start()
    {
        _apiClientPlayer = new APIClientPlayer();
        await InitMatchHistory();
    }

    private async System.Threading.Tasks.Task InitMatchHistory()
    {
        try
        {
            var listOfGames = await _apiClientPlayer.GetGamesByPlayerId(MultiplayerGameManager.getPlayerId());
            var finishedGames = listOfGames.Where(x => x.IsFinished);
            if (listOfGames == null || !finishedGames.Any())
            {
                CanvasNoEntries.SetActive(true);
                CanvasScrollView.SetActive(false);
            }
            else
            {
                CanvasScrollView.SetActive(true);
                CanvasNoEntries.SetActive(false);
                SetWonAndCurrentGamesToGUI(finishedGames.OrderByDescending(x=> x.ID));
            }
        }
        catch (Exception e)
        {
            UnityLogger.GetLogger().Error(e, nameof(MatchHistoryScript) + " InitMatchHistory");
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    async void SetWonAndCurrentGamesToGUI(IEnumerable<QuizGame> finishedGames)
    {
        if (ScrollView == null) return;
        var Temp = ScrollView.GetComponentsInChildren<Transform>();
        foreach (var Child in Temp)
        {
            if (Child.name != "Content") continue;

            foreach (var quizGame in finishedGames)
            {
                var game = InitListObject();
                ObjectInfoHelper.AddObjectInfo(game, quizGame.ID, "game");
                await SetInfoText(game, quizGame);
                SetButtonColorAndStatusText(game, quizGame);
                game.transform.SetParent(Child.transform, false);

            }
            break;
        }
    }

    private async System.Threading.Tasks.Task SetInfoText(GameObject game, QuizGame quizGame)
    {
        var textInfo = game.transform.Find("Info").gameObject.GetComponent<UnityEngine.UI.Text>();
        textInfo.color = Color.white;
        var opponentName = await new PlayerHelper().GetPlayerNameByGame(quizGame);
        textInfo.text = _spielGegen + opponentName + " (" +
                        quizGame.StartDateTime.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) + ")";
    }

    private void SetButtonColorAndStatusText(GameObject game, QuizGame quizGame)
    {
        var textStatus = game.transform.Find("Status").gameObject.GetComponent<UnityEngine.UI.Text>();
        textStatus.color = Color.gray;
        Color32 buttonColor = new Color32();

        if (quizGame.WonBy == MultiplayerGameManager.getPlayerId())
        {
            buttonColor = MultiplayerGameManager.cGewonnen;
            textStatus.text = _gewonnen;
        }
        else if (quizGame.WonBy == "None")
        {
            buttonColor = MultiplayerGameManager.cUnentschieden;
            textStatus.text = _unentschieden;
        }
        else
        {
            buttonColor = MultiplayerGameManager.cVerloren;
            textStatus.text = _verloren;
        }
        var button = game.transform.gameObject.GetComponent<Button>();
        ButtonHelper.SetButtonColor(button, buttonColor);
    }

    private GameObject InitListObject()
    {
        var game = Instantiate(EntryObjectPrefab);
        var btn = game.GetComponent<Button>();
        btn.onClick.AddListener(delegate { SetInfosToGameManager(game); });
        return game;
    }

    private void SetInfosToGameManager(GameObject listObject)
    {
        var infos = listObject.gameObject.GetComponent<ObjectInfo>();
        MultiplayerGameManager.SetGameId(infos.Id);
        SceneManager.LoadScene("ResultOverview");
    }
}
