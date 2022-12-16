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
using TMPro;
using Sirenix.OdinInspector;
using UniRx;
using Markdig.TextMeshPro.Extensions.TaskDialog;

public class TaskDialogHandler : MonoBehaviour {

  [SceneObjectsOnly]
  [Required]
  public Image npcImage;
  [SceneObjectsOnly]
  [Required]
  public TextMeshProUGUI npcName;
  [SceneObjectsOnly]
  [Required]
  public TextMeshProUGUI npcMessage;
  [SceneObjectsOnly]
  [Required]
  public GameObject taskDialogContainer;
  [SceneObjectsOnly]
  [Required]
  public Button nextMessageButton;
  [SceneObjectsOnly]
  [Required]
  public Button closeButton;

  [Required]
  public TaskNPCReactiveProperty taskNPC = new TaskNPCReactiveProperty();

  [Required]
  public TaskDataSOReactiveProperty taskData = new TaskDataSOReactiveProperty();

  private int currentDialogIndex = 0;
  
  // save currentNPC extra. It's bad practice to use taskNPC.Value
  private TaskNPC currenttaskNPC;
  private string[] currentDialogs;
  private IEnumerator currentDialogsEnumerator;
  private TaskDialogMarkdigFactory factory;
  
  private void Start() {

    taskDialogContainer.SetActive(false);

    IObservable<TaskDataSO> taskDataObservable = taskData
      .Where((TaskDataSO data) => data != null)
      .Do((TaskDataSO taskData) => {

        TaskDataSO.SolutionStep[] steps = taskData.steps;
        List<TaskInputVariable> inputs = new List<TaskInputVariable>();
        List<TaskOutputVariable> outputs = new List<TaskOutputVariable>();

        List<TaskInputVariable> dummyInputs = new List<TaskInputVariable>();

        foreach(TaskDataSO.SolutionStep step in steps) {

          if(step.inputs.Length > 0) {

            inputs.AddRange(step.inputs);
          }

          if(step.output != null) {

            outputs.Add(step.output);
          }

          if(step.dummyInputs.Length > 0) {
            dummyInputs.AddRange(step.dummyInputs);
          }
        }

        TaskDialogOptions options = new TaskDialogOptions() {
          taskInputs = inputs.ToArray(),
          taskOutputs = outputs.ToArray(),
          taskDummyInputs = dummyInputs.ToArray()
        };

        factory = new TaskDialogMarkdigFactory(ref options);
        RenderInputAndOutputVariables(ref options, factory);
      })
    ;

    taskNPC
      .Where((TaskNPC npc) => npc != null)
      .DistinctUntilChanged()
      .CombineLatest<TaskNPC, TaskDataSO, TaskNPC>(taskDataObservable, (TaskNPC npc, TaskDataSO taskData) => npc)
      .Do(HandleNewNPC)
      .Subscribe()
      .AddTo(this)
    ;
  }

  private void HandleNewNPC(TaskNPC taskNPC) {

    currenttaskNPC = taskNPC;
   
    currentDialogs = factory.RenderStringToTextMeshPro(taskNPC.dialogs);
    currentDialogsEnumerator = currentDialogs.GetEnumerator();
    
    npcName.text = taskNPC.npc.characterName;
    npcImage.sprite = taskNPC.npc.thumbnail;
  }

  public void Show() {

    taskDialogContainer.SetActive(true);
    nextMessageButton.onClick.RemoveAllListeners();
    if(currentDialogs.Length > 1) {

      nextMessageButton.gameObject.SetActive(true);
      nextMessageButton.onClick.AddListener(ShowNextDialog);

      closeButton.gameObject.SetActive(false);
    } else {

      nextMessageButton.gameObject.SetActive(false);
      closeButton.gameObject.SetActive(true);
    }

    currentDialogsEnumerator.Reset();
    currentDialogIndex = 0;
    
    currentDialogsEnumerator.MoveNext();
    npcMessage.text = (string)currentDialogsEnumerator.Current;
  }

  private void ShowNextDialog() {

    if(currentDialogIndex + 1 < currenttaskNPC.dialogs.Length) {

      currentDialogIndex++;
      currentDialogsEnumerator.MoveNext();

      npcMessage.text = (string) currentDialogsEnumerator.Current;

      if(currentDialogIndex == currenttaskNPC.dialogs.Length - 1) {

        nextMessageButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(true);
      }
    }
  }

  private static void RenderInputAndOutputVariables(ref TaskDialogOptions options, TaskDialogMarkdigFactory factory) {

    options.taskInputs = options.taskInputs.Select((TaskInputVariable input) => {

      string symbolizedName = SymbolHelper.GetSymbol(input.name);
      input.textMeshProName = factory.RenderStringToTextMeshPro(symbolizedName).TrimEnd('\n');
      input.textMeshProValue = factory.RenderStringToTextMeshPro(input.textValue).TrimEnd('\n');

      return input;
    }).ToArray();

    options.taskOutputs = options.taskOutputs.Select((TaskOutputVariable output) => {

      output.textMeshProName = factory.RenderStringToTextMeshPro(output.name).TrimEnd('\n');

      return output;
    }).ToArray();


    options.taskDummyInputs = options.taskDummyInputs.Select((TaskInputVariable dummy) => {

      string symbolizedName = SymbolHelper.GetSymbol(dummy.name);
      dummy.textMeshProName = factory.RenderStringToTextMeshPro(symbolizedName).TrimEnd('\n');
      dummy.textMeshProValue = factory.RenderStringToTextMeshPro(dummy.textValue).TrimEnd('\n');

      return dummy;
    }).ToArray();
  }
}
