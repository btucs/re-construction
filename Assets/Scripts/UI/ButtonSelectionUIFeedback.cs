#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectionUIFeedback : MonoBehaviour
{
  public Image buttonBG;
  public Image buttonIcon;
  public Color selectedBGColor, unselectedBGColor, selectedIconColor, unselectedIconColor;

  public List<GameObject> onSelectionElements = new List<GameObject>();
  private bool isSelected = false;

  public void SetSelected(bool newValue) {

    isSelected = newValue;
    buttonBG.color = newValue ? selectedBGColor : unselectedBGColor;
    buttonIcon.color = newValue ? selectedIconColor : unselectedIconColor;

    foreach(GameObject obj in onSelectionElements) {
      obj.SetActive(isSelected);
    }
  }

  public void ToggleSelection() {

    isSelected = !isSelected;

    if(isSelected) {

      buttonBG.color = selectedBGColor;
      buttonIcon.color = selectedIconColor;
    } else {

      buttonBG.color = unselectedBGColor;
      buttonIcon.color = unselectedIconColor;
    }

    foreach(GameObject obj in onSelectionElements) {

      obj.SetActive(isSelected);
    }
  }
}
