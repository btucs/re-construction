#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace Assets.Scripts.Multiplayer.API
{
    public class AsyncHttpClient: IDisposable
    {
        private readonly ISerializationOption _serializationOption;
        private bool _disposed = false;
        private readonly string reqToken = null;
        public AsyncHttpClient(ISerializationOption serializationOption)
        {
            _serializationOption = serializationOption;
            reqToken = MultiplayerGameManager.bearerToken;
        }

        public async Task<TResultType> Get<TResultType>(string url)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                return await SendWebRequest<TResultType>(url, www);
            }
        }

        public async Task<TResultType> Post<TResultType>(string url, WWWForm data = null)
          {
            using (UnityWebRequest www = UnityWebRequest.Post(url, data))
            {
                return await SendWebRequest<TResultType>(url, www);
            }
          }

        private async Task<T> SendWebRequest<T>(string url, UnityWebRequest www)
        {
            www.SetRequestHeader("Content-Type", _serializationOption.ContentType);
            www.SetRequestHeader("Authorization", "Bearer " + reqToken);
            // Request and wait for the desired page.
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task<T>.Yield();
            }

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            if (www.isNetworkError)
            {
                Console.WriteLine(pages[page] + ": Error: " + www.error);
            }
            else if (www.isHttpError)
            {
                Console.WriteLine(pages[page] + ": HTTP Error: " + www.error);
            }
            else if (www.isDone)
            {
                Console.WriteLine(pages[page] + ":\nReceived: " + www.downloadHandler.text);
            }
            
            return _serializationOption.Deserialize<T>(www.downloadHandler.data);
        }

        

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            // Dispose of any unmanaged resources not wrapped in safe handles.

            _disposed = true;
        }
    }
}