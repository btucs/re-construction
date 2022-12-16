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
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
        public GameObject textContainer;
        public ProgressBar progressBarScript;

        public Text TextOnboarding;

        Text currentText;
        GameObject currentTextBox;

        public TextAsset textFile;
        public string[] textLines;

        public int currentLine;
        public int endAtLine;

        public bool isActive;

        private bool isTyping = false;
        private bool cancelTyping = false;

        public float typeSpeed;

        // Use this for initialization
        void Start () {
                currentTextBox = textContainer;
                currentText = TextOnboarding;
                
                if(isActive)
                {
                    EnableTextBox(currentTextBox);
                } else {
                    DisableTextBox(currentTextBox);
                }
        }

        void Update () {
                if(!isActive)
                {
                    return;
                }

                if(endAtLine == 0 || endAtLine > textLines.Length - 1)
                {
                    endAtLine = textLines.Length - 1;
                }

                //theText.text = textLines[currentLine];

        }

        private IEnumerator TextScroll (string lineOfText, Text whichText)
        {
                int letter = 0;
                whichText.text = "";
                isTyping = true;
                cancelTyping = false;
                while (isTyping && !cancelTyping && (letter < lineOfText.Length -1))
                {
                        whichText.text += lineOfText[letter];
                        letter += 1;
                        yield return new WaitForSeconds(typeSpeed);
                }
                whichText.text = lineOfText;
                isTyping = false;
                cancelTyping = false;
        }

        public bool DisplayNextText ()
        {

            if(!isTyping)
            {
                currentLine += 1;
                if(progressBarScript!=null)
                {
                    progressBarScript.activePage = currentLine;
                    progressBarScript.updateActivePage();
                }

                if(currentLine > endAtLine)
            	{
                	DisableTextBox(currentTextBox);
                	isActive = false;
            	} else {
            		StartCoroutine(TextScroll(textLines[currentLine], currentText));
            	}
                return true;
            }
            else if(isTyping && !cancelTyping)
            {
                cancelTyping = true;
            }
            return false;
        }

        public void NextTextInstant()
        {
        	Debug.Log("next text called");
        	currentLine += 1;
            
			if(currentLine > textLines.Length)
            {
            	DisableTextBox(currentTextBox);
            	isActive = false;
            }
            else
            {
            	currentText.text = textLines[currentLine];
            }
        }

        public bool DisplayPreviousText ()
        {
            if(!isTyping)
            {
                currentLine -= 1;
                if(progressBarScript!=null)
                {
                    progressBarScript.activePage = currentLine;
                    progressBarScript.updateActivePage();
                }

                if(currentLine < 0)
            	{
            		currentLine = 0;
            	} else {
            		StartCoroutine(TextScroll(textLines[currentLine], currentText));
            	}
                return true;
            }
            else if(isTyping && !cancelTyping)
            {
                    cancelTyping = true;
            }
            return false;
        }

        public void EnableTextBox(GameObject whichTextBox)
        {
                whichTextBox.SetActive(true);
                isActive = true;

                currentText.text = textLines[currentLine];
                //StartCoroutine(TextScroll(textLines[currentLine], currentText));
        }

        public void DisableTextBox(GameObject whichTextBox)
        {
                whichTextBox.SetActive(false);
                isActive = false;
        }

        public void ReloadText(TextAsset theText)
        {
                if(theText != null){
                        textLines = new string[1];
                        textLines = (theText.text.Split('\n'));
                }
        }

}