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
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using TMPro;
using UniRx;
using MathUnits.Physics.Values;
using MathUnits.Physics.Units;

public class KonstruktorDrawController : StepControllerAbstract {

  [Required, FoldoutGroup("Configuration", expanded: false)]
  public UIPanelManager uiManager;

  [Required, FoldoutGroup("Configuration")]
  public Slider xSlider;
  [Required, FoldoutGroup("Configuration")]
  public Slider ySlider;
  [Required, FoldoutGroup("Configuration")]
  public GameObject givenAndSearchedItemsCanvas;
  [Required, FoldoutGroup("Configuration")]
  public TMP_Text scale;
  [Required, FoldoutGroup("Configuration")]
  public ScrollRect scrollContainer;

  [Required, FoldoutGroup("Configuration")]
  public RectTransform xMarker;
  [Required, FoldoutGroup("Configuration")]
  public RectTransform yMarker;

  [Required, FoldoutGroup("Configuration")]
  public RectTransform leftSidebarContent;
  [Required, FoldoutGroup("Configuration")]
  public RectTransform rightSidebarContent;

  [Required, FoldoutGroup("Templates", expanded: false)]
  public InputOutputDrawController inputOutputPlaceholderTemplate;
  [Required, FoldoutGroup("Templates")]
  public KonstructorDrawPointController pointTemplate;
  [Required, FoldoutGroup("Templates")]
  public KonstructorDrawAngleController angleTemplate;
  [Required, FoldoutGroup("Templates")]
  public KonstructorDrawVectorController vectorTemplate;
  
  [Required, FoldoutGroup("Configuration")]
  public RectTransform drawnItemsContainer;
  [Required, FoldoutGroup("Configuration")]
  public Text taskNameText;
  [Required, FoldoutGroup("Configuration")]
  public Button backButton;
  [Required, FoldoutGroup("Configuration")]
  public Button repeatButton;

  [Required, FoldoutGroup("Configuration")]
  public Button continueButton;
  [Required, FoldoutGroup("Configuration")]
  public Button confirmPointPositionButton;
  [Required, FoldoutGroup("Configuration")]
  public Toggle drawButton;
  [Required, FoldoutGroup("Configuration")]
  public Toggle editAngleButton;
  [Required, FoldoutGroup("Configuration")]
  public Toggle editPointButton;

  [Required, FoldoutGroup("Configuration")]
  public GenerateGridUI grid;
  [Required, FoldoutGroup("Configuration")]
  public MoveObjWithUIContent followGridScript;

  [Required, FoldoutGroup("Configuration")]
  public DrawLineUI drawLine;
  [Required, FoldoutGroup("Configuration")]
  public OutputMenuController outputMenuController;
  [Required, FoldoutGroup("Configuration")]
  public InputMenuController inputMenuController;
  [Required, FoldoutGroup("Configuration")]
  public SpriteTransparencyController spriteTransparencyController;

  [Required, FoldoutGroup("Configuration"), SuffixLabel("N")]
  public float scaleAmount = 100;
  [Required, FoldoutGroup("Configuration")]
  public float angleStep = 1;

  [HideInEditorMode]
  public TaskDataSO.CoordinateSystemData coordinateSystemData;

  private GridInfo currentGridInfo;
  private float stepMultiplier = 1;

  private UnityAction<float> xSliderValueChanged;
  private UnityAction<float> ySliderValueChanged;
  private UnityAction<Vector2> scrollContainerMoved;

  private GameController controller;
  private KonstructorModuleSO currentModuleSO;
  public AbstractDrawHandler currentDrawHandler {
    get; private set;
  }

  public TaskDataSO currentTask {
    get; private set;
  }

  public TaskObjectSO currentTaskObject {
    get; private set;
  }

  private InputOutputDrawController currentActiveItem;

  private Camera mainCam;
  private Canvas parentCanvas;
  private Vector3 objectPos;

  private bool resultCreated = false;

  private Vector2 gridAnchor;

