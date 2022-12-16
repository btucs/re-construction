#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Rewards/Cosmetic")]
public class CosmeticItem : PermanentItem
{
	public CosmeticCategory category;
	public string label;
	public bool availableOnGameStart = false;

	public string[] GetCategoryNames()
  	{
    string[] returnNames = new string[0];

    switch(category)
    {
      case CosmeticCategory.torso :
        returnNames = new string[] {"UpperBody_Torso#", "UpperBody_LowerFrontArm", "UpperBody_UpperFrontArm", "UpperBody_LowerBackArm", "UpperBody_UpperBackArm"};
        break;
      case CosmeticCategory.legs :
        returnNames = new string[] {"LowerBody_UpperLegBack", "LowerBody_LowerLegBack", "LowerBody_LowerLegFront", "LowerBody_UpperLegFront#"};
        break;
      case CosmeticCategory.foot : 
        returnNames = new string[] {"Foot_Front#", "Foot_Back"};
        break;
      case CosmeticCategory.hair : 
        returnNames = new string[] {"Hair_Front#", "Hair_Back"};
        break;
      case CosmeticCategory.eyes : 
        returnNames = new string[] {"Eyes"};
        break;
      case CosmeticCategory.mouth : 
        returnNames = new string[] {"Mouth"};
        break;
      case CosmeticCategory.nose : 
        returnNames = new string[] {"Nose"};
        break;
      case CosmeticCategory.decoration : 
        returnNames = new string[] {"Decoration"};
        break;
    }

    return returnNames;
  }

}

[Serializable]
public enum CosmeticCategory 
{
	torso, legs, hair, foot, eyes, nose, mouth, decoration
}