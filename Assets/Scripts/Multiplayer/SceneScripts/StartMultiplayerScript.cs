#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using Assets.Scripts.Multiplayer.API;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Assets.Scripts.Multiplayer.SceneScripts
{
  public class StartMultiplayerScript : MonoBehaviour
  {
    [Required]
    public Text loadingMessage;

    // Start is called before the first frame update
    async void Start()
    {

      string currentPlayerId = GameController.GetInstance().gameState.multiplayer.playerId;
      /* current player id can be in the wrong format as there was no check 
       * before to handle errors
       */
      if(
        !string.IsNullOrEmpty(currentPlayerId)
        // hashids minimal length is set to 5
        // general length should not get larger then 10, that would require more than 99,999,999,999,999 players,
        // but could happen if an error message slipped through
        && (currentPlayerId.Length < 5 || currentPlayerId.Length > 10)
      ) {
        currentPlayerId = null;
      }

      if (string.IsNullOrEmpty(currentPlayerId))
      {
        var playerId = await new APIClientPlayer().GetOrCreatePlayerId();
        // hashids minimal length is set to 5
        // general length should not get larger then 10, that would require more than 99,999,999,999,999 players,
        // but could happen if an error message slipped through
        if (playerId.Length >= 5 && playerId.Length < 10)
        {
          MultiplayerGameManager.SavePlayerIdToPlayer(playerId);
        }

        loadingMessage.text = "Es ist ein Fehler aufgetreten, bitte versuche es später noch einmal.";
        loadingMessage.color = new Color32(190, 102, 101, 255);
        return;
      }
      else
      {
        SceneManager.LoadScene("Overview");
      }
    }

    void Update()
    {

      string playerId = GameController.GetInstance().gameState.multiplayer.playerId;
      if (!string.IsNullOrEmpty(playerId) && playerId.Length >= 5 && playerId.Length < 10)
      {
        SceneManager.LoadScene("Overview");
      }
    }
  }
}
