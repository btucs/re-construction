#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class ProfileData
{
  public string playerName = "Anonymus"; // Start is called before the first frame update
  public string playerId;
  public InventoryData inventory = new InventoryData();
  public XorShift rng = new XorShift(Guid.NewGuid().GetHashCode());
  public AchievementData achievements = new AchievementData();
  public List<string> finishedQuests = new List<string>();
  public Dictionary<string, int> activeQuests = new Dictionary<string, int>();
  public string subscribedQuest;

  public bool IsQuestFinished(MainQuestSO quest) {
    bool contains = false;
    foreach(string singleQuest in finishedQuests) {
      if(singleQuest == quest.UID) {
        contains = true;
      }
    }
    return contains;
  }

  public bool IsQuestActive(MainQuestSO quest) {
    bool contains = false;

    foreach(string singleQuest in activeQuests.Keys) {
      if(singleQuest == quest.UID) {
        contains = true;
      }
    }
    return contains;
  }

  public bool IsQuestCompleted(MainQuestSO quest) {
    bool contains = false;

    foreach(string singleQuest in finishedQuests) {
      if(singleQuest == quest.UID) {
        contains = true;
      }
    }
    return contains;
  }

  public int GetStepOfActiveQuest(MainQuestSO quest) {
    int stepIndex = -1;
    foreach(string singleQuest in activeQuests.Keys) {
      if(singleQuest == quest.UID) {
        stepIndex = activeQuests[singleQuest];
      }
    }
    return stepIndex;
  }

  public void AddActiveQuest(MainQuestSO quest) {
    bool contains = false;
    foreach(string singleQuest in activeQuests.Keys) {
      if(singleQuest == quest.UID) {
        contains = true;
      }
    }
    if(!contains && quest != null) {
      activeQuests.Add(quest.UID, 0);
    }
  }

  public void OnQuestFinished(MainQuestSO finishedQuest) {
    RemoveActiveQuest(finishedQuest);
    AddFinishedQuest(finishedQuest);
    if(finishedQuest.followUpQuest != null) {
      AddActiveQuest(finishedQuest.followUpQuest);
    }
  }

  public bool RemoveActiveQuest(MainQuestSO questToRemove) {
    for(int i = activeQuests.Keys.Count - 1; i >= 0; i--) {
      if(activeQuests.ElementAt(i).Key == questToRemove.UID) {
        activeQuests.Remove(activeQuests.ElementAt(i).Key);
        return true;
      }
    }
    return false;
  }

  public bool RemoveFinishedQuest(MainQuestSO questToRemove) {
    for(int i = finishedQuests.Count - 1; i >= 0; i--) {
      if(finishedQuests.ElementAt(i) == questToRemove.UID) {
        finishedQuests.Remove(finishedQuests.ElementAt(i));
        return true;
      }
    }
    return false;
  }

  public void AddFinishedQuest(MainQuestSO questToAdd) {
    bool contains = false;
    foreach(string singleQuest in finishedQuests) {
      if(singleQuest == questToAdd.UID) {
        contains = true;
      }
    }

    if(!contains) {
      finishedQuests.Add(questToAdd.UID);
    }
  }

  public string generatePlayerId()
  {
    const int length = 5;
    const string base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    var sb = new StringBuilder(length);
    for(int i = 0; i < length; i++)
    {
      sb.Append(base62Chars[rng.Range(0, base62Chars.Length - 1)]);
    }

    return sb.ToString();
  }
}

[Serializable]
public class AchievementData
{
  public int dialogueNodes = 0;
  public int glossaryEntries = 0;
  public int forcesDrawn = 0;
  public int calculationsMade = 0;
  public int informationsScanned = 0;
}