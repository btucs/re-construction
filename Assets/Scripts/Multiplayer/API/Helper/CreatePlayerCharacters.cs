#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using Assets.Scripts.Multiplayer.API.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Multiplayer.API.Helper
{
    public class CreatePlayerCharacters
    {
        private readonly characterGraphicsUpdater _player1;
        private readonly characterGraphicsUpdater _player2;
        private readonly QuizGame _game;
        private readonly Text _p1Name;
        private readonly Text _p2Name;
        PlayerHelper playerHelper = new PlayerHelper();

        public CreatePlayerCharacters(characterGraphicsUpdater Player1, characterGraphicsUpdater Player2, QuizGame game,
            Text P1Name = null, Text P2Name = null)
        {
            _player1 = Player1;
            _player2 = Player2;
            _game = game;
            _p1Name = P1Name;
            _p2Name = P2Name;
        }
        
        public async System.Threading.Tasks.Task SetPlayersCharacters()
        {
            _player1.characterSOData.Value =  MultiplayerGameManager.getPlayerCharacter();
            var player2Data = await playerHelper.GetPlayerByGame(_game);
            MultiplayerGameManager.SetPlayer2(player2Data);
            _player2.characterSOData.Value = LoadCharacterData(player2Data.PlayerCharacter, player2Data.Colors);

            _player1.UpdateAllGraphics();
            _player2.UpdateAllGraphics();
            
            if(_p1Name != null)
                _p1Name.text = MultiplayerGameManager.getPlayerName();
            if(_p2Name != null)
                _p2Name.text = MultiplayerGameManager.Player2.PlayerName;
        }
        
        private static global::CharacterSO LoadCharacterData(string characterData, string colors)
        {
            var dic = CharacterAndBodyColorHelper.DeserializeCharacter(characterData);
            var character = GameObject.Instantiate(ScriptableObject.CreateInstance<CharacterSO>());
            character.spriteRenderDictionary = dic;
            CharacterAndBodyColorHelper.SetBodyColors(character, colors);
            return character;
        }
    }
}