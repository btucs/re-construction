#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldMapData
{
    public List<AreaData> areaList;

    public int GetAreaState(string sceneName)
    {
        int areaStateInt = 0;

        foreach(AreaData _area in areaList)
        {
            if(_area.IsAreaOfScene(sceneName))
            {
                areaStateInt = _area.GetAreaState();
            }
        }
        return areaStateInt;
    }

    public bool IsAreaUnlocked(string unlockSceneName)
    {
        foreach(AreaData singleArea in areaList)
        {
            if(singleArea.areaInfo.sceneName == unlockSceneName)
                return singleArea.unlockedOnMap;

            foreach(SubAreaData subArea in singleArea.subAreas)
            {
                if(subArea.areaInfo.sceneName == unlockSceneName)
                    return subArea.unlockedOnMap;
            }
        }

        return false;
    }

    public bool UnlockArea(string unlockSceneName)
    {
    	foreach(AreaData singleArea in areaList)
    	{
    		if(singleArea.areaInfo.sceneName == unlockSceneName)
    		{
    			if(!singleArea.unlockedOnMap)
    			{
    				singleArea.unlockedOnMap = true;
    				return true;
    			} else {
    				return false;
    			}
    		}	
    		foreach(SubAreaData subArea in singleArea.subAreas)
    		{
    			if(subArea.areaInfo.sceneName == unlockSceneName)
    			{
    				if(subArea.unlockedOnMap == false)
    				{
	    				subArea.unlockedOnMap = true;
	    				return true;
    				}else{
    					return false;
    				} 
    			}
    		}
    	}

    	return false;
    }
    //public Dictionary<WorldMapPosition, tileState> worldStateDictionary; 

    //private WorldPosEqualityComparer posEquComp = new WorldPosEqualityComparer();

    public WorldMapData()
    {
    	areaList = new List<AreaData>();
    	//worldStateDictionary = new Dictionary<WorldMapPosition, tileState>(posEquComp);
    }

    public class WorldPosEqualityComparer : IEqualityComparer<WorldMapPosition>
	{
	    public bool Equals(WorldMapPosition posOne, WorldMapPosition posTwo)
	    {
	    	//Debug.Log("Is comparing: " + posOne + " to: " + posTwo);
	    	return posOne.Equals(posTwo);
	    	
	        /*if(posOne == posTwo && posOne.y == posTwo.y)
	            return true;
	        else
	            return false;*/
	    }

	    public int GetHashCode(WorldMapPosition _worldPos)
	    {
	        return _worldPos.GetHashCode();
	    }
	}

}
