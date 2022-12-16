#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[Serializable]
public class AnswerSelectedEvent : UnityEvent<MLEAnswerController, MLEQuizChoice> {
  
}

public class MLEAnswerController : MonoBehaviour
{
  public GameObject selectedMarker;
  public Text choiceText;

  public bool IsSelected {
    get; private set;
  } = false;

  public AnswerSelectedEvent OnSelect = new AnswerSelectedEvent();

  private MLEQuizChoice connectedAnswer;

  public void SetChoice(MLEQuizChoice choice) {

    connectedAnswer = choice;
    choiceText.text = choice.name;
  }

  public void MarkSelected(bool isSelected = true) {

    IsSelected = isSelected;
    selectedMarker.SetActive(isSelected);
  }

  public void ToggleAnswerState() {

    IsSelected = !IsSelected;
    selectedMarker.SetActive(IsSelected);

    OnSelect.Invoke(this, connectedAnswer);
  }
}
