#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using MathUnits.Physics.Units;
using MathUnits.Physics.Values;
using UnityEngine;
using UnityEngine.UI;

public class ConstructorManager : StepControllerAbstract
{
  public GameObject varModulePrefab;
  public GameObject trigonometryVarModulePrefab;
  public GameObject constModulePrefab;
  public GameObject searchedVarModulePrefab;
  public GameObject equalsModulePrefab;
  public GameObject additionModulePrefab;
  public GameObject subtractionModulePrefab;
  public GameObject multiplicationModulePrefab;

  public Transform moduleContainer;
  public RectTransform startButton;
  public Button repeatButton;
  public GameObject startButtonHighlight;

  public KonstruktorVariableAnalysisController analyseController;
  public InputMenuController inputItemRef;
  public Image overlayPanel;

  private KonstructorModuleSO activeModule;
  private string searchedVariableName;

  private List<MathModuleController> modules = new List<MathModuleController>();
  private MathModuleController equalsModuleInstance;
  private List<MathVarHandler> varAreas = new List<MathVarHandler>();
  private MathVarHandler searchedVarArea;
  private bool formulaMode = true;
  private bool hasBeenExecuted = false;

  private GameController controller;
  private TextMeshProRendererFactory tmpRenderer;

  public void CheckForReadyCondition(InventoryItem obsoleteItem = null) {
    foreach(MathVarHandler _var in varAreas) {
      if(_var.droppedItem == null) {
        return;
      }
    }
    /*if(searchedVarArea.droppedItem == null) {
      return;
    }*/
    ActivateStartButton();
  }

  public bool AllInputsAssigned()
  {
    foreach(MathVarHandler _var in varAreas) {
      if(_var.droppedItem == null) {
        return false;
      }
    }
    return true;
  }

  public bool HasBeenExecuted()
  {
    return hasBeenExecuted;
  }

  public void ExecuteConstructor() {
    HideOverlayPanel();
    DisplayUIElementsBehind();
    Calculate();
    AssessInsertedValues();
    hasBeenExecuted = true;
  }

  public void DisplayFormulaMode() {
    ShowOverlayPanel();
    DisplayUIElementsInFront();
  }

  private void Calculate() {

    ScalarValue endScalar = null;
    MathModuleController[] moduleClones = modules
      .Select((MathModuleController module) => module.ResetResults())
      .OrderByDescending((MathModuleController module) => module.priority)
      .ToArray()
    ;

    foreach(MathModuleController module in moduleClones) {

      ScalarValue resultScalar = module.ExecuteCalculation();
      Newton newton = new Newton();
      try {
        bool isConvertable = Unit.Convertible(resultScalar.Unit, newton);
        if(isConvertable == true) {

          resultScalar.Unit = newton;
        }
      } catch(UnitsUnconvertibleException) {

        // not convertable
      }

      endScalar = resultScalar;
    }

    if(endScalar != null) {
      //Debug.Log("Should Show Result");

      int exponent = (int)searchedVarArea.GetExponentValue();
      if(exponent > 1) {

        endScalar = CalculationHelper.NthRoot(endScalar, exponent);
      }

      ShowResult(endScalar);
    }
    //Debug.Log("Result calculated: " + endScalar);
  }

  private void AssessInsertedValues() {

  }

  public override void SetupAndOpen(KonstructorModuleSO newModule) {
    activeModule = newModule;
    CreateConstructor();
    gameObject.SetActive(true);
  }

  public void CreateConstructor() {
    ResetModules();
    string[] konstruktorFormula = activeModule.SplitFormulaBySpace();
    for(int i = 0; i < konstruktorFormula.Length; i++) {
      CreateModule(konstruktorFormula[i]);
    }
    ConnectModulesWithVariables();
    //Debug.Log("Number of Modules: " + modules.Count);
  }

  private void ConnectModulesWithVariables() {
    for(int i = 0; i < modules.Count; i++) {
      //if there is var with same index, set it as VarA
      //if there is var with index + 1, set it as VarB
      if(varAreas.Count > i + 1) {
        modules[i].AssignValueLinks(varAreas[i], varAreas[i + 1]);
      } else {
        Debug.Log("There are not enough Variable Containers for the current number of math modules.");
      }
    }
  }

  private void CreateModule(string formulaFragment) {

    if(formulaFragment.Contains("?")) {
      CreateSearchedVarModule(formulaFragment);
    } else if(formulaFragment.Contains("#")) {
      CreateGivenVarWithConstantModule(formulaFragment);
    } else if(formulaFragment.Contains("cos") || formulaFragment.Contains("sin") || formulaFragment.Contains("tan")) {
      CreateGivenTrigonometryVar(formulaFragment);
    } else {
      switch(formulaFragment) {
        case "=":
          CreateEqualsModule();
          break;

        case "+":
          CreateAdditionModule();
          break;

        case "-":
          CreateSubstractionModule();
          break;

        case "*":
          CreateMultiplyModule();
          break;

        default:
          CreateGivenVarModule(formulaFragment);
          break;
      }
    }
  }

