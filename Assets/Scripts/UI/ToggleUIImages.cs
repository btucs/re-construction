#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[RequireComponent(typeof (Image))]
public class ToggleUIImages : MonoBehaviour
{
  [Required]
 	public Sprite changeImage;

 	private Sprite startImage;

  private bool useStartImage = true;

  private bool startCalled = false;
  private Image imageComponent;

 	void Start() {

    if(startCalled == false) {

      imageComponent = GetComponent<Image>();
      startImage = imageComponent.sprite;
      startCalled = true;
    }
 	}

  public void ChangeUiImage() {

    if(startCalled == false) {

      Start();
    }

    imageComponent.sprite = (useStartImage == true) ? startImage : changeImage;
    useStartImage = !useStartImage;
  }

  public void ChangeUiImage(bool useStartImage) {

    this.useStartImage = useStartImage;
    ChangeUiImage();
  }
}
