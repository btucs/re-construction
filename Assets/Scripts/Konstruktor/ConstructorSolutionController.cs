#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConstructorSolutionController : MonoBehaviour
{
  public TaskModuleResolver moduleListsForTasks;
  public List<KonstructorModuleSO> availableModules = new List<KonstructorModuleSO>();
  [Required]
  public Transform solutionModuleContainer;
  public Scrollbar modulesScrollbar;
  [Required]
  public GameObject solutionModuleEntryPrefab;
  [Required]
  public ButtonToggleMenu moduleButtonGroup;
  [Required]
  public ConstructorManager konstruktor;
  [Required]
  public KonstruktorDrawController drawController;
  [Required]
  public KonstruktorClassificationController classificationController;
  [Required]
  public MultistepController multistepController;
  [Required]
  public Transform uiContainer;
  [Required]
  public UIPanelManager panelManager;
  [Required]
  public GameObject[] showOnModuleInstanceShow;
  [Required]
  public GameObject[] showOnModuleInstanceHide;

  [Required]
  public GameObject moduleSelectionHint;
  [Required]
  public GameObject modulePreview;
  [Required]
  public GameObject vectorInfoElem;
  [Required]
  public GameObject forceInfoElem;
  [Required]
  public GameObject replacementModelInfoElem;

  [Required]
  public FormulaPrefabController formulaController;
  [Required]
  public TMP_Text moduleDescription;

  public GlossaryController glossary;

  private KonstructorModuleSO currentModuleSO;
  private TMP_LinkHandler linkHandler;

  void Start() {

    linkHandler = moduleDescription.GetComponent<TMP_LinkHandler>();
    if(glossary != null) {

      linkHandler.RegisterPrefix("glossary", HandleGlossaryLink);
    }

    KonstruktorSceneData konstructorData = GameController.GetInstance().gameState.konstruktorSceneData;
    CreateSolutionModuleSuggestions(konstructorData.taskData);
  }

  public void OpenModulePreview(KonstructorModuleSO previewData) {
    moduleSelectionHint.SetActive(false);

    currentModuleSO = previewData;
    moduleDescription.text = previewData.tmpDescription;

    formulaController.gameObject.SetActive(false);
    vectorInfoElem.SetActive(false);
    forceInfoElem.SetActive(false);
    replacementModelInfoElem.SetActive(false);

    switch(previewData.moduleType) {

      case KonstruktorModuleType.Vector:
        vectorInfoElem.SetActive(true);
        break;

      case KonstruktorModuleType.ForceGraphical:
        forceInfoElem.SetActive(true);
        break;

      case KonstruktorModuleType.ReplacementModel:
        replacementModelInfoElem.SetActive(true);
        break;

      default:
        string[] formulaElements = previewData.SplitFormulaBySpace();
        if(formulaElements.Length == 0) {
          Debug.Log("there is no formula specified for constructor: " + previewData.title);
        }

        CreateModuleFormulaPreview(formulaElements);
        formulaController.gameObject.SetActive(true);
        break;
    }

    modulePreview.SetActive(true);
  }

  private void CreateSolutionModuleSuggestions(TaskDataSO currentTask) {

    List<KonstructorModuleSO> specifiedModules = moduleListsForTasks.GetModulesOfTask(currentTask);
    if(specifiedModules == null)
      specifiedModules = GetUnlockedModules();

    //add required modules if they are not in the list already  
    TaskDataSO.SolutionStep[] steps = GameController.GetInstance().gameState.konstruktorSceneData.taskData.steps;
    for(int i = 0; i<steps.Length; i++)
    {
      TaskOutputVariableUnit outputUnit = steps[i].output.unit;
      VariableInfoSO info = GameController.GetInstance().gameAssets.variableInfo;
      CalculatorEnum requiredCalculatorType = info.GetInfoFor(outputUnit).calculator;
      foreach(KonstructorModuleSO availableModule in availableModules)
      {
        if(availableModule.calculator == requiredCalculatorType && specifiedModules.Contains(availableModule) == false)
          specifiedModules.Add(availableModule);
      }
    }


    foreach(KonstructorModuleSO module in specifiedModules) {
      GameObject moduleRef = Instantiate(solutionModuleEntryPrefab, solutionModuleContainer);
      SolutionModuleEntryController moduleController = moduleRef.GetComponent<SolutionModuleEntryController>();
      moduleController.SetData(module, this);
      moduleController.UpdateButtonContent();

      SingleMenuButton buttonRef = moduleRef.GetComponent<SingleMenuButton>();
      buttonRef.buttonGroup = moduleButtonGroup;
      moduleButtonGroup.menuButtons.Add(buttonRef);
    }

    if(modulesScrollbar != null) {

      modulesScrollbar.value = 1f;
    }
  }

  private void CreateModuleFormulaPreview(string[] elements) {
    formulaController.ClearFormulaContainer();

    for(int i = 0; i < elements.Length; i++) {
      formulaController.CreateFormulaEntry(elements[i]);
    }
  }

  private List<KonstructorModuleSO> GetUnlockedModules()
  {

    GameState saveData = GameController.GetInstance().gameState;
    List<KonstructorModuleSO> unlockedModules = new List<KonstructorModuleSO>();
    foreach(KonstructorModuleSO module in availableModules)
    {
      if(module.connectedTopic == null || module.connectedTopic.IsUnlocked())
      {
        unlockedModules.Add(module);
      }
    }
    return unlockedModules;
  }

  private void HandleGlossaryLink(PointerEventData data, string linkId, PointerEventType eventType) {

    if(eventType == PointerEventType.Click) {

      string entryName = linkId.Split(':')[1].Replace('_', ' ');
      glossary.transform.parent.gameObject.SetActive(true);
      glossary.ShowSingleEntry(entryName);
    }
  }

  public void OpenKonstruktor() {

    ModuleSolutionEnum solutionType = ModuleTypeHelper.GetSolutionType(currentModuleSO);

    switch(solutionType) {
      case ModuleSolutionEnum.Calculate:
        InstantiateAndInitialize<ConstructorManager>(konstruktor, currentModuleSO);
        break;
      case ModuleSolutionEnum.Draw:
        InstantiateAndInitialize<KonstruktorDrawController>(drawController, currentModuleSO);
        break;
      case ModuleSolutionEnum.Place:
        InstantiateAndInitialize<KonstruktorClassificationController>(classificationController, currentModuleSO);
        break;
    }
  }

  private void InstantiateAndInitialize<T>(T handler, KonstructorModuleSO module) where T : StepControllerAbstract {

    T clone = Instantiate(handler, uiContainer);
    clone.name = clone.name + " " + module.name;

    panelManager.Add(clone.gameObject, showOnModuleInstanceShow, showOnModuleInstanceHide);
    clone.SetupAndOpen(module);

    multistepController.AddController(clone, module);

    StepByStepExplanationController tutorialManager = clone.GetComponent<StepByStepExplanationController>();
    //Start tutorial script 
    tutorialManager.ActivateTutorialOfModule(module);
  }

  public string GetKonstruktorName() {
    string theName = currentModuleSO.title;
    return theName;
  }
}
