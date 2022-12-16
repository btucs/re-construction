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
using TMPro;
using TexDrawLib;

public class HandoutDisplay : MonoBehaviour
{
  public MLEHandoutsSO.MLEHandoutEntry handout;

  public TextMeshProUGUI titleText;
  public TEXDraw descriptionText;
  public Image pictureImage;

  public bool existsPicture = false;

  void Start()
  {
    titleText.text = handout.title;
    descriptionText.text = handout.description;
    if (existsPicture)
    {
      pictureImage.sprite = handout.picture;
    }
  }
}