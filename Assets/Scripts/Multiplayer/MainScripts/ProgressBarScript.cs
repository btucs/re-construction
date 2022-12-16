#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using Assets.Scripts.Multiplayer.SceneScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Multiplayer.MainScripts
{
    public class ProgressBarScript : MonoBehaviour
    {
        public Slider slider;
        private float targetProgress = 0;
        private float startValue = 1;
        public float FillSpeed = 0.5f;
        private bool stop = false;
        private void Awake()
        {
            FillSpeed = slider.value / 60f;
        }
        // Start is called before the first frame update
        void Start()
        {
            DecrementProgress(1f);
        }

        public void Reset()
        {
            slider.value = startValue;
            stop = false;
            slider.gameObject.SetActive(true);
            DecrementProgress(1f);
        }

        public void StopAndInvisible()
        {
            slider.gameObject.SetActive(false);
            stop = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!stop)
            {
                if (slider.value > targetProgress)
                {
                    slider.value -= FillSpeed * Time.deltaTime;
                }
                else if (slider.value == targetProgress)
                {
                    var questionScript = GameObject.FindObjectOfType(typeof(QuestionScript)) as QuestionScript;
                    System.Threading.Thread.Sleep(1000);
                    slider.gameObject.SetActive(false);
                    questionScript.TimeOver();
                    //Muss größer sein damit nicht endlos aufgerufen wird. Wert von Slider kann nicht kleiner 0 und größer 1 sein
                    targetProgress = 2;
                }
            }
        }

        public void DecrementProgress(float newProgress)
        {
            targetProgress = slider.value - newProgress;
        }
    }
}
