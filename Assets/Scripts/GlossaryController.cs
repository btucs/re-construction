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
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Sirenix.OdinInspector;
using TMPro;
using Cyotek.Collections.Generic;

public class GlossaryController : MonoBehaviour
{

  public static GlossaryController instance;

  [Required]
  public Text entryHeadlineRef;
  [Required]
  public Dropdown LanguageDropdown;

  [Required]
  public GameObject SingleEntryCanvas;
  [Required]
  public ScrollRect SingleEntryScrollRect;
  [Required]
  public Transform SingleEntryContainer;
  public GameObject singleEntryHeadlineContent;

  [Required]
  public GameObject glossaryEntryPrefab;
  [Required]
  public GameObject ListCanvas;
  [Required]
  public Transform glossaryEntryContainer;
  public GameObject listHeadlineContent;

  [Required]
  public GlossaryEntryController entryController;
  [Required]
  public TMP_LinkHandler linkHandler;

  [Required]
  public InputField SearchBar;
  [Required]
  public TMP_Text HistoryElement;
  [Required]
  public Button historyBackButton;
  public int historySize = 5;
  public bool disableListView = false;

  [Required]
  public GlossaryAnalyticsHandler glossaryAnalyticsHandler;

  private GlossaryEntrySO currentEntryData;
  private CircularBuffer<string> history = new CircularBuffer<string>(0);
  private TMP_LinkHandler historyLinkHandler;

  [AssetList(AutoPopulate = true, Path = "/DataObjects/Glossary")]
  public List<GlossaryEntrySO> entries;
  [OnValueChanged("UpdateGlossaryTypeOnEntry")]
  public GlossaryType type = GlossaryType.Medium;

  private Dictionary<string, GameObject> entryInstances = new Dictionary<string, GameObject>();

  private void Awake() {

    if(instance == null) {

      instance = this;
    }
  }

  private void Start() {

    history = new CircularBuffer<string>(historySize);
    LanguageDropdown.SetValueWithoutNotify(1);

    entries.Sort((GlossaryEntrySO a, GlossaryEntrySO b) => a.headline.CompareTo(b.headline));
    CreateEntries();

    if(SearchBar != null) {

      SearchBar.onValueChanged.AddListener(PerformSearch);
    }

    linkHandler.RegisterPrefix("glossary", HandleLinkClick);

    historyLinkHandler = HistoryElement.GetComponent<TMP_LinkHandler>();
    historyLinkHandler.RegisterPrefix("list", HandleListClick);
    historyLinkHandler.RegisterPrefix("glossary", HandleHistoryClick);

    if(SingleEntryScrollRect != null) {

      linkHandler.ForwardedOnBeginDrag.AddListener(SingleEntryScrollRect.OnBeginDrag);
      linkHandler.ForwardedOnDrag.AddListener(SingleEntryScrollRect.OnDrag);
      linkHandler.ForwardedOnEndDrag.AddListener(SingleEntryScrollRect.OnEndDrag);
    }
  }

  private void OnDisable() {

    history = new CircularBuffer<string>(historySize);
  }

  public void ShowSingleEntry(string entry) {

    GlossaryEntrySO found = FindEntryByName(entry);
    if(found != null) {

      ShowSingleEntry(found);
    }
  }

  public void ShowSingleEntry(GlossaryEntrySO _currentGlossaryData) {
    ListCanvas.SetActive(false);
    currentEntryData = _currentGlossaryData;

    entryController.SetContent(_currentGlossaryData, type);

    SingleEntryCanvas.SetActive(true);
    SingleEntryScrollRect.verticalNormalizedPosition = 1;
    RenderHistory();

    bool historyEmpty = history.IsEmpty && disableListView == true;
    historyBackButton.interactable = !historyEmpty;
    Image[] buttonImages = historyBackButton.GetComponentsInChildren<Image>();
    buttonImages[1].color = (historyEmpty == true ? buttonImages[0].color * historyBackButton.colors.disabledColor : buttonImages[0].color);

    listHeadlineContent.SetActive(false);
    singleEntryHeadlineContent.SetActive(true);

    GameController.GetInstance().gameState.profileData.achievements.glossaryEntries++;

    glossaryAnalyticsHandler.StartEvent(_currentGlossaryData);
  }

  public void SetType(string type) {

    Enum.TryParse(type, out GlossaryType result);
    SetType(result);
  }

