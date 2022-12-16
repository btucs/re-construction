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
using UnityEngine.Events;

[Serializable]
public class GameObjectObservableList: ObservableList<GameObject>
{

  public GameObjectObservableList() : base() {

  }

  public GameObjectObservableList(IEnumerable<GameObject> initial): base(initial) {

  }
}

[Serializable]
public class CollectionAddEvent<T> : UnityEvent<CollectionAddEventData<T>>
{
}

public struct CollectionAddEventData<T> : IEquatable<CollectionAddEventData<T>>
{
  public int Index {
    get; private set;
  }
  public T Value {
    get; private set;
  }

  public CollectionAddEventData(int index, T value)
      : this() {
    Index = index;
    Value = value;
  }

  public override string ToString() {
    return string.Format("Index:{0} Value:{1}", Index, Value);
  }

  public override int GetHashCode() {
    return Index.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(Value) << 2;
  }

  public bool Equals(CollectionAddEventData<T> other) {
    return Index.Equals(other.Index) && EqualityComparer<T>.Default.Equals(Value, other.Value);
  }
}

[Serializable]
public class CollectionRemoveEvent<T>: UnityEvent<CollectionRemoveEventData<T>>
{

}
public struct CollectionRemoveEventData<T> : IEquatable<CollectionRemoveEventData<T>>
{
  public int Index {
    get; private set;
  }
  public T Value {
    get; private set;
  }

  public CollectionRemoveEventData(int index, T value)
      : this() {
    Index = index;
    Value = value;
  }

  public override string ToString() {
    return string.Format("Index:{0} Value:{1}", Index, Value);
  }

  public override int GetHashCode() {
    return Index.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(Value) << 2;
  }

  public bool Equals(CollectionRemoveEventData<T> other) {
    return Index.Equals(other.Index) && EqualityComparer<T>.Default.Equals(Value, other.Value);
  }
}

[Serializable]
public class CollectionMoveEvent<T> : UnityEvent<CollectionRemoveEventData<T>>
{
}

public struct CollectionMoveEventData<T> : IEquatable<CollectionMoveEventData<T>>
{
  public int OldIndex {
    get; private set;
  }
  public int NewIndex {
    get; private set;
  }
  public T Value {
    get; private set;
  }

  public CollectionMoveEventData(int oldIndex, int newIndex, T value)
      : this() {
    OldIndex = oldIndex;
    NewIndex = newIndex;
    Value = value;
  }

  public override string ToString() {
    return string.Format("OldIndex:{0} NewIndex:{1} Value:{2}", OldIndex, NewIndex, Value);
  }

  public override int GetHashCode() {
    return OldIndex.GetHashCode() ^ NewIndex.GetHashCode() << 2 ^ EqualityComparer<T>.Default.GetHashCode(Value) >> 2;
  }

  public bool Equals(CollectionMoveEventData<T> other) {
    return OldIndex.Equals(other.OldIndex) && NewIndex.Equals(other.NewIndex) && EqualityComparer<T>.Default.Equals(Value, other.Value);
  }
}

[Serializable]
public class CollectionReplaceEvent<T> : UnityEvent<CollectionReplaceEventData<T>>
{
}

public struct CollectionReplaceEventData<T> : IEquatable<CollectionReplaceEventData<T>>
{
  public int Index {
    get; private set;
  }
  public T OldValue {
    get; private set;
  }
  public T NewValue {
    get; private set;
  }

  public CollectionReplaceEventData(int index, T oldValue, T newValue)
      : this() {
    Index = index;
    OldValue = oldValue;
    NewValue = newValue;
  }

  public override string ToString() {
    return string.Format("Index:{0} OldValue:{1} NewValue:{2}", Index, OldValue, NewValue);
  }

  public override int GetHashCode() {
    return Index.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(OldValue) << 2 ^ EqualityComparer<T>.Default.GetHashCode(NewValue) >> 2;
  }

  public bool Equals(CollectionReplaceEventData<T> other) {
    return Index.Equals(other.Index)
        && EqualityComparer<T>.Default.Equals(OldValue, other.OldValue)
        && EqualityComparer<T>.Default.Equals(NewValue, other.NewValue);
  }
}

[Serializable]
public class ObservableList<T> : IList<T> {
  public delegate void ListUpdateHandler(object sender, object updatedValue);
  public CollectionAddEvent<T> ItemAdded = new CollectionAddEvent<T>();
  public CollectionRemoveEvent<T> ItemRemoved = new CollectionRemoveEvent<T>();
  public CollectionReplaceEvent<T> ItemReplaced = new CollectionReplaceEvent<T>();
  //public CollectionMoveEvent<T> ItemMoved = new CollectionMoveEvent<T>();
  public event EventHandler ListCleared;
  [SerializeField]
  List<T> items = new List<T>();

  public ObservableList() {
  }

  public ObservableList(IEnumerable<T> initial) {

    items = initial.ToList();
  }

  #region IList[T] implementation
  public int IndexOf(T value) {
    return items.IndexOf(value);
  }

  public void Insert(int index, T value) {
    items.Insert(index, value);
    ItemAdded?.Invoke(new CollectionAddEventData<T>(index, value));
  }

  public void RemoveAt(int index) {
    T value = items[index];
    items.RemoveAt(index);
    ItemRemoved?.Invoke(new CollectionRemoveEventData<T>(index, value));
  }

  public T this[int index] {
    get {
      return items[index];
    }
    set {
      T oldValue = items[index];
      items[index] = value;
      ItemReplaced?.Invoke(new CollectionReplaceEventData<T>(index, oldValue, value));
    }
  }
  #endregion

  #region IEnumerable implementation
  public IEnumerator<T> GetEnumerator() {
    return items.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }
  #endregion

  #region ICollection[T] implementation
  public void Add(T item) {

    int index = items.Count;
    items.Add(item);
    ItemAdded?.Invoke(new CollectionAddEventData<T>(index, item));
}

  public void Clear() {
    items.Clear();

    if(ListCleared != null) {
      ListCleared(this, EventArgs.Empty);
    }
  }

  public bool Contains(T item) {
    return items.Contains(item);
  }

  public void CopyTo(T[] array, int arrayIndex) {
    items.CopyTo(array, arrayIndex);
  }

  public bool Remove(T item) {

    int index = items.IndexOf(item);
    if(index < 0) {
    
      return false;
    }

    bool success = items.Remove(item);
    ItemRemoved?.Invoke(new CollectionRemoveEventData<T>(index, item));

    return success;
  }

  public int Count {
    get {
      return items.Count;
    }
  }

  public bool IsReadOnly {
    get {
      return false;
    }
  }
  #endregion
}