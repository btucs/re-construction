#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SceneResolver : MonoBehaviour
{
  public characterGraphicsUpdater playerCharacterObj;
  public characterGraphicsUpdater mentorCharacterObj;

  [AssetsOnly]
  public GameObject sceneBackgroundPrefab;

  private GameController saveDataController;

  void Start()
  {
    saveDataController = GameController.GetInstance();
    LoadGraphics(playerCharacterObj, CharacterType.Player);
    LoadGraphics(mentorCharacterObj, CharacterType.Mentor);

    GameController.GetInstance().gameState.konstruktorSceneData.backgroundPrefab = sceneBackgroundPrefab;
  }

  public void LoadGraphics(characterGraphicsUpdater characterObj, CharacterType type)
  {
  
    if(characterObj != null) {

      switch(type) {

        case CharacterType.Player:
          characterObj.characterSOData.Value = saveDataController.gameState.characterData.player;
          break;
        case CharacterType.Mentor:
          characterObj.characterSOData.Value = saveDataController.gameState.characterData.mentor;
          break;
      }

      /*if(characterObj.characterData != null || characterObj.characterSOData != null) {

    	  characterObj.UpdateAllGraphics();
      }*/
    }
  }

  /*
  private Character FindCharacter(CharacterType type) {

    foreach(Character character in saveDataController.gameState.characterData.characters) {

      if(character.type == type) {

        return character;
      }
    }

    return default(Character);
  }*/
}
