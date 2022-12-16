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

public abstract class TaskAnalyzerAbstract {

  protected Task task;

  protected string[] successMessage;
  protected TriggerMessage[] failureMessages;

  protected TriggerMessage currentMessage;

  protected abstract bool Analyze();

  public void SetSuccessMessage(string[] message) {

    this.successMessage = message;
  }

  public void SetFailureMessages(TriggerMessage[] messages) {

    this.failureMessages = messages;
  }

  public void SetTask(Task task) {

    this.task = task;
  }

  public TaskAnalyzerResultAbstract GetResult() {

    bool result = Analyze();

    if (result == true) {

      return new PositiveTaskAnalyzerResult(successMessage);
    }

    if (currentMessage == null) {

      TriggerMessage message = FindMessage("FailureMessage");
      if (message != null) {

        return new NegativeTaskAnalyzerResult(message.message);
      }

      throw new Exception("No default \"FailureMessage\" trigger was set");
    }

    NegativeTaskAnalyzerResult response = new NegativeTaskAnalyzerResult(currentMessage.message);
    currentMessage = null;

    return response;
  }

  protected TriggerMessage FindMessage(string trigger) {

    return failureMessages.FirstOrDefault((TriggerMessage message) => message.trigger == trigger);
  }
}