  public override void SetupAndOpen(KonstructorModuleSO currentModuleSO) {

    uiManager.Show(name);
    taskNameText.text = currentModuleSO.title;
    scale.transform.parent.gameObject.SetActive(false);

    if(controller == null) {

      Start();
    }
    
    if(currentDrawHandler != null) {

      // just destroy the script itself, not the GameObject
      Destroy(currentDrawHandler);
      currentDrawHandler = null;
    }

    this.currentModuleSO = currentModuleSO;
    coordinateSystemData = currentTask.coordinateSystemData;
    scaleAmount = coordinateSystemData.scale;

    currentDrawHandler = DrawHandlerFactory.CreateAndAddDrawHandler(currentModuleSO.moduleType, this);
    currentDrawHandler.onTap.AddListener(HandleTaskVariableTap);
    currentDrawHandler.onEndDraw.AddListener(HandleEndDraw);

    if(currentGridInfo.isDrawFinished == false) {

      followGridScript.SetInactive();
      grid.Configure(
        CalculateGridOrigin(coordinateSystemData.origin),
        coordinateSystemData.dimensions,
        coordinateSystemData.unitSize,
        coordinateSystemData.intermediateSteps
      );

      // save initial grid position
      gridAnchor = grid.gridContainer.anchoredPosition;
      // wait for next frame to reactive followGrid script, as the grid shifts in that time
      Observable.NextFrame()
        .Take(1)
        .Do((_) => followGridScript.SetActive())
        .Subscribe()
      ;

      xSliderValueChanged = (float value) => currentDrawHandler.MoveActiveItem(DirectionEnum.X, SliderValueToGridPos(DirectionEnum.X, value));
      ySliderValueChanged = (float value) => currentDrawHandler.MoveActiveItem(DirectionEnum.Y, SliderValueToGridPos(DirectionEnum.Y, value));
      scrollContainerMoved = (Vector2 value) => UpdateSliderPositions(value);

      currentGridInfo = grid.CurrentGridInfo;
      if(currentGridInfo.isDrawFinished == false) {

        grid.onDrawFinished.AddListener(ConfigureSliders);

      } else {

        ConfigureSliders(currentGridInfo);
      }

      xSlider.onValueChanged.AddListener(xSliderValueChanged);
      ySlider.onValueChanged.AddListener(ySliderValueChanged);
      scrollContainer.onValueChanged.AddListener(scrollContainerMoved);
      //DrawSliderExtension xSliderEventController = xSlider.gameObject.GetComponent<DrawSliderExtension>();
      //xSliderEventController.onPointerUpEvent += this.SnapActiveItemToGrid();
      //DrawSliderExtension ySliderEventController = ySlider.gameObject.GetComponent<DrawSliderExtension>();
      //ySliderEventController.onPointerUpEvent.RemoveAllListeners();

      drawButton.onValueChanged.AddListener(HandleDrawButton);
      drawButton.interactable = false;

      if(editPointButton != null) {

        editPointButton.onValueChanged.AddListener(HandleDrawButton);
      }

      if(editAngleButton != null) {

        editAngleButton.onValueChanged.AddListener(HandleDrawButton);
      }
    } else {

      // restore initial grid position
      grid.gridContainer.anchoredPosition = gridAnchor;
      ConfigureSliders(currentGridInfo);
    }
  }

  public void Hide() {

    uiManager.Hide(name);
  }

  public KonstructorDrawPointController[] GetDrawnItems() {

    return currentDrawHandler.GetDrawPoints();
  }

  public bool ActiveItemHasType(string enumValue) {

    if(currentActiveItem == null) {

      return false;
    }

    if(Enum.TryParse<CalculatorParameterType>(enumValue, out CalculatorParameterType result)) {

      return result == currentActiveItem.expectedParameterType;
    }

    return false;
  }

  public bool AllInputsDropped() {

    return currentDrawHandler.AllInputsDropped();
  }


  public void HandleContinueButtonPressed() {

    // disable drawing, has to be first, otherwise sidebars get activated again
    drawButton.isOn = false;

    //leftSidebarContent.gameObject.SetActive(false);
    //rightSidebarContent.gameObject.SetActive(false);
    
    backButton.onClick.AddListener(HandleBackButtonPressed);
    repeatButton.onClick.AddListener(HandleBackButtonPressed);

    resultCreated = true;
    currentDrawHandler.CreateAndSaveResult();
  }

