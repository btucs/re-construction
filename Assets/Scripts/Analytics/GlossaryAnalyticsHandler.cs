using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlossaryAnalyticsHandler : AbstractAnalyticsHandlerMonoBehaviour
{
  private GameController controller;
  private GlossaryAnalyticsEventData logData;

  private void OnDisable()
  {
    EndEvent();
  }

  protected override void InitializeEvent()
  {
    controller = GameController.GetInstance();
    
    
  }

  public void StartEvent(GlossaryEntrySO glossary)
  {
    if (logData != null)
    {
      EndEvent();
    }
    logData = new GlossaryAnalyticsEventData(
      glossary,
      controller.gameAssets.FindTopicByGlossary(glossary)
    );
    controller.analyticsHandler.AddLogEntry(logData);
  }

  public override void EndEvent()
  {
    if(logData.status != AnalyticsEventStatus.Completed)
    {
      logData.EndEvent();
    }
  }
}
