#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Threading.Tasks;
using Assets.Scripts.Multiplayer.API.Models;
using UnityEngine;
using Proyecto26;

public static class NotificationType
{

  public static readonly string RequestAccepted = "RequestAccepted";
  public static readonly string RequestReceived = "RequestReceived";
  public static readonly string QuestionAnswered = "QuestionAnswered";
}

[Serializable]
public class ApiNotificationData
{
  public string type;

  public ApiNotificationData(string type) {

    this.type = type;
  }
}

[Serializable]
public class ApiNotification
{
  public string playerId;
  public string title;
  public string body;
  public ApiNotificationData data;

  public ApiNotification(string playerId, string title, string body, ApiNotificationData data)
  {

    this.playerId = playerId;
    this.title = title;
    this.body = body;
    this.data = data;
  }
}

public class APIClientPushNotificationClient
{
  private static string notificationEndpoint = MultiplayerGameManager.gameapi + "/QuizGameFirebase/PushMessageToFirebase";
  private static string registerTokenEndpoint = MultiplayerGameManager.gameapi + "/QuizGameFirebase/RegisterFirebaseToken";

  public APIClientPushNotificationClient(string bearerToken)
  {
    
    RestClient.DefaultRequestHeaders["Authorization"] = "Bearer " + bearerToken;
    RestClient.DefaultRequestHeaders["Content-Type"] = "application/json";
  }

  public async Task<string> SendNotification(ApiNotification notification) {

    /*WWWForm form = new WWWForm();
    form.AddField("turnMessage", turnMessage);
    form.AddField("bodyMessage", body);
    var topic = MultiplayerGameManager.Topic + playerId;
    form.AddField("topic", topic);

    if(data != null) {

      form.AddField("data", JsonConvert.SerializeObject(data));
    }

    using(AsyncHttpClient httpClient = new AsyncHttpClient(new JsonSerializationOption())) {

      return await httpClient.Post<string>(firebaseapi, form);
    }*/

    return await RestClient.Post<string>(notificationEndpoint, JsonUtility.ToJson(notification)).Await();
  }

  public async Task<FCMToken> RegisterFirebaseToken(string token, string playerId)
  {

    FCMToken tokenClass = new FCMToken()
    {
      Token = token,
      PlayerID = playerId
    };

    return await RestClient.Post<FCMToken>(registerTokenEndpoint, JsonUtility.ToJson(tokenClass)).Await();
  }
}
