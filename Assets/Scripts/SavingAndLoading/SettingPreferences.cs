#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingPreferences
{
	public AudioPreferences audioSettings;
    public int graphicsQualityIndex = 3;

    public SettingPreferences(SettingPreferences copiedSettings)
    {
    	graphicsQualityIndex = copiedSettings.graphicsQualityIndex;
    	audioSettings = new AudioPreferences(copiedSettings.audioSettings);
    }

    public SettingPreferences()
    {
    	graphicsQualityIndex = 3;
    	audioSettings = new AudioPreferences();
    }
}

[System.Serializable]
public class AudioPreferences
{
	public bool soundEnabled;
	public float UISoundVolume = 1f;
	public float musicVolume = 1f;
	public float environmentVolume = 1f;

	public AudioPreferences()
	{
		soundEnabled = true;
		UISoundVolume = 1f;
		musicVolume = 1f;
		environmentVolume = 0.5f;
	}

	public AudioPreferences(AudioPreferences copiedSettings)
	{
		soundEnabled = copiedSettings.soundEnabled;
		UISoundVolume = copiedSettings.UISoundVolume;
		musicVolume = copiedSettings.musicVolume;
		environmentVolume = copiedSettings.environmentVolume;
	}
}

[System.Serializable]
public class GameplayPreferences
{
	//amount of help?
}