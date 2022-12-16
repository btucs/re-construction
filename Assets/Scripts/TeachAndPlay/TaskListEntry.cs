using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskListEntry : MonoBehaviour
{
    public Text nameText;
    public Text statusText;
    public Image background;

    public Color defaultColor;
    public Color completedColor;

    public void Setup(CourseTask taskData, GameState gameData)
    {
        nameText.text = taskData.topic.name;
        if(taskData.HasBeenCompleted(gameData) == true)
        {
            background.color = completedColor;
            statusText.text = "abgeschlossen";
        } else {
            background.color = defaultColor;
            statusText.text = (gameData.course.GetNextTask(gameData).Compare(taskData) == true) ? "aktiv" : "gesperrt"; 
        }
    }
}
