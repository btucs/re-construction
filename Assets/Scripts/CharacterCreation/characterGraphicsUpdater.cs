#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using UnityEditor;
using UniRx;
using Sirenix.OdinInspector;

public class characterGraphicsUpdater : MonoBehaviour
{
	SpriteResolver spriteChangeScriptHolder;
	GameObject spriteGameObject;
	
  public CharacterSOReactiveProperty characterSOData;

  private Dictionary<string, string> characterGraphicsDict;
  private CharacterColorUpdater tintGraphicsController;

  private CharacterSO currentCharacterSO;

  private void Start() {

    tintGraphicsController = GetComponent<CharacterColorUpdater>();

    SpriteResolver[] allSpriteResolver = GetComponentsInChildren<SpriteResolver>();

    characterSOData
      .Where((CharacterSO character) => character != null)
      .Do((CharacterSO character) => {
        ConfigureResolvers(character, allSpriteResolver);
        InitializeGraphicsDictionary(allSpriteResolver, character);
        currentCharacterSO = character;
        if(tintGraphicsController != null) {

          tintGraphicsController.SetBodyColor(character);
          tintGraphicsController.SetHairColor(character);
        }
      })
      .Subscribe()
      .AddTo(this)
    ;

  }

  public void SaveDataToPlayerSO()
  {
    GameController.GetInstance().gameState.characterData.player = currentCharacterSO;
    GameController.GetInstance().SaveGame();
  }

  public void SetBodyColor(Color _bodyColor)
  {
    currentCharacterSO.bodyColor = _bodyColor;
    if(tintGraphicsController != null) 
    {
      tintGraphicsController.SetBodyColor(currentCharacterSO); 
    }
  }

  public void SetHairColor(Color _hairColor)
  {
    currentCharacterSO.hairColor = _hairColor;
    if(tintGraphicsController != null) 
    {
      tintGraphicsController.SetHairColor(currentCharacterSO); 
    }
  }

  public void InitializeGraphicsDictionary() {
    SpriteResolver[] allSpriteResolver = GetComponentsInChildren<SpriteResolver>();
    InitializeGraphicsDictionary(allSpriteResolver, currentCharacterSO);
  }

  public void InitializeGraphicsDictionary(SpriteResolver[] allSpriteResolver, CharacterSO character) {

    characterGraphicsDict = new Dictionary<string, string>();
	
		foreach (SpriteResolver spriteInfo in allSpriteResolver) 
		{
			characterGraphicsDict.Add(spriteInfo.GetCategory(), spriteInfo.GetLabel());
		}

    character.spriteRenderDictionary = characterGraphicsDict;
	}

	public void UpdateSingleData(string _categoryName, string _labelName)
	{
		if(currentCharacterSO.spriteRenderDictionary.ContainsKey(_categoryName))
		{
      currentCharacterSO.spriteRenderDictionary[_categoryName] = _labelName;
		} 
		else
		{
      currentCharacterSO.spriteRenderDictionary.Add(_categoryName, _labelName);
		}
	}

  public bool IsItemEquipped(string itemCategoryName, string itemLabelName)
  {
    //Debug.Log("Check if item of category " + itemCategoryName + " is " +  itemLabelName + " and it is " + currentCharacterSO.spriteRenderDictionary[itemCategoryName]);
    if(currentCharacterSO.spriteRenderDictionary.ContainsKey(itemCategoryName))
    {
      return (currentCharacterSO.spriteRenderDictionary[itemCategoryName].Contains(itemLabelName));
    }
    
    return false;
  }

  public void EquipItem(CosmeticItem itemToEquip)
  {
    string[] categoryStrings = itemToEquip.GetCategoryNames();
    for(int i = 0; i < categoryStrings.Length; i++)
    {

      IEnumerable <string> labelNamesOfObject = GetComponent<SpriteLibrary>().spriteLibraryAsset.GetCategoryLabelNames(categoryStrings[i]);
      //Check which complete label contains the shortened labelstring and return that one
      foreach(string singleLabelName in labelNamesOfObject)
      {
        if(singleLabelName.Contains(itemToEquip.label))
        {
          UpdateSingleData(categoryStrings[i], singleLabelName);
          UpdateImageOfCategory(categoryStrings[i], singleLabelName);
        }
      } 
    }
  }

	public void UpdateAllGraphics()
	{
		//OLD: characterGraphicsDict = characterData.spriteRenderDictionary;
		//I changed this because it gave an exception as of the dictionary was modified while we iterated over it
		Dictionary<string, string> tempDictionary = currentCharacterSO.spriteRenderDictionary;

		foreach(KeyValuePair<string,string> characterGraphic in tempDictionary)
		{
		    UpdateImageOfCategory(characterGraphic.Key, characterGraphic.Value);
		}
	}

	public void UpdateImageOfCategory(string _categoryName, string _labelName)
	{
		//for this to work, the name of the category has to be the exact same string as the gameobject holding the Sprite
		spriteGameObject = this.transform.Find(_categoryName).gameObject;
		if(spriteGameObject == null)
		{
			Debug.LogError("There is no gameObject with the name: " + _categoryName + ". Please make sure, the name of the sprite category equals the name of the corresponding gameObject.");
			return;
		}

		//set the desired sprite to the SpriteRenderer of this character
    	spriteChangeScriptHolder = spriteGameObject.transform.GetComponentInChildren<SpriteResolver>();
    	spriteChangeScriptHolder.SetCategoryAndLabel(_categoryName, _labelName);
    	spriteChangeScriptHolder.ResolveSpriteToSpriteRenderer();
	}

  private void ConfigureResolvers(CharacterSO characterSOData, SpriteResolver[] allSpriteResolver) {

    foreach(SpriteResolver spriteInfo in allSpriteResolver) {

      string category = spriteInfo.GetCategory();
      characterSOData.spriteRenderDictionary.TryGetValue(category, out string label);
     
      spriteInfo.SetCategoryAndLabel(category, label);
    }
  }

#if UNITY_EDITOR
  [Button("Save Preset back to CharacterSOData")]
  private void UpdateCharacterSOFromPreset() {

    currentCharacterSO = characterSOData.Value;

    
    currentCharacterSO.spriteRenderDictionary.Clear();
    SpriteResolver[] allSpriteResolver = GetComponentsInChildren<SpriteResolver>();
    InitializeGraphicsDictionary(allSpriteResolver, currentCharacterSO);
    EditorUtility.SetDirty(currentCharacterSO);
  }
#endif
}

