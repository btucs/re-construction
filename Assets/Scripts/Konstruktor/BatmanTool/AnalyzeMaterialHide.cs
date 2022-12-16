#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyzeMaterialHide : MonoBehaviour
{
	public Material analyzeMaterial;
    public Material hiddenObjMaterial;
    public Material hiddenTextMaterial;

    private void Start()
    {
    	SetMaterialOffset();
    }

    private void SetMaterialOffset()
    {
    	Vector3 offsetVector = new Vector3(100, 200, 0);
		analyzeMaterial.SetVector("_EffectorWorldPos", offsetVector);
		hiddenObjMaterial.SetVector("_EffectorWorldPos", offsetVector);
        hiddenTextMaterial.SetVector("_EffectorWorldPos", offsetVector);
    }
}
