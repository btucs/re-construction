#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class NextButton: MonoBehaviour {

  public Sprite hasNextSprite;
  public Sprite noMoreSprite;

  private bool hasNext = true;

  private void OnEnable() {

    hasNext = MonologManager.instance.HasNextMessage();
    GetComponent<Image>().sprite = hasNext ? hasNextSprite : noMoreSprite;
  }

  public void OnClick() {

    hasNext = MonologManager.instance.NextMessage();

    if (hasNext == false) {

      GetComponent<Image>().sprite = noMoreSprite;
    }
  }
}