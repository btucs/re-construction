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

public class ButtonController : MonoBehaviour {
	public TextManager myTextManager; 
  public int disableOn;

	int activeTextline = 0; 

	void Start() {

    UpdateButtonState();
	}

  public void UpdateButtonState() {

    activeTextline = myTextManager.currentLine;
    this.GetComponent<Button>().interactable = (activeTextline == disableOn ? false : true);    
  }
}
