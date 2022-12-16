#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion

using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class CourseMainMenu : MonoBehaviour
{
  [Required]
  public GameObject classroomOverviewContent;
  [Required]
  public GameObject noClassroomContent;
  [Required]
  public GameObject classroomJoinedContent;

  public MainQuestSO questLookUp;

  [Required]
  public Text newCourseName;
  [Required]
  public Text currentCourseName;
  [Required]
  public Text joinedDate;
  [Required]
  public Text courseID;

  [Required]
  public InputField accessCodeInput;

  [Required]
  public PopUpManager popupManager;

  [Obsolete]
  public List<TopicSO> courseTopicList = new List<TopicSO>();
    public SceneManagement sceneManager;

  private GameController controller;
    private UnityAction onCourseContinueConfirm;

  public async void JoinCourse()
  {
    // List<TopicSO> allTopics = controller.gameAssets.GetAllTopics();
    controller.gameState.taskHistoryData.courseHistoryData = new List<FinishedTaskData>();
    // controller.gameState.course = controller.gameState.CreateExampleCourse(allTopics, 4, 1);
    GameConfigSO config = controller.gameAssets.gameConfig;
    CourseApiClient client = new CourseApiClient(
      config.tupConfig.apiBaseURL,
      config.tupConfig.accessToken
    );

    string playerId = controller.gameState.profileData.playerId;
    string playerName = controller.gameState.profileData.playerName;

    EnrolData data = new EnrolData
    {
      accessCode = accessCodeInput.text,
      userId = playerId,
      userName = playerName,
    };

    Course course = await client.Enrol(data);
    /**
     * Check if course has default value
     * @see https://stackoverflow.com/a/1896035/1244727 
     */
    bool isDefault = EqualityComparer<Course>.Default.Equals(course, default(Course));

    if(isDefault && client.lastError.responseCode == 404)
    {
      popupManager.DisplayFeedbackPopUp("Der Zugangscode is nicht korrekt.");
      return;
    }

    if(isDefault && client.lastError.isNetworkError == true)
    {
      popupManager.DisplayFeedbackPopUp("Es ist ein Fehler mit der Verbindung aufgetreten.\nBitte überprüfe ob du online bist.");
      return;
    }

    if (isDefault == false)
    {
      controller.gameState.course = course;
      //reset save data from previous courses
      controller.gameState.profileData.RemoveFinishedQuest(questLookUp);
      controller.gameState.taskHistoryData.courseHistoryData = new List<FinishedTaskData>();
      controller.gameState.taskHistoryData.skippedCourseTaskData = new List<FinishedTaskData>();
      controller.SaveGame();
      newCourseName.text = course.name;

      noClassroomContent.SetActive(false);
      classroomJoinedContent.SetActive(true);

      return;
    }

    Debug.LogError("EnrolClientError: " + client.lastError.error);
    popupManager.DisplayFeedbackPopUp("Es ist ein Fehler aufgetreten.");
  }

  [Obsolete]
  public void CreateCourseByTopicList()
  {
    controller.gameState.taskHistoryData.courseHistoryData = new List<FinishedTaskData>();
    controller.gameState.course = controller.gameState.CreateExampleCourse(courseTopicList);

    controller.SaveGame();
    newCourseName.text = controller.gameState.course.name;

    noClassroomContent.SetActive(false);
    classroomJoinedContent.SetActive(true);
  }

  public void CourseJoinConfirmation()
  {
    SetCurrentCourseData();
    classroomJoinedContent.SetActive(false);
    classroomOverviewContent.SetActive(true);
  }

  public async void LeaveCourse()
  {
    string accessCode = controller.gameState.course.accessCode;
    string userId = controller.gameState.profileData.playerId;
    GameConfigSO config = controller.gameAssets.gameConfig;
    CourseApiClient client = new CourseApiClient(
      config.tupConfig.apiBaseURL,
      config.tupConfig.accessToken
    );

    CourseData data = new CourseData()
    {
      accessCode = accessCode,
      userId = userId,
    };

    bool success = await client.UnEnroll(data);
  
    if(success == true)
    {
      controller.gameState.course = null;
      controller.gameState.profileData.RemoveActiveQuest(questLookUp);
      controller.SaveGame();

      noClassroomContent.SetActive(true);
      classroomOverviewContent.SetActive(false);

      return;
    }

    Debug.LogError("EnrolClientError: " + client.lastError.error);
    popupManager.DisplayFeedbackPopUp("Es ist ein Fehler aufgetreten.");
  }

    public void ContinueWithCourse()
    {
        if(controller.gameState.onboardingData.bibUnlocked == true)
        {
            string jumpToAreaText = "Möchtest du zum Ausbildungsbereich springen, um die Kursaufgaben zu bearbeiten?";
            onCourseContinueConfirm = null;
            onCourseContinueConfirm += JumpToTeachAndPlayScene;
            PopUpManager.Instance.DisplayConfirmPopUp(jumpToAreaText, onCourseContinueConfirm);
        } else {
            string continueGameText = "Bevor du mit den Kursaufgaben starten kannst, musst du die Spieleinführung abschließen. Möchtest du jetzt weiterspielen?";
            onCourseContinueConfirm = null;
            onCourseContinueConfirm += ContinueFromGameSave;
            PopUpManager.Instance.DisplayConfirmPopUp(continueGameText, onCourseContinueConfirm);
        }

    }

    public void ContinueFromGameSave()
    {
        sceneManager.LoadlastScene();
    }

    public void JumpToTeachAndPlayScene()
    {
        sceneManager.LoadScene("TeachAndPlayScene");
    }

  public void OpenCourseMenu()
  {
    controller = GameController.GetInstance();
   
    if (controller.gameState.course == null)
    {
      classroomJoinedContent.SetActive(false);
      classroomOverviewContent.SetActive(false);
      noClassroomContent.SetActive(true);
    }
    else
    {
      SetCurrentCourseData();

      classroomJoinedContent.SetActive(false);
      classroomOverviewContent.SetActive(true);
      noClassroomContent.SetActive(false);
    }

    gameObject.SetActive(true);
  }

  private void SetCurrentCourseData()
  {
    currentCourseName.text = controller.gameState.course.name;
    joinedDate.text = "Beigetreten: " + controller.gameState.course.joinDate.ToString("d", CultureInfo.GetCultureInfo("de-DE"));
    courseID.text = "Kurs ID: " + controller.gameState.course.accessCode;
  }

  
}
