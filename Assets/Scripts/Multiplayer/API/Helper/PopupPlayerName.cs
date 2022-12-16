#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Multiplayer.API.Helper
{
    
    public class PopupPlayerName : MonoBehaviour
    {
        public GameObject popUpBox;
        public Text playerNameText;
        public void Popup()
        {
            playerNameText.text = GameController.GetInstance().gameState.characterData.player.characterName;
            popUpBox.SetActive(true);
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(playerNameText.text))
            {
                MultiplayerGameManager.SavePlayerName(playerNameText.text);
                MultiplayerGameManager.SendCharacterDataToDb();
                popUpBox.SetActive(false);
            }
        }
    }    
}