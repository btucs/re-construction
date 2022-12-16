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
using Sirenix.OdinInspector;
using FullSerializer;

[Serializable]
[fsObject(Converter = typeof(KonstruktorSceneDataConverter))]
public class KonstruktorSceneData {

  [AssetsOnly]
  public GameObject backgroundPrefab;
  public InteractableData[] interactablesPrefabs = new InteractableData[0];
  public TaskDataSO taskData;
  public TaskObjectSO taskObject;
  public Vector3 cameraPosition;
  public float cameraZoomFactor = 1f;
  public string returnSceneName;
  public float npcScale = 1f;
  public MLEQuiz[] followUpQuestions;

  // collectted inputs and outputs
  public MathMagnitude[] inputs = new MathMagnitude[0];
  public MathMagnitude[] outputs = new MathMagnitude[0];

  public ConverterResultData[] converterResults = new ConverterResultData[0];
  public int currentStep = -1;

  [Serializable]
  [fsObject(Converter = typeof(InteractableDataConverter))]
  public class InteractableData
  {

    [AssetsOnly]
    public TaskObjectSO taskObject;
    public Vector3 position;
  }
  
  public void Reset() {

    inputs = null;
    outputs = null;
    converterResults = null;
    currentStep = -1;
  }

  public void AddResult(ConverterResultData result) {

    if(converterResults == null) {

      converterResults = new ConverterResultData[1] {
        result
      };

      return;
    }

    /*int resultIndex = Array.FindIndex(converterResults, (ConverterResultData item) => item.step == result.step);
    if(resultIndex != -1) {

      converterResults[resultIndex] = result;
    } else {
    */
      converterResults = converterResults.Append(result).ToArray();
    //}
  }

  public ConverterResultData FindResultFor(TaskDataSO.SolutionStep solutionStep) {

    TaskOutputVariable output = solutionStep.output;

    return FindResultFor(output);
  }

  public ConverterResultData FindResultFor(TaskOutputVariable output) {

    if(converterResults == null) {

      return null;
    }

    ConverterResultData found = converterResults.FirstOrDefault((ConverterResultData result) => result.resultFor != null && result.resultFor.Equals(output));

    return found;
  }

  public ConverterResultData FindResultFor(TaskInputVariable resultInput) {

    if(converterResults == null) {

      return null;
    }

    return converterResults.FirstOrDefault((ConverterResultData result) => {

      if(result == null || result.calculatorResult == null) {

        return false;
      }

      TaskInputVariable input = result.calculatorResult.Value as TaskInputVariable;

      return input.Equals(resultInput);
    });
  }

  public ConverterResultData FindResultFor(MathMagnitude magnitude) {

    if(converterResults == null) {

      return null;
    }

    return converterResults.FirstOrDefault((ConverterResultData result) => {

      if(result == null || result.calculatorResult == null) {

        return false;
      }

      bool magnitudeEquals = magnitude.replacementModelMapping.SequenceEqual(result.calculatorResult.replacementModelMapping);

      return magnitudeEquals && result.calculatorResult.Value == magnitude.Value;
    });
  }
}
