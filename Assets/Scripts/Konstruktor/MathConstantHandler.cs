#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MathConstantHandler : MonoBehaviour
{
    public Text valueDisplay;
    
    private double multiplyValue = 1.0;

    public void SetValue(double newValue)
    {
    	multiplyValue = newValue;
    	valueDisplay.text = multiplyValue.ToString(CultureInfo.InvariantCulture);
    }

    public double GetValue()
    {
    	return multiplyValue;
    }
}
