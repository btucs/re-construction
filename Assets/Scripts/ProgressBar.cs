#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
	public int numberOfPages;
	public int activePage;
	public Transform pageIconInactivePrefab;
	public Transform pageIconActivePrefab;

	GameObject activePageObject;

    void Start()
    {
        createProgressbarPages();
    }

    public void updateActivePage ()
    {
    	if(activePageObject!= null)
    	{
			activePageObject.transform.SetSiblingIndex(activePage);
    	}
    }


    public void createProgressbarPages ()
    {
    	Transform activePageTransform = Instantiate(pageIconActivePrefab, this.transform);
    	activePageObject = activePageTransform.gameObject;

    	for (int i = 1; i < numberOfPages; i++)
    	{
    		Instantiate(pageIconInactivePrefab, this.transform);
    	}
    }
}
