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
[RequireComponent(typeof(SpriteRenderer))]
public class ObjectSubElement : MonoBehaviour
{
    public string areaName;
    public SpriteRenderer imgContainer;
    public SubElementState activeState;
    public List<SubElementState> possibleStates = new List<SubElementState>();

    void Start()
    {
    	if(imgContainer == null)
    	{
    		imgContainer = GetComponent<SpriteRenderer>();
    	}
    }

    public void LoadSprite()
    {
    	GameController controller = GameController.GetInstance();
    	//controller.gameState.gameWorldData.areas[0].ObjectSubElement.state = playerPrefContainer.characterSOData.Value;
    	controller.SaveGame();
    }

}
