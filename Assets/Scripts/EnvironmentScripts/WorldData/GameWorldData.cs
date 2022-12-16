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

[Serializable]
public class GameWorldData
{
    public List<AreaData> areas = new List<AreaData>();
    public List<TweetDataInstance> tweets = new List<TweetDataInstance>();

    
}

/*[Serializable]
public class AreaData
{
	public int unlockedLeft = 0;
	public int unlockedRight = 0;
	public List<ObjectData> objects = new List<ObjectData>();

	AreaData(int leftUnlock, int rightUnlock)
	{
		unlockedLeft = leftUnlock;
		unlockedRight = rightUnlock;
	}
}*/

[Serializable]
public class ObjectData
{
	//to which 
}

public class ObjectElementData
{
	
}