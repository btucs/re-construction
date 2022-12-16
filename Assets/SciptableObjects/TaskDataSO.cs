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

[CreateAssetMenu(menuName = "Tasks/TaskData")]
public class TaskDataSO : ScriptableObject, UIDSearchableInterface {

  [SerializeField, ReadOnly]
  private string uid;

  [LabelText("Titel")]
  public string taskName;

  [MultiLineProperty(4)]
  [FoldoutGroup("Aufgabentexte"), LabelText("Beschreibung Standart")]
  public string teaserDescription;

  [MultiLineProperty(4)]
  [FoldoutGroup("Aufgabentexte"), LabelText("Aufg.Beschreibung bei Erfolg")]
  public string teaserSolvedCorrect;

  [MultiLineProperty(4)]
  [FoldoutGroup("Aufgabentexte"), LabelText("Aufg.Beschreibung bei Fehlversuch")]
  public string teaserSolvedWrong;

  [MultiLineProperty(5)]
  [FoldoutGroup("Aufgabentexte"), LabelText("Vollbeschreibung")]
  public string fullDescription;

  [MultiLineProperty(5)]
  [FoldoutGroup("Aufgabentexte"), LabelText("Kursbeschreibung")]
  public string teachAndPlayDescription;

  [LabelText("Themengebiet")]
  public TopicSO topic;

  [LabelText("benötigte Aufgabe")]
  public TaskDataSO requiredTask;

  [LabelText("Aufgabentyp")]
  [ValueDropdown("@TaskTypeEnumHelper.GetTaskTypeEnumTranslation()")]
  public TaskTypeEnum taskType;

  [LabelText("Koordinatensystem")]
  public CoordinateSystemData coordinateSystemData = new CoordinateSystemData();

  [LabelText("Bearbeitungsschritte")]
  [ListDrawerSettings(Expanded = true)]
  public SolutionStep[] steps = new SolutionStep[1];

  [LabelText("Aufgabencharaktere")]
  [ListDrawerSettings(Expanded = true)]
  public TaskNPC[] taskNPCs = new TaskNPC[1];

  public List<string> hashtags = new List<string>();

  public TaskObjectSO connectedObject;

  [Serializable]
  public class SolutionStep {

    [LabelText("gegebene Werte")]
    //[ListDrawerSettings(Expanded = true)]
    public TaskInputVariable[] inputs = new TaskInputVariable[1];

    [LabelText("Dummy-Werte")]
    public TaskInputVariable[] dummyInputs = new TaskInputVariable[0];

    [LabelText("gesuchte Werte")]
    //[ListDrawerSettings(Expanded = true)]
    public TaskOutputVariable output;
  }

  public string UID {
    get {
      return uid;
    }
  }

  public TaskInputVariable FindInputVariable(string name) {

    return steps.Aggregate(null, (TaskInputVariable result, SolutionStep step) => result != null ? result : step.inputs.FirstOrDefault((TaskInputVariable input) => input.name == name));
  }

  public TaskOutputVariable FindOutputVariable(string name) {

    return steps.Aggregate(null, (TaskOutputVariable result, SolutionStep step) => result != null ? result : (step.output.name == name ? step.output : null));
  }

  public MLEDataSO GetMLE()
  {
    int stepCount = steps.Length;
    if(stepCount > 0)
    {
      TaskOutputVariableUnit outputUnit = steps[stepCount - 1].output.unit;
      VariableInfoSO.VariableInfoEntry infoEntry = GameController.GetInstance().gameAssets.variableInfo.GetInfoFor(outputUnit);

      if (infoEntry != null && infoEntry.mle != null){
        return infoEntry.mle;
      }
    }

    return null;
  }

  public string GetHashtagText()
  {
    string hashtagText = (topic!=null) ? '#' + topic.name : "#TechnischeMechanik";
    foreach(string hashtag in hashtags)
    {
      hashtagText = hashtagText + ", #" + hashtag;
    }
    return hashtagText;
  }

  [Serializable]
  public class CoordinateSystemData {
    [LabelText("Koordinatenursprung, relativ zum Objekt")]
    public Vector2 origin = Vector2.zero;
    [LabelText("Ausmaße")]
    public GridDimensions dimensions = new GridDimensions() { posX = 10, negX = 10, posY = 10, negY = 10 };
    [LabelText("Längeneinheit")]
    public int unitSize = 50;
    [LabelText("Zwischenschritte pro LE")]
    public int intermediateSteps = 1;
    [LabelText("Maßstab, 1LE =")]
    [SuffixLabel("N")]
    public float scale = 100;
  }

  private void OnValidate() {
#if UNITY_EDITOR
    if(uid == "" || uid == null) {
      uid = Guid.NewGuid().ToString();
      UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
  }
}
