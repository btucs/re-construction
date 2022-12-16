#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KonstruktorStateIdentifier : MonoBehaviour
{
	public OutputMenuController outputMenu;
	public InputMenuController inputMenu;
	public MultistepController multistepDropdown;

    private static KonstruktorStateIdentifier instance;
	public static KonstruktorStateIdentifier Instance
	{
    	get { return instance; }
	}

	private void Awake ()
	{
		if (instance == null) { instance = this; }
		else if (instance != this) { Destroy(this.gameObject); }
	}

	public bool SearchedVarIdentified()
	{
		return outputMenu.SearchVarsIdentified();
	}

	public bool ResultAssigned()
	{	
		return outputMenu.ResultsAssigned();
	}

	public bool ResultCreated()
	{
		return inputMenu.ContainsResultItem();
	}

	public bool TaskStepAdded()
	{
		return multistepDropdown.HasAddedStep();
	}
}
