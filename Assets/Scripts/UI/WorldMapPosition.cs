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
public class WorldMapPosition
{
	public int x;
   	public int y;

    public WorldMapPosition(int _xPos, int _yPos)
    {
    	x = _xPos;
    	y = _yPos;
    }

    public override string ToString()
    {
    	return x + ":" + y;
    }

	public override bool Equals(System.Object posTwo)
	{
	    return this.ToString().Equals(posTwo.ToString());
	    /*//Debug.Log("Is comparing: " + posOne + " to: " + posTwo);
	    if(posOne == posTwo && posOne.y == posTwo.y)
	        return true;
	    else
	        return false;*/
	}

	public override int GetHashCode()
	{
	    int hCode = x ^ y;
	    return hCode.GetHashCode();
	}
}
