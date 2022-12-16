#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChoiceSaver : MonoBehaviour
{
	public characterGraphicsUpdater playerPrefContainer;
  public characterCreationController ccController;
  private Dictionary<string, string> defaultCharacterSettings = new Dictionary<string, string>();
  private List<CosmeticItem> selectedItems = new List<CosmeticItem>();

  private void OnEnable()
  {
    foreach(KeyValuePair<string, string> pair in playerPrefContainer.characterSOData.Value.spriteRenderDictionary) {
      KeyValuePair<string, string> defaultData = new KeyValuePair<string, string>(pair.Key, pair.Value);
      defaultCharacterSettings.Add(defaultData.Key, defaultData.Value);
    }
    Debug.Log("Saved default settings");
  }


  public bool ChoiceHasChanged()
  {
    CharacterSO configuredCharacter = playerPrefContainer.characterSOData.Value;
    foreach(string category in defaultCharacterSettings.Keys)
    {
      Debug.Log("Comparing " + configuredCharacter.spriteRenderDictionary[category] + " with " + defaultCharacterSettings[category]);
      if(configuredCharacter.spriteRenderDictionary[category] != defaultCharacterSettings[category])
        return true;
    }
    return false;
  }


  public void SaveCustomPlayerPreferences()
  {

    CharacterSO character = CreateCharacterSO();
    GameController controller = GameController.GetInstance();
    
    selectedItems = ccController.GetAllSelectedItems();
    InventoryData playerInventory = controller.gameState.profileData.inventory;
    playerInventory.AddItems(selectedItems);
    controller.gameState.profileData.inventory = playerInventory;

    controller.gameState.characterData.player = character;
    controller.SaveGame();
  }

  public void SaveMentorPreferences()
  {
    GameController controller = GameController.GetInstance();
    controller.gameState.characterData.mentor = playerPrefContainer.characterSOData.Value;
    controller.SaveGame();
  }

  private CharacterSO CreateCharacterSO() {

    CharacterSO configuredCharacter = playerPrefContainer.characterSOData.Value;
    CharacterSO character = ScriptableObject.CreateInstance<CharacterSO>();

    foreach(KeyValuePair<string, string> pair in configuredCharacter.spriteRenderDictionary) {

      character.spriteRenderDictionary.Add(pair.Key, pair.Value);
    }
    character.bodyColor = configuredCharacter.bodyColor;
    character.hairColor = configuredCharacter.hairColor;

    return character;
  }
}
