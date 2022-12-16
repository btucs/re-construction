using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;

public class UIPanelManagerPropertyProcessor : OdinPropertyProcessor<UIPanelManager>, IDisposable {

  private List<UIPanelManager.UIPanelItem> syncList;
  private GameObjectObservableList panelsToHandle;
  public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos) {

    UIPanelManager manager = Property.ValueEntry.WeakSmartValue as UIPanelManager;
    panelsToHandle = manager.panelsToHandle;

    FieldInfo prop = manager.GetType()
      .GetField("handledItems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
    ;

    syncList = (List<UIPanelManager.UIPanelItem>)prop.GetValue(manager);

    panelsToHandle.ItemAdded.AddListener(OnAdd);
    panelsToHandle.ItemRemoved.AddListener(OnRemove);
    panelsToHandle.ItemReplaced.AddListener(OnReplace);
  }
  
  public void Dispose() {

    panelsToHandle.ItemAdded.RemoveListener(OnAdd);
    panelsToHandle.ItemRemoved.RemoveListener(OnRemove);
    panelsToHandle.ItemReplaced.RemoveListener(OnReplace);
  }

  private void OnAdd(CollectionAddEventData<GameObject> eventData) {

    UIPanelManager.UIPanelItem newItem = new UIPanelManager.UIPanelItem() {
      targetPanel = eventData.Value
    };

    syncList.Add(newItem);
    List<GameObject> handledObjects = panelsToHandle.ToList();
    UIPanelItemSortOrderComparer comparer = new UIPanelItemSortOrderComparer(handledObjects);
    syncList.Sort(comparer);
    AddToShowHide(handledObjects);
  }

  private void OnRemove(CollectionRemoveEventData<GameObject> eventData) {

    syncList.RemoveAll((UIPanelManager.UIPanelItem item) => item.targetPanel == eventData.Value);
    RemoveFromShowHide(eventData.Value);
  }

  private void OnReplace(CollectionReplaceEventData<GameObject> eventData) {

    UIPanelManager.UIPanelItem newItem = new UIPanelManager.UIPanelItem() {
      targetPanel = eventData.NewValue
    };

    syncList[eventData.Index] = newItem;
    UpdateShowHide(eventData.OldValue, eventData.NewValue);
  }

  /*private void OnMove(CollectionMoveEventData<GameObject> eventData) {

    Debug.Log("Move");

    List<GameObject> handledItems = panelsToHandle.ToList();
    UIPanelShowHideSortOrderComparer comparer = new UIPanelShowHideSortOrderComparer(handledItems);
    foreach(UIPanelManager.UIPanelItem item in syncList) {

      item.onShow.Sort(comparer);
      item.onHide.Sort(comparer);
    }
  }*/

  private void AddToShowHide(List<GameObject> handledItems) {

    UIPanelShowHideSortOrderComparer comparer = new UIPanelShowHideSortOrderComparer(handledItems);

    foreach (UIPanelManager.UIPanelItem item in syncList) {

      foreach(GameObject handledItem in handledItems) {

        if(item.targetPanel != handledItem) {

          int index = GetIndexOf(item.onShow, handledItem);
          if(index == -1) {

            UIPanelManager.ShowHideItem newItem = new UIPanelManager.ShowHideItem() {
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

  private void RemoveFromShowHide(GameObject handledItem) {

    syncList.ForEach((UIPanelManager.UIPanelItem item) => {

      Func<UIPanelManager.ShowHideItem, bool> CanStay = (UIPanelManager.ShowHideItem compareTo) => compareTo.targetPanel != handledItem;

      item.onHide = item.onHide.Where(CanStay).ToList();
      item.onShow = item.onShow.Where(CanStay).ToList();
    });
  }

  private void UpdateShowHide(GameObject oldValue, GameObject newValue) {

    syncList.ForEach((UIPanelManager.UIPanelItem item) => {

      FindAndReplace(item.onShow, oldValue, newValue);
      FindAndReplace(item.onHide, oldValue, newValue);
    });
  }

  private void FindAndReplace(List<UIPanelManager.ShowHideItem> list, GameObject oldValue, GameObject newValue) {

    int index = GetIndexOf(list, oldValue);

    UIPanelManager.ShowHideItem newItem = new UIPanelManager.ShowHideItem() {
      targetPanel = newValue
    };

    if(index == -1) {

      list.Add(newItem);
    } else {

      list[index] = newItem;
    }
  }

  private int GetIndexOf(List<UIPanelManager.ShowHideItem> list, GameObject toFind) {

    UIPanelManager.ShowHideItem foundItem = list.FirstOrDefault((UIPanelManager.ShowHideItem item) => item.targetPanel == toFind);

    if(foundItem == null) {

      return -1;
    }

    return list.IndexOf(foundItem);
  }
}