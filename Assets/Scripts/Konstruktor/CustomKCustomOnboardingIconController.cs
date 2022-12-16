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

public class CustomKCustomOnboardingIconController : MonoBehaviour
{
	private MathMagnitude wert;
	public Image IconHolder;
	public Sprite gewichtskraftIcon;
	public Sprite reibungskraftIcon;
	public Sprite muskelkraftIcon; 
	public Sprite kraftartenIcon;
    // Start is called before the first frame update
    void Start()
    {
      wert = GetComponent<InventoryItem>().magnitude;
      if(wert.Value != null) {

        SetVariableIcon();
      }
    }

    public void SetVariableIcon()
    {
    	string varName = wert.Value.name;
    	Debug.Log(varName);
    	if(varName == "F_G")
			IconHolder.sprite = gewichtskraftIcon;
		else if(varName == "F_R")
			IconHolder.sprite = reibungskraftIcon;
		else if(varName == "F_M")
			IconHolder.sprite = muskelkraftIcon;
		else if(varName == "F")
			IconHolder.sprite = kraftartenIcon;
    }
}
