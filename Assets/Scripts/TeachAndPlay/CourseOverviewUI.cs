using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CourseOverviewUI : MonoBehaviour
{
  public Text courseNameText;
  public Text courseIDText;
  public Text courseTasksText;
  [Obsolete]
  public Text joinedText;

  public CanvasGroup activeMissionHeadline;
  public CanvasGroup allMissionsHeadline;

  public GameObject activeMissionContent;
  public GameObject allMissionsContent;
  public GameObject noDataContent;

  public GameObject missionEntryPrefab;
  public Transform allMissionsContainer;

  public Text activeMissionText;
  public Text activeMissionDeadline;
  public GameObject startButton;
  public GameObject infoBox;

  private GameController controller;
  private Course courseData;
  private CourseTask nextTask;
  private TeachAndPlayQuestController questController;

  public void InitializeContent()
  {
    questController = gameObject.GetComponent<TeachAndPlayQuestController>();
    controller = GameController.GetInstance();
    courseData = controller.gameState.course;

    if(courseData == null)
  {
      courseNameText.text = "Keine Kursdaten.";
      courseIDText.text = "Aktuell bist du in keinem Kurs eingeschrieben.";
      joinedText.gameObject.SetActive(false);
      courseTasksText.gameObject.SetActive(false);
      activeMissionContent.SetActive(false);
      allMissionsContent.SetActive(false);
      noDataContent.SetActive(true);
      return;
    }

    nextTask = courseData.GetNextTask(controller.gameState);

    courseNameText.text = courseData.name;
    courseIDText.text = "Kurs ID: " + courseData.accessCode;
    courseTasksText.text = "Aufgaben: " + courseData.TopicsFinished(controller.gameState) + '/' + courseData.tasks.Length + " abgeschlossen";
    joinedText.text = "Beigetreten: " + courseData.joinDate.ToString("d", CultureInfo.GetCultureInfo("de-DE"));

    activeMissionText.text = GetActiveMissionText();

    activeMissionDeadline.transform.parent.gameObject.SetActive(nextTask != null && nextTask.IsDateValid());
    if(nextTask != null)
    {
      activeMissionDeadline.text = "Deadline: " + nextTask.accessibleUntil.ToString("d", CultureInfo.GetCultureInfo("de-DE"));
    }
    
    startButton.SetActive(nextTask != null && questController.IsQuestActive() == false);
    infoBox.SetActive(questController.IsQuestActive() != false);

    allMissionsHeadline.interactable = nextTask != null;
    DisplayActiveMissionContent();
    InitializeMissionList();
  }

  public void DisplayActiveMissionContent()
  {
    activeMissionContent.SetActive(true);
    allMissionsContent.SetActive(false);
    activeMissionHeadline.alpha = 1f;
    allMissionsHeadline.alpha = 0.5f;
  }

  public void DisplayMissionList()
  {
    activeMissionContent.SetActive(false);
    allMissionsContent.SetActive(true);
    activeMissionHeadline.alpha = 0.5f;
    allMissionsHeadline.alpha = 1f;
  }

  private void InitializeMissionList()
  {
    foreach (Transform child in allMissionsContainer)
    {
      Destroy(child.gameObject);
    }

    for (int i = 0; i < courseData.tasks.Length; i++)
    {
      Instantiate(missionEntryPrefab, allMissionsContainer).GetComponent<TaskListEntry>().Setup(courseData.tasks[i], controller.gameState);
    }
  }

  private string GetActiveMissionText()
  {
    string returnstring;
    if(nextTask == null)
    {
      returnstring = "Aktuell gibt es keine Aufgaben zu erledigen. Schaue später nocheinmal nach ob deine Lehrperson Aufgaben eingestellt hat.";

      return returnstring;
    }

    if (nextTask.IsDateValid())
    {
      string taskAmountText = (nextTask.taskAmount > 1) ? nextTask.taskAmount + " Aufgaben" : "eine Aufgabe";
      string firstpart = questController.IsQuestActive() ? "Setze dich mit dem Inhalt " : "Starte die Mission und setze dich mit dem Inhalt ";
      returnstring = firstpart + nextTask.topic.name + " auseinander, indem du " + taskAmountText + " zu dem Thema absolvierst.";
    }
    else
    {
      returnstring = "Aktuell gibt es keine Aufgaben zu erledigen. Das nächste Thema ist ab dem " + nextTask.accessibleFrom.ToString("d", CultureInfo.GetCultureInfo("de-DE")) + " verfügbar.";
    }

    return returnstring;
  }
}
