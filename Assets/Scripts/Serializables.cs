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
using System.Globalization;
using UnityEngine;
using FullSerializer;

[Serializable]
public class TaskHistoryData
{
  public List<FinishedMLEData> mleHistory = new List<FinishedMLEData>();
  public List<FinishedTaskData> taskHistoryData = new List<FinishedTaskData>();
  public List<FinishedTaskData> courseHistoryData = new List<FinishedTaskData>();
  public List<FinishedTaskData> skippedCourseTaskData = new List<FinishedTaskData>();


  public FinishedTaskData FindFinished(TaskDataSO taskData) {

    return taskHistoryData.FirstOrDefault((FinishedTaskData data) => data.solvedTask == taskData);
  }

  //currently: Get average points per task
  public float GetPlayerPerformance()
  {
    int pointsAbs = 0;
    foreach(FinishedTaskData singleData in taskHistoryData)
    {
      pointsAbs += singleData.achievedPoints;

    }

    return (float)pointsAbs / (float)taskHistoryData.Count;
  }

  public int GetCourseTaskAmount(TopicSO _taskType)
  {
    int amount = 0;
    foreach(FinishedTaskData singleTask in courseHistoryData)
    {
      if(singleTask.solvedTask.topic != null && singleTask.solvedTask.topic.name == _taskType.name)
        amount ++;
    }
    foreach(FinishedTaskData singleTask in skippedCourseTaskData)
    {
      if(singleTask.solvedTask.topic != null && singleTask.solvedTask.topic.name == _taskType.name)
        amount ++;
    }
    //Debug.Log("Task amount for: " + _taskType.ToString() + " is: " + amount);
    return amount;
  }

  public bool IsCourseTaskDone(TaskDataSO theTask)
  {
    foreach(FinishedTaskData singleTask in courseHistoryData)
    {
      if(theTask.UID == singleTask.solvedTask.UID)
        return true;
    }
    foreach(FinishedTaskData singleTask in skippedCourseTaskData)
    {
      if(theTask.UID == singleTask.solvedTask.UID)
        return true; 
    }
    return false;
  }

  //returns a value > 2 if two tasks for the requested MLE have been completed at least to 66% 
  public float GetTopicProgressIndex(MLEDataSO theTopic)
  {
    float progressFloat = 0f;

    GameController controller = GameController.GetInstance();
    VariableInfoSO info = controller.gameAssets.variableInfo;
    foreach(FinishedTaskData singleTask in taskHistoryData)
    {
      int stepCount = singleTask.solvedTask.steps.Length;
      if(stepCount > 0)
      {

        TaskOutputVariableUnit outputUnit = singleTask.solvedTask.steps[stepCount - 1].output.unit;
        //VariableInfoSO.VariableInfoEntry infoEntry = info.GetInfoFor(outputUnit);

        MLEDataSO taskMLE = info.GetInfoFor(outputUnit).mle;
        if(taskMLE == theTopic)
        {
          int maxExp = MaxExpHelper.GetMaxExp(info, singleTask.solvedTask);
          progressFloat += ((float)maxExp / (float)singleTask.achievedPoints) / 0.66f;
        }
      }
    }

    return progressFloat;
  } 

  public float GetTopicProgressIndex(TopicSO theTopic)
  {
    float progressFloat = 0f;

    TaskDataSO[] destinctTasks = GetDistinctTasks();

    GameController controller = GameController.GetInstance();
    VariableInfoSO info = controller.gameAssets.variableInfo;

    for(int i=0; i<destinctTasks.Length; i++)
    {
      if(destinctTasks[i].topic == theTopic)
      {
        int maxExp = MaxExpHelper.GetMaxExp(info, destinctTasks[i]);
        float addedValue = (float)FindFinishedMaxPoints(destinctTasks[i]).achievedPoints;
        addedValue = (addedValue/(float)maxExp) / 0.66f;
        addedValue = Mathf.Clamp(addedValue, 0f, 1f);
        progressFloat += (addedValue * 0.5f); //2x with more than 66% right -> Value > 1
      }
    }

    foreach(MLEDataSO mle in theTopic.mles)
    {
      if(mle == null)
        continue;
      FinishedMLEData bestMLE = FindFinishedMaxPoints(mle);
      if(bestMLE == null)
        continue;
      int maxPoints = mle.GetMaxPoints();
      progressFloat += ((float)bestMLE.achievedPoints/(float)maxPoints) * 0.25f;
    }

    return progressFloat;
  }

