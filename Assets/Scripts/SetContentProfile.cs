﻿#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetContentProfile : MonoBehaviour
{
	public Text playerName;
  public Image playerImage;
	public PlayerDataSO PlayerData;

  // Start is called before the first frame update
  private void Start()
  {

    SetProfileContent();
  }


  private void SetProfileContent()
  {

    if (playerImage != null) {

      playerImage.sprite = PlayerData.avatar.image;
    }

    if(playerName != null) {

      playerName.text = PlayerData.playerName;
    }
  }
}
