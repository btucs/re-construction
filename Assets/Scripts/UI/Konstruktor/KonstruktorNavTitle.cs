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

public class KonstruktorNavTitle : MonoBehaviour
{
    public Text prevLayerText;
    public Text activeLayerText;
    public KonstruktorNavTitlesManager manager;

    public void SetPrevLayerText()
    {
    	prevLayerText.text = manager.ReturnPrevLayerName();
    }

    public void SetActiveTextToKonstruktorName()
    {
    	activeLayerText.text = manager.ReturnActiveKonstruktorName();
    }

    public void SetActiveTextToTaskName()
    {
    	activeLayerText.text = manager.ReturnActiveTaskName();
    }

    public void SetPrevLayerTextToTaskName()
    {
    	prevLayerText.text = manager.ReturnActiveTaskName();
    }
}
