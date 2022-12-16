#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FullSerializer;

[System.Serializable]
public class AreaData
{
  public AreaDataSO areaInfo;
  [HideInInspector]
  public List<WorldMapTile> impactTiles = new List<WorldMapTile>();
  public List<SubAreaData> subAreas = new List<SubAreaData>();
  public bool unlockedOnMap = true;

  public int GetAreaState()
  {
  	int areaStateInt = 0;
  	foreach(WorldMapTile impactTile in impactTiles)
  	{
  		if(impactTile.tileStatus == tileState.bad)
  		{
  			areaStateInt -= 1;
  		} else if (impactTile.tileStatus == tileState.good)
  		{
  			areaStateInt += 1;
  		}
  	}

  	return areaStateInt;
  }

  public bool IsAreaOfScene(string _sceneName)
  {
  	if(areaInfo.sceneName == _sceneName)
  		return true;

  	foreach(SubAreaData subarea in subAreas)
  	{
  		if(subarea.areaInfo.sceneName == _sceneName)
  			return true;
  	}
  	return false;
  }
}

[System.Serializable]
public class SubAreaData
{
	public AreaDataSO areaInfo;
	public bool unlockedOnMap = false;
}
