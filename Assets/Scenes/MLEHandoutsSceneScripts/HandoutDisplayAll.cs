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
using TMPro;
using TexDrawLib;

public class HandoutDisplayAll : MonoBehaviour
{
  public MLEHandoutsSO handout;
  public MLEHandoutsController mleHandoutManager;
  public TextMeshProUGUI mleName;

  public void ActivateThisEntry()
  {
    mleHandoutManager.CreateEntryDetails(handout);
  }
}