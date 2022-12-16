#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MLEListController : MonoBehaviour
{
  [AssetList(AutoPopulate = true, Path = "/DataObjects/MLE")]
  public List<MLEDataSO> mleData;
  public List<TopicSO> topics;
  private List<MLEDataSO> displayedMLEs;

  public MLESceneLoader mleLoaderRef;
  public GameObject MLEPreviewPrefab;
  public Transform MLEContainer;

  void Start() {
    CreateEntries();
  }

  private void CreateEntries() {

    displayedMLEs = CreateMLEDisplayList();

    foreach(MLEDataSO singleMLE in displayedMLEs) {

      GameController saveGameController = GameController.GetInstance();
      List<FinishedMLEData> solvedMLEData = saveGameController.gameState.taskHistoryData.mleHistory;

      GameObject buttonObject = Instantiate(MLEPreviewPrefab, MLEContainer);
      mlePreviewButton buttonScript = buttonObject.GetComponent<mlePreviewButton>();
      buttonScript.relatedData = singleMLE;
      buttonScript.mleLoadingScript = mleLoaderRef;
      //buttonScript.glossaryManager = this;
      buttonScript.SetContent(solvedMLEData);

    }
  }

  private List<MLEDataSO> CreateMLEDisplayList()
  {
    List<MLEDataSO> MLEdisplayList = new List<MLEDataSO>();

    foreach(TopicSO topic in topics)
    {
      if(topic.IsUnlocked())
      {
        foreach(MLEDataSO topicMLE in topic.mles)
        {
          if(MLEdisplayList.Contains(topicMLE) == false)
            MLEdisplayList.Add(topicMLE);
        }
      }
    }

    return MLEdisplayList;
  }

}