  public void HandleBackButtonPressed() {

    // remove the event since we only want to handle the click
    // after continue button was pressed
    backButton.onClick.RemoveListener(HandleBackButtonPressed);
    repeatButton.onClick.RemoveListener(HandleBackButtonPressed);

    leftSidebarContent.gameObject.SetActive(true);
    rightSidebarContent.gameObject.SetActive(true);
    givenAndSearchedItemsCanvas.SetActive(true);
    drawButton.SetIsOnWithoutNotify(false);
    drawButton.interactable = false;
    xSlider.gameObject.SetActive(false);
    ySlider.gameObject.SetActive(false);

    currentDrawHandler.ClearDrawnLine();
  }

  public bool ResultCreated()
  {
    return resultCreated;
  }

  public bool EverythingWasEdited()
  {
    return (currentDrawHandler != null) ? currentDrawHandler.AllAdjustmentsMade() : false;
  }

  public bool IsEditingForceAmount()
  {
    if(currentDrawHandler is DrawForceHandler)
    {
      DrawForceHandler currentForceHandler = currentDrawHandler as DrawForceHandler;
      return currentForceHandler.EditingParameter(CalculatorParameterType.Force);
    }
    return false;
  }

  public bool IsEditingForceAngle()
  {
    if(currentDrawHandler is DrawForceHandler)
    {
      DrawForceHandler currentForceHandler = currentDrawHandler as DrawForceHandler;
      return currentForceHandler.EditingParameter(CalculatorParameterType.Force);
    }
    return false;
  }

  public bool IsEditingForcePoint()
  {
    if(currentDrawHandler is DrawForceHandler)
    {
      DrawForceHandler currentForceHandler = currentDrawHandler as DrawForceHandler;
      return currentForceHandler.EditingParameter(CalculatorParameterType.Point);
    }
    return false;
  }

  private Vector2 CalculateGridOrigin(Vector2 originRelativeToObject) {

    return ConvertWorldPositionToRectPosition(objectPos) + originRelativeToObject;
  }

  private Vector2 ConvertWorldPositionToRectPosition(Vector2 position) {

    Vector2 localPoint = parentCanvas.WorldToCanvasPosition(position, parentCanvas.worldCamera);

    return localPoint;
  }

  private void HandleTaskVariableTap(InputOutputDrawController item) {

    continueButton.transform.parent.gameObject.SetActive(!item.IsHighlighted);
    givenAndSearchedItemsCanvas.SetActive(!item.IsHighlighted);

    if(
      item.expectedParameterType == CalculatorParameterType.Point ||
      item.expectedParameterType == CalculatorParameterType.Vector
    ) {

      xSlider.gameObject.SetActive(item.IsHighlighted);
      ySlider.gameObject.SetActive(item.IsHighlighted);
    }

    if(item.IsHighlighted == true) {

      currentActiveItem = item;
      Vector3 activeItemPos = currentDrawHandler.GetActiveItemPos();
      Vector3 activeItemWorldPos = currentDrawHandler.GetActiveItemWorldPos();
      //Debug.Log("Item Pos is: " + activeItemWorldPos);

      xSlider.SetValueWithoutNotify(WorldPosToSliderValue(DirectionEnum.X, activeItemWorldPos));
      ySlider.SetValueWithoutNotify(WorldPosToSliderValue(DirectionEnum.Y, activeItemWorldPos));
    } else {

      currentActiveItem = null;
    }
  }

  public void SnapActiveItemToGrid()
  {
    if(currentDrawHandler != null)
    {
      Vector3 activeItemPos = currentDrawHandler.GetActiveItemPos();
      float moveX = GetNextValidGridPos(activeItemPos.x);
      float moveY = GetNextValidGridPos(activeItemPos.y);
      currentDrawHandler.MoveActiveItem(DirectionEnum.X, moveX);
      currentDrawHandler.MoveActiveItem(DirectionEnum.Y, moveY);

      UpdateSliderPositions(Vector2.zero);  
    }
  }

