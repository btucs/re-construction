#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
[CreateAssetMenu(menuName = "Topic")]
public class TopicSO : ScriptableObject
{
  public new string name = "Freischneiden";
  public string internalName = "";
  public List<MLEDataSO> mles = new List<MLEDataSO>();
  public List<TopicSO> prerequisites = new List<TopicSO>();
  public string descriptionText = "Wird angezeigt, wenn der Inhalt in der Liste ausgewählt wurde.";
  public List<GlossaryEntrySO> glossaryLinks = new List<GlossaryEntrySO>();
  public bool hasConnectedTasks = true;
  public Sprite icon;

  public bool IsDisplayed()
  {
    foreach (TopicSO prerequisite in prerequisites)
    {
      if (prerequisite.IsUnlocked() == false)
        return false;
    }
    return true;
  }

  public bool IsUnlocked()
  {
    foreach (TopicSO prerequisite in prerequisites)
    {
      if (prerequisite.GetProgressValue() < 1f)
        return false;
    }
    return true;
  }

  public string GetRequirementString()
  {
    int prereqCount = 0;
    string reqCollection = "";
    foreach (TopicSO prerequisite in prerequisites)
    {
      if (prerequisite.GetProgressValue() < 1f)
      {
        reqCollection = (prereqCount > 0) ? reqCollection + " und " + prerequisite.name : reqCollection + prerequisite.name;
        prereqCount++;
      }
    }
    reqCollection = (prereqCount > 1) ? "die Inhalte " + reqCollection : "den Inhalt " + reqCollection;
    string finalstring = "Schließe " + reqCollection + " ab, um diesen Inhalt freizuschalten.";
    return finalstring;
  }

  public float GetProgressValue()
  {
    float val = GameController.GetInstance().gameState.taskHistoryData.GetTopicProgressIndex(this);
    return val;
  }

}
