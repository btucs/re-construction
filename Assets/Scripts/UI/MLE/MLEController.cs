#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using RenderHeads.Media.AVProVideo;

[RequireComponent(typeof(MLEAnimator))]
public class MLEController : MonoBehaviour
{
  public GameObject StartScreenCanvas;
  public GameObject answerButtonPrefab;
  public Text questionText;
  public Transform answerContainer;
  public Button sendAnswerButton;
  public GameObject interactionPanelObject;
  public GameObject ProgressBarElementPrefab;
  public Transform ProgressBarContainer;
  public GameObject HandoutScreen;
  public GameObject NoInternetCanvas;
  public GameObject HandoutContent;
  public GameObject VideoUI;

  public Animator mleIntroAnimator;

  [HideInInspector]
  public MLEQuizChoice selectedAnswerInstance;

  public MLEDataSO MLEContent;

  public MLEHandoutsSO MLEHandout;

  [Required]
  public CustomVCR videoController;

  [Required]
  public MLEFeedbackController feedbackController;
  [Required]
  public MLEProgressBarController progressBarController;
  [Required]
  public MLEQuestionController questionController;

  public int score = 0;
  public Text mleTitleStart;
  public Text mleDescriptionStart;

  public Text endScoreText;

  public UIHideAndExpand videoUIAnim;
  public MLEAnimator MLEAnimationController;

  [HideInInspector]
  public bool firstVideoFinished = false;

  [HideInInspector]
  public int videoPlayed = 1;   // starts from 1, because for the first video DisplayMilestone is not called

  [HideInInspector]
  public bool caseReadyToPlay = false;

  [HideInInspector]
  public bool caseDisplayVideo = false;

  [HideInInspector]
  public bool caseStarted = false;

  private MLEQuiz currentQuestion;
  
  private int maxPoints;

  private GameObject dataObject;

  private List<MLEProgressBarMilestone> milestones = new List<MLEProgressBarMilestone>();
  private Dictionary<MLEProgressBarMilestone, Vector2Int> positionInMLESO = new Dictionary<MLEProgressBarMilestone, Vector2Int>();

  private MLEMode mode = MLEMode.video;

  public void Awake() {

    dataObject = GameObject.FindWithTag("MLEDataObj");
    if(dataObject) {
      MLEContent = dataObject.GetComponent<MLEDataTransporter>().mleTransportData;
    }

    SetVideoURLsToVideoController(MLEContent);
  }

  public void Start() {
    
    mleTitleStart.text = MLEContent.mleName;
    maxPoints = MLEContent.GetMaxPoints();
    mleDescriptionStart.text = MLEContent.mleIntroText;
    CreateMilestones(MLEContent);
    progressBarController.Initialize(MLEContent.mleName, milestones);
    progressBarController.SetScore(score + " / " + maxPoints);
    videoController.Events.AddListener(OnVideoEvent);
    questionController.OnSubmit.AddListener(OnSubmitAnswer);
  }

  private void SetVideoURLsToVideoController(MLEDataSO MLEContent) {

    string[] videoURLs = MLEContent.videoData.Select((MLEVideoData videoData) => videoData.videoUrl).ToArray();

    videoController._videoFiles = videoURLs;
  }

  public void DisplayNextMilestone() {

    progressBarController.NextStep();
    DisplayCurrentMilestoneOrAssessment();
  }

  public void RepeatMilestone() {

    DisplayMilestone(progressBarController.CurrentMilestone);
  }

  public void SkipMilestone() {

    interactionPanelObject.SetActive(false);
    if(videoController.videoIsPlaying) {
      videoController.OnPlayPauseButton();
    }
    progressBarController.SkipCurrentStep();
    DisplayCurrentMilestoneOrAssessment();
  }

  public void CurrentVideoSuccess() {
    progressBarController.MarkSuccess();
    progressBarController.NextStep();
  }

  public void CloseStartScreen() {
    //checks the internet connection and if there is a video error
    if (Application.internetReachability == NetworkReachability.NotReachable || videoController.errorVideo)
    {
      NoInternetCanvas.SetActive(true);
    }
    else
    {
      StartScreenCanvas.SetActive(false);
    }
  }

  public void StartMLEIntro()
  {
    mleIntroAnimator.gameObject.SetActive(true);
    mleIntroAnimator.SetTrigger("StartIntro");
  }

  public void StartVideo()
  {
    interactionPanelObject.transform.parent.gameObject.SetActive(true);
    progressBarController.gameObject.SetActive(true);
    
    if (caseReadyToPlay)
      {
        StartPlay();
      }
      caseDisplayVideo = true;
      firstVideoFinished = true;
  }

  //public List<VariableInfoSO> entries;
  public MLEHandoutsController mleHandoutManager ;
  public void CloseStartScreenMLEHandout()
  {
    //StartScreenCanvas.SetActive(false);
    mode = MLEMode.handout;
    HandoutScreen.SetActive(true);

    mleHandoutManager.CreateEntryDetails(MLEContent.mleHandout);
  }

  public void WriteScoreToDataObject() {
    if(dataObject) {
      dataObject.GetComponent<MLEDataTransporter>().achievedPoints = score;
    }
  }

  public void ContinueAfterQuizAnswer()
  {
    if(mode == MLEMode.video)
    {
      DisplayNextMilestone();
    } else {
      DisplayNextQuestionHandout();
    }
  }