  private void ShowResult(ScalarValue _resultScalar) {

    ScalarValue rounded = ScalarValueHelper.Round(_resultScalar, 2);

    MathMagnitude resultMagnitude = CreateResultMagnitude(rounded);
    CreateNewInventoryItem(resultMagnitude, rounded);

    ConverterResultData result = CreateResultData(resultMagnitude, new Dictionary<string, MathMagnitude>());
    controller.gameState.konstruktorSceneData.AddResult(result);

    inputItemRef.AddInput(result.calculatorResult, true);
    inputItemRef.Persist(false);

    controller.SaveGame();
  }

  private ConverterResultData CreateResultData(MathMagnitude result, Dictionary<string, MathMagnitude> parameters) {

    return new ConverterResultData() {
      //step = outputController.StepIndex,
      calculatorType = activeModule.calculator,
      calculatorParams = parameters,
      calculatorResult = result,
      moduleType = activeModule.moduleType,
    };
  }

  private InventoryItem CreateNewInventoryItem(MathMagnitude item, ScalarValue resultRef) {
    Canvas searchedVarCanvas = searchedVarArea.connectedDropArea.GetComponent<Canvas>();
    InventoryItem instance = InventoryItemFactory.Instantiate(inputItemRef.inventoryItemPrefab, searchedVarCanvas, item, true);
    instance.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    instance.isResult = true;
    instance.enableDrag = false;
        
    instance.variableName.text = tmpRenderer.RenderMarkdownStringToTextMeshProString(resultRef.ToString(CultureInfo.InvariantCulture));
    
    //change Object color
    instance.GetComponent<Image>().color = inputItemRef.inputVarColor;

    return instance;
  }

  private MathMagnitude CreateResultMagnitude(ScalarValue item) {

    KonstruktorSceneData konstruktorSceneData = controller.gameState.konstruktorSceneData;
    string inputName = CleanVariableName(searchedVariableName);
    TaskInputVariable input = new TaskInputVariable();
    input.SetScalarValue(item);
    input.textMeshProValue = tmpRenderer.RenderMarkdownStringToTextMeshProString(item.ToString(CultureInfo.InvariantCulture));
    input.name = inputName;
    input.shortDescription = "Ein ausgerechneter Wert, erstellt durch das Modul: " + activeModule.title;
    input.textMeshProName = inputName;

    return new MathMagnitude() {
      Value = input,
      taskData = konstruktorSceneData.taskData,
      taskObject = konstruktorSceneData.taskObject,
    };
  }

  private string CleanVariableName(string name) {

    // remove exponent from name, since in consinelaw squareroot is calculated in the end
    return name.Split('^')[0];
  }

  private void CreateEqualsModule() {
    GameObject moduleObject = Instantiate(equalsModulePrefab, moduleContainer);
    MathModuleController moduleController = moduleObject.GetComponent<MathModuleController>();
    moduleController.Setup(MathFunctionType.equals, modules.Count);
    equalsModuleInstance = moduleController;
  }

  private void CreateMultiplyModule() {
    GameObject moduleObject = Instantiate(multiplicationModulePrefab, moduleContainer);
    MathModuleController moduleController = moduleObject.GetComponent<MathModuleController>();
    moduleController.Setup(MathFunctionType.multiply, modules.Count);
    modules.Add(moduleController);
  }

  private void CreateAdditionModule() {
    GameObject moduleObject = Instantiate(additionModulePrefab, moduleContainer);
    MathModuleController moduleController = moduleObject.GetComponent<MathModuleController>();
    moduleController.Setup(MathFunctionType.addition, modules.Count);
    modules.Add(moduleController);
  }

  private void CreateSubstractionModule() {
    GameObject moduleObject = Instantiate(subtractionModulePrefab, moduleContainer);
    MathModuleController moduleController = moduleObject.GetComponent<MathModuleController>();
    moduleController.Setup(MathFunctionType.subtract, modules.Count);
    modules.Add(moduleController);
  }

  private void CreateGivenVarModule(string _formulaFragment) {
    GameObject varObject = Instantiate(varModulePrefab, moduleContainer);
    MathVarHandler varController = varObject.GetComponent<MathVarHandler>();
    varController.SetUp(_formulaFragment);
    varController.connectedDropArea.itemDropped.AddListener(CheckForReadyCondition);
    varAreas.Add(varController);

    varObject.GetComponent<FormularVarController>().ResolveFormulaTextFragment(_formulaFragment);
  }

