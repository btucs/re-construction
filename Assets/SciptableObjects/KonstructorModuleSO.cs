#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MathUnits.Physics.Values;

public enum KonstruktorModuleType {
  Placeholder = 0,
  Vector = 1,
  Input = 2,
  Cosine = 3,
  Pythagoras = 4,
  Force = 5,
  ForceGraphical = 6,
  ReplacementModel = 7,
  FreeCut = 8,
  Equilibrium = 9,
  LineVolatility = 10,
  Interaction = 11,
}

[CreateAssetMenu(menuName = "KonstruktorModule")]
public class KonstructorModuleSO : ScriptableObject, UIDSearchableInterface, IEquatable<KonstructorModuleSO> {

  [SerializeField, ReadOnly]
  private string uid;
  [LabelWidth(100)]
  public KonstruktorModuleType moduleType;
  [LabelWidth(100)]
  public CalculatorEnum calculator;
  [LabelWidth(100)]
  public string title;
  [MultiLineProperty(5), LabelWidth(100), OnValueChanged("CreateTmpDescription")]
  public string description;
  [LabelWidth(100)]
  public string formula;

  [HideInInspector]
  public string tmpDescription;

  public TopicSO connectedTopic;
  
  private Tuple<string, CalculatorParameterType>[] inputParameters;

  public string[] SplitFormulaBySpace()
  {
    string[] splitArray = formula.Split(' ');
    return splitArray;
  }

  public string UID {
    get {
      return uid;
    }
  }

  public bool Equals(KonstructorModuleSO other)
  {
    return (this.uid == other.UID);
  }

  private void OnValidate() {
#if UNITY_EDITOR
    if(uid == "" || uid == null) {
      uid = Guid.NewGuid().ToString();
      UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
  }

  private void CreateTmpDescription() {

    TextMeshProRendererFactory factory = new TextMeshProRendererFactory();

    tmpDescription = factory.RenderMarkdownStringToTextMeshProString(description);
  }
}
