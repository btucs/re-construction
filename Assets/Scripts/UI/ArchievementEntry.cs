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

public class ArchievementEntry : MonoBehaviour
{
	public Text title;
	public Text description;
	public Text progress;
	public Image iconBG;
	public Image iconMain;

	[SerializeField]
	private Sprite lvl1BG;
	[SerializeField]
	private Sprite lvl2BG;
	[SerializeField]
	private Sprite lvl3BG;
	[SerializeField]
	private Sprite lvl4BG;
    
	public void UpdateEntryData()
	{
		
	}

}
