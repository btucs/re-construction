#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Multiplayer.API;
using Assets.Scripts.Multiplayer.API.Models;
using Assets.Scripts.Multiplayer.Logging;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardFinishedScript: MonoBehaviour
{
    public Text txtInfo;
    public Text txtPoints;
    private APIClientLeaderboard _apiClientLeaderboard;
    private IList<QuizLeaderboard> oldSeasonsOfPlayer;

    void Start()
    {
        _apiClientLeaderboard = new APIClientLeaderboard();
    }

    public async void GetNotSeenOldSeasonResult(IList<QuizLeaderboard> results)
    {
        try
        {
            var points = CalculatePoints(results);
            var position = await CalculatePosition(results, points);
            var earnedPoints = CalculatEarnedPoints(points);
            SetText(points, position, earnedPoints);
            MultiplayerGameManager.SavePointsToPlayer(earnedPoints);
        }
        catch (Exception e)
        {
            UnityLogger.GetLogger().Error(e, nameof(LeaderboardFinishedScript) + " GetNotSeenOldSeasonResult");
        }

    }

    private async Task<int> CalculatePosition(IList<QuizLeaderboard> results, int points)
    {
        var lastEntry = results.OrderByDescending(x => x.ID).FirstOrDefault();
        var lastSeason = await _apiClientLeaderboard.GetLeaderboardBySeason(lastEntry.SeasonID);
        SortedList list = new SortedList();
        foreach (var value in lastSeason)
        {
            list.Add(value.PlayerID, points);
        }

        var position = list.IndexOfKey(MultiplayerGameManager.getPlayerId());
        return position;
    }

    private int CalculatePoints(IList<QuizLeaderboard> results)
    {
        var points = 0;
        oldSeasonsOfPlayer = results;
        foreach (var result in results)
        {
            points += result.Points;
        }

        return points;
    }

    private int CalculatEarnedPoints(int points)
    {
        return points / 10;
    }

    private void SetText(int points, int position, int earnedPoints)
    {
        txtInfo.text = "Du hast " + points + " Punkte erzielt. " +
                       "Und damit den " + position + " Platz erreicht";
        txtPoints.text = "+ " + earnedPoints;
    }

    public async void NewSeason()
    {
        foreach (var season in oldSeasonsOfPlayer)
        {
           await _apiClientLeaderboard.InsertOrUpdateEntry(season.PlayerID, 0, season.SeasonID, true);
        }
        oldSeasonsOfPlayer = null;
        var leaderboardScript = GameObject.FindObjectOfType(typeof(LeaderboardScript)) as LeaderboardScript;
        leaderboardScript.InitLeaderboard();
    }

    void Update()
    {
        
    }
    
}