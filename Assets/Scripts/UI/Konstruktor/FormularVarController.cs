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

public class FormularVarController : MonoBehaviour
{
    public Text varText;
    public Text upperText;
    public Text lowerText;

    public void SetVarContent(string _varText)
    {
    	varText.text = _varText;
    }

    public void SetVarContentWithUpper(string _varText, string _upperText)
    {
    	upperText.text = _upperText;
    	upperText.transform.parent.gameObject.SetActive(true);

    	varText.text = _varText;
    }

    public void SetVarContentWithLower(string _varText, string _lowerText)
    {
    	lowerText.text = _lowerText;
    	lowerText.gameObject.SetActive(true);

    	varText.text = _varText;
    }

    public void ResolveFormulaTextFragment(string formulaFragment)
    {
        if(formulaFragment.Contains("^"))
        {
            string[] myStringArray = formulaFragment.Split('^');
            SetVarContentWithUpper(myStringArray[0], myStringArray[1]);
        }
        else if(formulaFragment.Contains("_"))
        {
            string[] myStringArray = formulaFragment.Split('_');
            SetVarContentWithLower(myStringArray[0], myStringArray[1]);
        }
        else if(formulaFragment.Contains("-") && formulaFragment.Length > 2)
        {
            string[] myStringArray = formulaFragment.Split('-');
            SetVarContent(myStringArray[1]);
            //TODO: SetText of cos/tan/sin-Textholder
        }
        else
        {
            SetVarContent(formulaFragment);
        }
    }
}
