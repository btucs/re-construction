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
using TMPro;

public class DragOutOnboardingController : MonoBehaviour
{
	public TextMeshProUGUI dialogText;
	public string rootWord;
	public RectTransform canvasTransform;
	public GameObject AnimationPrefab;
	public Transform draggedItemContainer;
	private RectTransform animObjTransform;
	private bool checkForTextChange = false;
	private bool checkForDrag = false;
	private int itemContainerChildcount;

    void Update()
    {
    	if(checkForTextChange)
    	{
    		if(dialogText.text.Contains(rootWord))
    		{
    			EnableOnboardingAnimation();
    			checkForTextChange = false;
    		}
    	}

    	if(checkForDrag)
    	{
    		if(draggedItemContainer.childCount > itemContainerChildcount)
    		{
    			Destroy(animObjTransform.gameObject);
    		}
    	}
    }

    //script tried to start Onbaording Animation, before neccessary dialogtext was changed 
    public void StartAnimationProcess()
    {
    	checkForTextChange = true;
    }

    private void EnableOnboardingAnimation()
    {
    	GameObject animObj = Instantiate(AnimationPrefab, canvasTransform);
    	//Debug.Log("Obj Instantiated");
    	animObjTransform = animObj.GetComponent<RectTransform>();

    	PositionUIElementToWordPos();

    	itemContainerChildcount = draggedItemContainer.childCount;
    	checkForDrag = true;
    }

    private void PositionUIElementToWordPos()
    {
        //int myIndex = GetIndexOfString(rootWord);
        //Debug.Log(myIndex);
        //Vector3 textWorldPosition = GetTMPIndexPos(myIndex);
        //Debug.Log(textWorldPosition);
        //MoveUIElementToWorldPos(animObjTransform, textWorldPosition);

    	Vector3 textWorldPosition = GetTMPWordPos(rootWord);
        MoveUIElementToWorldPos(animObjTransform, textWorldPosition);
    }

    private void MoveUIElementToWorldPos(RectTransform moveObj, Vector3 worldVector3)
    {
    	Vector3 uiSpacePos = canvasTransform.InverseTransformPoint(worldVector3);
    	moveObj.localPosition = uiSpacePos;
    }

	private Vector3 GetTMPWordPos(string theWord)
    {
    	dialogText.ForceMeshUpdate();
    	foreach(TMP_WordInfo singleWord in dialogText.textInfo.wordInfo)
    	{
    		string textWordString = singleWord.GetWord(); 
    		if(textWordString == theWord)
    		{
    			Vector3 bottomLeft = dialogText.textInfo.characterInfo[singleWord.lastCharacterIndex].bottomLeft;
    			Debug.Log("Char Pos: " + bottomLeft);
    			Vector3 worldBottomLeft = dialogText.GetComponent<RectTransform>().TransformPoint(bottomLeft);
    			Debug.Log("Char World Pos: " + worldBottomLeft);
    	
    			return worldBottomLeft;
    		}
    	}

    	Debug.LogError("Word " + theWord + " not found in Text.");
    	return Vector3.zero;
    }






    private int GetIndexOfString(string _word)
    {
    	int theIndex = 0;
    	string searchText = dialogText.text;
    	theIndex = searchText.IndexOf(_word);
    	Debug.Log("Index of: " + _word + " in: " + searchText + " is: " + theIndex);
    	return theIndex;
    }

    private Vector3 GetTMPIndexPos(int charIndex)
    {
    	Vector3 bottomLeft = dialogText.textInfo.characterInfo[charIndex].bottomLeft;
    	Debug.Log("Char Pos: " + bottomLeft);
    	Vector3 worldBottomLeft = dialogText.GetComponent<RectTransform>().TransformPoint(bottomLeft);
    	Debug.Log("Char World Pos: " + worldBottomLeft);
    	
    	return worldBottomLeft;
    }

    

    /*private Vector3 GetTextIndexPos(int charIndex)
    {
    	Vector3 worldPos = new Vector3();

    	string text = dialogText.text;
 
        if (charIndex >= text.Length)
            return Vector3.zero;
 
        TextGenerator textGen = new TextGenerator (text.Length);
        Vector2 extents = dialogText.gameObject.GetComponent<RectTransform>().rect.size;
        textGen.Populate (text, dialogText.GetGenerationSettings(extents));
 
        int newLine = text.Substring(0, charIndex).Split('\n').Length - 1;
        int whiteSpace = text.Substring(0, charIndex).Split(' ').Length - 1;
        int indexOfTextQuad = (charIndex * 4) + (newLine * 4) - 4;
        if (indexOfTextQuad < textGen.vertexCount)
        {
            Vector3 avgPos = (textGen.verts[indexOfTextQuad].position + 
                textGen.verts[indexOfTextQuad + 1].position + 
                textGen.verts[indexOfTextQuad + 2].position + 
                textGen.verts[indexOfTextQuad + 3].position) / 4f;
 
 			Debug.Log("canvas pos? : " + avgPos);
            worldPos = dialogText.transform.TransformPoint(avgPos);
            worldPos /= canvasTransform.GetComponent<Canvas>().scaleFactor;
        }
        else {
            Debug.LogError ("Out of text bound");
        }

        return worldPos;
    }*/

    
}