  public FinishedTaskData FindFinishedMaxPoints(TaskDataSO taskData)
  {
    FinishedTaskData maxPointsTask = null;

    foreach(FinishedTaskData singleData in taskHistoryData)
    {
      if(singleData.solvedTask == taskData)
      {
        if(maxPointsTask == null || singleData.achievedPoints > maxPointsTask.achievedPoints)
        {
          maxPointsTask = singleData;
        }
      }
    }
    return maxPointsTask;
  }

  public FinishedTaskData GetNewestTaskData(TaskDataSO taskData) {

    return taskHistoryData.LastOrDefault((FinishedTaskData data) => data.solvedTask == taskData);
  }

  public FinishedTaskData GetNewestCourseTaskData(TaskDataSO taskData) {

    return courseHistoryData.LastOrDefault((FinishedTaskData data) => data.solvedTask == taskData);
  }

  public FinishedMLEData FindFinishedMaxPoints(MLEDataSO mleData) {

    return mleHistory.Aggregate(
      null,
      (FinishedMLEData found, FinishedMLEData current) => {

        if(
          (found == null && current.solvedMLE == mleData) ||
          (found != null && current.solvedMLE == mleData && found.achievedPoints > current.achievedPoints)
        ) {

          return current;
        }

        return found;
      }
    );
  }

  public TaskState GetStateOfTask(TaskDataSO taskData)
  {
    TaskState stateOfTask = TaskState.unsolved;

    if(ExistsEntry(taskData))
    {
      FinishedTaskData bestTaskSave = FindFinishedMaxPoints(taskData);
      if(bestTaskSave.achievedPoints > 2 || taskData.taskName == "Kraftarten identifizieren") //Spezialfall für onboarding-aufgabe
      {
        stateOfTask = TaskState.solvedCorrect;
      } else {
        stateOfTask = TaskState.solvedWrong;
      }
    }

    return stateOfTask;
  }

  public FinishedMLEData FindFinished(MLEDataSO mleData) {

    return mleHistory.FirstOrDefault((FinishedMLEData data) => data.solvedMLE == mleData);
  }

  public bool ExistsEntry(TaskDataSO taskData) {

    return FindFinished(taskData) != null;
  }

  public bool ExistsEntry(MLEDataSO mleData) {

    return FindFinished(mleData) != null;
  }  

  public int CalculateCurrentEXP() {

    int taskPoints = taskHistoryData.GroupBy(
      (FinishedTaskData finishedTask) => finishedTask.solvedTask,
      (FinishedTaskData finishedTask) => finishedTask.achievedPoints,
      (TaskDataSO solvedTask, IEnumerable<int> points) => points.Max()
    ).Sum();

    int mlePoints = mleHistory.GroupBy(
      (FinishedMLEData mleData) => mleData.solvedMLE,
      (FinishedMLEData mleData) => mleData.achievedPoints,
      (MLEDataSO mleData, IEnumerable<int> points) => points.Max()
    ).Sum();

    int totalPoints = taskPoints + mlePoints;

    return totalPoints;
  }

  public int CountDistinctTasks() {

    int count = taskHistoryData.GroupBy(
      (FinishedTaskData finishedTask) => finishedTask.solvedTask
    ).Count();

    return count;
  }

  public TaskDataSO[] GetDistinctTasks() {

    IEnumerable<TaskDataSO> tasks = taskHistoryData.GroupBy(
      (FinishedTaskData finishedTask) => finishedTask.solvedTask
    ).Select((IGrouping<TaskDataSO, FinishedTaskData> item) => item.Key);

    return tasks.ToArray();
  }

  public int GetFinishedTasksByMLE(MLEDataSO requiredMLE, VariableInfoSO taskMLEDeriver)
  {
    int tasksFound = 0;

    foreach(FinishedTaskData taskHistoryEntry in taskHistoryData)
    {
      int stepCount = taskHistoryEntry.solvedTask.steps.Length;
      if(stepCount > 0)
      {

        TaskOutputVariableUnit outputUnit = taskHistoryEntry.solvedTask.steps[stepCount - 1].output.unit;
        VariableInfoSO.VariableInfoEntry infoEntry = taskMLEDeriver.GetInfoFor(outputUnit);
        if(infoEntry != null && infoEntry.mle == requiredMLE)
        {

          tasksFound ++;
        }
      }      
    }
    
    return tasksFound;
  }
}

