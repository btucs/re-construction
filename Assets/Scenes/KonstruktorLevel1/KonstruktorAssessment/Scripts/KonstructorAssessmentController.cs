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
using Sirenix.OdinInspector;
using TMPro;

public class KonstructorAssessmentController : MonoBehaviour {

  [Required]
  public Text EPRewardText;

  [Required]
  public Text CoinRewardText;

  [Required]
  public Text HeadlineText;

  [Required]
  public GameObject ContinuePanel;
  [Required]
  public GameObject RepeatPanel;

  [Required]
  public AssessmentScriptWrapper[] assessments;

  [Required]
  public OutputMenuController outputMenuController;

  [Required]
  public RectTransform assessmentContainer;
  [Required]
  public GameObject stepHeaderTemplate;

  public TaskAnalyticsHandler taskAnalyticsHandler;

  public GameObject rewardDataTransporterPrefab;

  private int repeatCounter = 0;

  private GameController controller;

  private void Start() {

    controller = GameController.GetInstance();

    DisplayResult();

    if(RepeatPanel != null) {

      RepeatPanel.GetComponentInChildren<Button>().onClick.AddListener(() => repeatCounter++);
    }
  }

  private void OnEnable() {
    // if controller is not available, Start has not been called yet
    if(controller != null) {

      DisplayResult();
    }
  }

  public void DisplayResult() {

    KonstruktorSceneData data = controller.gameState.konstruktorSceneData;
    HeadlineText.text = data.taskData.taskName + ": Auswertung";

    Tuple<int, int> totalEP = CalculateTotalEP(data);
    float successRate = 0;

    try {

      successRate = totalEP.Item1 / totalEP.Item2;
    } catch(DivideByZeroException) {

      // do nothing, since it successRate stays 0
    }

    EPRewardText.text = totalEP.Item1.ToString("+ #;- #;0");
    CoinRewardText.text = (totalEP.Item1 * 20).ToString();

    GameObject rewardDataInstance = Instantiate(rewardDataTransporterPrefab);
    rewardDataInstance.GetComponent<RewardNotificationData>().currencyGained = (totalEP.Item1 * 20);
    rewardDataInstance.GetComponent<RewardNotificationData>().expGained = totalEP.Item1;
    DontDestroyOnLoad(rewardDataInstance);

    FinishedTaskData finishedTaskData = new FinishedTaskData(data.taskData, data.taskObject, totalEP.Item1);
    if(taskAnalyticsHandler != null)
    {
      taskAnalyticsHandler.SetRepetitions(repeatCounter);
      taskAnalyticsHandler.SetCurrentPoints(totalEP.Item1);
    }

    if(successRate < 0.6f && repeatCounter < 2) {
      ContinuePanel.SetActive(false);
      RepeatPanel.SetActive(true);
    } else {

      RepeatPanel.SetActive(false);
      ContinuePanel.SetActive(true);
    }

    controller.gameState.profileData.inventory.currencyAmount += (totalEP.Item1 * 20);
    if(data.returnSceneName == "TeachAndPlayScene")
    {
      controller.gameState.taskHistoryData.courseHistoryData.Add(finishedTaskData);
    } else {
      controller.gameState.taskHistoryData.taskHistoryData.Add(finishedTaskData);
    }
    controller.SaveGame();
  }

  private Tuple<int, int> CalculateTotalEP(KonstruktorSceneData data) {

    TaskDataSO.SolutionStep[] steps = data.taskData.steps;
    ConverterResultData[] results = data.converterResults;
    HelperFunctions.DestroyChildren(assessmentContainer);
    GameAssetsSO gameAssets = controller.gameAssets;

    /*if(results == null || results.Length == 0) {

      return Tuple.Create(0, 0);
    }*/

    int stepCounter = 0;
    return steps.Aggregate(Tuple.Create(0, 0), (Tuple<int, int> innerTotal, TaskDataSO.SolutionStep solutionStep) => {

      TaskOutputVariableUnit expectedUnit = solutionStep.output.unit;
      ConverterResultData result = data.FindResultFor(solutionStep);
      OutputMenuItemController foundOutput = outputMenuController.FindFor(solutionStep);
      
      AssessmentType[] responsibleTypes = AssessmentFactory.GetResponsibleAssessmentTypes();

      if(result != null && foundOutput != null && foundOutput.droppedItem.hasResult == true) {

        responsibleTypes = AssessmentFactory.GetResponsibleAssessmentTypes(expectedUnit);
      }

      GameObject stepHeader = Instantiate(stepHeaderTemplate, assessmentContainer);
      stepHeader.SetActive(true);
      TMP_Text headlineElem = stepHeader.GetComponentInChildren<TMP_Text>();
      

      string moduleName = "Kein Wert zugewiesen";
      if(result != null) {

        KonstructorModuleSO module = gameAssets.FindKonstructorModule(result.moduleType);

        if(module != null) {

          moduleName = module.title + " " + (stepCounter + 1);
        }
      }

      headlineElem.text = "Modul: " + moduleName + ", ges. " + solutionStep.output.textMeshProName;
      stepCounter++;

      return assessments.Aggregate(innerTotal, (Tuple<int, int> total, AssessmentScriptWrapper assessment) => {

        if(responsibleTypes.Contains(assessment.assessmentType)) {

          AssessmentScriptWrapper clone = Instantiate(assessment, assessmentContainer);
          AssessmentTaskStepEntry step = clone.GetComponent<AssessmentTaskStepEntry>();
          
          clone.Configure(solutionStep, result);
          int maxPoints = step.maxPoints = clone.MaxPoints;
          int currentPoints = step.currentPoints = clone.CurrentPoints;
          step.SetFeedback(clone.GetFeedback());
          step.UpdateTextFields();
          step.UpdateStateIcon();

          return Tuple.Create(total.Item1 + currentPoints, total.Item2 + maxPoints);
        }

        assessment.gameObject.SetActive(false);

        return total;
      });
    });
  }
}
