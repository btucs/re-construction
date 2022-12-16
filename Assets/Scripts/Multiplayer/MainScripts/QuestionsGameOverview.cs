#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Multiplayer.API;
using Assets.Scripts.Multiplayer.API.Helper;
using Assets.Scripts.Multiplayer.API.Models;
using Assets.Scripts.Multiplayer.Logging;
using Assets.Scripts.Multiplayer.SceneScripts;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsGameOverview : MonoBehaviour
{
    public Text P1Name;
    public Text P2Name;
    public Text PointsP1;
    public Text PointsP2;
    public Button[] QuestionButtons = new Button[9];
    public Button selectedQuestion;
    public Sprite compelteAnswers;
    
    string[] separatingStrings = {";" };
    private IList<QuizRound> rounds;
    private QuizGame game;
    private APIClientCategoriesQuestions _apiClientCategoriesQuestions;
    private APIClientRound _apiClientRound;
    private APIClientGame _apiClientGame;

    // Start is called before the first frame update
    async void Start()
    {
        InitClients();
        await InitQuestionGameOverview();
    }

    private async System.Threading.Tasks.Task InitQuestionGameOverview()
    {
        try
        {
            game = (await _apiClientGame.GetGame(MultiplayerGameManager.GameId)).FirstOrDefault();
            rounds = await _apiClientRound.GetAllRoundsByGameId(MultiplayerGameManager.GameId);
            InitText();
            InitPoints();
            InitQuestionsResults();
        }
        catch (Exception e)
        {
            UnityLogger.GetLogger().Error(e , nameof(QuestionsGameOverview) + "InitQuestionGameOverview");
        }

    }

    private void InitClients()
    {
        _apiClientRound = new APIClientRound();
        _apiClientGame = new APIClientGame();
        _apiClientCategoriesQuestions = new APIClientCategoriesQuestions();
    }

    public void SetCurrentQuestion(int id)
    {
        selectedQuestion.transform.position = QuestionButtons[id].transform.position;
        var resultOverviewScript = GameObject.FindObjectOfType(typeof(ResultOverviewScript)) as ResultOverviewScript;

        if (id == resultOverviewScript.currentHighestQuestionId && resultOverviewScript.currentRound != null && !resultOverviewScript.currentRound.IsFinished)
        {
            InitQuestionToButton(QuestionButtons[id], null, null, id);
        }

        for (int i = id + 1; i < QuestionButtons.Length; i++)
        {
            QuestionButtons[i].enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitText()
    {
        P1Name.text = MultiplayerGameManager.getPlayerName();
        P2Name.text = MultiplayerGameManager.Player2.PlayerName;
    }
    
    private void InitPoints()
    {
        var points = RoundHelper.CalculatePoints(rounds);
        if ( MultiplayerGameManager.getPlayerId() == game.Player1ID)
        {
            PointsP1.text = points[0].ToString();
            PointsP2.text = points[1].ToString();
        }
        else
        {
            PointsP1.text = points[1].ToString();
            PointsP2.text = points[0].ToString();
        }

        PointsP1.text += " /" + points[2];
        PointsP2.text += " /" + points[2];
    }
    
    async void InitQuestionsResults()
    {
        var counter = 0;
        var playerId = MultiplayerGameManager.getPlayerId();
        foreach (var round in rounds)
        {
            var resultP1 = GetPlayersResults(round, out var resultP2, out var questions, playerId);

            if (resultP1 != null && resultP2 != null)
            {
                for (var i = 0; i < resultP1.Count(); i++)
                {
                    var qId = Convert.ToInt16(questions[i]);
                    var question = (await _apiClientCategoriesQuestions.GetQuestionsById(qId)).FirstOrDefault();
                    var color = SetResultReturnColor(resultP1, i, resultP2, question);
                    SetImageSpriteAndColor(counter, color, compelteAnswers);
                    InitQuestionToButton(QuestionButtons[counter], round, question, counter);
                    ButtonHelper.SetButtonColor(QuestionButtons[counter], color);
                    counter++;
                }
            }
        }
    }

    private string[] GetPlayersResults(QuizRound round, out string[] resultP2, out string[] questions, string playerId)
    {
        var resultP1 = SplitString(round.P1AnswerIds);
        resultP2 = SplitString(round.P2AnswerIds);
        questions = SplitString(round.QuestionIDs);
        
        if (playerId != game.Player1ID)
        {
            resultP1 = SplitString(round.P2AnswerIds);
            resultP2 = SplitString(round.P1AnswerIds);
        }

        return resultP1;
    }

    private string[] SplitString(string stringToSplit)
    {
        return stringToSplit?.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
    }

    private static Color32 SetResultReturnColor(string[] resultP1, int i, string[] resultP2, QuizQuestion question)
    {
        int P1Erg = Convert.ToInt16(resultP1[i]);
        int P2Erg = Convert.ToInt16(resultP2[i]);
        var color = MultiplayerGameManager.cUnentschieden;

        if (P1Erg == question.CorrectAnswerID && P2Erg != question.CorrectAnswerID)
        {
            color = MultiplayerGameManager.cGewonnen;
        }
        else if (P1Erg != question.CorrectAnswerID && P2Erg == question.CorrectAnswerID)
        {
            color = MultiplayerGameManager.cVerloren;
        }

        return color;
    }

    private void SetImageSpriteAndColor(int counter, Color32 color, Sprite sprite)
    {
        var image = QuestionButtons[counter].gameObject.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;
    }

    private void InitQuestionToButton(Button btn, QuizRound round, QuizQuestion question, int id)
    {
        btn.onClick.AddListener(delegate { SetInfosToObject(question, round, btn, id); });
    }

    private void SetInfosToObject(QuizQuestion question, QuizRound round, Button btn, int id)
    {
        selectedQuestion.transform.position = btn.transform.position;
        var resultOverviewScript = GameObject.FindObjectOfType(typeof(ResultOverviewScript)) as ResultOverviewScript;
        resultOverviewScript.QuestionSelected(question, round, id);
    }
}
