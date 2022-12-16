#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskStateSpriteSwapper : MonoBehaviour
{
    public Image stateIconImage;
    public Sprite successSprite;
    public Sprite failureSprite;
    public Color successColor = new Color();
    public Color failureColor = new Color();

    public void UpdateIcon(int maxPoints, int currentPoints)
    {
    	if(currentPoints >= maxPoints)
    	{
    		stateIconImage.sprite = successSprite;
    		stateIconImage.color = successColor;
    	} else {
    		stateIconImage.sprite = failureSprite;
    		stateIconImage.color = failureColor;
    	} 
    }
}
