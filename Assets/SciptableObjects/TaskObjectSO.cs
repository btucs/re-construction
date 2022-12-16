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
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Tasks/TaskObject")]
public class TaskObjectSO : ScriptableObject, UIDSearchableInterface {

  [SerializeField, ReadOnly, LabelWidth(150)]
  private string uid;
  [LabelText("Name")]
  [VerticalGroup("object/left")]
  [LabelWidth(150)]
  public string objectName;
  [MultiLineProperty(5)]
  [LabelText("Objektbeschreibung")]
  [VerticalGroup("object/left")]
  [LabelWidth(150)]
  public string objectDescription;
  [AssetsOnly]
  [PreviewField(100, ObjectFieldAlignment.Right)]
  [HorizontalGroup("object", 100), VerticalGroup("object/right")]
  [HideLabel]
  public GameObject objectPrefab;
  [PreviewField(100, ObjectFieldAlignment.Right)]
  [HorizontalGroup("object", 100)]
  [HideLabel]
  public Sprite objectThumbnail;

  [LabelText("Individueller Kamera-fokus")]
  public Vector3 cameraOffset = new Vector3 (0f, 0f, 0f);
  [LabelText("Position des Objekt-Buttons")]
  public Vector2 markerOffset = new Vector2 (0f, 0f);
  [LabelText("Kamerazoomfaktor beim untersuchen")]
  public float cameraZoom = 1.25f;

  [LabelText("Aufgaben"), HorizontalGroup("Tasks", 0.85f)]
  [ListDrawerSettings(HideAddButton = true, Expanded = true)]
  public List<ObjectTaskData> taskInfos = new List<ObjectTaskData>();
  [HorizontalGroup("Tasks", 0.15f)]
  [Button("Add Task")]
  private void AddNewTaskEntry()
  {
    taskInfos.Add(new ObjectTaskData(objectPrefab));
  }

  public List<TaskDataSO> GetTasks()
  {
    List<TaskDataSO> returnList = new List<TaskDataSO>();
    foreach(ObjectTaskData taskInfo in taskInfos)
    {
      returnList.Add(taskInfo.task);
    }
    return returnList;
  }


  public string UID {
    get {
      return uid;
    }
  }

  private void OnValidate() {
#if UNITY_EDITOR
    if(uid == "" || uid == null) {
      uid = Guid.NewGuid().ToString();
      UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
  }
}

[System.Serializable]
public class ObjectTaskData
{
  public TaskDataSO task;
  public Vector2 taskButtonPosition;
  public bool isRepeatable = true;

  [ListDrawerSettings(HideAddButton = true, HideRemoveButton = false, Expanded = true)]
  public List<HiddenTaskVariable> hiddenVariables = new List<HiddenTaskVariable>();
#if UNITY_EDITOR
  [Button("Create Variables from Task")]
  private void CreateHiddenVariables()
  {
    OnCreateVarButton();
  }
#endif
  private GameObject objectPrefab;

  public ObjectTaskData(GameObject prefabRef)
  {
    objectPrefab = prefabRef;
  }

  private void OnCreateVarButton()
  {
    List<Vector3> prevSetLocations = new List<Vector3>();
    foreach(HiddenTaskVariable prevVar in hiddenVariables)
    {
      prevSetLocations.Add(prevVar.localAnalyzePosition);
    }
    hiddenVariables.Clear();

    Debug.Log("Setting Variables from assigned Task Data");
    if(task != null)
    {
      for(int j = 0; j < task.steps.Length; j++)
      {
        for(int i = 0; i < task.steps[j].inputs.Length; i++)
        {
          Vector3 itemPos = new Vector3(2f * i, 2f * j, 0f);
          if(i < prevSetLocations.Count)
          {
            itemPos = prevSetLocations.ElementAt(i);
          }
          HiddenTaskVariable newHiddenVar = new HiddenTaskVariable(task.steps[j].inputs[i], itemPos, objectPrefab);
          hiddenVariables.Add(newHiddenVar);
        }
      }  
    } else {
      Debug.LogError("Task is not set!");   
    }
  }
}

[System.Serializable]
public class HiddenTaskVariable
{
  [ReadOnly]
  public TaskInputVariable itemData;

  [HorizontalGroup("ItemPosition", 0.6f, LabelWidth = 50)]
  [LabelText("Position des Werts im Analyze Tool.")]
  public Vector3 localAnalyzePosition;
#if UNITY_EDITOR
  [HorizontalGroup("ItemPosition")]
  [Button("Set Item Position")]
  private void PositionButtonFunction()
  {
    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

    string previousSceneName = EditorSceneManager.GetActiveScene().path;
    EditorSceneManager.OpenScene("Assets/Scenes/EditorOnlyScenes/HiddenItemPositionScene.unity");
    
    GameObject prefabObject = GameObject.Instantiate(objectPrefab) as GameObject;
    
    Transform positioningTransform = GameObject.FindWithTag("HiddenItemPositioner").transform;
    positioningTransform.SetParent(prefabObject.transform, true);
    positioningTransform.localPosition = localAnalyzePosition;
    Camera.main.transform.position = new Vector3(positioningTransform.position.x, positioningTransform.position.y, -10);

    var returnButton = new PositionEditorSubmit(previousSceneName, this, positioningTransform);
  }
#endif
  private GameObject objectPrefab;

  public HiddenTaskVariable(TaskInputVariable item, Vector3 posData, GameObject prefab)
  {
    itemData = item;
    localAnalyzePosition = posData;

    objectPrefab = prefab;
  }
}



#if UNITY_EDITOR
public sealed class PositionEditorSubmit
{
  private string previousScene;
  private Transform positioningElement;
  private HiddenTaskVariable hiddenVarToEdit;

  //var returnButton = new ReturnToSceneGUI(currentScene);
  public PositionEditorSubmit(string lastSceneName, HiddenTaskVariable hiddenItemData, Transform posMarker) {
    SceneView.duringSceneGui += RenderSceneGUI;
    previousScene = lastSceneName;
    hiddenVarToEdit = hiddenItemData;

    positioningElement = posMarker;

  }
  
  public void RenderSceneGUI(SceneView sceneview) {
    var style = new GUIStyle();
    style.margin = new RectOffset(30, 30, 30, 30);
    
    Handles.BeginGUI();
      GUILayout.BeginArea(new Rect(40, 40, 180, 300), style);
        var rect = EditorGUILayout.BeginVertical();
        GUI.Box(rect, GUIContent.none);
        
        if (GUILayout.Button("Speichern und zurückkehren", GUILayout.Height(100))) {
          //PrefabUtility.ReplacePrefab(prefabInstance, PrefabUtility.GetPrefabParent(prefabInstance), ReplacePrefabOptions.ConnectToPrefab);
          SceneView.duringSceneGui -= RenderSceneGUI;
          
          if(hiddenVarToEdit != null && positioningElement != null)
          {
            hiddenVarToEdit.localAnalyzePosition = positioningElement.localPosition;
          } else { Debug.Log("Something was not set correctly..."); }

          EditorSceneManager.OpenScene(previousScene);
          
        }
        
        EditorGUILayout.EndVertical();
      GUILayout.EndArea();
    Handles.EndGUI();
  }
}
#endif