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
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Assets.Scripts.Multiplayer.SceneScripts
{
    public class ResultOverviewScript : MonoBehaviour
    {
        public Text GegenspielerTxt;
        public Text WarteGegenspieler;
        public Button BtnFragen;
        public Button BtnRound;
        public GameObject currentSelectedQuestion;
        public GameObject currentStatus;
        public GameObject EntryObjectPrefab;
        public GameObject ScrollView;
        public GameObject QuestionObject;
        public characterGraphicsUpdater Player1;
        public characterGraphicsUpdater Player2;
        public GameObject P1Image;
        public GameObject P2Image;
        public int currentHighestQuestionId;
        public QuizRound currentRound;

        private APIClientRound _apiClientRound;
        private APIClientGame _apiClientGame;
        private bool isPlayer1;

        private IList<QuizRound> rounds;
        private QuizGame game;
        const int MAX_ROUND = 3;
        string[] separatingStrings = {";" };
        GameObject P1AnswerObject;
        GameObject P2AnswerObject;
        int highestRound3QuestionId = 8;
        int highestRound2QuestionId = 6;
        int highestRound1QuestionId = 3;
        CreatePlayerCharacters createPlayerCharacters;
        bool yourturn = false;
        // Start is called before the first frame update
        
        async void Start()
        {
            try
            {
                InitClients();
                await InitGameAndRound();
                await InitPlayerCharacters();
                InitResultOverView();
                SetHighestQuestionId();
                SetCurrentQuestion();
            }
            catch (Exception e)
            {
                UnityLogger.GetLogger().Error(e, nameof(ResultOverviewScript) + " Start");
            }

        }
        
        public async void QuestionSelected(QuizQuestion question, QuizRound round, int id)
        {
            try
            {
                InitClients();
                await InitGameAndRound();
                await InitPlayerCharacters();
                EnableDisableQuestionCanvas(question, round, id);
                SetHighestQuestionId();
            }
            catch (Exception e)
            {
                UnityLogger.GetLogger().Error(e, nameof(ResultOverviewScript) + " QuestionSelected");
            }
        }

        private void EnableDisableQuestionCanvas(QuizQuestion question, QuizRound round, int id)
        {
            if (id == currentHighestQuestionId && (round == null || !round.IsFinished))
            {
                currentStatus.SetActive(true);
                currentSelectedQuestion.SetActive(false);
            }
            else
            {
                InitOldQuestion(question, round, id);
                currentStatus.SetActive(false);
                currentSelectedQuestion.SetActive(true);
            }
        }

        private async System.Threading.Tasks.Task InitGameAndRound()
        {
            rounds = await _apiClientRound.GetAllRoundsByGameId(MultiplayerGameManager.GameId);
            currentRound = rounds.OrderByDescending(x => x.RoundNumber).First();
            game = (await _apiClientGame.GetGame(MultiplayerGameManager.GameId)).FirstOrDefault();
        }

        private async System.Threading.Tasks.Task InitPlayerCharacters()
        {
            createPlayerCharacters = new CreatePlayerCharacters(Player1, Player2, game);
            await createPlayerCharacters.SetPlayersCharacters();
        }

        private void InitClients()
        {
            _apiClientRound = new APIClientRound();
            _apiClientGame = new APIClientGame();
        }

        private void SetHighestQuestionId()
        {
            currentHighestQuestionId = 0;
            if (!currentRound.IsFinished)
            {
                if (yourturn)
                {
                    if (currentRound.RoundNumber == 2)
                    {
                        currentHighestQuestionId = 3;
                    }
                    else if (currentRound.RoundNumber == 3)
                    {
                        currentHighestQuestionId = highestRound2QuestionId;
                    }
                }
                else
                {
                    currentHighestQuestionId = 2;
                    if (currentRound.RoundNumber == 2)
                    {
                        currentHighestQuestionId = 5;
                    }
                    else if (currentRound.RoundNumber == 3)
                    {
                        currentHighestQuestionId = 8;
                    }
                }
            }
            else
            {
                currentHighestQuestionId = highestRound1QuestionId;
                if (currentRound.RoundNumber == 2)
                {
                    currentHighestQuestionId = highestRound2QuestionId;
                }
                else if (currentRound.RoundNumber == 3)
                {
                    currentHighestQuestionId = highestRound3QuestionId;
                }
            }
        }

        private void SetCurrentQuestion()
        {
            var questionsGameOverview = FindObjectOfType(typeof(QuestionsGameOverview)) as QuestionsGameOverview;
            questionsGameOverview.SetCurrentQuestion(currentHighestQuestionId);
        }

        private void InitResultOverView()
        {
            if (MultiplayerGameManager.getPlayerId() == game.Player1ID)
            {
                isPlayer1 = true;
            }
            InitTextAndButtons();
        }

        private void InitTextAndButtons()
        {
            BtnFragen.gameObject.SetActive(false);
            BtnRound.gameObject.SetActive(false);
            currentStatus.SetActive(true);
            currentSelectedQuestion.SetActive(false);
            
            if (currentRound.RoundNumber == MAX_ROUND && game.IsFinished)
            {
                WarteGegenspieler.gameObject.SetActive(false);
                currentHighestQuestionId = 8;
                GegenspielerTxt.text = "Das Spiel ist beendet";
            }
            else
            {
                WarteGegenspieler.gameObject.SetActive(true);
                if (currentRound.QuizCatId > 0)
                {
                    NextQuestionsText();
                }
                else if (currentRound.RoundNumber % 2 == 1 && !isPlayer1 || currentRound.RoundNumber == 2 && isPlayer1)
                {
                    NextRoundText();
                }
            }
        }
        
        private void NextQuestionsText()
        {
            if (string.IsNullOrEmpty(currentRound.P2AnswerIds) && !isPlayer1 && currentRound.RoundNumber % 2 == 1
                || string.IsNullOrEmpty(currentRound.P2AnswerIds) && !isPlayer1  && currentRound.RoundNumber == 2 && !string.IsNullOrEmpty(currentRound.P1AnswerIds)
                || (string.IsNullOrEmpty(currentRound.P1AnswerIds) && isPlayer1 && (!string.IsNullOrEmpty(currentRound.P2AnswerIds) || currentRound.RoundNumber == 2)))
            {
                GegenspielerTxt.text = "Du bist an der Reihe";
                BtnFragen.gameObject.SetActive(true);
                WarteGegenspieler.gameObject.SetActive(false);
                yourturn = true;
            }
        }

        private void NextRoundText()
        {
            GegenspielerTxt.text = "Du bist an der Reihe";
            WarteGegenspieler.gameObject.SetActive(false);
            BtnRound.gameObject.SetActive(true);
            yourturn = true;
        }

        private void InitOldQuestion(QuizQuestion question, QuizRound round, int id)
        {
            if (ScrollView == null) return;
            var Temp = ScrollView.GetComponentsInChildren<Transform>();
            foreach (var Child in Temp)
            {
                if (RemoveOldChildren(Child)) continue;

                var answerId = id % 3;
                var p1Answers = round.P1AnswerIds.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
                var p2Answers = round.P2AnswerIds.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
                bool noAnswer = p1Answers.Contains("-1") || p2Answers.Contains("-1");
                AddQuestionsToScrollView(question, Child, noAnswer);
                SetImagesToAnswerPosition(Child, p1Answers, answerId, p2Answers, noAnswer);
                break;
            }
        }

        private void SetImagesToAnswerPosition(Transform Child, string[] p1Answers, int answerId, string[] p2Answers, bool noAnswer)
        {
            for (int i = 0; i < Child.transform.childCount - 1; i++)
            {
                var children = Child.transform.GetChild(i).gameObject;
                if (Convert.ToInt32(p1Answers[answerId]) == i + 1)
                {
                    SetImagePosition(children, true);
                }

                if (Convert.ToInt32(p2Answers[answerId]) == i + 1)
                {
                    SetImagePosition(children, false);
                }
            }

            if (!noAnswer) return;
            
            var lastChild = Child.transform.GetChild(Child.transform.childCount - 1).gameObject;
            if (Convert.ToInt32(p1Answers[answerId]) == -1)
            {
                SetImagePosition(lastChild, true);
            }
            if (Convert.ToInt32(p2Answers[answerId]) == -1)
            {
                SetImagePosition(lastChild, false);
            }
        }

        private void AddQuestionsToScrollView(QuizQuestion question, Transform Child, bool noAnswers)
        {
            if (QuestionObject != null)
                QuestionObject.gameObject.GetComponent<Text>().text = question.Question;

            var listOfAnswers = question.Answers.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries); ;
            for (int i = 0; i < listOfAnswers.Count(); i++)
            {
                var AnswerPref = Instantiate(EntryObjectPrefab);
                var textInfo = AnswerPref.transform.Find("Info").gameObject.GetComponent<Text>();
                textInfo.text = listOfAnswers[i];
                if (i + 1 == question.CorrectAnswerID)
                {
                    var button = AnswerPref.transform.gameObject.GetComponent<Button>();
                    ButtonHelper.SetButtonColor(button, MultiplayerGameManager.cGewonnen);
                }

                AnswerPref.transform.SetParent(Child.transform, false);
            }

            if (noAnswers)
            {
                var AnswerPref = Instantiate(EntryObjectPrefab);
                var textInfo = AnswerPref.transform.Find("Info").gameObject.GetComponent<Text>();
                textInfo.text = "Keine Antwort ausgewählt";
                AnswerPref.transform.SetParent(Child.transform, false);
            }
        }

        private static bool RemoveOldChildren(Transform Child)
        {
            if (Child.name != "Content") return true;
            if (Child.transform.childCount > 0)
            {
                int count = Child.transform.childCount;
                for (int i = count - 1; i >= 0; i--)
                {
                    GameObject.DestroyImmediate(Child.transform.GetChild(i).gameObject);
                }
            }

            return false;
        }

        private void SetImagePosition(GameObject AnswerPref, bool p1Answer)
        {
            if(isPlayer1 && p1Answer || !isPlayer1 && !p1Answer)
            {
                P1AnswerObject = AnswerPref;
            }
            else
            {
                P2AnswerObject = AnswerPref;
            }

        }

        public void ClickQuestions()
        {
            SceneManager.LoadScene("Question");
        }

        public void NextRound()
        {
            SceneManager.LoadScene("ChooseCategory");
        }

        // Update is called once per frame
        void Update()
        {
            if (P1AnswerObject != null)
            {
                P1Image.transform.position = new Vector3(P1Image.transform.position.x, P1AnswerObject.transform.position.y, P1AnswerObject.transform.position.z);
            }
            if(P2AnswerObject != null)
            {
                P2Image.transform.position = new Vector3(P2Image.transform.position.x, P2AnswerObject.transform.position.y, P2AnswerObject.transform.position.z);
            }

        }
    }
}