  private float GetNextValidGridPos(float unroudedValue)
  {
    float stepSize = currentGridInfo.unitSize / currentGridInfo.intermediateSteps;
    float roundedValue = unroudedValue;
    int floorGridUnit = (int)(unroudedValue / stepSize);
    float remainer = unroudedValue % stepSize;
    float direction = remainer < 0 ? -1 : 1;

    if(Math.Abs(remainer) > (stepSize / 2)) {
      roundedValue = (floorGridUnit + (1 * direction)) * stepSize;
    } else {
      roundedValue = floorGridUnit * stepSize;
    }

    return roundedValue;
  }

  private void ConfigureSliders(GridInfo gridInfo) {

    xSlider.maxValue = 1f;
    xSlider.minValue = 0f;
    ySlider.maxValue = 1f;
    ySlider.minValue = 0f;

    stepMultiplier = 1 / gridInfo.unitSize;
    currentDrawHandler.SetStepMultiplier(stepMultiplier);

    currentGridInfo = gridInfo;
  }

  private void UpdateSliderPositions(Vector2 moveAmount)
  {
    Vector3 activeItemWorldPos = currentDrawHandler.GetActiveItemWorldPos();

    xSlider.SetValueWithoutNotify(WorldPosToSliderValue(DirectionEnum.X, activeItemWorldPos));
    ySlider.SetValueWithoutNotify(WorldPosToSliderValue(DirectionEnum.Y, activeItemWorldPos));
  }

  private float ValueToSliderPos(float value) {

    float stepSize = 1;
    if(currentGridInfo.isDrawFinished == true) {

      stepSize = currentGridInfo.unitSize / currentGridInfo.intermediateSteps;
    }

    return value / stepSize;
  }

  private float SliderPosToValue(float value) {

    float stepSize = 0;
    if(currentGridInfo.isDrawFinished == true) {

      stepSize = currentGridInfo.unitSize / currentGridInfo.intermediateSteps;
    }

    return value * stepSize;
  }

  private float SliderValueToValidGridPos(DirectionEnum direction, float value){
    float unroundedValue = SliderValueToGridPos(direction, value);
    float roundedValue = GetNextValidGridPos(unroundedValue);
    return roundedValue;
  }

  private float SliderValueToGridPos(DirectionEnum direction, float value)
  {
    float screenCoordValue = SliderValueToScreenCoord(direction, value);
    //Debug.Log("Screen Coord is: " + screenCoordValue);
    float gridPosValue;
    if(direction == DirectionEnum.X)
    {
      float worldPosValue = ScreenCoordToWorldPos(new Vector3(screenCoordValue, 0, 0)).x;
      gridPosValue = WorldPosToGridPos(new Vector3(worldPosValue, 0, 0)).x;
    } else {
      float worldPosValue = ScreenCoordToWorldPos(new Vector3(0, screenCoordValue, 0)).y;
      gridPosValue = WorldPosToGridPos(new Vector3(0, worldPosValue, 0)).y;
    }
    
    return gridPosValue;
  }

  private float WorldPosToSliderValue(DirectionEnum dir, Vector3 worldspacePos)
  {
    //Debug.Log("in worldspace: " + worldspacePos);
    Vector2 screenPoint = WorldPosToScreenPoint(worldspacePos);
    //Debug.Log("screen Coords: " + screenPoint);
    float returnVal = dir == DirectionEnum.X ? ScreenCoordToSliderValue(dir, screenPoint.x) : ScreenCoordToSliderValue(dir, screenPoint.y);
    //Debug.Log("slider Value: " + returnVal);
    return returnVal;
  }

  private float SliderValueToScreenCoord(DirectionEnum direction, float value) {
    //Get screen coord that equals slider value
    float screenMinCoord, screenMaxCoord;

    RectTransform slideArea = direction == DirectionEnum.X ? xSlider.GetComponent<RectTransform>() : ySlider.GetComponent<RectTransform>();
    Vector3[] rectCorners = new Vector3[4];
    slideArea.GetWorldCorners(rectCorners);
      
    Vector3 minScreenPos = mainCam.WorldToScreenPoint(rectCorners[0]);
    Vector3 maxScreenPos = mainCam.WorldToScreenPoint(rectCorners[2]);
      
    screenMinCoord = direction == DirectionEnum.X ? minScreenPos.x : minScreenPos.y;
    screenMaxCoord = direction == DirectionEnum.X ? maxScreenPos.x : maxScreenPos.y;

    float returnValue = Mathf.Lerp(screenMinCoord, screenMaxCoord, value);
    return returnValue;
  }

