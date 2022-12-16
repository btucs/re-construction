#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Yarn.Unity;

[Serializable]
public class EventPart
{
	public List<EventAction> actions = new List<EventAction>();
    public float delayTime = 0f;
}

[Serializable]
public abstract class EventAction : MonoBehaviour
{
	public bool autoContinue = false;
	public Button continueButton;
	protected bool hasFinished = false;
    protected ScriptedEventManager manager;
    private UnityAction onActionFinished;

	public abstract void Invoke(ScriptedEventManager eventManager);

	public virtual void SetActionFinished()
	{
		hasFinished = true;
		manager.CheckForContinue();
	}

	private void FinishButtonAction()
	{
		SetActionFinished();
		continueButton.onClick.RemoveListener(onActionFinished);
	}

	public void SetupContinueCondition()
	{
		if(autoContinue == true)
		{
			SetActionFinished();
		} else if(continueButton != null)
		{
			onActionFinished = new UnityAction(SetActionFinished);
			continueButton.onClick.AddListener(onActionFinished);
		}
	}

	public bool IsFinished()
	{
		return hasFinished;
	}
}