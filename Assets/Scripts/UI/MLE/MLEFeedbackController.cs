#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class MLEFeedbackController : MonoBehaviour {

#pragma warning disable CS0649
  [Required, ShowInInspector, SerializeField, ChildGameObjectsOnly]
  private TMP_Text questionText;
  [Required, ShowInInspector, SerializeField, ChildGameObjectsOnly]
  private TMP_Text answerText;
  [Required, ShowInInspector, SerializeField, ChildGameObjectsOnly]
  private TMP_Text feedbackText;
  [Required, ShowInInspector, SerializeField, ChildGameObjectsOnly]
  // correctAnswer and correctFeedback are only used when user makes 3 mistakes
  private TMP_Text correctAnswerText;
  [Required, ShowInInspector, SerializeField, ChildGameObjectsOnly]
  private TMP_Text correctFeedbackText;
#pragma warning restore CS0649

  public Color correctAnswerColor;
  public Color wrongAnswerColor;

  public bool questionsRepeatable = true;

  [Required]
  public Button continueButton;
  [Required]
  public Button repeatButton;

  private MLEQuiz lastQuestion = null;
  private int repeatCounter = 0;

  public void SetView(MLEQuiz quiz, MLEQuizChoice mleQuizChoice) {

    Debug.Log("now setting view");

    if(quiz != lastQuestion) {

      lastQuestion = quiz;
      repeatCounter = 0;
    }

    questionText.text = quiz.question;
    answerText.text = mleQuizChoice.name;
    feedbackText.text = mleQuizChoice.feedbackMessage;

    DisableButtons();
    DisableCorrectAnswerFeedback();

    if(mleQuizChoice.isAnswer == true) {

      answerText.color = correctAnswerColor;
      //feedbackText.color = correctAnswerColor;

      continueButton.gameObject.SetActive(true);
    } else {

      if(repeatCounter == 2 || questionsRepeatable == false) {

        MLEQuizChoice correctAnswer = FindCorrectAnswer(quiz);
        correctAnswerText.text = correctAnswer.name;
        correctAnswerText.transform.parent.gameObject.SetActive(true);
        correctFeedbackText.text = correctAnswer.feedbackMessage;
        correctFeedbackText.gameObject.SetActive(true);

        continueButton.gameObject.SetActive(true);
      } else {

        repeatButton.gameObject.SetActive(true);
        repeatCounter++;
      }

      answerText.color = wrongAnswerColor;
      //feedbackText.color = wrongAnswerColor;
    }
  }

  private MLEQuizChoice FindCorrectAnswer(MLEQuiz quiz) {

    return quiz.choices.Single((MLEQuizChoice choice) => choice.isAnswer == true);
  }

  private void DisableButtons() {

    continueButton.gameObject.SetActive(false);
    repeatButton.gameObject.SetActive(false);
  }

  private void DisableCorrectAnswerFeedback() {

    correctAnswerText.transform.parent.gameObject.SetActive(false);
    correctFeedbackText.gameObject.SetActive(false);
  }

  private void OnDisable() {

    DisableButtons();
    DisableCorrectAnswerFeedback();
  }

  private void OnEnable() {

    continueButton.gameObject.SetActive(true);
  }
}
