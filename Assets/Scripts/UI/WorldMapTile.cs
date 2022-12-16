#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldMapTile
{
	public Vector2 mapPosition;
   	public tileState tileStatus = tileState.average; 

   	public WorldMapTile (Vector2Int _pos, tileState _status)
   	{
   		mapPosition = _pos;
   		tileStatus = _status;
   	}

    public void ChangeTileState()
    {
    	//return x + ":" + y;
    }

    public void ImproveTileCondition()
    {
    	if(tileStatus == tileState.average)
    	{
    		tileStatus = tileState.good;
    	}
    	else if(tileStatus == tileState.bad)
    	{
    		tileStatus = tileState.average;
    	}
    }

    public void AggrevateTileCondition()
    {
    	if(tileStatus == tileState.average)
    	{
    		tileStatus = tileState.bad;
    	}
    	else if(tileStatus == tileState.good)
    	{
    		tileStatus = tileState.average;
    	}
    }

    public int CanImproveBy()
    {
    	if(tileStatus==tileState.average)
    		return 1;

    	if(tileStatus==tileState.bad)
    		return 2;

    	return 0;
    }
}
