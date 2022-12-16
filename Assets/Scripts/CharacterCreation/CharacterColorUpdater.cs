#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(characterGraphicsUpdater))]
public class CharacterColorUpdater : MonoBehaviour
{
	public List<SpriteRenderer> hairGraphics;
	public List<SpriteRenderer> bodyGraphics;

	private Color hairTintColor;
	private Color bodyTintColor;
	private characterGraphicsUpdater graphicsControllerRef;

	void Start()
	{
		graphicsControllerRef = GetComponent<characterGraphicsUpdater>();
		GetColorFromCharacterData(graphicsControllerRef.characterSOData.Value);
		TintAllGraphics();
	}

	public void RefreshColors()
	{
		GetColorFromCharacterData(graphicsControllerRef.characterSOData.Value);
		TintAllGraphics();
	}

	public void SetGraphicsColor(CharacterSO _characterSO)
	{
		GetColorFromCharacterData(_characterSO);
		TintAllGraphics();
	}

	public void SetBodyColor(CharacterSO _characterSO)
	{
		GetColorFromCharacterData(_characterSO);
		TintBodyGraphics();
	}

	public void SetHairColor(CharacterSO _characterSO)
	{
		GetColorFromCharacterData(_characterSO);
		TintHairGraphics();
	}

	public void GetColorFromCharacterData(CharacterSO characterData)
	{
		hairTintColor = characterData.hairColor;
		bodyTintColor = characterData.bodyColor;
	}

    public void TintAllGraphics()
    {
    	foreach(SpriteRenderer hairSprite in hairGraphics)
    	{
    		hairSprite.color = hairTintColor;
    	}
    	foreach(SpriteRenderer bodySprite in bodyGraphics)
    	{
    		bodySprite.color = bodyTintColor;
    	}
    }

    public void TintBodyGraphics()
    {
    	foreach(SpriteRenderer bodySprite in bodyGraphics)
    	{
    		bodySprite.color = bodyTintColor;
    	}
    }

    public void TintHairGraphics()
    {
    	foreach(SpriteRenderer hairSprite in hairGraphics)
    	{
    		hairSprite.color = hairTintColor;
    	}
    }
}
