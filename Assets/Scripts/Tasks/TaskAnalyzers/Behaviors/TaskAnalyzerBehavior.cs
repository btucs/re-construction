#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskAnalyzerBehavior : MonoBehaviour {
  
  public string[] successMessage;
  public TriggerMessage[] failureMessages;

  public abstract TaskAnalyzerAbstract GetAnalyzer();

  protected void SetMessages(TaskAnalyzerAbstract analyzer) {

    if (successMessage.Length > 0) {

      analyzer.SetSuccessMessage(successMessage);
    }

    if (failureMessages.Length > 0) {

      analyzer.SetFailureMessages(failureMessages);
    }
  }
}
