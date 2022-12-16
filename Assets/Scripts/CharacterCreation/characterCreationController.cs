#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.UI;

public class characterCreationController : MonoBehaviour
{
  public SpriteLibrary mySpriteLibrary;
  SpriteLibraryAsset mySpriteLibraryAsset;
  public Text headerText;
  //public GameObject categoryButtonPrefab;
  public GameObject categoryList;
  public GameObject containerOfLabelButtons;
  public List<CharacterCreationCategoryToggle> categoryButtonList = new List<CharacterCreationCategoryToggle>();
  Dictionary<string, List<string>> categoryDictionary;
  public StudioEventEmitter fmodEmitter;
  private string defaultHeaderText;

  void Awake() {
    categoryDictionary = new Dictionary<string, List<string>>();
    mySpriteLibraryAsset = mySpriteLibrary.spriteLibraryAsset;
    //createCharacterCreationButtons();
    SendDataToCategoryButtons();

    defaultHeaderText = headerText.text;
  }


  public string[] GetCategoryNames(CosmeticCategory category)
  {
    string[] returnNames = new string[0];

    switch(category)
    {
      case CosmeticCategory.torso :
        returnNames = new string[] {"UpperBody_Torso#", "UpperBody_LowerFrontArm", "UpperBody_UpperFrontArm", "UpperBody_LowerBackArm", "UpperBody_UpperBackArm"};
        break;
      case CosmeticCategory.legs :
        returnNames = new string[] {"LowerBody_UpperLegBack", "LowerBody_LowerLegBack", "LowerBody_LowerLegFront", "LowerBody_UpperLegFront#"};
        break;
      case CosmeticCategory.foot : 
        returnNames = new string[] {"Foot_Front#", "Foot_Back"};
        break;
      case CosmeticCategory.hair : 
        returnNames = new string[] {"Hair_Front#", "Hair_Back"};
        break;
      case CosmeticCategory.eyes : 
        returnNames = new string[] {"Eyes"};
        break;
      case CosmeticCategory.mouth : 
        returnNames = new string[] {"Mouth"};
        break;
      case CosmeticCategory.nose : 
        returnNames = new string[] {"Nose"};
        break;
      case CosmeticCategory.decoration : 
        returnNames = new string[] {"Decoration"};
        break;
    }

    return returnNames;
  }

  private List<CosmeticItem> GetDefaultCosmetics(CosmeticCategory _category)
  {
    return GameController.GetInstance().gameAssets.GetCosmeticsOfCategory(_category, true);
  } 

  public List<CosmeticItem> GetAllSelectedItems()
  {
    List<CosmeticItem> playerItems = new List<CosmeticItem>();
    foreach(CharacterCreationCategoryToggle categoryButton in categoryButtonList)
    {
      CosmeticItem additem = categoryButton.GetSelected();
      if(additem != null)
        playerItems.Add(additem);
    }

    return playerItems;
  }

  public void UpdateHeaderToFirstStep()
  {
    headerText.text = defaultHeaderText;
  }

  public void UpdateHeaderToSecondStep()
  {
    headerText.text = "Erstelle deine Spielfigur: Anpassungen vornehmen";
  }
  
  /*private void createCharacterCreationButtons() {
    GameObject categoryButtonObj;
    CharacterCreationCategoryButton categoryButtonScript;

    IEnumerable<string> categoryNames = mySpriteLibraryAsset.GetCategoryNames();

    SummUpCategories(categoryNames);

    foreach(KeyValuePair<string, List<string>> categoryEntry in categoryDictionary) {
      string categoryName = categoryEntry.Key;
      categoryButtonObj = Instantiate(categoryButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, categoryList.transform);
      categoryButtonScript = categoryButtonObj.GetComponent<CharacterCreationCategoryButton>();

      if(categoryEntry.Key == categoryEntry.Value[0]) {
        categoryButtonScript.SetButtonContent(categoryName, mySpriteLibrary, containerOfLabelButtons);
      } else {
        Debug.Log("created a button with List for: " + categoryName);
        categoryButtonScript.SetButtonContentWithList(categoryName, categoryEntry.Value, mySpriteLibrary, containerOfLabelButtons);
      }

      Button categoryButton = categoryButtonObj.GetComponent<Button>();
      Debug.Log(categoryButton);
      categoryButton.onClick.AddListener(() => fmodEmitter.Play());
    }

    foreach (string categoryName in categoryNames)
    {
      categoryButtonObj = Instantiate(categoryButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, categoryList.transform);
      categoryButtonScript = categoryButtonObj.GetComponent<CharacterCreationCategoryButton>();

      string[] categoryNames = GetCategoryNames(categoryButtonScript);

      if(categoryNames.Length == 1) {
        categoryButtonScript.SetButtonContent(categoryNames[0], mySpriteLibrary, containerOfLabelButtons);
      } else if (categoryNames.Length > 1)
        categoryButtonScript.SetButtonContentWithList(categoryNames, categoryEntry.Value, mySpriteLibrary, containerOfLabelButtons);
      }
      categoryButtonScript.SetButtonContent(categoryName, mySpriteLibrary, containerOfLabelButtons);
    }
  }*/

  private void SendDataToCategoryButtons() {
    //GameObject categoryButtonObj;

    /*IEnumerable<string> categoryNames = mySpriteLibraryAsset.GetCategoryNames();
    SummUpCategories(categoryNames);

    foreach(KeyValuePair<string, List<string>> categoryEntry in categoryDictionary) {
      CharacterCreationCategoryToggle categoryButtonScript = null;
      string categoryName = categoryEntry.Key;
      //categoryButtonObj = Instantiate(categoryButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, categoryList.transform);
      //categoryButtonScript = categoryButtonObj.GetComponent<CharacterCreationCategoryButton>();

      for(int i = 0; i < categoryButtonList.Count; i++) {
        if(categoryButtonList[i].categoryName == categoryName) {
          categoryButtonScript = categoryButtonList[i];
        }
      }

      if(categoryEntry.Key == categoryEntry.Value[0] && categoryButtonScript != null) {
        categoryButtonScript.SetButtonContent(categoryName, mySpriteLibrary, containerOfLabelButtons);
      } else if(categoryButtonScript != null) {
        Debug.Log("created a button with List for: " + categoryName);
        categoryButtonScript.SetButtonContentWithList(categoryName, categoryEntry.Value, mySpriteLibrary, containerOfLabelButtons);
      }
    }
*/
    foreach (CharacterCreationCategoryToggle categoryButton in categoryButtonList)
    {
      string[] categoryNames = GetCategoryNames(categoryButton.category);

      categoryButton.SetButtonContent(categoryNames, mySpriteLibrary, containerOfLabelButtons);
    }

  }


  void SummUpCategories(IEnumerable<string> _stringEnum) {
    string subCategoryString;
    string mainCategoryString;

    foreach(string singleString in _stringEnum) {
      string[] stringSplitElements = singleString.Split('_');
      if(stringSplitElements.Length >= 2) {
        subCategoryString = stringSplitElements[1];
        mainCategoryString = stringSplitElements[0];

        if(categoryDictionary.ContainsKey(mainCategoryString)) {
          categoryDictionary[mainCategoryString].Add(subCategoryString);
        } else {
          categoryDictionary.Add(mainCategoryString, new List<string> { subCategoryString });
        }
      } else if(!singleString.Contains('_')) {
        categoryDictionary.Add(singleString, new List<string> { singleString });
      }
    }
  }



}
