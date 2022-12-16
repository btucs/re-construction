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

public class SaveTextInputAsName : MonoBehaviour {
	
	public PlayerDataSO playerDataObj;
	public Button continueButton;

  // Start is called before the first frame update
  void Start() {

    if (continueButton != null) {

      continueButton.interactable = false;
    }
  }

  public void savePlayerName(string value) {

    if (playerDataObj != null)	{
    		
    	playerDataObj.playerName = value;
    	if(continueButton != null && playerDataObj.playerName.Length > 0)	{

        continueButton.interactable = true;
      } else if(continueButton != null && playerDataObj.playerName.Length == 0)	{
        continueButton.interactable = false;
      }
    }
  }

  public void SaveNameToProfileData()
  {
    GameController controller = GameController.GetInstance();
    controller.gameState.profileData.playerName = playerDataObj.playerName;
    controller.SaveGame();  
  }
}
