#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universit?t Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
namespace Assets.Scripts.Multiplayer.API.Helper
{
    using System;
    using System.Collections;
    using Assets.Scripts.Multiplayer.Logging;
    using UnityEngine;
    using UnityEngine.Networking;

    public class CheckInternetConnection : MonoBehaviour
    {
        public GameObject CanvasConnectionError;
        private bool connected;

        void Start()
        {
            StartCoroutine(CheckConnection(isConnected =>
            {
                if (isConnected)
                {
                    Debug.Log("Internet Available!");
                }
                else
                {
                    CanvasConnectionError.SetActive(true);
                    UnityLogger.GetLogger().Debug("Internet Not Available");
                }
            }));
        }

        public static IEnumerator CheckConnection(Action<bool> syncResult)
        {
            const string echoServer = "https://google.com";

            bool result;
            using (var request = UnityWebRequest.Head(echoServer))
            {
                request.timeout = 5;
                yield return request.SendWebRequest();
                result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
            }
            syncResult(result);
        }
        
    }
}