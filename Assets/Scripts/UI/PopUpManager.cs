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
using UnityEngine.Events;
using TMPro;

public class PopUpManager : MonoBehaviour
{
    public TextMeshProUGUI feedbackMSG;
    public TextMeshProUGUI confirmMSG;

    public GameObject confirmPopUp;
    public GameObject feedbackPopUp;

    public Button confirmButton;
    private static PopUpManager instance;
	private UnityAction tempAction;

	public static PopUpManager Instance
	{
		get { return instance; }
	}

	private void Awake ()
	{
		if (instance == null) { instance = this; }
		else if (instance != this) { Destroy(this.gameObject); }
	}

	public void DisplayFeedbackPopUp(string _msg)
	{
		feedbackMSG.text = _msg;
		feedbackPopUp.SetActive(true);
	}

	public void DisplayConfirmPopUp(string _msg, UnityAction buttonAction)
	{
		if(buttonAction == null)
		{
			Debug.LogError("Confirm-Button has no valid Action attached.");
			return;
		}
		tempAction = new UnityAction(buttonAction);
		tempAction += RemoveTempButtonAction;

		confirmMSG.text = _msg;
		confirmButton.onClick.AddListener(tempAction);
		confirmPopUp.SetActive(true);
	}

	public void RemoveTempButtonAction()
	{
		confirmPopUp.SetActive(false);
		confirmButton.onClick.RemoveAllListeners();
		tempAction = null;
	}

	public void CancelPopUpConfirmation()
	{
		confirmButton.onClick.RemoveAllListeners();
	}
}