[Serializable]
[fsObject(Converter = typeof(FinishedMLEDataConverter))]
public class FinishedMLEData {
	public MLEDataSO solvedMLE;
	public int achievedPoints;

  public FinishedMLEData() {

  }

	public FinishedMLEData(MLEDataSO solvedMLEData, int receivedPoints)
	{
		solvedMLE = solvedMLEData;
		achievedPoints = receivedPoints;
	}
}

[Serializable]
[fsObject(Converter = typeof(FinishedTaskDataConverter))]
public class FinishedTaskData
{
  public TaskDataSO solvedTask;
  public TaskObjectSO taskObject;
  public int achievedPoints;

  public FinishedTaskData() {

  }

  public FinishedTaskData(TaskDataSO solvedTask, TaskObjectSO taskObject, int achievedPoints) {

    this.solvedTask = solvedTask;
    this.taskObject = taskObject;
    this.achievedPoints = achievedPoints;
  }
}

[Serializable]
public class MultiplayerData {
  public string playerId;
  public string playerName;
  public string fcmToken;
}

[Serializable]
public class GameState {
  public CharacterData characterData = new CharacterData();
  public KonstruktorSceneData konstruktorSceneData = new KonstruktorSceneData();
  public TaskHistoryData taskHistoryData = new TaskHistoryData();
  public OnboardingData onboardingData = new OnboardingData();
  public ProfileData profileData = new ProfileData();
  public WorldData gameworldData = new WorldData();
  public SettingPreferences settings = new SettingPreferences();
  public MultiplayerData multiplayer = new MultiplayerData();
  public Course course = null;

  // keep characterData
  public void SoftReset() {

    konstruktorSceneData = new KonstruktorSceneData();
    taskHistoryData = new TaskHistoryData();
    onboardingData = new OnboardingData();
    profileData = new ProfileData();
    gameworldData = new WorldData();
    multiplayer = new MultiplayerData();
  }

  [Obsolete]
  public Course CreateExampleCourse(List<TopicSO> specifiedTopics)
  {
    Debug.Log("Setting new CourseData");
    Course returnVal = new Course();
    returnVal.name = "BauIng BTU WiSe 2026";
    returnVal.accessCode ="1234567";
    returnVal.accessable = true;
    returnVal.joinDate = DateTime.Now;

    CourseTask[] taskarray = new CourseTask[specifiedTopics.Count];

    for(int i=0; i<specifiedTopics.Count; i++)
    {
      TopicSO unit = specifiedTopics.ElementAt(i);

      DateTime startDate = DateTime.Today.AddDays(-1);
      DateTime endDate = DateTime.Today.AddDays(14);
      
      if(unit != null)
      {
        taskarray[i] = new CourseTask(unit, 1, startDate, endDate);
      } else {
        Debug.LogError("Task at position " + i + " was not setup correctly.");
      }
        if(unit.name == "Vektor zeichnen")
        {
          MLEQuiz[] mleQuestions = new MLEQuiz[1];
          mleQuestions[0] = new MLEQuiz();
          mleQuestions[0].question = "Welche Aussage beschreibt die Richtung eines Vektors?";

          MLEQuizChoice[] answers = new MLEQuizChoice[4];
          answers[0] = new MLEQuizChoice("Sie wird über die Wirkungslinie beschrieben.", "Richtig. Die Wirkungslinie beschreibt die Richtung und die Pfeilspitze beschreibt den Richtungssinn.", true);
          answers[1] = new MLEQuizChoice("Sie wird durch den Endpunkt beschrieben.", "Die Pfeilspitze beschreibt den Richtungssinn und die Verbindungslinie der beiden Punkte die Richtung.", false);
          answers[2] = new MLEQuizChoice("Sie wird durch die Größe des Vektors beschrieben.", "Die Größe des Vektors hat mit der Richtung oder dem Richtungssinn nichts zu tun. Diese sind immer gleich, unabhängig von der Größe.", false);
          answers[3] = new MLEQuizChoice("Sie wird durch den Pfeil beschrieben.", "Sehr nah, aber nicht ganz richtig. Die Verbindungslinie zwischen Anfangs- und Endpunkt beschreibt die Richtung und der Pfeil den Richtungssinn. Das ist nicht dasselbe!", false);
          mleQuestions[0].choices = answers;

          taskarray[i].singleChoiceQuestions = mleQuestions;

        } else if(unit.name == "Kraft zeichnen") {

          MLEQuiz[] mleQuestions = new MLEQuiz[1];
          mleQuestions[0] = new MLEQuiz();
          mleQuestions[0].question = "Was ist der Unterschied von grafischen und analytischen Angeben / Umrechnen der Kraft?";

          MLEQuizChoice[] answers = new MLEQuizChoice[4];
          answers[0] = new MLEQuizChoice("Die analytische Lösung ist nicht so genau, wie die grafische.", "Das ist nicht korrekt, überlege, wie genau beide verfahren sein können.", false);
          answers[1] = new MLEQuizChoice("Die grafische Lösung ist nicht so genau wie die analytische.", "Genau, denn die Berechnung kann auf viele Kommastellen berechnet werden.", true);
          answers[2] = new MLEQuizChoice("Beide Lösungen sind gleich genau.", "Nein, eine der Lösung ist genauer als die andere...", false);
          answers[3] = new MLEQuizChoice("Die grafische Lösung beschreibt einen Zwischenschritt bei der analytischen Lösung", "Das ist nicht richtig. Beide Lösungen können sich ergänzen aber auch unabhängig voneinander sein.", false);
          mleQuestions[0].choices = answers;

          taskarray[i].singleChoiceQuestions = mleQuestions;
        }
    }
    returnVal.tasks = taskarray;

    return returnVal;
  }