  private float ScreenCoordToSliderValue(DirectionEnum direction, float screenCoord) {
    //Get slider value that equals screen coordinates
    float screenMinCoord, screenMaxCoord;

    RectTransform slideArea = direction == DirectionEnum.X ? xSlider.GetComponent<RectTransform>() : ySlider.GetComponent<RectTransform>();
    Vector3[] rectCorners = new Vector3[4];
    slideArea.GetWorldCorners(rectCorners);
      
    Vector3 minScreenPos = mainCam.WorldToScreenPoint(rectCorners[0]);
    //Debug.Log("top left of slider area is at: " + minScreenPos + " worldspace is: " + rectCorners[0]);
    Vector3 maxScreenPos = mainCam.WorldToScreenPoint(rectCorners[2]);
    //Debug.Log("bottom right of slider area is at: " + maxScreenPos);  

    screenMinCoord = direction == DirectionEnum.X ? minScreenPos.x : minScreenPos.y;
    screenMaxCoord = direction == DirectionEnum.X ? maxScreenPos.x : maxScreenPos.y;

    float returnValue = Mathf.InverseLerp(screenMinCoord, screenMaxCoord, screenCoord);
    return returnValue;
  }

  private Vector3 WorldPosToGridPos(Vector3 worldPosToConvert)
  {
    Vector3 localPos = drawnItemsContainer.InverseTransformPoint(worldPosToConvert);
    return localPos;
  }

  private Vector3 GridPosToWorldPos(Vector3 localPosToConvert)
  {
    Vector3 worldspacePos = drawnItemsContainer.TransformPoint(localPosToConvert);
    //Vector3 worldspacePos = grid.gridContainer.TransformPoint(localPosToConvert);
    return worldspacePos;
  }

  private Vector2 WorldPosToScreenPoint(Vector3 worldPos)
  {
    return mainCam.WorldToScreenPoint(worldPos);
  }

  private Vector3 ScreenCoordToWorldPos(Vector2 screenCoord)
  {
    Vector3 screenVector3 = new Vector3(screenCoord.x, screenCoord.y, 0);
    Vector3 worldPoint = mainCam.ScreenToWorldPoint(screenVector3);
    return worldPoint;
  }

  private void HandleDrawButton(bool isActive) {

    givenAndSearchedItemsCanvas.SetActive(!isActive);
    continueButton.gameObject.SetActive(!isActive);
  }

  private void HandleEndDraw(LineControllerUI lineController) {

    continueButton.interactable = lineController != null;
  }

  private void Start() {

    controller = GameController.GetInstance();
    KonstruktorSceneData konstructorData = controller.gameState.konstruktorSceneData;
    currentTask = konstructorData.taskData;
    parentCanvas = GetComponentInParent<Canvas>();
    mainCam = parentCanvas.worldCamera;

    currentTaskObject = konstructorData.taskObject;
    KonstruktorSceneData.InteractableData foundInteractable = konstructorData.interactablesPrefabs.FirstOrDefault((KonstruktorSceneData.InteractableData interactable) => interactable.taskObject == currentTaskObject);
    objectPos = foundInteractable.position;
  }

  private void OnDestroy() {

    grid.onDrawFinished.RemoveListener(ConfigureSliders);

    if(xSliderValueChanged != null) {

      xSlider.onValueChanged.RemoveListener(xSliderValueChanged);
    }

    if(ySliderValueChanged != null) {

      ySlider.onValueChanged.RemoveListener(ySliderValueChanged);
    }

    if(drawButton != null) {

      drawButton.onValueChanged.RemoveListener(HandleDrawButton);
    }

    if(editAngleButton != null) {

      editAngleButton.onValueChanged.RemoveListener(HandleDrawButton);
    }

    if(editPointButton != null) {

      editPointButton.onValueChanged.RemoveListener(HandleDrawButton);
    }
  }
}
