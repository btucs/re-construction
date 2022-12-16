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
using UnityEngine.UI;

public class ButtonToggleMenu : MonoBehaviour
{
  public List<SingleMenuButton> menuButtons = new List<SingleMenuButton>();
  public Vector3 scaleVector = new Vector3(1.1f, 1.1f, 1.1f);
  public Sprite activeBGsprite;
  public Sprite inactiveBGsprite;
  #pragma warning disable 0649
  [SerializeField] private Color acticeBGColor;
  [SerializeField] private Color inactiveBGColor;
  [SerializeField] private Color activeIconColor;
  [SerializeField] private Color inactiveIconColor;
  #pragma warning restore 0649

  public void ActivateButton(SingleMenuButton activeButton)
  {
    foreach(SingleMenuButton currentButton in menuButtons)
    {
    	if(currentButton == activeButton)
    	{
    		currentButton.isSelected = true;
        if(currentButton.backgroundImg != null)
           currentButton.backgroundImg.color = acticeBGColor;
        if(currentButton.iconImg != null)
          currentButton.iconImg.color = activeIconColor;
        if(activeBGsprite!=null)
          currentButton.backgroundImg.sprite = activeBGsprite;
        foreach(Text textElement in currentButton.textContent)
        {
          textElement.color = activeIconColor;
        }
    		currentButton.GetComponent<RectTransform>().localScale = scaleVector;
    	}
    	else
    	{
    		DeactivateSingleButton(currentButton);
    	}
    }
  }

  public void DeactivateAllButtons()
  {
    foreach(SingleMenuButton currentButton in menuButtons)
    {
      DeactivateSingleButton(currentButton);
    }
  }

  private void DeactivateSingleButton(SingleMenuButton deactButton)
  {
    deactButton.isSelected = false;
    
    if(deactButton.backgroundImg != null)
    {
      deactButton.backgroundImg.color = inactiveBGColor;
      if(inactiveBGsprite!=null)
        deactButton.backgroundImg.sprite = inactiveBGsprite;
    }

    if(deactButton.iconImg != null)
      deactButton.iconImg.color = inactiveIconColor;
            
    foreach(Text textElement in deactButton.textContent)
    {
      textElement.color = inactiveIconColor;
    }
    deactButton.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
  }
}
