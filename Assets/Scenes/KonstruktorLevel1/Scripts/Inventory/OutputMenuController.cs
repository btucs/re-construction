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
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class OutputMenuController : MonoBehaviour {

  [Required]
  public GameObject outputMenuItemTemplate;
  [Required]
  public Transform outputMenuItemContainer;

  public InventoryItemEvent onDrop = new InventoryItemEvent();
  
  [Required]
  [AssetsOnly]
  public GameObject inventoryItemPrefab;
  public InputMenuSizeExpander sizeController;

  public CanvasGroup submitButtonColorController;
  public Color32 inputVarColor = new Color32(81, 112, 94, 255);
  public Color32 outputVarColor = new Color32(214, 195, 114, 255);

  private GameController controller;
  private KonstruktorSceneData konstruktorSceneData;
  private Canvas parentCanvas;
  private List<OutputMenuItemController> outputItems = new List<OutputMenuItemController>();

  public OutputMenuItemController[] GetOutputItemControllers() {

    return outputItems.ToArray();
  }

  public void TrySubmitOutputs()
  {
    foreach(OutputMenuItemController outputItemController in outputItems)
    {
      if(outputItemController.droppedItem == null)
      {
        Debug.Log("Feedback that not all items have been identified");
        return;
      } else if(outputItemController.droppedItem.hasResult == false){
        Debug.Log("Feedback that inventory Item has no result assigned");
        return;
      }
    }
    if(UIPanelManager.Instance != null)
      UIPanelManager.Instance.ShowAndPersist("Assessment Canvas");

  }

  public void UpdateButton()
  {
    if(submitButtonColorController != null)
      submitButtonColorController.alpha = EvaluateButtonState() ? 1f : 0.3f;
  }

  public bool SearchVarsIdentified()
  {
    foreach(OutputMenuItemController outputItemController in outputItems)
    {
      if(outputItemController.droppedItem == null)
        return false;
    }
    return true;
  }

  public bool ResultsAssigned()
  {
    return EvaluateButtonState();
  }

  private bool EvaluateButtonState()
  {
    Debug.Log("Evaluating button state");
    foreach(OutputMenuItemController outputItemController in outputItems)
    {
      if(outputItemController.droppedItem == null || outputItemController.droppedItem.hasResult == false)
      {
        return false;
      }
    }
    return true;
  }

  public OutputMenuItemController GetOutputItemController(int index) {

    return outputItems[index];
  }

  public OutputMenuItemController FindFor(TaskDataSO.SolutionStep solutionStep) {

    return outputItems.FirstOrDefault((OutputMenuItemController item) => {

      InventoryItem droppedItem = item.droppedItem;

      if(droppedItem == null) {

        return false;
      }

      TaskOutputVariable output = droppedItem.magnitude.Value as TaskOutputVariable;

      return output.Equals(solutionStep.output);
    });
  }

  public void Persist() {

    if(outputItems.Count > 0) {

      MathMagnitude[] items = outputItems
        .Where((OutputMenuItemController item) => item.droppedItem?.magnitude != null)
        .Select((OutputMenuItemController item) => item.droppedItem.magnitude)
        .ToArray()
       ;

      controller.gameState.konstruktorSceneData.outputs = items;
      controller.SaveGame();
    }
  }
  
  private void Start() {

    outputMenuItemTemplate.SetActive(false);
    controller = GameController.GetInstance();
    konstruktorSceneData = controller.gameState.konstruktorSceneData;
    parentCanvas = GetComponentInParent<Canvas>();

    TaskDataSO.SolutionStep[] steps = konstruktorSceneData.taskData.steps;
    GenerateOutputItems(steps);
    UpdateButton();
  }

  private void OnDestroy() {

    Persist();
  }

  private void GenerateOutputItems(TaskDataSO.SolutionStep[] steps) {

    for(int i = 0; i < steps.Length; i++) {

      TaskDataSO.SolutionStep step = steps[i];
      GameObject instance = Instantiate(outputMenuItemTemplate, outputMenuItemContainer);
      instance.SetActive(true);

      OutputMenuItemController instanceOutputItemController = instance.GetComponent<OutputMenuItemController>();
      instanceOutputItemController.Initialize();
      instanceOutputItemController.outputDropArea.itemDropped.AddListener(HandleDrop);
      instanceOutputItemController.resultDropArea.itemDropped.AddListener(HandleResultDrop);

      if(instanceOutputItemController != null) {

        outputItems.Add(instanceOutputItemController);

        ConverterResultData matchingResult = konstruktorSceneData.FindResultFor(step);
        if(konstruktorSceneData.outputs != null && i < konstruktorSceneData.outputs.Length && konstruktorSceneData.outputs[i] != null) {

          InventoryItem item = CreateInventoryItem(konstruktorSceneData.outputs[i], matchingResult);
          instanceOutputItemController.outputDropArea.DropItem(item, false);
        }
      }
    }

    if (sizeController != null)
    {
       sizeController.UpdateTransformSize();
    }
  }

  private InventoryItem CreateInventoryItem(MathMagnitude item, ConverterResultData result) {

    InventoryItem instance = InventoryItemFactory.Instantiate(inventoryItemPrefab, parentCanvas, item);
    //instance.enableDrag = false;
    //instance.cloneOnDrag = true;

    Image imageComponent = instance.GetComponent<Image>();
    TaskOutputVariable value = (TaskOutputVariable)instance.magnitude.Value;
    instance.hasResult = result != null;

    //change Object color to yellow if it is an output variable
    if(instance.hasResult == true ) {
      imageComponent.color = inputVarColor;
      TaskInputVariable resultVariable = result.calculatorResult.Value as TaskInputVariable;
      value.name = resultVariable.textValue;
    } else {
      imageComponent.color = outputVarColor;
    }

    return instance;
  }

  private void HandleDrop(InventoryItem item) {

    Persist();
    UpdateButton();

    onDrop?.Invoke(item);
  }

  private void HandleResultDrop(InventoryItem item) {

    UpdateButton();
  }
}
