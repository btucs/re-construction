#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReplacementModelPopupButtonEvent : UnityEvent<ReplacementModelType> {
}

public class ReplacementModelPopupController : MonoBehaviour {

  public ReplacementModelPopupButtonEvent onClick = new ReplacementModelPopupButtonEvent();

  public Button ropeButton;
  public Button rodButton;
  public Button beamButton;
  public Button massButton;

  public void ShowPopup() {

    gameObject.SetActive(true);
  }

  public void ClosePopup() {

    gameObject.SetActive(false);
  }

  private void Start() {

    ropeButton.onClick.AddListener(() => InvokeEventAndClose(ReplacementModelType.Rope));
    rodButton.onClick.AddListener(() => InvokeEventAndClose(ReplacementModelType.Rod));
    beamButton.onClick.AddListener(() => InvokeEventAndClose(ReplacementModelType.Beam));
    massButton.onClick.AddListener(() => InvokeEventAndClose(ReplacementModelType.Mass));
  }

  private void InvokeEventAndClose(ReplacementModelType type) {

    onClick.Invoke(type);
    ClosePopup();
  }

  private void OnDisable() {

    onClick.RemoveAllListeners();
  }
}
