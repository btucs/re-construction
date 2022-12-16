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
using Sirenix.OdinInspector;
using TMPro;

public class ConfirmationPopupController : MonoBehaviour {

  [Required]
  public TMP_Text MessageText;
  [Required]
  public Text CancelButtonText;
  [Required]
  public Text ConfirmButtonText;

  [Required]
  public Button CancelButton;
  [Required]
  public Button ConfirmButton;


  public void Show() {

    gameObject.SetActive(true);
  }

  public void Hide() {

    gameObject.SetActive(false);
  }
}
