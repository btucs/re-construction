#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
/// <see cref="https://stackoverflow.com/a/56539595/1244727"/>
public struct HashCode : IEquatable<HashCode>
{
  private const int EmptyCollectionPrimeNumber = 19;
  private readonly int value;

  private HashCode(int value) => this.value = value;

  public static implicit operator int(HashCode hashCode) => hashCode.value;

  public static bool operator ==(HashCode left, HashCode right) => left.Equals(right);

  public static bool operator !=(HashCode left, HashCode right) => !(left == right);

  public static HashCode Of<T>(T item) => new HashCode(GetHashCode(item));

  public static HashCode OfEach<T>(IEnumerable<T> items) =>
      items == null ? new HashCode(0) : new HashCode(GetHashCode(items, 0));

  public HashCode And<T>(T item) =>
      new HashCode(CombineHashCodes(this.value, GetHashCode(item)));

  public HashCode AndEach<T>(IEnumerable<T> items) {
    if(items == null) {
      return new HashCode(this.value);
    }

    return new HashCode(GetHashCode(items, this.value));
  }

  public bool Equals(HashCode other) => this.value.Equals(other.value);

  public override bool Equals(object obj) {
    if(obj is HashCode) {
      return this.Equals((HashCode)obj);
    }

    return false;
  }

  public override int GetHashCode() => this.value.GetHashCode();

  private static int CombineHashCodes(int h1, int h2) {
    unchecked {
      // Code copied from System.Tuple a good way to combine hashes.
      return ((h1 << 5) + h1) ^ h2;
    }
  }

  private static int GetHashCode<T>(T item) => item?.GetHashCode() ?? 0;

  private static int GetHashCode<T>(IEnumerable<T> items, int startHashCode) {
    var temp = startHashCode;

    var enumerator = items.GetEnumerator();
    if(enumerator.MoveNext()) {
      temp = CombineHashCodes(temp, GetHashCode(enumerator.Current));

      while(enumerator.MoveNext()) {
        temp = CombineHashCodes(temp, GetHashCode(enumerator.Current));
      }
    } else {
      temp = CombineHashCodes(temp, EmptyCollectionPrimeNumber);
    }

    return temp;
  }
}