#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using Serilog;
using Serilog.Sinks.Unity3D;
using UnityEngine;

namespace Assets.Scripts.Multiplayer.Logging
{
    
    public class UnityLogger : MonoBehaviour
    {
        private static Serilog.ILogger _logger;
        private static UnityLogger instance;
        
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
            {
                //if not, set instance to this
                instance = this;
            }
            //If instance already exists and it's not this:
            else if (instance != this)
            {
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            }

            CreateLogger();

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }

        public static Serilog.ILogger GetLogger()
        {
            CreateLogger();
            return _logger;
        }

        private static void CreateLogger()
        {
            if(_logger == null)
            {
                _logger = new LoggerConfiguration()
                    .MinimumLevel.Error()
                    .WriteTo.Unity3D()
                    .CreateLogger();
            }
        }
        
    }
}