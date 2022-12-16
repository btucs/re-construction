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

public class SingleMenuButton : MonoBehaviour
{
	public Button buttonScript;
	public Image backgroundImg;
	public Image iconImg;
	public List<Text> textContent = new List<Text>();
	public bool isSelected = false;
	public ButtonToggleMenu buttonGroup;

	public void SetThisButtonActive()
	{
		buttonGroup.ActivateButton(this);
	}
}
