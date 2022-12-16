#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MathUnits.Physics;
using MathUnits.Physics.Values;
using FullSerializer;

public enum TaskVariableType {
  Scalar,
  Vector
}

[Serializable]
public abstract class TaskVariable {

  [LabelText("Variablenname")]
  [InfoBox(@"Sonderzeichen der Winkel müssen als Text ausgeschrieben werden (alpha, beta, gamma, delta), sie werden dann bei der Darstellung automatisch ersetzt.
In den NPC Dialogen werden Sie aber über Ihren Text-Wert verlinkt z.B. ~i:alpha~
")]
  public string name;

  [LabelText("Symbol")]
  public Sprite icon;

  [HideInInspector]
  [fsIgnore]
  public string textMeshProName;

  public override string ToString() {

    return name;
  }
}

[Serializable]
public class TaskInputVariable: TaskVariable, IEquatable<TaskInputVariable>
{

  [LabelText("Typ")]
  [InfoBox("Der Typ Vector kann für kann für Punkte und Vektoren verwendet werden. Bei Vektoren sollte ein Startpunkt angegeben werden")]
  public TaskVariableType type;
  [InfoBox(@"Dezimalwerte müssen mit '.' anstatt mit ',' geschrieben werden.
Winkel können mit ° angegeben werden.
Vektoren werden im Format '(x y z)' geschrieben zusätzlich muss der Typ auf 'Vector' umgestellt werden. Für 2D Vektoren ist z=0 zu setzen.
")]
  [InfoBox("$parseError", "IsNotValidValue", InfoMessageType = InfoMessageType.Error)]
  [LabelText("Wert")]
  public string textValue;
  [InfoBox(@"Dezimalwerte müssen mit '.' anstatt mit ',' geschrieben werden.
Vektoren werden im Format '(x y z)' geschrieben. Für 2D Vektoren ist z=0 zu setzen.
")]
  [InfoBox("$parseError", "IsNotValidValue", InfoMessageType = InfoMessageType.Error)]
  [LabelText("StartPunkt"), ShowIf("type", TaskVariableType.Vector)]
  public string startPointText;
  [LabelText("Typ im Ersatzmodell")]
  [ValueDropdown("@VariableHelper.GetReplacementModelTypeTranslation()")]
  public ReplacementModelType replacementModelType = ReplacementModelType.None;
  [LabelText("Beschreibung")]
  public string shortDescription;
  [LabelText("Minidefinition")]
  public string definition;
  
  public ScalarValue GetScalarValue() {
    
    if(String.IsNullOrEmpty(textValue) || type != TaskVariableType.Scalar) {

      return CalculationHelper.CreateScalarError();
    }

    ScalarValue.TryParse(textValue, CultureInfo.InvariantCulture, out ScalarValue parsedScalar, out parseError);

    return parsedScalar;
  }

  public void SetScalarValue(ScalarValue value) {

    textValue = value.ToString(CultureInfo.InvariantCulture);
    type = TaskVariableType.Scalar;
  }

  public VectorValue GetVectorValue() {

    if(String.IsNullOrEmpty(textValue) || type != TaskVariableType.Vector) {

      return CalculationHelper.CreateVectorError();
    }

    VectorValue.TryParse(textValue, CultureInfo.InvariantCulture, out VectorValue parsedVector, out parseError);

    return parsedVector;
  }

  public VectorValue GetStartPoint() {

    if(String.IsNullOrEmpty(startPointText) || type != TaskVariableType.Vector) {

      return CalculationHelper.CreateVectorError();
    }

    VectorValue.TryParse(startPointText, CultureInfo.InvariantCulture, out VectorValue parsedVector, out parseError);

    return parsedVector;
  }

  public void SetVectorValue(VectorValue value) {

    textValue = value.ToString(CultureInfo.InvariantCulture);
    type = TaskVariableType.Vector;
  }

  public void SetVectorValue(VectorValue value, VectorValue startPoint) {

    textValue = value.ToString(CultureInfo.InvariantCulture);
    type = TaskVariableType.Vector;
    startPointText = startPointText.ToString(CultureInfo.InvariantCulture);
  }

  [HideInInspector]
  [fsIgnore]
  public string textMeshProValue;

  public bool Equals(TaskInputVariable other) {

    return textValue == other.textValue && name == other.name;
  }

  public override bool Equals(object obj) {

    if(obj == null)
      return false;

    TaskInputVariable input = obj as TaskInputVariable;
    if(input == null) {

      return false;
    }

    return Equals(input);
  }

  public override int GetHashCode() {

    return HashCode.Of(textValue).And(name);
  }

  public TaskInputVariable Clone() {

    return new TaskInputVariable() {
      name = name,
      textValue = textValue,
      type = type,
      replacementModelType = replacementModelType,
      shortDescription = shortDescription,
      definition = definition,
    };
  }

  public override string ToString() {

    return SymbolHelper.GetSymbol(name) + " = " + textValue;
  }

  #region OdinInspector
  private string parseError;
  // since IsNotScalar is running on EditorUpdate we need a separate variable to not check every frame
  private string tmpValue;

  private bool IsNotValidValue(string value) {

    if(tmpValue == value) {
      // return current error if nothing is changed
      return parseError != null && parseError.Length > 0;
    }

    if(value == null || value.Length == 0) {

      tmpValue = value;

      return false;
    }

    if(type == TaskVariableType.Scalar) {

      return IsNotScalar(value);
    }

    return IsNotVector(value);
  }

  private bool IsNotVector(string value) {

    VectorValue.TryParse(value, out VectorValue parsed, out parseError);
    tmpValue = value;

    return parseError != null && parseError.Length > 0;
  }

  private bool IsNotScalar(string value) {

    ScalarValue.TryParse(value, CultureInfo.InvariantCulture, out ScalarValue parsed, out parseError);
    tmpValue = value;

    return parseError != null && parseError.Length > 0;
  }
  #endregion
}

public enum TaskOutputVariableUnit
{
  Force,
  ForceVector, // Kraftpfeil angeben
  ForceVectorGG, // Kraftpfeil Geleichgewicht
  ForceVectorWW, // Kraftpfeil Wechselwirkung
  ForceVectorFL, // Kraftpfeile Linienfl.
  Vector,
  ReplacementModel, // Ersatzmodell
  SeiteL,
  Hypotenuse,
  WeightForce,
  ForceComponentXA, // Kraftkomponente in X analytisch
  ForceComponentXG, // Kraftkomponente in X grafisch
  ForceComponentYA, // Kraftkomponente in Y analytisch
  ForceComponentYG, // Kraftkomponente in Y grafisch
  SubstituteForceA, // Ersatzkraft analytisch
  SubstituteForceG, // Ersatzkraft grafisch
  SubstituteForceAngle,
  ResultingForceA, // Resultierende analytisch
  ResultingForceG, // Resultierende grafisch
  ResultingForceAngle,
  // Don't use directly, used as default value in InputOutputDrawController
  Unspecified,
}

[Serializable]
public class TaskOutputVariable: TaskVariable, IEquatable<TaskOutputVariable>
{
  [LabelText("Gesuchter Wert")]
  [ValueDropdown("@VariableHelper.GetTaskOutVariableUnitTranslation()")]
  public TaskOutputVariableUnit unit;

  [HideInInspector]
  [fsIgnore]
  public string textMeshProUnit;

  [LabelText("Erwarteter Angiffspunkt")]
  [ShowIf("@CanShow(unit)")]
  [InfoBox(@"Dezimalwerte müssen mit '.' anstatt mit ',' geschrieben werden.
Vektoren werden im Format '(x y z)' geschrieben. Für 2D Vektoren ist z=0 zu setzen.
")]
  [InfoBox("$parseError", "IsNotValidValue", InfoMessageType = InfoMessageType.Error)]
  public string expectedStartingPointText;

  public VectorValue GetExpectedStartingPoint() {

    if(String.IsNullOrEmpty(expectedStartingPointText)) {

      return CalculationHelper.CreateVectorError();
    }

    VectorValue.TryParse(expectedStartingPointText, CultureInfo.InvariantCulture, out VectorValue parsedVector, out parseError);

    return parsedVector;
  }
  
  public bool Equals(TaskOutputVariable other) {

    return unit == other.unit && name == other.name;
  }

  public override bool Equals(object obj) {

    if(obj == null)
      return false;

    TaskOutputVariable input = obj as TaskOutputVariable;
    if(input == null) {

      return false;
    }

    return Equals(input);
  }

  public override int GetHashCode() {

    return HashCode.Of(name).And(unit);
  }

  public TaskOutputVariable Clone() {

    return new TaskOutputVariable() {
      name = name,
      unit = unit,
    };
  }

  #region OdinInspector
  private string parseError;
  // since IsNotScalar is running on EditorUpdate we need a separate variable to not check every frame
  private string tmpValue;

  private bool IsNotValidValue(string value) {

    if(tmpValue == value) {
      // return current error if nothing is changed
      return parseError != null && parseError.Length > 0;
    }

    if(value == null || value.Length == 0) {

      tmpValue = value;

      return false;
    }

    return IsNotVector(value);
  }

  private bool IsNotVector(string value) {

    VectorValue.TryParse(value, out VectorValue parsed, out parseError);
    tmpValue = value;

    return parseError != null && parseError.Length > 0;
  }

  private bool CanShow(TaskOutputVariableUnit unit) {

    switch(unit) {
      case TaskOutputVariableUnit.ForceVectorGG:
      case TaskOutputVariableUnit.ForceVectorFL:
        return true;

      default: return false;
    }
  }
  #endregion
}