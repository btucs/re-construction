#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MLEController))]
public class MLEAnimator : MonoBehaviour
{
  public GameObject InteractionUIContainer;
  public GameObject AsessmentContainer;
  public GameObject FeedbackPanel;
  public GameObject QuestionPanel;

  //public GameObject SkipButton;
  //public GameObject SendAnswerButton;
  //public GameObject ContinueButton;
  //public GameObject RepeatButton;

  //public Color correctAnswerColor;
  //public Color wrongAnswerColor;

  public Text QuestionFeedback;

  public Text scoreText;
  public Text endFeedbackText;
  private MLEController mleDatenContainer;

  void Start() {
    mleDatenContainer = GetComponent<MLEController>();
  }

  public void DisplayAsessmentScreen() {
    string feedbackText = "Herzlichen Glückwunsch! Du hast die Lektion " + mleDatenContainer.MLEContent.mleName + " erfolgreich abgeschlossen.";
    endFeedbackText.text = feedbackText;
    scoreText.text = mleDatenContainer.score.ToString() + " EP";
    AsessmentContainer.SetActive(true);
  }

  public void DisplayInteractionUI() {
    InteractionUIContainer.SetActive(true);
    FeedbackPanel.SetActive(false);
    QuestionPanel.SetActive(true);

    /*SendAnswerButton.SetActive(true);
    //SkipButton.SetActive(true);
    ContinueButton.SetActive(false);
    RepeatButton.SetActive(false);*/
  }

  public void DisplayCorrectAnswer() {
    QuestionPanel.SetActive(false);
    FeedbackPanel.SetActive(true);

    //QuestionFeedback.color = correctAnswerColor;
    /*SendAnswerButton.SetActive(false);
    SkipButton.SetActive(false);
    ContinueButton.SetActive(true);
    RepeatButton.SetActive(false);*/
  }

  public void DisplayWrongAnswer() {
    QuestionPanel.SetActive(false);
    FeedbackPanel.SetActive(true);

    //QuestionFeedback.color = wrongAnswerColor;
    /*SendAnswerButton.SetActive(false);
    SkipButton.SetActive(false);
    ContinueButton.SetActive(false);
    RepeatButton.SetActive(true);*/
  }

  public void DisplayVideoPlayer() {
    InteractionUIContainer.SetActive(false);
  }
}
