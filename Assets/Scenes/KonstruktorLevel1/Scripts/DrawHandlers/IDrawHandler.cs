#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct DrawVariableData
{
  public String name;
  public CalculatorParameterType expectedType;
  public bool disableTap;
  public String valHeadline;
  public String valDescription;
}

public struct DrawResultData
{
  public String name;
  public TaskOutputVariableUnit expectedType;
}

public interface IDrawHandler {

  DrawVariableData[] GetInputs();
  DrawResultData[] GetOutputs();

  void Initialize();
  void Terminate();
  //KonstructorDrawEntityAbstract[] GetDrawnItems();

  void TryActivateDrawButton();
  /// <summary>
  /// Multiplier to display coordinates corresponding to the coordinate system
  /// </summary>
  /// <param name="multiplier"></param>
  void SetStepMultiplier(float multiplier);
}
