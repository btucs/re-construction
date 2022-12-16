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
using Sirenix.OdinInspector;
using UniRx;

public class UIPanelManager : MonoBehaviour {

  public GameObjectObservableList panelsToHandle = new GameObjectObservableList();

  [SerializeField, HideInInspector]
  private GameObject[] serializableHandledPanels;

  [SerializeField, ShowInInspector, ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, Expanded = true, DraggableItems = false)]
  private List<UIPanelItem> handledItems = new List<UIPanelItem>();

  private static UIPanelManager instance;
  public static UIPanelManager Instance
  {
    get { return instance; }
  }

  private void Awake ()
  {
    if (instance == null) { instance = this; }
  }


  public void Show(string handledPanelName) {

    ShowHide(handledPanelName, true);
  }

  public void Hide(string handledPanelName) {

    ShowHide(handledPanelName, false);
  }

  public void ShowAndPersist(string handledPanelName) {

    ShowHide(handledPanelName, true, true);
  }

  public void Restore(string handledPanelName) {

    UIPanelItem targetPanelItem = FindPanelItem(handledPanelName);
    targetPanelItem.Restore();
  }

  public void Add(GameObject item, GameObject[] showOnShow, GameObject[] showOnHide) {

    panelsToHandle.Add(item);

    UIPanelItem newItem = new UIPanelItem() {
      targetPanel = item
    };

    handledItems.Add(newItem);

    AddToShowHide(panelsToHandle.ToList());

    SyncShowHide(newItem.onShow, showOnShow);
    SyncShowHide(newItem.onHide, showOnHide);
  }

  private void AddToShowHide(List<GameObject> handledItems) {

    UIPanelShowHideSortOrderComparer comparer = new UIPanelShowHideSortOrderComparer(handledItems);

    foreach(UIPanelItem item in this.handledItems) {

      foreach(GameObject handledItem in handledItems) {

        if(item.targetPanel != handledItem) {

          int index = GetIndexOf(item.onShow, handledItem);
          if(index == -1) {

            ShowHideItem newItem = new ShowHideItem() {
              targetPanel = handledItem
            };

            item.onShow.Add(newItem);
            item.onHide.Add(newItem.Clone());
          }
        }
      }

      item.onShow.Sort(comparer);
      item.onHide.Sort(comparer);
    }
  }

  private int GetIndexOf(List<ShowHideItem> list, GameObject toFind) {

    ShowHideItem foundItem = list.FirstOrDefault((ShowHideItem item) => item.targetPanel == toFind);

    if(foundItem == null) {

      return -1;
    }

    return list.IndexOf(foundItem);
  }

  private void SyncShowHide(List<ShowHideItem> showHideItems, GameObject[] shouldShowList) {

    foreach(ShowHideItem showHideItem in showHideItems) {

      if(shouldShowList.Contains(showHideItem.targetPanel)) {

        showHideItem.shouldShow = true;
      }
    }
  }

  private void ShowHide(string handledPanelName, bool shouldShow) {

    UIPanelItem targetPanelItem = FindPanelItem(handledPanelName);
    targetPanelItem.ShowHide(shouldShow);
  }

  private void ShowHide(string handledPanelName, bool shouldShow, bool shouldPersist) {

    UIPanelItem targetPanelItem = FindPanelItem(handledPanelName);
    targetPanelItem.ShowHide(shouldShow, shouldPersist);
  }

  [Serializable]
  public class UIPanelItem {

    [ReadOnly]
    public GameObject targetPanel;
    [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
    public List<ShowHideItem> onShow = new List<ShowHideItem>();
    [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
    public List<ShowHideItem> onHide = new List<ShowHideItem>();

    private List<ShowHideItem> persistedState = new List<ShowHideItem>();

    public void ShowHide(bool shouldShow) {

      targetPanel.SetActive(shouldShow);
      List<ShowHideItem> targetItems = shouldShow ? onShow : onHide;
      targetItems.ForEach((ShowHideItem item) => item.targetPanel.SetActive(item.shouldShow));
    }

    public void ShowHide(bool shouldShow, bool persistState) {

      if(shouldShow == false || persistState == false) {

        ShowHide(shouldShow);
      } else {

        targetPanel.SetActive(shouldShow);
        List<ShowHideItem> targetItems = shouldShow ? onShow : onHide;
        persistedState = new List<ShowHideItem>();

        targetItems.ForEach((ShowHideItem item) => {

          persistedState.Add(new ShowHideItem { shouldShow = item.targetPanel.activeSelf, targetPanel = item.targetPanel });
          item.targetPanel.SetActive(item.shouldShow);
        });
      }
    }

    // hides the panel and restores the state related items
    public void Restore() {

      targetPanel.SetActive(false);
      persistedState.ForEach((ShowHideItem item) => item.targetPanel.SetActive(item.shouldShow));
      persistedState = new List<ShowHideItem>();
    }
  }

  [Serializable]
  public class ShowHideItem {

    [ReadOnly]
    public GameObject targetPanel;
    public bool shouldShow = false;

    public ShowHideItem Clone() {

      return new ShowHideItem() {
        targetPanel = targetPanel
      };
    }
  }

  private UIPanelItem FindPanelItem(string handledPanelName) {

    return handledItems.FirstOrDefault((UIPanelItem panel) => panel.targetPanel.name == handledPanelName);
  }
}
