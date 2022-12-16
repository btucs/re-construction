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

public class SinCosTanController : MonoBehaviour
{
    public Text preText;
    public Text varText;

    public void SetContent(string _preText, string _varText)
    {
    	preText.text = _preText;
    	varText.text = _varText;
    }
}
