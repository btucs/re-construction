#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MathUnits.Physics.Values;
using Mathematics.LA;

public class HiddenItem : MonoBehaviour
{
    public Image radialProgressBar;
    public GameObject unidentGraphics;
    public GameObject identGraphics;
    public ParticleSystem scanParticles;
    public Text itemNameText;

    public float requiredScanTime = 1f;

    public UnityEvent onScanComplete = new UnityEvent();
    public UnityEvent onScanStart = new UnityEvent();
    public UnityEvent onScanAbort = new UnityEvent();

    private TaskVariable variable;
    private HiddenItemFactory itemFactory;
    private bool isCurrentlyScanned = false;
    private InventoryItem connectedItem;
    private bool isIdentified = false;

    void Start()
    {
    	radialProgressBar.fillAmount = 0f;
    }

    void Update()
    {

        if(isCurrentlyScanned)
        {
        	if(radialProgressBar.fillAmount < 1)
        	{
        		if(connectedItem != null)
        			radialProgressBar.fillAmount = 1f;

        		radialProgressBar.fillAmount += 1f / requiredScanTime * Time.deltaTime;
        	} else {
        		//Debug.Log("Spawn Inventory Item now...");
        		if(connectedItem == null && !isIdentified)
        		{
        			SetIdentified(true);
                    if(onScanComplete != null)
                        onScanComplete.Invoke();
	        		CreateInventoryItem();
        		}
			}
        } else if(connectedItem == null && radialProgressBar.fillAmount > 0)
        {
        	radialProgressBar.fillAmount -= 1f / requiredScanTime * Time.deltaTime;
        }

        if(connectedItem == null && isIdentified)
        {
            radialProgressBar.fillAmount = 0;
            SetIdentified(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collisionInfo)
    {
    	if(collisionInfo.gameObject.CompareTag("AnalyzeTool"))
    	{
    		isCurrentlyScanned = true;
            
            if(connectedItem==null && onScanStart != null)
            {
                onScanStart.Invoke();
            }
    	}
    }

    void OnTriggerExit2D(Collider2D collisionInfo)
    {
    	if(collisionInfo.gameObject.CompareTag("AnalyzeTool"))
    	{
    		isCurrentlyScanned = false;

            if(connectedItem==null && onScanAbort != null)
            {
                onScanAbort.Invoke();
            }
    	}
    }

    public void Setup(TaskVariable newVar, HiddenItemFactory factory)
    {
    	variable = newVar;
    	itemFactory = factory;
        if(newVar is TaskInputVariable)
        {
            TaskInputVariable inputVar = newVar as TaskInputVariable;
            itemNameText.text = inputVar.textValue;
        } else {
            itemNameText.text = variable.name;
        }

    }

    private void CreateInventoryItem()
    {
    	Debug.Log("Creating Inventory Item now");
    	connectedItem = itemFactory.CreateItem(variable);
    	connectedItem.transform.position = this.transform.position;

    	//GameController.GetInstance().gameState.profileData.achievements.informationsScanned = GameController.GetInstance().gameState.profileData.achievements.informationsScanned + 1;
        GameController.GetInstance().SaveGame();
    	//connectedItem.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
    }

    private void SetIdentified(bool ident)
    {
        isIdentified = ident;
        unidentGraphics.SetActive(!ident);
        itemNameText.gameObject.SetActive(!ident);
        identGraphics.SetActive(ident);

        if(isIdentified && scanParticles != null)
            scanParticles.Play();
    }

}
