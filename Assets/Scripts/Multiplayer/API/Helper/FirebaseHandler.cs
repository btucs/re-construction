#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universit?t Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.Multiplayer.Logging;
using Assets.Scripts.Multiplayer.API;
using Firebase;
using Firebase.Extensions;
using Sirenix.OdinInspector;

namespace Assets.Scripts.Multiplayer.API.Helper
{
  public class FirebaseHandler : MonoBehaviour
  {
    [Required]
    public ConfirmationPopupController popupController;

    private bool firebaseInitialized = false;
    private GameController controller;

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    void Start() {

      controller = GameController.GetInstance();
     
      Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        dependencyStatus = task.Result;
        if(dependencyStatus == Firebase.DependencyStatus.Available) {

          if (String.IsNullOrEmpty(controller.gameState.multiplayer.playerId) == false)
          {
            InitializeFirebase();
          }
        } else {
          UnityLogger.GetLogger().Debug(
            "Could not resolve all Firebase dependencies: " + dependencyStatus
          );
        }
      });

      popupController?.ConfirmButton?.onClick.AddListener(GotoMultiplayer);
    }

    // Setup message event handlers.
    public async void InitializeFirebase() {

      try {

        // This will display the prompt to request permission to receive
        // notifications if the prompt has not already been displayed before. (If
        // the user already responded to the prompt, thier decision is cached by
        // the OS and can be changed in the OS settings).
        await Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync();
        //LogTaskCompletion(task, "RequestPermissionAsync");

        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        string token = await Firebase.Messaging.FirebaseMessaging.GetTokenAsync();
        controller.gameState.multiplayer.fcmToken = token;

        string bearerToken = controller.gameAssets.gameConfig.apiConfig.accessToken;
        string playerId = controller.gameState.multiplayer.playerId;
        
        APIClientPushNotificationClient apiClient = new APIClientPushNotificationClient(bearerToken);

        await apiClient.RegisterFirebaseToken(token, playerId);

        //var topic = MultiplayerGameManager.Topic + MultiplayerGameManager.getPlayerId();
        //await Firebase.Messaging.FirebaseMessaging.SubscribeAsync(topic);
        //LogTaskCompletion(topic, "SubscribeAsync");

        UnityLogger.GetLogger().Debug("Firebase Messaging Initialized");
        firebaseInitialized = true;
      } catch(Exception e) {

        UnityLogger.GetLogger().Debug(e.Message);
      }
    }

    public virtual void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
      UnityLogger.GetLogger().Debug("Received a new message");
      var notification = e.Message.Notification;
      if(notification != null) {
        UnityLogger.GetLogger().Debug("title: " + notification.Title);
        UnityLogger.GetLogger().Debug("body: " + notification.Body);
        var android = notification.Android;
        if(android != null) {
          UnityLogger.GetLogger().Debug("android channel_id: " + android.ChannelId);
        }
      }
      if(e.Message.From.Length > 0) {
        UnityLogger.GetLogger().Debug("from: " + e.Message.From);
      }

      if(e.Message.Link != null) {
        UnityLogger.GetLogger().Debug("link: " + e.Message.Link.ToString());
      }
      if(e.Message.Data.Count > 0) {
        UnityLogger.GetLogger().Debug("data:");
        foreach(KeyValuePair<string, string> iter in
                 e.Message.Data) {
          UnityLogger.GetLogger().Debug("  " + iter.Key + ": " + iter.Value);
        }
      }

      string currentScenePath = SceneManager.GetActiveScene().path;

      if(notification != null && currentScenePath.Contains("Multiplayer") == false) {

        popupController.MessageText.text = notification.Title + "\n\n" + "Multiplayer laden?";
        popupController.Show();
      }
    }

    public virtual void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
      UnityLogger.GetLogger().Debug("Received Registration Token: " + token.Token);


    }

    // End our messaging session when the program exits.
    public void OnDestroy() {

      if(firebaseInitialized == true) {

        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
      }

      popupController?.ConfirmButton?.onClick.RemoveListener(GotoMultiplayer);
    }

    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    protected bool LogTaskCompletion(System.Threading.Tasks.Task task, string operation) {
      bool complete = false;
      if(task.IsCanceled) {
        UnityLogger.GetLogger().Debug(operation + " canceled.");
      } else if(task.IsFaulted) {
        UnityLogger.GetLogger().Debug(operation + " encounted an error.");
        foreach(Exception exception in task.Exception.Flatten().InnerExceptions) {
          string errorCode = "";
          if(exception is FirebaseException firebaseEx) {
            errorCode = String.Format("Error.{0}: ",
              ((Firebase.Messaging.Error)firebaseEx.ErrorCode).ToString());
          }
          UnityLogger.GetLogger().Debug(errorCode + exception.ToString());
        }
      } else if(task.IsCompleted) {
        UnityLogger.GetLogger().Debug(operation + " completed");
        complete = true;
      }
      return complete;
    }

    private void OnApplicationPause(bool isPaused) {

      if(Application.isEditor) {

        return;
      }

      if(isPaused == false) {

        // Returning to Application
        StartCoroutine(LoadFromFCM());
      }
    }

    private IEnumerator LoadFromFCM() {
#if UNITY_ANDROID
      try {

        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject curActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject curIntent = curActivity.Call<AndroidJavaObject>("getIntent");

        string notificationType = curIntent.Call<string>("getStringExtra", "type");

        Scene curScene = SceneManager.GetActiveScene();

        if(notificationType != null && curScene.name.Contains("Multiplayer") == false) {

          SceneManager.LoadScene("StartMultiplayer");
        }
      } catch(Exception e) {


      }
      #endif
      yield return true;
    }

    private void GotoMultiplayer()
    {
      SceneManager.LoadScene("StartMultiplayer");
    }
  }
}