  public void DisplayNextQuestionHandout()
  {
    HandoutContent.SetActive(false);
    StartScreenCanvas.SetActive(false);
    mleIntroAnimator.gameObject.SetActive(false);
    VideoUI.SetActive(true);

    progressBarController.NextStep();
    MLEProgressBarMilestone currentMileStone = progressBarController.CurrentMilestone;
    while(currentMileStone.type != MilestoneType.interactive && progressBarController.CurrentStep < progressBarController.TotalSteps)
    {
      progressBarController.NextStep();
      currentMileStone = progressBarController.CurrentMilestone;
    }

    DisplayCurrentMilestoneOrAssessment();

    progressBarController.SetProgressBarActive(false);

  }

  private void OnDestroy() {

    if(videoController != null) {

      videoController.Events.RemoveListener(OnVideoEvent);
    }

    questionController.OnSubmit.RemoveListener(OnSubmitAnswer);
  }

  private void CreateMilestones(MLEDataSO MLEContent) {

    //creating gameObjects for Progress Bar 
    for(int i = 0; i < MLEContent.videoData.Length; i++) {

      GameObject videoObject = Instantiate(ProgressBarElementPrefab, ProgressBarContainer);
      videoObject.GetComponent<MLEProgressBarMilestone>().type = MilestoneType.video;
      videoObject.GetComponent<MLEProgressBarMilestone>().titleString = MLEContent.videoData[i].navTitle;
      MLEProgressBarMilestone videoMilestone = videoObject.GetComponent<MLEProgressBarMilestone>();
      milestones.Add(videoMilestone);
      positionInMLESO.Add(videoMilestone, new Vector2Int(i, -1));

      for(int j = 0; j < MLEContent.videoData[i].questions.Length; j++) {

        GameObject questionObject = Instantiate(ProgressBarElementPrefab, ProgressBarContainer);
        questionObject.GetComponent<MLEProgressBarMilestone>().type = MilestoneType.interactive;
        MLEProgressBarMilestone questionMilestone = questionObject.GetComponent<MLEProgressBarMilestone>();
        milestones.Add(questionMilestone);
        positionInMLESO.Add(questionMilestone, new Vector2Int(i, j));
      }
    }
  }

  private void DisplayCurrentMilestoneOrAssessment() {

    MLEProgressBarMilestone currentMileStone = progressBarController.CurrentMilestone;

    if(
      progressBarController.CurrentStep == progressBarController.TotalSteps
      && currentMileStone.state != MilestoneState.upcoming
    ) {

      endScoreText.text = " " + score + " ";
      MLEAnimationController.DisplayAsessmentScreen();
    } else {

      DisplayMilestone(currentMileStone);
    }
  }

  private void DisplayMilestone(MLEProgressBarMilestone milestone) {

    positionInMLESO.TryGetValue(milestone, out Vector2Int position);

    switch (milestone.type) {
      case MilestoneType.interactive:
        caseReadyToPlay = false;
        caseDisplayVideo = false;
        caseStarted = false;
        MLEQuiz quiz = MLEContent.videoData[position.x].questions[position.y];
        DisplayInteractiveContent(quiz);
        if(videoPlayed < videoController._videoFiles.Length) 
        {
          videoController.OnOpenVideoFile();
          videoPlayed++;
        }
        break;

      case MilestoneType.video:
        DisplayVideo(milestone);
        break;
    }
  }

  private void DisplayInteractiveContent(MLEQuiz quiz) {

    //UpdateInteractionUI(quiz);
    questionController.SetQuestion(quiz);
    MLEAnimationController.DisplayInteractionUI();
  }

  private void DisplayVideo(MLEProgressBarMilestone milestone) {

    MLEAnimationController.DisplayVideoPlayer();
    //videoController.OnOpenVideoFile();
    if (caseReadyToPlay) 
    {
      StartPlay();
    }
    caseDisplayVideo = true;
  }

  private void StartPlay() {
    //starts MLE video automatically
    if (videoController && videoController.startVideo)
    {
      videoController.OnPlayPauseButton();
      //videoController.startVideo = false;         //is present in OnPlayPauseButton
    }
  }

  private void HasFinishedPlaying() {

    progressBarController.MarkSuccess();
    progressBarController.NextStep();
    DisplayCurrentMilestoneOrAssessment();
  }

  private void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode) {

    switch(et) {
      case MediaPlayerEvent.EventType.ReadyToPlay:
        caseReadyToPlay = true;
        if (caseDisplayVideo == true && caseStarted == false) 
        {
          StartPlay();
        }  
        //if (firstVideoFinished) 
        //{
        //StartPlay();
        //}
        break;
      case MediaPlayerEvent.EventType.Started:
        caseStarted = true;
        break;
      case MediaPlayerEvent.EventType.FirstFrameReady:
        //if (firstVideoFinished) 
        //{
          //StartPlay();
        //}
        break;
      case MediaPlayerEvent.EventType.FinishedPlaying:
        HasFinishedPlaying();
        //caseReadyToPlay = false;
        //caseDisplayVideo = false;
        //caseStarted = false;
        break;
    }
  }

  private void OnSubmitAnswer(MLEQuiz quiz, MLEQuizChoice selectedAnswer) {

    feedbackController.gameObject.SetActive(true);
    feedbackController.SetView(
      quiz,
      selectedAnswer
    );

    if(selectedAnswer.isAnswer) {
      //reward
      score += 2;
      progressBarController.SetScore(score + " / " + maxPoints);
      progressBarController.MarkSuccess();
      MLEAnimationController.DisplayCorrectAnswer();
    } else {
      progressBarController.MarkFailure();
      MLEAnimationController.DisplayWrongAnswer();
    }
  }
}

public enum MLEMode{
  handout, video
}