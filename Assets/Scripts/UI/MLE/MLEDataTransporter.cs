#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLEDataTransporter : MonoBehaviour
{
    public MLEDataSO mleTransportData;
    public string originSceneName;
    public OriginType typeOfAccess;
    public TaskObjectSO originObject;
    public TaskDataSO originSceneTask;
    public int achievedPoints = 0;
    public bool openPrevUI = true;
}

//when returned to office Scene, data from MLE is added to savegame in UIStateLoader Script

public enum OriginType
{
	bibList, taskPreview
}