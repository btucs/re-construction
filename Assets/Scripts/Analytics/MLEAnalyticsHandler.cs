using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLEAnalyticsHandler : AbstractAnalyticsHandlerMonoBehaviour
{
  private GameController controller;
  private AnalyticsEventData logData;
  private MLEDataTransporter mleData;

  private int repetitions = 0;

  void Start()
  {
    controller = GameController.GetInstance();
    GameObject dataObject = GameObject.FindWithTag("MLEDataObj");
    if(dataObject)
    {
      mleData = dataObject.GetComponent<MLEDataTransporter>();
    }
  }

  public override void EndEvent()
  {
    if(logData == null)
    {
      return;
    }

    if(logData is MLEAnalyticsEventData) {

      (logData as MLEAnalyticsEventData).EndEvent(mleData.achievedPoints, repetitions);
      return;
    }

    if(logData is HandoutsAnalyticsEventData)
    {
      (logData as HandoutsAnalyticsEventData).EndEvent(mleData.achievedPoints, repetitions);
      return;
    }
  }

  protected override void InitializeEvent()
  {
    
  }

  public void SetRepetitions(int repetitions)
  {
    this.repetitions = repetitions;
  }

  public void InitializeMLEEvent()
  {

    logData = new MLEAnalyticsEventData(
      mleData.mleTransportData,
      controller.gameAssets.FindTopicByMLE(mleData.mleTransportData)
    );
    controller.analyticsHandler.AddLogEntry(logData);
  }

  public void InitializeHandoutEvent()
  {
    logData = new HandoutsAnalyticsEventData(
      mleData.mleTransportData.mleHandout,
      mleData.mleTransportData.GetMaxPoints(),
      controller.gameAssets.FindTopicByMLE(mleData.mleTransportData)
    );
    controller.analyticsHandler.AddLogEntry(logData);
  }
}
