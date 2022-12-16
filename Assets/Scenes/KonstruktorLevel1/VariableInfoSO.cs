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

public class VariableInfoSO : ScriptableObject {

  public List<VariableIconInfo> iconInfos = new List<VariableIconInfo>();

  [ListDrawerSettings(Expanded = true, IsReadOnly = true)]
  public VariableInfoEntry[] entries;
  
  public VariableInfoEntry GetInfoFor(TaskOutputVariableUnit unit) {

    return entries.FirstOrDefault((VariableInfoEntry entry) => entry.variableType == unit);
  }

  [Button]
  private void Populate() {

    TaskOutputVariableUnit[] values = ((TaskOutputVariableUnit[])Enum.GetValues(typeof(TaskOutputVariableUnit)))
      .Where((TaskOutputVariableUnit unit) => unit != TaskOutputVariableUnit.Unspecified)
      .ToArray()
    ;
    
    List<VariableInfoEntry> tmp = new List<VariableInfoEntry>();
    foreach(TaskOutputVariableUnit value in values) {

      VariableInfoEntry found = entries.FirstOrDefault((VariableInfoEntry entry) => entry.variableType == value);
      if(found != null) {

        tmp.Add(found);
      } else {

        VariableInfoEntry newEntry = new VariableInfoEntry() {
          variableType = value
        };

        tmp.Add(newEntry);
      }
    }

    entries = tmp
      .OrderBy(
        (VariableInfoEntry entry) => VariableHelper.Translate(entry.variableType)
      )
      .ToArray()
    ;
  }

  [Serializable]
  public class VariableInfoEntry {

    [HideInInspector]
    public TaskOutputVariableUnit variableType;

    [Title("@VariableHelper.Translate(this.variableType)")]
    [LabelText("Beschreibung")]
    [MultiLineProperty(5), OnValueChanged("CreateTmpDescription")]
    public string description;

    [HideInInspector]
    public string tmpDescription;

    [LabelText("MLE")]
    public MLEDataSO mle;

    [LabelText("Glossareinträge")]
    [ListDrawerSettings(Expanded = true)]
    public string[] glossaryEntries = new string[0];

    [LabelText("Erwarteter Umwandler")]
    public CalculatorEnum calculator = CalculatorEnum.None;

    [LabelText("Linienart (wenn nötig)")]
    public LineUISO line;

    [LabelText("Formel (wenn anwendbar)"), MultiLineProperty(3)]
    public string formula;

    private void CreateTmpDescription()
    {

        TextMeshProRendererFactory factory = new TextMeshProRendererFactory();

        tmpDescription = factory.RenderMarkdownStringToTextMeshProString(description);
    }
    }

  public Sprite GetVariableIcon(MathMagnitude.MathVariableType varType)
  {
    Sprite varIcon = null;

    foreach(VariableIconInfo iconInfo in iconInfos)
    {
      if(iconInfo.type == varType)
        varIcon = iconInfo.icon;
    }

    return varIcon;
  }

  [Serializable]
  public class VariableIconInfo{
    public MathMagnitude.MathVariableType type;
    public Sprite icon;
  }
}
