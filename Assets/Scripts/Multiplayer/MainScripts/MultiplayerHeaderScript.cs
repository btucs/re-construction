#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;

public class MultiplayerHeaderScript : MonoBehaviour
{
    public GameObject txtPlayerId;
    public GameObject txtSceneName;
    public GameObject txtArrow;
    public string SceneText;
    public bool ActivArrow;

    // Start is called before the first frame update
    void Start()
    {
      
        txtArrow.gameObject.SetActive(ActivArrow);
        txtSceneName.GetComponent<UnityEngine.UI.Text>().text = SceneText;
        if (SceneText.Contains("Spiel gegen"))
        {
            txtSceneName.GetComponent<UnityEngine.UI.Text>().text += " " + MultiplayerGameManager.Player2.PlayerName;
        }
        InitPlayerId();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitPlayerId()
    {
        txtPlayerId.GetComponent<UnityEngine.UI.Text>().text = MultiplayerGameManager.getPlayerId();
    }
}
