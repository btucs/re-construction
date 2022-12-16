#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEnvironment : MonoBehaviour
{
    public FloorTypes type;

    private static SoundEnvironment instance;

    public static SoundEnvironment Instance
    {
        get
        {
            return instance;
        }
    }


    public int GetEnvironmentSoundIndex()
    {
        return (int)type;
    }

    private void Awake ()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        Debug.Log("Setting global parameter");
        float indexFloat = (float)GetEnvironmentSoundIndex();
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("FloorType", indexFloat);
    }

}

[System.Serializable]
public enum FloorTypes
{
	indoors = 0, concrete = 1, grass = 2, dirt = 3, water = 4, metal = 5, wood = 6
}