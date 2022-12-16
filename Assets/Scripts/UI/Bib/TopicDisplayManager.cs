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
using UnityEngine.EventSystems;
using TMPro;

public class TopicDisplayManager : MonoBehaviour
{
  public Transform topicContainer;
  public GameObject topicEntryPrefab;
  public List<TopicSO> topics = new List<TopicSO>();

  public GameObject topicDetailWrapper;
  public TMP_Text moduleDescription;
  public GameObject glossaryLinkPreset;
  public GameObject mleEntryPreset;
  public Transform mleContainer;
  public MLESceneLoader mleLoadScript;
  private List<GameObject> glossaryLinkElements = new List<GameObject>();

  private GameController controller;
  private bool listInitialized = false;

  public GlossaryController glossary;

  private KonstructorModuleSO currentModuleSO;
  private TMP_LinkHandler linkHandler;

  public void Setup()
  {
    if (!listInitialized)
      InitializeTopicList();
  }

  public void OpenTopicDetails(TopicSO selectedTopic)
  {
    moduleDescription.text = selectedTopic.descriptionText;

    UpdateGlossaryLinks(selectedTopic);
    UpdateMLEEntries(selectedTopic);

    MenuUIController.Instance.breadcrumController.openSecondLayer(topicDetailWrapper, selectedTopic.name);
    //topicDetailWrapper.SetActive(true);
  }

  private void UpdateMLEEntries(TopicSO selectedTopic)
  {
    foreach (Transform child in mleContainer)
    {
      Destroy(child.gameObject);
    }

    foreach (MLEDataSO mle in selectedTopic.mles)
    {
      mlePreviewButton mleButtonScript = Instantiate(mleEntryPreset, mleContainer).GetComponent<mlePreviewButton>();
      mleButtonScript.relatedData = mle;
      mleButtonScript.mleLoadingScript = mleLoadScript;
      mleButtonScript.SetContent(controller.gameState.taskHistoryData.mleHistory);
    }
  }

  private void UpdateGlossaryLinks(TopicSO selectedTopic)
  {
    foreach (GameObject linkObj in glossaryLinkElements)
    {
      Destroy(linkObj);
    }
    glossaryLinkElements.Clear();

    foreach (GlossaryEntrySO glossaryEntry in selectedTopic.glossaryLinks)
    {
      GameObject newLinkObj = Instantiate(glossaryLinkPreset, glossaryLinkPreset.transform.parent);
      linkHandler = newLinkObj.GetComponent<TMP_LinkHandler>();
      if (glossary != null)
      {
        linkHandler.RegisterPrefix("glossary", HandleGlossaryLink);
      }

      string glossaryLink = "<u><link=\"glossary:" + glossaryEntry.name + "\">" + glossaryEntry.headline + "</link></u>";
      newLinkObj.GetComponent<TMP_Text>().text = glossaryLink;
      glossaryLinkElements.Add(newLinkObj);
      newLinkObj.SetActive(true);
    }
  }

  private void InitializeTopicList()
  {
    controller = GameController.GetInstance();
    //eine Liste aller Aufgabenarten abrufen

    foreach (Transform child in topicContainer)
    {
      Destroy(child.gameObject);
    }

    foreach (TopicSO topic in topics)
    {
      if (topic.IsDisplayed())
      {
        TopicEntry topicUIScript = Instantiate(topicEntryPrefab, topicContainer).GetComponent<TopicEntry>();
        topicUIScript.topicManager = this;
        if (topic.IsUnlocked())
        {
          topicUIScript.SetupAsUnlocked(topic);
        }
        else
        {
          topicUIScript.SetupAsPreview(topic);
        }
      }
    }

    listInitialized = true;
  }

  private void HandleGlossaryLink(PointerEventData data, string linkId, PointerEventType eventType)
  {

    Debug.Log("Opening glossary entry for: " + linkId);

    //if(eventType == PointerEventType.Click)
    //{
    string entryName = linkId.Split(':')[1].Replace('_', ' ');
    glossary.gameObject.SetActive(true);
    glossary.ShowSingleEntry(entryName);
    //}
  }

}
