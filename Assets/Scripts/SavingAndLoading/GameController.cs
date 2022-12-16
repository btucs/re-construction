#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FullSerializer;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameController : MonoBehaviour
{
  public GameState gameState = new GameState();
  public GameAssetsSO gameAssets;

  [ReadOnly, fsIgnore]
  public playerScript playerScript;
  [ReadOnly, fsIgnore]
  public AnalyticsHandler analyticsHandler;
  
  public bool IsLoaded {
    get; private set;
  }
  
  private static GameController Instance;
  private string filePath;
  private fsSerializer serializer;

  public static GameController GetInstance() {
    return Instance;
  }

  void Awake() {

    filePath = Application.persistentDataPath + "/gamesave.save";
    InitializeSerializer();

    if(Instance == null) {

      DontDestroyOnLoad(gameObject);
      Instance = this;

      LoadGame();

      // make sure playerId exists
      string playerId = gameState.profileData.playerId;
      if (string.IsNullOrEmpty(playerId))
      {
        playerId = gameState.profileData.playerId = gameState.profileData.generatePlayerId();
      }

      analyticsHandler = new AnalyticsHandler(
        gameAssets.gameConfig.tupConfig.apiBaseURL,
        gameAssets.gameConfig.tupConfig.accessToken,
        playerId
      );
      analyticsHandler.AddLogEntry(new GameAnalyticsEventData());

      InvokeRepeating("TrySubmitAnalytics", 60f, 60f);
    } else if(Instance != this) {

      Destroy(gameObject);

      return;
    }    
  }

  private void TrySubmitAnalytics()
  {
    analyticsHandler.TrySubmit();
  }

  private void OnApplicationFocus(bool focus)
  {
    HandleAnalytics(focus);
  }

  private void OnApplicationPause(bool pause)
  {
    HandleAnalytics(!pause);
  }

  private void OnApplicationQuit()
  {
    analyticsHandler.CompleteAllOngoingEvents();
    analyticsHandler.TrySubmit();
  }

  private void OnDestroy() {

    // SaveGame();
  }

  private void HandleAnalytics(bool shouldContinue)
  {
    if (shouldContinue == true)
    {
      analyticsHandler.ContinueCounting();
    }
    else
    {
      analyticsHandler.PauseCounting();
    }
  }

#if UNITY_EDITOR
  [Button(Name = "Save Data")]
  public void SaveFromEditor() {

    filePath = Application.persistentDataPath + "/gamesave.save";
    InitializeSerializer();
    // fsPrettyPrinter.PrettyJson is broken
    SaveGame(false);
  }
#endif
  public void SaveGame() {

    SaveGame(false);
  }

  public void SaveGame(bool prettyPrint) {

    serializer.TrySerialize<GameState>(gameState, out fsData data).AssertSuccessWithoutWarnings();
    string json = prettyPrint ? fsJsonPrinter.PrettyJson(data) : fsJsonPrinter.CompressedJson(data);
    StreamWriter file = new StreamWriter(filePath);
    file.WriteLine(json);
    file.Flush();
    file.Close();

    Debug.Log("Game Saved to " + filePath);
  }

  public void LoadGame() {
    if(SavegameExists()) {

      StreamReader file = new StreamReader(filePath);
      string stringData = file.ReadToEnd();
      file.Close();
      fsData data = fsJsonParser.Parse(stringData);
      fsSerializer serializer = new fsSerializer();
      serializer.TryDeserialize<GameState>(data, ref gameState);

      Debug.Log("Game Loaded");
    } else {
      Debug.Log("No game saved!");
    }

    IsLoaded = true;
  }

  public bool SavegameExists() {

    return File.Exists(filePath);
  }

  private void InitializeSerializer() {

    serializer = new fsSerializer();
    serializer.AddConverter(new XorShiftConverter());
  }

  public void SaveGameInDocuments()
  {
    string pathlocation = filePath;

    #if UNITY_IPHONE
      string path = Application.dataPath.Substring (0, Application.dataPath.Length - 5);
      path = path.Substring(0, path.LastIndexOf('/'));  
      pathlocation = path + "/Documents/gamesave.save";
    #endif

    serializer.TrySerialize<GameState>(gameState, out fsData data).AssertSuccessWithoutWarnings();
    string json = fsJsonPrinter.CompressedJson(data);
    StreamWriter file = new StreamWriter(filePath);
    file.WriteLine(json);
    file.Flush();
    file.Close();

    Debug.Log("Game Saved to " + pathlocation);
  }
}
