#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Multiplayer.API;
using Assets.Scripts.Multiplayer.API.Helper;
using Assets.Scripts.Multiplayer.API.Models;
using Assets.Scripts.Multiplayer.Logging;
using Assets.Scripts.Multiplayer.MainScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Multiplayer.SceneScripts
{
  public class QuestionScript : MonoBehaviour
  {
    public GameObject ScrollView;
    public GameObject EntryObjectPrefab;
    public GameObject QuestionObject;
    public Text RoundNumber;
    public Text QuestionNumber;
    public Text Correct;
    public Button BtnConfirm;
    public Button BtnNext;
    public GameObject NoAnswer;

    private APIClientCategoriesQuestions _apiClientQuestions;
    private APIClientRound _apiClientRound;
    private APIClientGame _apiClientGame;
    private List<QuizQuestion> currentQuestions;
    private int currentQuestionId = 0;
    private const int MAX_QUESTIONS = 3;
    private const int MAX_ROUNDS = 3;
    const string content = "Content";
    const string richtig = "Richtig";
    const string falsch = "Falsch";
    string[] separatingStrings = { ";" };

    private GameObject oldSelectedAnswer;
    private Button correctAnswerButton;
    private QuizGame game;
    private QuizRound round;
    private APIClientPushNotificationClient _apiClientPushNotificationClient;
    private bool confirmed = false;
    // Start is called before the first frame update
    async void Start()
    {
      GameController controller = GameController.GetInstance();
      string bearerToken = controller.gameAssets.gameConfig.apiConfig.accessToken;

      _apiClientQuestions = new APIClientCategoriesQuestions();
      _apiClientRound = new APIClientRound();
      _apiClientGame = new APIClientGame();
      _apiClientPushNotificationClient = new APIClientPushNotificationClient(bearerToken);

      await InitQuestionScene();
    }

    public async void SaveAnswer()
    {
      if (currentQuestionId + 1 >= MAX_QUESTIONS)
      {
        await RoundFinished();
      }
      else
      {
        Correct.text = string.Empty;
        NextQuestion();
      }
    }

    public void ConfirmAnswer()
    {
      if (oldSelectedAnswer != null)
      {
        var infos = oldSelectedAnswer.gameObject.GetComponent<ObjectInfo>();
        MultiplayerGameManager.AnswerIdsAsString += infos.Id + ";";
        var value = CheckAnswer(infos);
        ButtonHelper.SetButtonColor(correctAnswerButton, MultiplayerGameManager.cGewonnen);
        MultiplayerGameManager.AddAnswerEntry(currentQuestionId, value);
        BtnNext.gameObject.SetActive(true);
        BtnConfirm.gameObject.SetActive(false);
        StopProgressbar();
        confirmed = true;
      }
    }
    // Wird von der Progressbar aufgerufen
    public void TimeOver()
    {
      NoAnswer.SetActive(true);
      MultiplayerGameManager.AnswerIdsAsString += "-1;";
      MultiplayerGameManager.AddAnswerEntry(currentQuestionId, false);
      BtnNext.gameObject.SetActive(true);
      BtnConfirm.gameObject.SetActive(false);
    }


    private async System.Threading.Tasks.Task InitQuestionScene()
    {
      try
      {
        currentQuestions = new List<QuizQuestion>();
        MultiplayerGameManager.ResetRoundValues();
        if (await InitQuestions())
        {
          InitScrollView();
        }
      }
      catch (Exception e)
      {
        UnityLogger.GetLogger().Error(e, nameof(QuestionScript) + " InitQuestionScene");
      }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool CheckAnswer(ObjectInfo infos)
    {
      bool value = false;
      if (currentQuestions[currentQuestionId].CorrectAnswerID == infos.Id)
      {
        value = true;
        MultiplayerGameManager.PointsOfCurrentGame += 1;
        Correct.text = richtig;
      }
      else
      {
        Correct.text = falsch;
      }

      return value;
    }

    private async System.Threading.Tasks.Task RoundFinished()
    {
      await UpdateRound();

      if (round.RoundNumber == MAX_ROUNDS && round.IsFinished)
      {
        SceneManager.LoadScene("GameFinished");
      }
      else
      {
        var newRoundNumber = round.RoundNumber + 1;
        if (round.IsFinished && newRoundNumber <= MAX_ROUNDS)
        {
          MultiplayerGameManager.SetRoundNumber(newRoundNumber);
          await _apiClientRound.InsertNewRound(MultiplayerGameManager.GameId, newRoundNumber);
        }

        SceneManager.LoadScene("ResultOverview");
      }
    }

    private async System.Threading.Tasks.Task UpdateRound()
    {
      if (game.Player1ID == MultiplayerGameManager.getPlayerId())
      {
        round.P1AnswerIds = MultiplayerGameManager.AnswerIdsAsString;
        round.S1Points = MultiplayerGameManager.PointsOfCurrentGame;
      }
      else
      {
        round.P2AnswerIds = MultiplayerGameManager.AnswerIdsAsString;
        round.S2Points = MultiplayerGameManager.PointsOfCurrentGame;
      }

      if (round.P2AnswerIds != null && round.P1AnswerIds != null)
      {
        round.IsFinished = true;
      }
      else
      {
        await _apiClientPushNotificationClient.SendNotification(
          new ApiNotification(
            MultiplayerGameManager.Player2.PlayerID,
            "Die Fragen wurden vom Gegenspieler beantwortet",
            "Du bist dran",
            new ApiNotificationData(NotificationType.QuestionAnswered)
          )
        );
      }

      await _apiClientRound.UpdateRound(round);

    }


    private async Task<bool> InitQuestions()
    {

      round = (await _apiClientRound.GetOpenRoundsByGameId(MultiplayerGameManager.GameId))
          .OrderByDescending(x => x.RoundNumber).First();
      game = (await _apiClientGame.GetGame(MultiplayerGameManager.GameId)).FirstOrDefault();
      SetRoundValues();
      List<int> listQuestion;

      if (string.IsNullOrEmpty(round.QuestionIDs))
      {
        var questions = await _apiClientQuestions.GetQuestionsByCategory(MultiplayerGameManager.CategoryId);
        if (questions == null || !questions.Any())
        {
          var exception = new Exception("No Questions of Category Id " + MultiplayerGameManager.CategoryId +
                                " found");
          UnityLogger.GetLogger().Error(exception, nameof(QuestionScript) + " InitQuestions");
          Debug.Log(exception);
        }
        listQuestion = RandomQuestions(questions);
        await UpdateRound(listQuestion);
      }
      else
      {
        listQuestion = round.QuestionIDs.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();
      }

      if (listQuestion?.Count > 0)
      {
        return await InitQuestionsToList(listQuestion);
      }

      return false;
    }

    private void SetRoundValues()
    {
      MultiplayerGameManager.SetRoundNumber(round.RoundNumber);
    }

    private async System.Threading.Tasks.Task UpdateRound(List<int> listQuestion)
    {
      round.QuestionIDs = String.Join(";", listQuestion.ToArray());
      round.RoundNumber = MultiplayerGameManager.RoundNumber;
      round.QuizCatId = MultiplayerGameManager.CategoryId;
      await _apiClientRound.UpdateRound(round);
    }

    private async Task<bool> InitQuestionsToList(List<int> listQuestion)
    {
      foreach (var questionId in listQuestion)
      {
        var question = (await _apiClientQuestions.GetQuestionsById(questionId)).FirstOrDefault();
        if (question == null)
        {
          throw new Exception("QuestionId not exists");
        }

        currentQuestions.Add(question);
      }

      return true;
    }

    private void InitScrollView()
    {
      if (ScrollView == null) return;
      confirmed = false;
      var Temp = ScrollView.GetComponentsInChildren<Transform>();
      foreach (var Child in Temp)
      {
        if (Child.name != content) continue;

        RemoveOldChildrens(Child);
        var question = GetQuestionAndListOfAnswers(out var listOfAnswers);
        for (var i = 0; i < listOfAnswers.Count(); i++)
        {
          InitAnswers(listOfAnswers, i, question, Child);
        }

        SetQuestionRoundNumber();

        break;
      }
    }

    private QuizQuestion GetQuestionAndListOfAnswers(out string[] listOfAnswers)
    {
      currentQuestionId = MultiplayerGameManager.AnswerIdsAsString
          ?.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries)?.Length ?? 0;
      var question = currentQuestions[currentQuestionId];

      if (QuestionObject != null)
        QuestionObject.gameObject.GetComponent<Text>().text = question.Question;

      listOfAnswers = question.Answers?.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries); ;
      return question;
    }

    private static void RemoveOldChildrens(Transform Child)
    {
      if (Child.transform.childCount > 0)
      {
        var count = Child.transform.childCount;
        for (var i = count - 1; i >= 0; i--)
        {
          GameObject.DestroyImmediate(Child.transform.GetChild(i).gameObject);
        }
      }
    }

    private void InitAnswers(string[] listOfAnswers, int i, QuizQuestion question, Transform Child)
    {
      var listObject = InitListObject(listOfAnswers[i]);
      ObjectInfoHelper.AddObjectInfo(listObject, i + 1, question.CorrectAnswerID.ToString());
      listObject.transform.SetParent(Child.transform, false);

      if (i + 1 == question.CorrectAnswerID)
      {
        correctAnswerButton = listObject.transform.gameObject.GetComponent<Button>();
      }
    }

    private void SetQuestionRoundNumber()
    {
      RoundNumber.text = MultiplayerGameManager.RoundNumber.ToString();
      QuestionNumber.text = (currentQuestionId + 1).ToString();
      BtnNext.gameObject.SetActive(false);
      BtnConfirm.gameObject.SetActive(true);
    }

    private GameObject InitListObject(string answer)
    {
      var AnswerPref = Instantiate(EntryObjectPrefab);
      var textInfo = AnswerPref.transform.Find("Info").gameObject.GetComponent<Text>();
      textInfo.text = answer;
      var btn = AnswerPref.GetComponent<Button>();
      btn.onClick.AddListener(delegate { ChooseAnswer(AnswerPref, textInfo); });
      return AnswerPref;
    }


    void NextQuestion()
    {
      ResetProgressbar();
      InitScrollView();
    }

    private void StopProgressbar()
    {
      NoAnswer.SetActive(false);
      var progressBarScript = GameObject.FindObjectOfType(typeof(ProgressBarScript)) as ProgressBarScript;
      progressBarScript.StopAndInvisible();
    }

    private void ResetProgressbar()
    {
      NoAnswer.SetActive(false);
      var progressBarScript = GameObject.FindObjectOfType(typeof(ProgressBarScript)) as ProgressBarScript;
      progressBarScript.Reset();
    }

    void ChooseAnswer(GameObject answer, Text text)
    {
      if (!confirmed)
      {
        if (oldSelectedAnswer != null)
        {
          oldSelectedAnswer.transform.Find("Info").gameObject.GetComponent<Text>().color = Color.black;
          var oldSelBtn = oldSelectedAnswer.transform.gameObject.GetComponent<Button>();
          ButtonHelper.SetButtonColor(oldSelBtn, MultiplayerGameManager.cWhite);
        }

        oldSelectedAnswer = answer;
        text.color = Color.white;
        var button = answer.transform.gameObject.GetComponent<Button>();
        ButtonHelper.SetButtonColor(button, MultiplayerGameManager.cHighlight);
      }
    }

    private List<int> RandomQuestions(IList<QuizQuestion> questions)
    {
      var randomQList = new List<int>();
      while (randomQList.Count < MAX_QUESTIONS)
      {
        var question = Random.Range(0, questions.Count);
        var id = questions[question].ID;
        if (!randomQList.Contains(id))
        {
          randomQList.Add(id);
        }
      }

      return randomQList;
    }
  }
}
