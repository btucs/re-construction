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
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public abstract class AbstractClassificationHandler : MonoBehaviour, IClassificationHandler
{

  [Required, FoldoutGroup("General Configuration", expanded: false)]
  public RectTransform leftSidebarContent;
  [Required, FoldoutGroup("General Configuration")]
  public RectTransform rightSidebarContent;
  [Required, FoldoutGroup("General Configuration")]
  public RectTransform placedItemsContainer;
  [Required, FoldoutGroup("General Configuration")]
  public InputOutputDrawController inputOutputPlaceholderTemplate;
  [Required, FoldoutGroup("General Configuration")]
  public Button continueButton;
  [Required, FoldoutGroup("General Configuration")]
  public InputMenuController inputMenuController;

  public abstract DrawVariableData[] GetInputs();
  public abstract DrawResultData[] GetOutputs();
  public abstract void HandleInputItemDrop(InputOutputDrawController input);
  public abstract void HandleOutputItemDrop(InputOutputDrawController output);
  public abstract void CreateAndSaveResult();
  public abstract int GetDroppedItemCount();
  public abstract bool AllItemsDropped();

  protected abstract void HandleTaskVariableTap(InputOutputDrawController item);
  protected abstract ConverterResultData CreateResultData();

  protected List<InputOutputDrawController> inputs = new List<InputOutputDrawController>();

  public virtual void Initialize() {

    GenerateInputs();
    //GenerateOutputs();
  }

  public virtual void Terminate() {

  } 

  private void GenerateInputs() {

    DrawVariableData[] variableNames = GetInputs();
    HelperFunctions.DestroyChildren<InputOutputDrawController>(leftSidebarContent);

    for(int i = 0; i < variableNames.Length; i++) {

      InputOutputDrawController obj = Instantiate(inputOutputPlaceholderTemplate, leftSidebarContent);
      obj.expectedParameterType = variableNames[i].expectedType;
      obj.gameObject.SetActive(true);
      obj.onTap.AddListener(HandleTaskVariableTap);
      obj.onItemDropped.AddListener(HandleInputItemDrop);
      // when item is dropped it is also selected, so every other selected item has to deselected
      obj.onItemDropped.AddListener(HandleTaskVariableTap);
      obj.GetComponentInChildren<TMP_Text>().text = variableNames[i].name;
      inputs.Add(obj);
    }
  }

  /*private void GenerateOutputs() {

    DrawResultData[] variableNames = GetOutputs();
    HelperFunctions.DestroyChildren<InputOutputDrawController>(rightSidebarContent);

    for(int i = 0; i < variableNames.Length; i++) {

      InputOutputDrawController obj = GameObject.Instantiate(inputOutputPlaceholderTemplate, rightSidebarContent);
      obj.gameObject.SetActive(true);
      obj.onItemDropped.AddListener(HandleOutputItemDrop);
      obj.GetComponentInChildren<TMP_Text>().text = variableNames[i].name;
      obj.dropArea.acceptedTypeString = "TaskOutputVariable";
      obj.expectedOutputUnit = variableNames[i].expectedType;
    }
  }*/
}