  private void CreateGivenVarWithConstantModule(string _formulaFragment) {
    string[] myStringArray = _formulaFragment.Split('#');
    MathConstantHandler theConstant = null;
    if(Double.TryParse(myStringArray[0], out double j)) {
      GameObject constObject = Instantiate(constModulePrefab, moduleContainer);
      theConstant = constObject.GetComponent<MathConstantHandler>();
      theConstant.SetValue(j);
    } else {
      Debug.Log("Die Konstante, mit der " + myStringArray[1] + " multipliziert werden soll, kann nicht in eine Zahl überführt werden.");
    }

    if(myStringArray.Length > 1) {
      GameObject varObject = Instantiate(varModulePrefab, moduleContainer);
      MathVarHandler varController = varObject.GetComponent<MathVarHandler>();
      varController.SetUp(myStringArray[1]);
      varController.connectedDropArea.itemDropped.AddListener(CheckForReadyCondition);
      if(theConstant != null) {
        Debug.Log("Konstante gesetzt ");
        varController.SetConstant(theConstant);
      }
      varAreas.Add(varController);

      varObject.GetComponent<FormularVarController>().ResolveFormulaTextFragment(myStringArray[1]);
    }
  }

  private void CreateGivenTrigonometryVar(string _formulaFragment) {
    GameObject varObject = Instantiate(trigonometryVarModulePrefab, moduleContainer);
    MathVarHandler varController = varObject.GetComponent<MathVarHandler>();
    varController.SetUp(_formulaFragment);
    varController.connectedDropArea.itemDropped.AddListener(CheckForReadyCondition);
    varAreas.Add(varController);

    varObject.GetComponent<FormularVarController>().ResolveFormulaTextFragment(_formulaFragment);
  }

  private void CreateSearchedVarModule(string _formulaFragment) {
    string shortenedString = _formulaFragment.Replace("?", "");
    searchedVariableName = shortenedString;
    GameObject varObject = Instantiate(searchedVarModulePrefab, moduleContainer);
    MathVarHandler varController = varObject.GetComponent<MathVarHandler>();
    varController.SetUp(shortenedString);
    varController.connectedDropArea.itemDropped.AddListener(CheckForReadyCondition);
    searchedVarArea = varController;

    varObject.GetComponent<FormularVarController>().ResolveFormulaTextFragment(shortenedString);
  }

  private void ActivateStartButton() {
    startButton.GetComponent<Button>().interactable = true;
    startButtonHighlight.SetActive(true);
  }

  private void DisplayUIElementsBehind() {
    if(formulaMode) {
      foreach(MathVarHandler _var in varAreas) {
        //Debug.Log("UI Element should be displayed behind - " + _var.gameObject.name);
        _var.SetSortingOrder(_var.varCanvas.sortingOrder - 4);
      }
      if(searchedVarArea) {
        searchedVarArea.varCanvas.sortingOrder = searchedVarArea.varCanvas.sortingOrder - 4;
      }

      foreach(MathModuleController module in modules) {
        if(module.iconGraphicObj) {
          module.iconGraphicObj.SetActive(false);
        }
      }
      if(equalsModuleInstance != null && equalsModuleInstance.iconGraphicObj) {
        equalsModuleInstance.iconGraphicObj.SetActive(false);
      }
      formulaMode = false;
    }
  }

  private void DisplayUIElementsInFront() {
    if(!formulaMode) {
      foreach(MathVarHandler _var in varAreas) {
        //Debug.Log("UI Element should be displayed behind - " + _var.gameObject.name);
        _var.SetSortingOrder(_var.varCanvas.sortingOrder + 4);
      }
      if(searchedVarArea) {
        searchedVarArea.varCanvas.sortingOrder = searchedVarArea.varCanvas.sortingOrder + 4;
      }

      foreach(MathModuleController module in modules) {
        if(module.iconGraphicObj) {
          module.iconGraphicObj.SetActive(true);
        }
      }
      if(equalsModuleInstance != null && equalsModuleInstance.iconGraphicObj) {
        equalsModuleInstance.iconGraphicObj.SetActive(true);
      }

      formulaMode = true;
    }
  }

  private void HideOverlayPanel() {
    overlayPanel.GetComponent<Canvas>().sortingOrder = 5;
  }

  private void ShowOverlayPanel() {
    overlayPanel.GetComponent<Canvas>().sortingOrder = 8;
  }

  private void ResetModules() {
    modules.Clear();
    varAreas.Clear();
    foreach(Transform child in moduleContainer) {
      GameObject.Destroy(child.gameObject);
    }
  }

  private void Start() {

    controller = GameController.GetInstance();
    if(repeatButton != null) {

      repeatButton.onClick.AddListener(() => DisplayFormulaMode());
    }

    tmpRenderer = new TextMeshProRendererFactory();
  }
}