  public void SetType(GlossaryType type) {

    this.type = type;
  }

  private void CreateEntries() {

    foreach(GlossaryEntrySO entry in entries) {

      GameObject buttonObject = Instantiate(glossaryEntryPrefab, glossaryEntryContainer.transform);
      GlossaryEntryButton buttonScript = buttonObject.GetComponent<GlossaryEntryButton>();
      buttonScript.glossaryData = entry;
      buttonScript.glossaryManager = this;
      buttonScript.nameDisplay.text = entry.headline;

      entryInstances.Add(entry.headline.ToLower(), buttonObject);
    }
  }

  public void UpdateLanguageSetting() {
    if(LanguageDropdown.value == 0 && type != GlossaryType.Easy) {

      SetType(GlossaryType.Easy);
      entryController.type = GlossaryType.Easy;
    } else if(LanguageDropdown.value == 1 && type != GlossaryType.Medium) {
      SetType(GlossaryType.Medium);
      entryController.type = GlossaryType.Medium;
    } else if(LanguageDropdown.value == 2 && type != GlossaryType.Scientific) {
      SetType(GlossaryType.Scientific);
      entryController.type = GlossaryType.Scientific;
    }

    entryController.UpdateEntryContent();
    SingleEntryScrollRect.verticalNormalizedPosition = 1;
  }

  public void ReturnToList() {

    SingleEntryCanvas.SetActive(false);
    ListCanvas.SetActive(true);
    listHeadlineContent.SetActive(true);
    singleEntryHeadlineContent.SetActive(false);
  }

  public void HistoryBack() {

    if(history.IsEmpty == false) {

      string item = history.GetLast();
      ShowSingleEntry(item);
    } else {

      if(disableListView == false) {

        ReturnToList();
      }
    }
  }

  private void UpdateGlossaryTypeOnEntry() {

    entryController.type = type;
    entryController.UpdateEntryContent();
  }

  public void PerformSearch(string searchText) {

    foreach(string key in entryInstances.Keys) {

      entryInstances.TryGetValue(key, out GameObject instance);
      bool isActive = searchText == String.Empty || key.Contains(searchText.ToLower());
      instance.SetActive(isActive);
    }
  }

  private GlossaryEntrySO FindEntryByName(string name) {

    return entries.Aggregate(null, (GlossaryEntrySO agg, GlossaryEntrySO entry) => {

      if(agg != null) {

        return agg;
      }

      if(entry.headline == name) {

        return entry;
      }

      return null;
    });
  }

  private void HandleLinkClick(PointerEventData data, string linkId, PointerEventType eventType) {

    if(eventType == PointerEventType.Click) {

      string entryName = linkId.Split(':')[1].Replace('_', ' ');
      GlossaryEntrySO entry = FindEntryByName(entryName);
      if(entry != null) {

        history.Put(currentEntryData.name);
        ShowSingleEntry(entry);
      }
    }
  }

  private void HandleHistoryClick(PointerEventData data, string linkId, PointerEventType eventType) {

    if(eventType == PointerEventType.Click) {

      string entryName = linkId.Split(':')[1].Replace('_', ' ');
      GlossaryEntrySO entry = FindEntryByName(entryName);
      if(entry != null) {

        JumpToHistoryEntry(entryName);
        ShowSingleEntry(entry);
      }
    }
  }

  private void HandleListClick(PointerEventData data, string linkId, PointerEventType eventType) {

    if(eventType == PointerEventType.Click) {

      history = new CircularBuffer<string>(historySize);
      ReturnToList();
    }
  }

  private void RenderHistory() {

    IEnumerable<string> historyEnumerable = history.Select((item) => "<u><link=\"glossary:" + item + "\">" + item + "</link></u>");

    if(disableListView == false) {

      historyEnumerable = historyEnumerable.Prepend("<u><link=\"list\">Übersicht</link></u>");
    }

    HistoryElement.text = String.Join(" - ", historyEnumerable);
  }

  private void JumpToHistoryEntry(string entryName) {

    string[] historyArr = history.ToArray();
    int index = Array.IndexOf(historyArr, entryName);
    string[] newHistory = history.Get(index);

    history = new CircularBuffer<string>(historySize);
    foreach(string item in newHistory) {

      history.Put(item);
    }
  }
}
