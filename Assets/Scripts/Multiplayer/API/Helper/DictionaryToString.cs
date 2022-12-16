#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

public static class DictionaryToString
{

    public static string MyToString<TKey, TValue>
        (this IDictionary<TKey, TValue> dictionary)
    {
        if (dictionary == null)
            throw new ArgumentNullException("dictionary");

        var items = from kvp in dictionary
            select kvp.Key + "=" + kvp.Value;

        return "{" + string.Join(",", items) + "}";
    }
}
