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

public class MentorSelector : MonoBehaviour
{
    public Text nameText;
    public Text descriptionText;

    private List<MentorOption> options = new List<MentorOption>();

    private void SetTextContent(string name, string description)
    {
    	nameText.text = name;
    	descriptionText.text = description;
    }

    public void SetSelectedMentor(MentorOption selected)
    {
    	foreach(MentorOption option in options)
    	{
    		bool isSelected = (selected == option);
    		option.SetSelected(isSelected);
    	}

    	SetTextContent(selected.mentorName, selected.mentorDescription);
    }

    public void Subscribe(MentorOption subscriber)
    {
    	options.Add(subscriber);
    }
}
