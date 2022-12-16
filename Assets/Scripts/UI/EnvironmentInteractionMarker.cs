

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

public class EnvironmentInteractionMarker : MonoBehaviour
{
	public GameObject buttonObj;
    public GameWorldObject worldObject;
    public GameObject expandedPulse;
    public GameObject smallPulse;
    public GameObject expandCircle;
    private bool isExpanded = false;
    private bool newInfo = true;
    private MenuUIController uiController;
    private CustomAnimationController animController;

    private void Start()
    {
        animController = GetComponent<CustomAnimationController>();
    }

    public void Setup(GameWorldObject objRef)
    {
        worldObject = objRef;

        animController = GetComponent<CustomAnimationController>();
        RectTransform objBody = this.GetComponent<RectTransform>();
        objBody.anchoredPosition = objRef.GetMarkerPosition();

        newInfo = worldObject.GetExaminedState();
        RefreshAppearance();

        if(MenuUIController.Instance != null)
        {
            uiController = MenuUIController.Instance;
        }
    }

    public void MovePlayerToObject()
    {
    	playerScript playerController = playerScript.Instance;
    	if(playerController != null && playerController.GetPlayerMovementScript() != null)
    	{
    		Vector3 markerWorldPos = this.transform.position;
    		playerController.GetPlayerMovementScript().SetXGoalPosition(markerWorldPos);
    	}
    }

    public void RefreshAppearance()
    {
        if(!newInfo && animController != null)
        {
            animController.AddActiveAnimationState("examined");
        }
        expandedPulse.SetActive(!newInfo);
        expandCircle.SetActive(false);
    }

    public void OpenObjectMenu()
    {
        newInfo = false;
        RefreshAppearance();

        if(uiController != null)
            uiController.worldSpaceUI.SetSingleMarkerActive(this);

        worldObject.OpenObjectMenu();
    }

    private void OnTriggerEnter2D(Collider2D otherCol)
    {
    	if(otherCol.gameObject.CompareTag("Player") && !isExpanded)
    	{
    		animController.AddActiveAnimationState("expanded");
    		isExpanded = true;
    	}
    }

    private void OnTriggerExit2D(Collider2D otherCol)
    {
    	if(otherCol.gameObject.CompareTag("Player") && isExpanded && this.gameObject.activeSelf)
    	{
    		animController.AddActiveAnimationState("shrunk");
    		isExpanded = false;
    	}
    }

}