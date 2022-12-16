#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormulaPrefabController : MonoBehaviour
{
    public GameObject equalsGraphicPrefab;
    public GameObject varGraphicPrefab;
    public GameObject operatorGraphicPrefab;
    public GameObject sinusGraphicPrefab;

    public Transform FormularContainer;

    public void ClearFormulaContainer()
    {
    	foreach (Transform child in FormularContainer) {
    		GameObject.Destroy(child.gameObject);
		}
    }

    public void CreateFormulaEntry(string entryString)
    {
        if(entryString.Contains("?"))
        {
            entryString = entryString.Replace("?","");
        }

        if(entryString.Contains("#"))
        {
            entryString = entryString.Replace("#", "");
        }

        if (entryString.Contains("cos") || entryString.Contains("sin") || entryString.Contains("tan"))
    	{
    		string[] myStringArray = entryString.Split('-');
    		GameObject entryObj = Instantiate(sinusGraphicPrefab, FormularContainer);
    		entryObj.GetComponent<SinCosTanController>().SetContent(myStringArray[0], myStringArray[1]);
    	} 
    	else
    	{
    		GameObject entryObj;
    		switch(entryString)
    		{
    			case "=": 
    			entryObj = Instantiate(equalsGraphicPrefab, FormularContainer);
    			break;

    			case "+": 
    			entryObj = Instantiate(operatorGraphicPrefab, FormularContainer); 
	    		entryObj.GetComponent<OperatorController>().DisplayAdditionIcon();
    			break;

    			case "*": 
    			entryObj = Instantiate(operatorGraphicPrefab, FormularContainer); 
	    		entryObj.GetComponent<OperatorController>().DisplayMultiplicationIcon();
    			break;

    			case "-": 
    			entryObj = Instantiate(operatorGraphicPrefab, FormularContainer); 
	    		entryObj.GetComponent<OperatorController>().DisplaySubstractionIcon();
    			break;

    			default:
    			entryObj = Instantiate(varGraphicPrefab, FormularContainer);
	    		entryObj.GetComponent<FormularVarController>().ResolveFormulaTextFragment(entryString);
	    		break;
    		}
    	}
    	

    }
}
