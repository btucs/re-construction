#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SymbolHelper {

  public static readonly char GreekAlpha = 'α';
  public static readonly char GreekBeta = 'β';
  public static readonly char GreekGamma = 'γ';
  public static readonly char GreekDelta = 'δ';
  public static readonly string ForceArrow = "F\u20D7";

  private static readonly Dictionary<string, string> mapping = new Dictionary<string, string>() {
    { "alpha" , GreekAlpha.ToString() },
    { "beta", GreekBeta.ToString() },
    { "gamma", GreekGamma.ToString() },
    { "delta", GreekDelta.ToString() },
  };

  public static string GetSymbol(string name) {

    if(name == null) {

      return null;
    }

    string[] split = name.Split('_');

    mapping.TryGetValue(split[0], out split[0]);

    if(split[0] != null) {

      return String.Join("_", split);
    }

    return name;
  }

  public static string GetString(string symbol) {

    if(symbol == null) {

      return null;
    }

    string[] split = symbol.Split('_');

    KeyValuePair<string, string> found = mapping.FirstOrDefault((KeyValuePair<string, string> pair) => pair.Value == split[0]);
    if(found.Key != null) {

      split[0] = found.Key;
      return String.Join("_", split);
    }

    return symbol;
  }
}
