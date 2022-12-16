#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class SubmitAnswerEvent : UnityEvent<MLEQuiz, MLEQuizChoice> {}

public class MLEQuestionController : MonoBehaviour {

  [Required]
  public Text questionText;
  [Required]
  public Transform answerContainer;
  [Required, AssetsOnly]
  public GameObject answerButtonPrefab;

  [Required]
  public Button sendAnswerButton;

  public SubmitAnswerEvent OnSubmit = new SubmitAnswerEvent();

  private MLEQuiz currentQuestion;
  private MLEQuizChoice selectedAnswer;
  private System.Random rnd = new System.Random();

  private List<MLEAnswerController> answerControllers = new List<MLEAnswerController>();

  public void SetQuestion(MLEQuiz question) {

    ClearAnswers();
    currentQuestion = question;

    questionText.text = question.question;
    CreateAnswerElements();
    sendAnswerButton.interactable = false;
  }

  private void ClearAnswers() {

    foreach(Transform child in answerContainer) {

      Destroy(child.gameObject);
    }

    answerControllers.Clear();
  }

  private void CreateAnswerElements() {

    MLEQuizChoice[] choices = currentQuestion.choices.OrderBy(x => rnd.Next()).ToArray();

    for(int k = 0; k < choices.Length; k++) {
      GameObject answerToggleObj = Instantiate(answerButtonPrefab, answerContainer.position, Quaternion.identity, answerContainer);

      MLEAnswerController answerOptionController = answerToggleObj.GetComponent<MLEAnswerController>();
      if(answerOptionController) {
        answerOptionController.SetChoice(choices[k]);
        answerOptionController.OnSelect.AddListener(UpdateSelectedAnswer);
        //answerOptionController.answerMLEController = this;
        answerControllers.Add(answerOptionController);
      }
    }
  }

  private void UpdateSelectedAnswer(MLEAnswerController selectedAnswerScript, MLEQuizChoice answer) {

    selectedAnswer = answer;

    foreach(MLEAnswerController answerOption in answerControllers) {
      if(answerOption != selectedAnswerScript) {

        answerOption.MarkSelected(false);
      }
    }

    sendAnswerButton.interactable = selectedAnswerScript != null ? selectedAnswerScript.IsSelected : false;
  }

  private void DisableButtons() {

    sendAnswerButton.gameObject.SetActive(false);
  }

  private void Start() {

    sendAnswerButton.onClick.AddListener(SendSelectedAnswer);
  }

  private void SendSelectedAnswer() {
    Debug.Log("Sending selected answer.");
    OnSubmit?.Invoke(currentQuestion, selectedAnswer);
  }

  private void OnDisable() {

    DisableButtons();
    selectedAnswer = null;
  }

  private void OnEnable() {

    sendAnswerButton.gameObject.SetActive(true);
  }

  private void OnDestroy() {

    if(answerControllers != null && answerControllers.Count > 0) {

      answerControllers.ForEach((MLEAnswerController answerController) => answerController.OnSelect.RemoveListener(UpdateSelectedAnswer));
    }

    sendAnswerButton.onClick.RemoveListener(SendSelectedAnswer);
  }
}
