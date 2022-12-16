#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using UnityEngine.UI;

public class OperatorController : MonoBehaviour
{
  public Sprite multiplicationIcon;
  public Sprite subtractionIcon;
  public Sprite additionIcon;
  public Sprite divisionIcon;

  public Image iconHolder;

  private Vector2 defaultDimensions = Vector2.zero;
  private RectTransform iconHolderTransform;  

  public void DisplayMultiplicationIcon() {
    iconHolder.sprite = multiplicationIcon;
    secureDefaultSize();
    iconHolderTransform.sizeDelta = new Vector2(8, 8);
  }

  public void DisplaySubstractionIcon() {
    iconHolder.sprite = subtractionIcon;
    secureDefaultSize();
    iconHolderTransform.sizeDelta = defaultDimensions;
  }

  public void DisplayAdditionIcon() {
    iconHolder.sprite = additionIcon;
    secureDefaultSize();
    iconHolderTransform.sizeDelta = defaultDimensions;
  }

  public void DisplayDivisionIcon() {
    iconHolder.sprite = divisionIcon;
    secureDefaultSize();
    iconHolderTransform.sizeDelta = defaultDimensions;
  }

  private void secureDefaultSize() {

    if(defaultDimensions == Vector2.zero) {

      iconHolderTransform = iconHolder.transform as RectTransform;
      defaultDimensions = iconHolderTransform.sizeDelta;
    }
  }
}
