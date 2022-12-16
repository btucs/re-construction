#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Multiplayer.API;
using Assets.Scripts.Multiplayer.API.Helper;
using Assets.Scripts.Multiplayer.API.Models;
using Assets.Scripts.Multiplayer.Logging;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScript : MonoBehaviour
{
    public GameObject ScrollView;
    public GameObject EntryObjectPrefab;
    public GameObject txtReset;
    public GameObject LeaderBoardCanvas;
    public GameObject OldSeasonCanvas;
    public GameObject NoEntries;
    private APIClientLeaderboard _apiLeaderboard;
    // Start is called before the first frame update
    async void Start()
    {
        _apiLeaderboard = new APIClientLeaderboard();
        await CheckSeasonSwitch();
    }

    private async System.Threading.Tasks.Task CheckSeasonSwitch()
    {
        try
        {
            var notseenResults =
                await _apiLeaderboard.GetLeaderboardByPlayerIdNotSeenSwitch(MultiplayerGameManager.getPlayerId());
            if (notseenResults?.Count() > 0)
            {
                OldSeasonCanvas.SetActive(true);
                LeaderBoardCanvas.SetActive(false);
                NoEntries.SetActive(false);
                var leaderboardFinishedScript =
                    GameObject.FindObjectOfType(typeof(LeaderboardFinishedScript)) as LeaderboardFinishedScript;
                leaderboardFinishedScript.GetNotSeenOldSeasonResult(notseenResults);
            }
            else
            {
                InitLeaderboard();
            }
        }
        catch (Exception e)
        {
           UnityLogger.GetLogger().Error(e, nameof(LeaderboardScript) + " CheckSeasonSwitch");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void InitLeaderboard()
    {
        OldSeasonCanvas.SetActive(false);
        ResetLeaderboard();
        await SetActiveCanvas();
    }

    private async System.Threading.Tasks.Task SetActiveCanvas()
    {
        var listOfEntries = await _apiLeaderboard.GetLeaderboardOfCurrentSeason();
        if (listOfEntries == null || listOfEntries.Count == 0)
        {
            NoEntries.SetActive(true);
        }
        else
        {
            LeaderBoardCanvas.SetActive(true);
            InitScrollViewWithData(listOfEntries);
        }
    }

    private async void ResetLeaderboard()
    {
        var resetValues = await _apiLeaderboard.GetResetDateTime();
        txtReset.GetComponent<Text>().text = "In "+ resetValues[0] + " Tagen, " + resetValues[1] +" Stunden startet eine neue Saison!";
    }

    async void InitScrollViewWithData(IList<QuizLeaderboard> listOfEntries)
    {
        if (ScrollView != null)
        {
            var orderedList = listOfEntries.ToList().OrderByDescending(x => x.Points);
            Transform[] Temp = ScrollView.GetComponentsInChildren<Transform>();
            foreach (Transform Child in Temp)
            {
                if (Child.name == "Content")
                {
                    Transform[] TempChild = Child.GetComponentsInChildren<Transform>();
                    int i = 1;
                    foreach (QuizLeaderboard entry in orderedList)
                    {
                        GameObject leaderboard = Instantiate(EntryObjectPrefab) as GameObject;
                        Text textPos = leaderboard.transform.Find("txtPos").gameObject
                            .GetComponent<UnityEngine.UI.Text>();
                        textPos.text = i.ToString() + ".";
                        Text textName = leaderboard.transform.Find("txtName").gameObject
                            .GetComponent<UnityEngine.UI.Text>();
                        var playerName = await new PlayerHelper().GetPlayerNameById(entry.PlayerID);
                        textName.text = entry.PlayerID+ " / " + playerName;
                        Text textPoints = leaderboard.transform.Find("txtPoints").gameObject
                            .GetComponent<UnityEngine.UI.Text>();
                        textPoints.text = entry.Points.ToString();
                        leaderboard.transform.SetParent(Child.transform, false);
                        i++;
                    }

                    break;
                }
            }
        }
    }
}
