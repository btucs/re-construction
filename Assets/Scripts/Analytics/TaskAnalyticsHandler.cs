using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskAnalyticsHandler : AbstractAnalyticsHandlerMonoBehaviour
{
  private GameController controller;
  private TaskAnalyticsEventData logData;
  private int repetitions = 0;
  private int currentPoints = 0;

  void Start()
  {
    InitializeEvent();
  }

  protected override void InitializeEvent()
  {
    controller = GameController.GetInstance();
    TaskDataSO taskData = controller.gameState.konstruktorSceneData.taskData;
    int maxExp = MaxExpHelper.GetMaxExp(
      controller.gameAssets.variableInfo,
      taskData
    );
    logData = new TaskAnalyticsEventData(taskData, maxExp);
    controller.analyticsHandler.AddLogEntry(logData);
  }

  public override void EndEvent()
  {
    logData.EndEvent(currentPoints, repetitions);
  }

  public void SetCurrentPoints(int currentPoints)
  {
    this.currentPoints = currentPoints;
  }

  public void SetRepetitions(int repetitions)
  {
    this.repetitions = repetitions;
  }
}
