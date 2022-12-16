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
using UniRx;

public class GameManager: MonoBehaviour {
  
  [Obsolete()]
  public Canvas contextMenu;

  public BehaviorSubject<GameObject> selectedObjectSource = new BehaviorSubject<GameObject>(null);
  public IObservable<GameObject> selectedObject;

  public HashSet<GameObject> handlePool = new HashSet<GameObject>();

  public static GameManager instance;

  [SerializeField]
  private PlayerDataSO _playerData;

  [Header("Configuration")]
  public BoolReactiveProperty enableHandleMove = new BoolReactiveProperty(true);
  public BoolReactiveProperty enableLineMove = new BoolReactiveProperty(true);
  public BoolReactiveProperty enableLineMoveUpdatesRelatedHandles = new BoolReactiveProperty(true);
  public BoolReactiveProperty enableLineDelete = new BoolReactiveProperty(true);

  private void Awake() {

    if (instance == null) {

      instance = this;
    }

    selectedObject = selectedObjectSource.AsObservable();
  }

  private void Start() {

    CreateHighlightObservable();
    CollectExistingHandles();

    CreateHandleMoveObservable();
    CreateLineMoveObservable();
    CreateLineMoveUpdatesRelatedHandlesObservable();
    CreateLineDeleteObservable();
  }

  private void CreateLineMoveUpdatesRelatedHandlesObservable() {

    enableLineMoveUpdatesRelatedHandles
      .Do((bool isActive) => 
        FindObjectsAndSetValue<MoveLine>("MoveUpdatesRelatedHandles", isActive)
      )
      .Subscribe()
      .AddTo(this)
    ;
  }

  private void CreateLineMoveObservable() {

    enableLineMove
      .Do((bool isActive) => FindObjectsAndSetValue<MoveLine>("IsActive", isActive))
      .Subscribe()
      .AddTo(this)
    ;
  }

  private void CreateHandleMoveObservable() {
    
    enableHandleMove
      .Do((bool isActive) => FindObjectsAndSetValue<MoveHandle>("IsActive", isActive))
      .Subscribe()
      .AddTo(this)
    ;
  }

  private void CreateLineDeleteObservable() {

    enableLineDelete
      .Do((bool isActive) => FindObjectsAndSetValue<RemoveLine>("IsActive", isActive))
      .Subscribe()
      .AddTo(this)
    ;
  }

  private void FindObjectsAndSetValue<T>(string property, bool value) where T: UnityEngine.Object {

    T[] foundObjects = FindObjectsOfType<T>();
    for (int i = 0; i < foundObjects.Length; i++) {

      typeof(T).GetField(property).SetValue(foundObjects[i], value);
    }
  }

  private void HighlightRelated(GameObject obj, Boolean highlight) {

    HashSet<GameObject> pool = GetPoolForHandle(obj);
    foreach (GameObject handle in pool) {

      Highlighter highlighter = handle.GetComponentInChildren<Highlighter>();
      if (highlight == true) {

        highlighter.Highlight();
      } else {

        highlighter.UnHighlight();
      }
    }
  }

  private void CreateHighlightObservable() {

    selectedObject
      .Buffer(2, 1)
      .Do((IList<GameObject> objs) => {

        if (objs[0] != null) {

          HighlightRelated(objs[0], false);
        }

        if (objs[1] != null) {

          HighlightRelated(objs[1], true);
        }
      })
      .Select((IList<GameObject> objs) => objs[1])
      .Subscribe()
      .AddTo(this)
    ;
  }

  private void CollectExistingHandles() {

    Component[] handles = UnityEngine.Object.FindObjectsOfType<Handle>();
    for (int i = 0; i < handles.Length; i++) {

      GameObject currentHandle = handles[i].gameObject;
      handlePool.Add(currentHandle);
    }
  }

  public void AddToHandlePool(GameObject handle) {

    handlePool.Add(handle);
  }

  public HashSet<GameObject> GetPoolForHandle(GameObject handle) {

    Vector2 pos = (Vector2) handle.transform.position;

    return GetPoolForPos(pos);
  }

  public HashSet<GameObject> GetPoolForPos(Vector2 pos) {

    HashSet<GameObject> pool = new HashSet<GameObject>();

    foreach (GameObject poolItem in handlePool) {

      Vector2 poolItemPos = poolItem.transform.position;
      if (poolItemPos == pos) {

        pool.Add(poolItem);
      }
    }

    return pool;
  }

  public void RemoveFromHandlePool(GameObject handle) {

    handlePool.Remove(handle);
  }

  public bool IsLineExisting(GameObject startHandle, GameObject endHandle) {

    HashSet<GameObject> startPool = GetPoolForHandle(startHandle);
    HashSet<GameObject> linesOfStartPos = GetLinesForHandles(startPool);
    HashSet<GameObject> endPool = GetPoolForHandle(endHandle);
    HashSet<GameObject> linesOfEndPos = GetLinesForHandles(endPool);

    HashSet<GameObject> allLines = new HashSet<GameObject>(linesOfStartPos);
    allLines.UnionWith(linesOfEndPos);

    bool isExisting = allLines.Count < (linesOfStartPos.Count + linesOfEndPos.Count);

    return isExisting;
  }

  public HashSet<GameObject> GetLinesForHandles(HashSet<GameObject> handlePool) {

    return handlePool.ToList().Aggregate(new HashSet<GameObject>(), (HashSet<GameObject> acc, GameObject current) => {

      acc.Add(current.transform.parent.gameObject);

      return acc;
    });
  }
}
