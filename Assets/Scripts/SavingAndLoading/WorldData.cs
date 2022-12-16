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

[Serializable]
public class WorldData
{
  [ValueDropdown("GetActiveAreas")]
  public string lastScene = "OfficeStartScene";
  public string activeScene = "none";
  public List<WorldAreaData> areas = new List<WorldAreaData>();
  public WorldMapData mapData = new WorldMapData();
  public List<TweetDataInstance> tweets = new List<TweetDataInstance>();
  public List<string> visitedDialogueNodes = new List<string>();
  public int finishedDialogues = 0;
  public List<string> sceneRegistration = new List<string>();


  public void MarkSceneVisited(string sceneName)
  {
    sceneRegistration.Add(sceneName);
  }

  public bool WasSceneVisited(string _sceneName)
  {
    foreach(string scenename in sceneRegistration)
    {
      if(scenename == _sceneName)
        return true;
    }
    return false;
  }

  public void SetPlayerPos(Vector3 pos) {

    WorldAreaData area = FindAreaData(lastScene);

    if(area != null) {

      NPCData playerData = area.FindNPC("Player");

      if(playerData != null) {

        Vector3 playerPos = playerData.worldPos;

        // only set the exposition and keep the original y position to avoid floating player
        playerData.worldPos = new Vector3(pos.x, playerPos.y);
      }
    }
  }

  public WorldAreaData FindAreaData(string areaName) {

    return areas.FirstOrDefault((WorldAreaData area) => area.sceneName == areaName);
  }

  private IEnumerable<string> GetActiveAreas() {

    return areas.Select((WorldAreaData areaData) => areaData.sceneName);
  }
}

[Serializable]
public class WorldAreaData
{
	public string sceneName;
	public int unlockedLeft;
	public int unlockedRight;
	public List<NPCData> NPCs = new List<NPCData>();
  public List<WorldObjectData> objects = new List<WorldObjectData>();

  public NPCData FindNPC(string name) {

    return NPCs.FirstOrDefault((NPCData npc) => npc.characterName == name);
  }

  public WorldObjectData GetSaveDataOfObject(GameWorldObject obj)
  {
    foreach(WorldObjectData objectSaveData in objects)
    {
      if(objectSaveData.Equals(obj))
      {
        Debug.Log("Save data found");
        return objectSaveData;
      }
    }
    return null;
  }

  public void UpdateOrAddObjectData(GameWorldObject obj)
  {
    bool dataFound = false;
    for(int i = 0; i < objects.Count; i++)
    {
        if(objects[i].Equals(obj))
        {
          dataFound = true;
          objects[i] = new WorldObjectData(obj);
        }
    }
    if(!dataFound)
    {
      WorldObjectData addObject = new WorldObjectData(obj);
      objects.Add(addObject);
    }
  }
}

[Serializable]
public class NPCData
{
	public string characterName;
	public Vector3 worldPos;
}

[Serializable]
public class WorldObjectData
{
  public string taskObjectUID;
  public bool isExamined;

  public WorldObjectData(GameWorldObject saveData)
  {
    taskObjectUID = saveData.objectData.UID;
    isExamined = saveData.GetExaminedState();
  }

  public bool Equals(GameWorldObject saveData)
  {
    bool isEqual = taskObjectUID == saveData.objectData.UID;
    return isEqual;
  }
}