  [Obsolete]
  public Course CreateExampleCourse(List<TopicSO> topics, int topicAmount, int taskAmount = 1)
  {
    Debug.Log("Setting new CourseData");
    Course returnVal = new Course();
    returnVal.name = "BauIng BTU WiSe 2026";
    returnVal.accessCode ="1234567";
    returnVal.accessable = true;
    returnVal.joinDate = DateTime.Now;

    CourseTask[] taskarray = new CourseTask[topicAmount];

    for(int i=0; i<topicAmount; i++)
    {
      TopicSO unit = null;
      if(topics.Count > i)
      {
        unit = topics.ElementAt(i);
      } else {
        Debug.LogError("Not enough topics. Topics required: " + topicAmount);
      }

      DateTime startDate = DateTime.Today.AddDays(-1);
      DateTime endDate = DateTime.Today.AddDays(3);
      
      if(unit != null)
      {
        taskarray[i] = new CourseTask(unit, taskAmount, startDate, endDate);
      } else {
        Debug.LogError("Task at position " + i + " was not setup correctly.");
      }

      
        MLEQuiz[] mleQuestions = new MLEQuiz[2];

        mleQuestions[0] = new MLEQuiz();
        mleQuestions[0].question = "Wie ist das Wetter heute?";
        MLEQuizChoice[] answers = new MLEQuizChoice[4];
        answers[0] = new MLEQuizChoice("Ziemlich gut. Die Sonne scheint, der Himmel ist blau", "Dann genieße es!", true);
        answers[1] = new MLEQuizChoice("Ziemlich mies. Es regnet und Wolken verhängen den Himmel", "Dann nutz die Zeit und erledige vielleicht etwas Hausarbeit.", false);
        answers[2] = new MLEQuizChoice("Geht so...", "Es kommen auch wieder bessere Tage", false);
        answers[3] = new MLEQuizChoice("Keine Ahnung. Habe noch nicht aus dem Fenster geguckt.", "Dann weg vom Bildschirm und ab nach draußen!", false);
        mleQuestions[0].choices = answers;

        mleQuestions[1] = new MLEQuiz();
        mleQuestions[1].question = "Wird auch die zweite Frage angezeigt?";
        MLEQuizChoice[] _answers = new MLEQuizChoice[4];
        _answers[0] = new MLEQuizChoice("Ja, ich sehe sie ganz deutlich!", "Na, Gott sei dank!", true);
        _answers[1] = new MLEQuizChoice("Ich glaube nicht. Wo soll die sein?", "Mh.. Überprüf das lieber nochmal.", false);
        _answers[2] = new MLEQuizChoice("Definitiv nein!", "Definitiv falsch!", false);
        _answers[3] = new MLEQuizChoice("Kein Plan wovon du redest.", "Junge! Lass die Drogen weg!", false);
        mleQuestions[1].choices = _answers;

        taskarray[i].singleChoiceQuestions = mleQuestions;
    }
    returnVal.tasks = taskarray;

    return returnVal;
  } 

  public void Reset() {

    SoftReset();
    characterData = new CharacterData();
  }
}