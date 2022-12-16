using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using FullSerializer;

public class AnalyticsHandler
{
  public ResponseData lastError;

  private readonly string targetUrl;
  private readonly string userId;
  private readonly string accessToken;

  private fsSerializer serializer;
  private string filePath = Application.persistentDataPath + "/logs.json";
  private List<AnalyticsEventData> eventLog = new List<AnalyticsEventData>();
  
  public AnalyticsHandler(string baseUrl, string accessToken, string userId)
  {
    targetUrl = baseUrl + "/api/course/log.json";
    this.userId = userId;
    this.accessToken = accessToken;

    serializer = new fsSerializer();
    LoadFromSaveGame();
  }

  public void ContinueCounting()
  {
    foreach(AnalyticsEventData data in eventLog)
    {
      data.ContinueCounting();
    }
  }

  public void PauseCounting()
  {
    foreach (AnalyticsEventData data in eventLog)
    {
      data.PauseCounting();
    }
  }

  public void CompleteAllOngoingEvents()
  {
    foreach (AnalyticsEventData data in eventLog)
    {
      data.ForceEnd();
    }
  }

  public void AddLogEntry(AnalyticsEventData data)
  {
    data.SetUserId(userId);
    eventLog.Add(data);
  }

  /**
    * Try to submit the logdata, if it fails keep it
    * local and try again later
    */
  public async void TrySubmit()
  {
    if(eventLog.Count > 0)
    {
      List<AnalyticsEventData> finishedEvents = eventLog.Where((AnalyticsEventData logItem) => logItem.status == AnalyticsEventStatus.Completed && logItem.activeTimeInS > 0).ToList();
      
      if(finishedEvents.Count == 0)
      {
        return;
      }
      
      DateTime submitTime = DateTime.Now;

      bool success = await Submit(finishedEvents);
      if(success == true)
      {
        // keep ongoing events and events that have finished while
        // waiting for the request to end
        eventLog = eventLog.Where(
          (AnalyticsEventData logData) => 
            logData.status != AnalyticsEventStatus.Completed ||
            (
              logData.endTime != DateTime.MinValue &&
              logData.endTime.Subtract(submitTime).TotalSeconds > 0
            )
        ).ToList();
      }

      PersistToSaveGame();
    }
  }

  private async Task<bool> Submit(List<AnalyticsEventData> finishedEvents)
  {
    serializer.TrySerialize(finishedEvents, out fsData data).AssertSuccessWithoutWarnings();
    string json = fsJsonPrinter.CompressedJson(data);

    using (UnityWebRequest www = UnityWebRequest.Put(targetUrl, json))
    {
      www.SetRequestHeader("Content-Type", "application/json");
      www.SetRequestHeader("Authorization", "Bearer " + accessToken);
      www.timeout = 5;

      var operation = www.SendWebRequest();
      while (!operation.isDone)
      {
        await System.Threading.Tasks.Task.Yield();
      }

      if (www.responseCode >= 200 && www.responseCode < 300)
      {
        return true;
      }

      lastError = CreateErrorFromRequest(www);

      return false;
    }
  }

  private void PersistToSaveGame()
  {
    serializer.TrySerialize(eventLog, out fsData data).AssertSuccessWithoutWarnings();
    string json = fsJsonPrinter.CompressedJson(data);
    StreamWriter file = new StreamWriter(filePath);
    file.WriteLine(json);
    file.Flush();
    file.Close();
  }

  private void LoadFromSaveGame()
  {
    if(LogfileExists() == true)
    {
      StreamReader file = new StreamReader(filePath);
      string stringData = file.ReadToEnd();
      file.Close();
      fsData data = fsJsonParser.Parse(stringData);
      serializer.TryDeserialize(data, ref eventLog);
    }
  }

  private bool LogfileExists()
  {

    return File.Exists(filePath);
  }

  private ResponseData CreateErrorFromRequest(UnityWebRequest www)
  {
    return new ResponseData()
    {
      error = www.error,
      isHttpError = www.isHttpError,
      isNetworkError = www.isNetworkError,
      responseCode = www.responseCode,
      isDone = www.isDone,
    };
  }
}