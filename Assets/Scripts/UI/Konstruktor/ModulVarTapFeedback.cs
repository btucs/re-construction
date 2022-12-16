using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModulVarTapFeedback : MonoBehaviour
{
	public Text headline;
	public Text description;
	public InputOutputDrawController slotController;
	public KonstruktorDrawController konstruktorController;
	private bool isDisplayed = false;

    public void DisplayEmptySlotFeedback()
	{
		//get expected item type and assign feedback texts depending on type
		//headline.text
		
		headline.text = slotController.varData.valHeadline;
		description.text = slotController.varData.valDescription;

		isDisplayed = true;
    	this.gameObject.SetActive(true);
	}

	private void Update()
	{
		if(isDisplayed)
		{
	        foreach (Touch touch in Input.touches)
	        {
	            if (touch.phase == TouchPhase.Ended)
	            {
	                HideEmptySlotFeedback();
	                return;
	            }
	        }
	        if(Input.GetMouseButtonUp(0))
	        {
	        	HideEmptySlotFeedback();
	            return;
	        }
		}
	}

	public void HideEmptySlotFeedback()
	{
		isDisplayed = false;
		Debug.Log("Disable SlotFeedback is called");
		this.gameObject.SetActive(false);
	}
}
