using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;

public enum AnalyticsEventType
{
  Task,
  MLE,
  Glossary,
  Handout,
  Game,
}

public enum AnalyticsEventStatus
{
  Running,
  Paused,
  Completed,
}

[Serializable]
public abstract class AnalyticsEventData 
{
  public AnalyticsEventStatus status { protected set; get; } = AnalyticsEventStatus.Running;
  public string userId { protected set; get; }
  public string name { protected set; get; }
  public AnalyticsEventType eventType { protected set; get; }
  public DateTime startTime { protected set; get; } = DateTime.Now;
  public DateTime endTime { protected set; get; }
  public string topic { protected set; get; }
  public int maxPoints { protected set; get; } = -1;
  public int currentPoints { protected set; get; } = -1;
  public int repetitions { protected set; get; } = -1;
  public int activeTimeInS { private set; get; }

  private float startS = -1;

  protected AnalyticsEventData()
  {
    startS = Time.time;
  }

  public void SetUserId(string userId)
  {
    this.userId = userId;
  }

  public void ContinueCounting()
  {
    if(status == AnalyticsEventStatus.Paused)
    {
      startS = Time.time;
      status = AnalyticsEventStatus.Running;
    }
  }

  public void PauseCounting()
  {
    if(status == AnalyticsEventStatus.Running && startS != -1)
    {
      activeTimeInS += (int) Math.Round(Time.time - startS);
      startS = -1;
      status = AnalyticsEventStatus.Paused;
    }
  }

  protected void Completed()
  {
    PauseCounting();

    endTime = DateTime.Now;
    status = AnalyticsEventStatus.Completed;
  }

  abstract public void ForceEnd();

  ~AnalyticsEventData()
  {
    if(startS != -1)
    {
      throw new Exception("PauseCounting has not been called in EndEvent");
    }
  }
}

public class TaskAnalyticsEventData : AnalyticsEventData
{
  // Task optional how many tries?
  public TaskAnalyticsEventData(TaskDataSO task, int maxPoints) : base()
  {
    eventType = AnalyticsEventType.Task;
    name = task.name;
    topic = task.topic.internalName;
    this.maxPoints = maxPoints;
  }

  public void EndEvent(int currentPoints, int repetitions )
  {
    this.currentPoints = currentPoints;
    this.repetitions = repetitions;

    Completed();
  }

  public override void ForceEnd()
  {
    EndEvent(-1, -1);
  }
}

public class MLEAnalyticsEventData : AnalyticsEventData
{
  public MLEAnalyticsEventData(MLEDataSO mle, TopicSO topic) : base()
  {
    eventType = AnalyticsEventType.MLE;
    name = mle.name;
    maxPoints = mle.GetMaxPoints();
    this.topic = topic.internalName;
  }

  public void EndEvent(int currentPoints, int repetitions)
  {
    this.currentPoints = currentPoints;
    this.repetitions = repetitions;

    Completed();
  }

  public override void ForceEnd()
  {
    EndEvent(-1, -1);
  }
}

public class HandoutsAnalyticsEventData : AnalyticsEventData
{
  public HandoutsAnalyticsEventData(MLEHandoutsSO handout, int maxPoints, TopicSO topic) : base()
  {
    eventType = AnalyticsEventType.Handout;
    name = handout.name;
    this.maxPoints = maxPoints;
    this.topic = topic.internalName;
  }

  public void EndEvent(int currentPoints, int repetitions)
  {
    this.currentPoints = currentPoints;
    this.repetitions = repetitions;
    Completed();
  }

  public override void ForceEnd()
  {
    EndEvent(-1, -1);
  }
}

public class GameAnalyticsEventData : AnalyticsEventData
{
  public GameAnalyticsEventData() : base()
  {
    eventType = AnalyticsEventType.Game;
  }

  public void EndEvent()
  {
    Completed();
  }

  public override void ForceEnd()
  {
    EndEvent();
  }
}

public class GlossaryAnalyticsEventData : AnalyticsEventData
{
  public GlossaryAnalyticsEventData(GlossaryEntrySO glossaryEntry, TopicSO topic): base()
  {
    eventType = AnalyticsEventType.Glossary;
    name = glossaryEntry.headline;
    this.topic = topic.internalName;
  }

  public void EndEvent()
  {
    Completed();
  }

  public override void ForceEnd()
  {
    EndEvent();
  }
}