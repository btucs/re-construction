using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Multiplayer.API;

public struct ResponseData
{
  public string error;
  public bool isNetworkError;
  public bool isHttpError;
  public long responseCode;
  public bool isDone;
}

public struct EnrolData
{
  public string accessCode;
  public string userId;
  public string userName;
}

public struct CourseData
{
  public string accessCode;
  public string userId;
}

public class CourseApiClient {

  private readonly string baseUrl;
  private readonly string accessToken;
  private JsonSerializationOption jsonDeserialization;

  public ResponseData lastError;

  public CourseApiClient(string baseUrl, string accessToken)
  {
    this.baseUrl = baseUrl;
    this.accessToken = accessToken;
    jsonDeserialization = new JsonSerializationOption();
  }

  public async Task<Course> Enrol(EnrolData data )
  {
    string url = baseUrl + "/api/course/enrol.json";
    string json = JsonUtility.ToJson(data);


    using (UnityWebRequest www = UnityWebRequest.Put(url, json))
    {
      return await SendWebRequest<Course>(url, www);
    }
  }

  public async Task<Course> GetCourse(CourseData data)
  {
    string url = baseUrl + "/api/course/" + data.accessCode + "-" + data.userId + ".json";
    using (UnityWebRequest www = UnityWebRequest.Get(url))
    {
      return await SendWebRequest<Course>(url, www);
    }
  }

  public async System.Threading.Tasks.Task<bool> UnEnroll(CourseData data)
  {
    string url = baseUrl + "/api/course/" + data.accessCode + "-" + data.userId + ".json";

    using (UnityWebRequest www = UnityWebRequest.Delete(url))
    {
      return await SendWebRequest(url, www);
    }
  }

  private async System.Threading.Tasks.Task<bool> SendWebRequest(string url, UnityWebRequest www)
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

  private async Task<T> SendWebRequest<T>(string url, UnityWebRequest www)
  {
    www.SetRequestHeader("Content-Type", "application/json");
    www.SetRequestHeader("Authorization", "Bearer " + accessToken);
    www.timeout = 5;
    // Request and wait for the desired page.
    var operation = www.SendWebRequest();
    while (!operation.isDone)
    {
      await Task<T>.Yield();
    }

    if(www.responseCode >= 200 && www.responseCode < 300)
    {
      return jsonDeserialization.Deserialize<T>(www.downloadHandler.data);
    }

    lastError = CreateErrorFromRequest(www);

    return default(T);
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
