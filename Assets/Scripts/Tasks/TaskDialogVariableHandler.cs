#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using UniRx;

public class TaskDialogVariableHandler : MonoBehaviour {

  [Required]
  public Canvas parentContainer;

  [Required]
  public Camera konstructorCamera;

  [Required]
  public TMP_LinkHandler linkHandler;

  [Required]
  [AssetsOnly]
  public GameObject draggableMenuItemPrefab;

  [Required]
  public DropAreaHighlightManager dropAreaHighlightScript;

  public Color32 inputVarColor = new Color32(81, 112, 94, 255);
  public Color32 outputVarColor = new Color32(214, 195, 114, 255);

  public GlossaryController glossary;

  public UnityEvent onCreateIventoryItem = new UnityEvent();

  private TaskDataSO taskData;
  private TaskObjectSO taskObject;
  private InventoryItem handledVariableObject;

  private void Start() {

    GameController controller = GameController.GetInstance();
    KonstruktorSceneData konstructorData = controller.gameState.konstruktorSceneData;
    taskData = konstructorData.taskData;
    taskObject = konstructorData.taskObject;

    linkHandler.RegisterPrefix("input", HandleInputVariable);
    linkHandler.RegisterPrefix("output", HandleOutputVariable);

    if(glossary != null) {

      linkHandler.RegisterPrefix("glossary", HandleGlossaryLink);
    }
  }

  private void HandleInputVariable(PointerEventData data, string linkId, PointerEventType eventType) {

    TaskInputVariable input = GetInputVariableByLinkId(linkId);
    HandleVariable(data, input, eventType);
  }

  private void HandleOutputVariable(PointerEventData data, string linkId, PointerEventType eventType) {

    TaskOutputVariable output = GetOutputVariableByLinkId(linkId);
    HandleVariable(data, output, eventType);
  }

  private void HandleGlossaryLink(PointerEventData data, string linkId, PointerEventType eventType) {

    if(eventType == PointerEventType.Click) {

      string entryName = linkId.Split(':')[1].Replace('_', ' ');
      glossary.transform.parent.gameObject.SetActive(true);
      glossary.ShowSingleEntry(entryName);
    }
  }

  private void HandleVariable(PointerEventData data, TaskVariable variable, PointerEventType eventType) {

    if(handledVariableObject != null) {

      data.pointerDrag = handledVariableObject.gameObject;
      data.pointerPress = handledVariableObject.gameObject;
    }

    switch(eventType) {

      case PointerEventType.Down:
        handledVariableObject = PrepareInventoryItem(data, variable);
        break;
      case PointerEventType.BeginDrag:
        handledVariableObject.OnBeginDrag(data);
        break;

      case PointerEventType.Drag:
        handledVariableObject.OnDrag(data);
        break;

      case PointerEventType.EndDrag:
        handledVariableObject.OnEndDrag(data);
        break;

      case PointerEventType.Up:
        handledVariableObject.OnPointerUp(data);
        break;
    }
  }

  private TaskInputVariable GetInputVariableByLinkId(string linkId) {

    string[] split = linkId.Split(':');
    string type = split[0];
    string variableName = split[1];

    if(type != "input") {

      return null;
    }

    TaskInputVariable returnVariable = taskData.steps
      .SelectMany((TaskDataSO.SolutionStep step) => step.inputs)
      .FirstOrDefault((TaskInputVariable variable) => variable.name == variableName)
    ;

    if(returnVariable == null)
    {
      returnVariable = taskData.steps
      .SelectMany((TaskDataSO.SolutionStep step) => step.dummyInputs)
      .FirstOrDefault((TaskInputVariable variable) => variable.name == variableName);
    }

    return returnVariable;
  }

  private TaskOutputVariable GetOutputVariableByLinkId(string linkId) {

    string[] split = linkId.Split(':');
    string type = split[0];
    string variableName = split[1];

    if(type != "output") {

      return null;
    }

    return taskData.steps
      .Select((TaskDataSO.SolutionStep step) => step.output)
      .FirstOrDefault((TaskOutputVariable variable) => variable.name == variableName)
    ;
  }

  private InventoryItem PrepareInventoryItem(PointerEventData data, TaskVariable variable) {

    InventoryItem inventoryItemScript = InventoryItemFactory.Instantiate(
      draggableMenuItemPrefab,
      parentContainer,
      variable,
      taskData,
      taskObject
    );

    Vector3 worldPos = konstructorCamera.ScreenToWorldPoint(data.position);
    worldPos.z = inventoryItemScript.transform.position.z - 0.001f;
    inventoryItemScript.transform.position = worldPos;

    if(inventoryItemScript.magnitude.Value is TaskInputVariable) {
      inventoryItemScript.GetComponent<Image>().color = inputVarColor;
    } else {
      inventoryItemScript.GetComponent<Image>().color = outputVarColor;
    }

    data.pointerDrag = inventoryItemScript.gameObject;
    data.pointerPress = inventoryItemScript.gameObject;

    if(onCreateIventoryItem != null)
      onCreateIventoryItem.Invoke();

    return inventoryItemScript;
  }
}
