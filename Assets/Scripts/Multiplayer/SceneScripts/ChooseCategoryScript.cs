#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using Assets.Scripts.Multiplayer.API;
using Assets.Scripts.Multiplayer.API.Models;
using Assets.Scripts.Multiplayer.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCategoryScript : MonoBehaviour
{
  public ChangeSceneScript changeScene;

  private APIClientCategoriesQuestions _clientCategoriesQuestions;
  private APIClientRound _apiClientRound;
  public GameObject parent;
  private const int MAX_CATEGORIES = 3;
  string notEnoughCategories = "Not enough categories";
  // Start is called before the first frame update
  void Start()
  {
    _clientCategoriesQuestions = new APIClientCategoriesQuestions();
    _apiClientRound = new APIClientRound();
    InitCategories();
  }

  // Update is called once per frame
  void Update()
  {

  }

  private async void InitCategories()
  {
    try
    {
      var categories = await _clientCategoriesQuestions.GetValidCategories();
      if (categories?.Count < MAX_CATEGORIES && categories.Any())
      {
        for (var i = 0; i < categories.Count; i++)
        {
          AddCategorieToView(i, categories);
        }
      }

      else if (categories?.Count >= MAX_CATEGORIES)
      {
        var randomCat = RandomCategories(categories);
        for (var i = 0; i < randomCat.Count; i++)
        {
          AddCategorieToView(i, randomCat);
        }
      }
      else
      {

        Debug.Log(notEnoughCategories);
        UnityLogger.GetLogger().Information(notEnoughCategories);
      }

    }
    catch (Exception e)
    {
      UnityLogger.GetLogger().Error(e, nameof(ChooseCategoryScript) + " InitCategories");
    }

  }

  private List<QuizCategory> RandomCategories(IList<QuizCategory> categories)
  {
    List<QuizCategory> randomCategories = new List<QuizCategory>();
    while (randomCategories.Count < MAX_CATEGORIES)
    {
      int cat = UnityEngine.Random.Range(0, categories.Count);
      QuizCategory randomCategory = categories[cat];
      if (!randomCategories.Contains(randomCategory))
      {
        randomCategories.Add(randomCategory);
      }
    }

    return randomCategories;
  }

  void AddCategorieToView(int i, IList<QuizCategory> categories)
  {
    var katItem = parent.transform.Find("Kat" + (i + 1));
    ObjectInfo infos = katItem.gameObject.GetComponent<ObjectInfo>();
    infos.Id = categories[i].ID;
    infos.Text = categories[i].Name;
    katItem.transform.Find("KatText").gameObject.GetComponent<Text>().text = categories[i].Name;
  }

  public async void CategoryClicked(GameObject clickedObject)
  {
    ObjectInfo infos = clickedObject.gameObject.GetComponent<ObjectInfo>();
    if (infos != null)
    {
      var currentRounds = await _apiClientRound.GetOpenRoundsByGameId(MultiplayerGameManager.GameId);
      var currentRound = currentRounds?.OrderByDescending(x => x.RoundNumber).First();
      if (currentRounds == null || currentRounds.Count == 0)
      {
        MultiplayerGameManager.SetCategoryId(infos.Id);
        MultiplayerGameManager.SetRoundNumber(1);
        currentRound = new QuizRound
        {
          ID = await _apiClientRound.InsertNewRound(MultiplayerGameManager.GameId, 1),
          QuizCatId = infos.Id,
          RoundNumber = 1
        };
      }
      else
      {
        currentRound.QuizCatId = infos.Id;
        MultiplayerGameManager.SetCategoryId(infos.Id);
        MultiplayerGameManager.SetRoundNumber(currentRound.RoundNumber);
        currentRound.QuestionIDs = "";
        await _apiClientRound.UpdateRound(currentRound);
      }
    }
    else
    {
      throw new Exception("ObjectInfo can not be found");
    }

    changeScene.ChangeScene("Question");
  }

}
