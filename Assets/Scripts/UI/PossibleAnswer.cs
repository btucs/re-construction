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

[System.Serializable]
	public class PossibleAnswer
	{
		[TextArea]
		public string answerText;
		[TextArea]
		public string feedbackText;
		public bool isCorrect;
		public bool isSelected;
		[HideInInspector]
		public MLEAnswerController answerControllerRef = null;

		public PossibleAnswer(string _answerText, string _feedbackText, bool _isCorrect, bool _isSelected)
		{
			answerText = _answerText;
			feedbackText = _feedbackText;
			isCorrect = _isCorrect;
			isSelected = _isSelected;
		}
	}