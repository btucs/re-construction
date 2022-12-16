#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Sirenix.OdinInspector;

public class MultistepController : MonoBehaviour {

  [Required]
  public DropDownList dropdownList;
  [Required]
  public UIPanelManager uiManager;

  public ItemDetection itemDetectionScript;
  public ButtonSelectionUIFeedback analyzeButtonScript;

  public DropDownListItem newStep = new DropDownListItem();
  public DropDownListItem defaultStep = new DropDownListItem();

  private List<DropDownListItem> steps = new List<DropDownListItem>();
  private Dictionary<DropDownListItem, StepControllerAbstract> controllers = new Dictionary<DropDownListItem, StepControllerAbstract>();

  private StepControllerAbstract activeController;

  private int selectedIndex = 0;

  void Start() {

    RefreshList();
    dropdownList.OnSelectionChanged.AddListener(HandleSelect);
  }

  public void RefreshList(bool resetSelectedindex = true) {

    List<object> newList = new List<object>();
    newList.Add(defaultStep);
    newList.AddRange(steps);
    newList.Add(newStep);

    dropdownList.RefreshItems(newList.ToArray());

    if(resetSelectedindex == true) {

      dropdownList.SelectItem(0);
    }
  }

  public bool HasAddedStep()
  {
    return (steps.Count >= 1);
  }

  public void AddController(StepControllerAbstract controller, KonstructorModuleSO module) {

    DropDownListItem step = new DropDownListItem() {
      Caption = "Modul " + (steps.Count + 1) + ": " + module.title,
    };

    steps.Add(step);
    controllers.Add(step, controller);
    ActivateController(controller);
    RefreshList(false);

    // default step is always the 0 then steps, last is newStep, therefore new index is count
    selectedIndex = steps.Count;
    dropdownList.SelectItem(selectedIndex);
  }

  public void RemoveController(DropDownListItem step) {

    steps.Remove(step);
    // only step gets removed. The controller stays for assessment
  }

  public void ActivateController(DropDownListItem step) {

    controllers.TryGetValue(step, out StepControllerAbstract controller);
    if(controller != null) {

      ActivateController(controller);
    }
  }

  public void ActivateController(StepControllerAbstract controller) {

    if(activeController != null) {

      activeController.gameObject.SetActive(false);
    }

    activeController = controller;
    controller.gameObject.SetActive(true);

    if(itemDetectionScript != null) {

      itemDetectionScript.analyzeButtonPanel.SetActive(false);
    }
  }

  public void ActivateDefaultStep() {

    HandleSelect(0);
    dropdownList.SelectItem(0);
  }

  private void HandleSelect(int index) {

    if(itemDetectionScript != null) {

      itemDetectionScript.Disable();
    }

    if(analyzeButtonScript != null) {

      analyzeButtonScript.SetSelected(false);
    }

    selectedIndex = index;
    DropDownListItem selectedItem = dropdownList.Items[index];
    if(selectedItem == newStep) {

      ShowModuleList();
      return;
    }

    if(selectedItem == defaultStep) {

      ShowAnalysisPanel();
      return;
    }

    ActivateController(selectedItem);
  }

  public void ShowModuleList() {

    uiManager.Show("Solution Canvas");
  }

  private void ShowAnalysisPanel() {

    Debug.Log("Show Analysis Panel");
    if(activeController != null) {

      activeController.gameObject.SetActive(false);
      activeController = null;
    }

    if(itemDetectionScript != null) {

      itemDetectionScript.ShowOrHideAnalyzeButton();
    }
  }
}
