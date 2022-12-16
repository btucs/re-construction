using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class WelcomeScreenController : MonoBehaviour
{
  [Required]
  public Button multiplayerButton;
  [Required]
  public Button classRoomButton;

  private GameController controller;
  private CourseApiClient client;

  // Start is called before the first frame update
  void Start()
  {
    controller = GameController.GetInstance();
    GameConfigSO config = controller.gameAssets.gameConfig;
    client = new CourseApiClient(
      config.tupConfig.apiBaseURL,
      config.tupConfig.accessToken
    );

    bool onboardingDone = controller.gameState.onboardingData.OnboardingFinished();
    if (multiplayerButton != null)
    {
      multiplayerButton.interactable = onboardingDone;
    }

    if (classRoomButton != null)
    {
      classRoomButton.interactable = onboardingDone;
    }

    Course currentCourse = controller.gameState.course;
    if(currentCourse != null)
    {
      _ = UpdateCurrentCourse(currentCourse, controller.gameState.profileData);
    }
  }

  private async System.Threading.Tasks.Task UpdateCurrentCourse(Course course, ProfileData profile)
  {

    if(String.IsNullOrEmpty(course.accessCode) || String.IsNullOrEmpty(profile.playerId))
    {
      return;
    }

    CourseData data = new CourseData()
    {
      accessCode = course.accessCode,
      userId = profile.playerId,
    };

    Course updatedCourseData = await client.GetCourse(data);
    if(updatedCourseData != null)
    {
      controller.gameState.course = updatedCourseData;
      controller.SaveGame();
    }
  }
}
