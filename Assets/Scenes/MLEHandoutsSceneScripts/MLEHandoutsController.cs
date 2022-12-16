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

public class MLEHandoutsController : MonoBehaviour
{
  public static MLEHandoutsController instance;

  [Required]
  public GameObject accordionWithPicturePrefab;

  [Required]
  public GameObject accordionWithoutPicturePrefab;

  [Required]
  public GameObject buttonHandoutPrefab;

  [Required]
  public Transform mleHandoutContainer;

  [Required]
  public Transform buttonHandoutContainer;

  [Required]
  public GameObject PanelEntries;

  [Required]
  public GameObject PanelEntryDetails;

  [Required]
  public GameObject historyBackButton;

  [Required]
  public GameObject startQuizButton; 

  public MLEController mleController;
  public TextMeshProUGUI mleNameText;

  [AssetList(AutoPopulate = true, Path = "/DataObjects/Handouts")]
  public List<MLEHandoutsSO> entries;

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
  }

  private void Start()
  {
    //CreateEntries();
  }

  // ----- display all handouts from DataObjects/Handouts -----
  private void CreateEntries()
  {
    foreach (MLEHandoutsSO entry in entries)
    {
      mleNameText.text = "MLE Handouts";

      GameObject buttonObject = Instantiate(buttonHandoutPrefab, buttonHandoutContainer.transform);
      HandoutDisplayAll buttonScript = buttonObject.GetComponent<HandoutDisplayAll>();
      buttonScript.handout = entry;
      buttonScript.mleHandoutManager = this;
      buttonScript.mleName.text = entry.mleName;
    }
  }
  // ----- ----- -----

  // ----- display the data of a handout from DataObjects/Handouts -----
  public void CreateEntryDetails(MLEHandoutsSO handout)
  {
    PanelEntries.SetActive(false);
    PanelEntryDetails.SetActive(true);
    //historyBackButton.SetActive(true);

    mleNameText.text = handout.mleName;

    foreach (MLEHandoutsSO.MLEHandoutEntry detail in handout.entries)
    {
      if (detail.picture)
      {
        GameObject accordionObject = Instantiate(accordionWithPicturePrefab, mleHandoutContainer.transform);
        HandoutDisplay accordionScript = accordionObject.GetComponent<HandoutDisplay>();
        accordionScript.handout = detail;
        accordionScript.existsPicture = true;
      }
      else
      {
        GameObject accordionObject = Instantiate(accordionWithoutPicturePrefab, mleHandoutContainer.transform);
        HandoutDisplay accordionScript = accordionObject.GetComponent<HandoutDisplay>();
        accordionScript.handout = detail;
      }
    }

    GameObject quizButtonObj = Instantiate(startQuizButton, mleHandoutContainer.transform);
    Button quizButtonRef = quizButtonObj.GetComponentInChildren<Button>();
    quizButtonRef.onClick.AddListener(() => {
        mleController.DisplayNextQuestionHandout();
      });
  }
  // ----- ----- -----

  // ----- back from the data of one handout to all handouts -----
  public void HistoryBack()
  {
    var clones = GameObject.FindGameObjectsWithTag("AccordionClone");
    
    foreach (var clone in clones)
    {
      Destroy(clone);
    }

    PanelEntries.SetActive(true);
    PanelEntryDetails.SetActive(false);
    historyBackButton.SetActive(false);
    mleNameText.text = "MLE Handouts";
  }
  // ----- ----- -----
}