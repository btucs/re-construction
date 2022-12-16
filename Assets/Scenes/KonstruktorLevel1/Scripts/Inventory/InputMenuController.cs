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
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class InputMenuController : MonoBehaviour, IDropHandler {

  [Required]
  public GameObject inputMenuItemTemplate;
  [Required]
  public Transform inputMenutItemContainer;

  public InputMenuSizeExpander sizeController;

  public InventoryTrashController trashController;

  public InventoryItemEvent onDrop = new InventoryItemEvent();

  [Required]
  [AssetsOnly]
  public GameObject inventoryItemPrefab;

  public Color32 inputVarColor = new Color32(81, 112, 94, 255);
  public Color32 outputVarColor = new Color32(214, 195, 114, 255);

  private GameController controller;
  private List<InputMenuItemController> inputItems = new List<InputMenuItemController>();
  private Canvas parentCanvas;
  private int maxInputs = 1; // max inputs which are not results
  // ignore drops when loading a putting inventory items on startup
  private bool temporaryIgnoreDrops = false;

  private DelegateCondition<InventoryItem> dropAreaCondition;

  public InputMenuItemController[] GetInputItemControllers() {

    return inputItems.ToArray();
  }

  public InputMenuItemController GetInputItemControllerAt(int index) {
    return inputItems[index];
  }

  public void Persist(bool save = true) {

    if(inputItems.Count > 0) {

      MathMagnitude[] inputs = inputItems
        .Where((InputMenuItemController item) => item.droppedItem != null)
        .Select((InputMenuItemController item) => item.droppedItem.magnitude).ToArray()
      ;

      controller.gameState.konstruktorSceneData.inputs = inputs;

      if(save == true) {

        controller.SaveGame();
      }
    }
  }

  public bool ContainsResultItem()
  {
    foreach(InputMenuItemController itemController in inputItems)
    {
      if(itemController.droppedItem != null && itemController.droppedItem.isResult == true)
        return true;
    }
    return false;
  }

  public int GetNumberOfInputItems() {
    return inputItems.Count;
  }

  public void RemoveInputController(InputMenuItemController input) {

    List<InputMenuItemController> found = FindEmptyInputItems();
    int nonResultCount = GetNonResultInputsCount();

    if(nonResultCount + found.Count > maxInputs || found.Count > 1) {

      inputItems.Remove(input);
      found.Remove(input);
      Destroy(input.gameObject);

      if(sizeController != null) {

        sizeController.UpdateTransformSizeDelayed();
      }
    }

    if(found.Count > 0) {

      found[0].transform.SetAsLastSibling();
    }

    Persist();
  }

  public void AddInput(MathMagnitude input, bool isResult = false) {

    InputMenuItemController instanceInputItemController = FindEmptyInputItem();

    if(instanceInputItemController == null) {

      instanceInputItemController = GenerateInputPlaceholder();
    }

    InventoryItem inventoryItem = CreateInventoryItem(input);
    inventoryItem.isResult = isResult;

    instanceInputItemController.dropArea.DropItem(inventoryItem, isResult);
  }

  private void Start() {

    inputMenuItemTemplate.SetActive(false);
    controller = GameController.GetInstance();
    parentCanvas = GetComponentInParent<Canvas>();
    KonstruktorSceneData konstruktorSceneData = controller.gameState.konstruktorSceneData;
    dropAreaCondition = CreateDropAreaCondition();
    maxInputs = konstruktorSceneData.taskData.steps.Sum((TaskDataSO.SolutionStep step) => step.inputs.Length);

    MathMagnitude[] inputs = konstruktorSceneData.inputs;
    GenerateIdentifiedInputItems(inputs);

    int inputItemCount = GetNonResultInputsCount();

    if(inputItemCount < maxInputs) {

      GenerateInputPlaceholder();
    }

    OnEndItemDrag();
  }

  private void OnDestroy() {

    try {

      Persist();
    } catch(Exception) {

    }
  }

  private int GetNonResultInputsCount() {

    return inputItems.Count((InputMenuItemController item) => item.droppedItem != null && item.droppedItem.isResult == false);
  }

  private InputMenuItemController FindEmptyInputItem() {

    return inputItems.FirstOrDefault((InputMenuItemController inputItem) => inputItem.droppedItem == null);
  }

  private List<InputMenuItemController> FindEmptyInputItems() {

    return inputItems.Where((InputMenuItemController inputItem) => inputItem.droppedItem == null).ToList();
  }

  private void GenerateIdentifiedInputItems(MathMagnitude[] inputs) {

    if(inputs == null) {

      return;
    }

    temporaryIgnoreDrops = true;
    foreach(MathMagnitude input in inputs) {

      ConverterResultData foundResult = controller.gameState.konstruktorSceneData.FindResultFor(input.Value as TaskInputVariable);
      AddInput(input, foundResult != null);
    }
    temporaryIgnoreDrops = false;
  }

  private InputMenuItemController GenerateInputPlaceholder() {

    GameObject instance = Instantiate(inputMenuItemTemplate, inputMenutItemContainer);
    instance.SetActive(true);
    InputMenuItemController instanceInputItemController = instance.GetComponent<InputMenuItemController>();
    instanceInputItemController.SetParent(this);
    instanceInputItemController.dropArea.Initialize();

    int newItemIndex = inputItems.Count();
    inputItems.Add(instanceInputItemController);
    instanceInputItemController.dropArea.AddCondition(dropAreaCondition);
    instanceInputItemController.dropArea.itemDropped.AddListener((InventoryItem item) => HandleDrop(item, newItemIndex));
  
    if(sizeController != null)
    {
      sizeController.UpdateTransformSize();
    }
  
    return instanceInputItemController;
  }

  private InventoryItem CreateInventoryItem(MathMagnitude item) {

    InventoryItem instance = InventoryItemFactory.Instantiate(inventoryItemPrefab, parentCanvas, item);
    //instance.cloneOnDrag = true;

    //change Object color to yellow if it is an output variable
    if(instance.magnitude.Value is TaskInputVariable) {
      instance.GetComponent<Image>().color = inputVarColor;
    } else {
      instance.GetComponent<Image>().color = outputVarColor;
    }

    if(sizeController != null)
    {
      sizeController.UpdateTransformSize();
    }

    return instance;
  }

  private void HandleDrop(InventoryItem item, int inputsIndex) {

    int nonResultCount = GetNonResultInputsCount();

    if(temporaryIgnoreDrops == false && nonResultCount < maxInputs && FindEmptyInputItem() == null) {

      GenerateInputPlaceholder();
    }

    Persist();

    IObservable<InventoryItem> onDestroy = item.OnDestroyEvent.AsObservable();

    item.OnMoveEvent.AddListener(OnDragOutItem);
    item.OnReleaseEvent.AsObservable()
      .SelectMany((InventoryItem _item) => Observable.Timer(TimeSpan.FromMilliseconds(120))
        .Take(1)
        .Do((_) => {
          OnEndItemDrag(_item);
        })
        .Select((_) => _item)
      )
      .TakeUntilDestroy(this)
      .Subscribe()
    ;

    onDrop?.Invoke(item);
  }

  private DelegateCondition<InventoryItem> CreateDropAreaCondition() {

    DelegateCondition<InventoryItem> condition = new DelegateCondition<InventoryItem>();
    DelegateCondition<InventoryItem>.DelegateConditionDelegate delegateMethod = CanDropItem;
    condition.SetReferenceValue(delegateMethod);

    return condition;
  }

  private bool CanDropItem(InventoryItem droppedItem) {

    return inputItems.Any(
      (InputMenuItemController inputController) => inputController.droppedItem != null && inputController.droppedItem.Equals(droppedItem)
    ) == false;
  }

  public void OnDrop(PointerEventData eventData) {
    Debug.Log("input menut controller drop");
  }

  private void OnDragOutItem(InventoryItem eventItem = null)
  {
    if(trashController != null)
    {
      trashController.SetEnabled(true);
    }
  }

  private void OnEndItemDrag(InventoryItem eventItem = null)
  {
    if(trashController == null)
      return;

    trashController.SetEnabled(false);
  }
}
