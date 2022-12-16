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

public class DrawReplacementModelHandler : AbstractClassificationHandler {

  [Required, FoldoutGroup("Individual Configuration", expanded: false)]
  public ReplacementModelController replacementModelController;
  [Required, FoldoutGroup("Individual Configuration")]
  public ReplacementModelDropAreaController dropAreaController;

  private Canvas parentCanvas;
  private Vector2 offset;
  private GameController controller;

  //private InputOutputDrawController currentlyActiveOutput;
  private List<ReplacementModelDropAreaController> dropAreas = new List<ReplacementModelDropAreaController>();

  public override DrawVariableData[] GetInputs() {

    return new DrawVariableData[0];
  }

  public override DrawResultData[] GetOutputs() {

    return new DrawResultData[1] {
      new DrawResultData() { name = "E", expectedType = TaskOutputVariableUnit.ReplacementModel }
    };
  }

  public override void HandleInputItemDrop(InputOutputDrawController input) {
    throw new System.NotImplementedException();
  }

  public override void HandleOutputItemDrop(InputOutputDrawController output) {

    /*currentlyActiveOutput = output;
    TryActivateContinue();*/
  }

  public void TryActivateContinue() {

    if(
      //currentlyActiveOutput != null && 
      dropAreas.All(
        (ReplacementModelDropAreaController dropAreaController) => dropAreaController.droppedItem != null
      )
    ) {

      continueButton.interactable = true;
    }
  }

  public override void CreateAndSaveResult() {
    Debug.Log("Save result");
    ConverterResultData result = CreateResultData();

    controller.gameState.konstruktorSceneData.AddResult(result);

    inputMenuController.AddInput(result.calculatorResult, true);
    inputMenuController.Persist(false);

    controller.SaveGame();
  }

  public override int GetDroppedItemCount() {

    return dropAreas.Where((ReplacementModelDropAreaController dropArea) => dropArea.droppedItem != null).Count();
  }

  public override bool AllItemsDropped() {

    return GetDroppedItemCount() == dropAreas.Count;
  }

  protected override ConverterResultData CreateResultData() {

    ReplacementModelMapping[] mapping = dropAreas.Select(
      (ReplacementModelDropAreaController repController) => {

        TaskInputVariable inputVariable = repController.droppedItem.magnitude.Value as TaskInputVariable;

        return new ReplacementModelMapping() {
          variableName = inputVariable.name,
          selectedType = repController.selectedType,
          expectedType = repController.expectedType,
          variableType = inputVariable.replacementModelType,
        };
      }).ToArray()
    ;

    KonstruktorSceneData konstruktorSceneData = controller.gameState.konstruktorSceneData;

    MathMagnitude calculatorResult = new MathMagnitude() {
      Value = new TaskInputVariable() {
        name = "E",
        shortDescription = "Ersatzmodell",
      },
      replacementModelMapping = mapping,
      taskData = konstruktorSceneData.taskData,
      taskObject = konstruktorSceneData.taskObject,
    };

    return new ConverterResultData() {
      calculatorType = CalculatorEnum.ReplacementModelCalculator,
      calculatorResult = calculatorResult,
      moduleType = KonstruktorModuleType.ReplacementModel,
    };
  }

  protected override void HandleTaskVariableTap(InputOutputDrawController item) {
    throw new System.NotImplementedException();
  }

  private void InitializeDropAreas() {

    ReplacementModelController.ReplacementModelItem[] parts = replacementModelController.replacementTargets;

    foreach(ReplacementModelController.ReplacementModelItem part in parts) {

      BoxCollider2D collider = part.replacementTarget.GetComponent<BoxCollider2D>();
      Vector2 colliderSize = collider.size;
      Vector3 colliderOffset = collider.offset;

      Quaternion rotation = part.replacementTarget.transform.localRotation;
      // calculate collider corners
      float top = collider.offset.y + (collider.size.y / 2f);
      float btm = collider.offset.y - (collider.size.y / 2f);
      float left = collider.offset.x - (collider.size.x / 2f);
      float right = collider.offset.x + (collider.size.x / 2f);

      // convert collider corners into world space and then canvas space
      Vector3 topLeft = ConvertWorldPositionToRectPosition(Quaternion.Inverse(rotation) * collider.transform.TransformPoint(new Vector3(left, top, 0f)));
      Vector3 topRight = ConvertWorldPositionToRectPosition(Quaternion.Inverse(rotation) * collider.transform.TransformPoint(new Vector3(right, top, 0f)));
      Vector3 btmLeft = ConvertWorldPositionToRectPosition(Quaternion.Inverse(rotation) * collider.transform.TransformPoint(new Vector3(left, btm, 0f)));
      Vector3 btmRight = ConvertWorldPositionToRectPosition(Quaternion.Inverse(rotation) * collider.transform.TransformPoint(new Vector3(right, btm, 0f)));

      ReplacementModelDropAreaController dropArea = Instantiate(dropAreaController, placedItemsContainer);
      dropArea.Initialize();
      dropArea.name = part.replacementTarget.name + " ReplacementModdel";
      dropArea.expectedType = part.replacementType;
      dropAreas.Add(dropArea);
      dropArea.dropArea.itemDropped.AddListener(HandleVariableDrop);

      // calculate the pivot of the Sprite
      SpriteRenderer spriteRenderer = part.replacementTarget.GetComponent<SpriteRenderer>();
      Bounds bounds = spriteRenderer.sprite.bounds;
      float pivotX = -bounds.center.x / bounds.extents.x / 2 + 0.5f;
      float pivotY = -bounds.center.y / bounds.extents.y / 2 + 0.5f;

      RectTransform rectTransform = dropArea.transform as RectTransform;
      rectTransform.pivot = new Vector2(pivotX, pivotY);

      Vector2 center = spriteRenderer.bounds.extents;
      Vector3 offset = Vector3.zero;

      if(rectTransform.pivot.y == 0) {

        offset = new Vector3(0, colliderSize.y / 2);
      }

      rectTransform.sizeDelta = new Vector2((topRight - topLeft).x, (topLeft - btmLeft).y);
      Vector3 localPosition = part.replacementTarget.transform.localPosition + rotation * (colliderOffset - offset);

      rectTransform.anchoredPosition = ConvertWorldPositionToRectPosition((Vector2) part.replacementTarget.transform.parent.TransformPoint(localPosition));
      rectTransform.localRotation = rotation;

      BoxCollider targetCollider = dropArea.GetComponent<BoxCollider>();
      targetCollider.size = rectTransform.sizeDelta;
      targetCollider.center = rectTransform.rect.center;

      RectTransform replacementTypeImageTransform = dropArea.replacementTypeImage.transform as RectTransform;
      replacementTypeImageTransform.rotation = Quaternion.identity;
    }
  }

  private void HandleVariableDrop(InventoryItem item) {

    TryActivateContinue();
  }

  private Vector2 ConvertWorldPositionToRectPosition(Vector2 position) {

    Vector2 localPoint = parentCanvas.WorldToCanvasPosition(position, parentCanvas.worldCamera);

    return localPoint;
  }

  private void Start() {

    parentCanvas = GetComponentInParent<Canvas>();
    controller = GameController.GetInstance();

    Initialize();
    InitializeDropAreas();
  }

  private void OnDestroy() {

    foreach(ReplacementModelDropAreaController dropArea in dropAreas) {

      Destroy(dropArea.gameObject);
    }
  }
}
