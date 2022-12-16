using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class TaskEditor : OdinMenuEditorWindow {

  private string tasksFolder = "Assets/DataObjects/Tasks";
  private string objectsFolder = "Assets/DataObjects/TaskObjects";

  [MenuItem("Tools/Aufgaben Editor")]
  public static void OpenWindow() {

    GetWindow<TaskEditor>().Show();
  }

  private CreateNewTaskData createNewTaskData;
  private CreateNewTaskObjectData createNewObjectData;

  protected override void OnDestroy() {

    base.OnDestroy();

    if(createNewTaskData != null) {

      DestroyImmediate(createNewTaskData.taskData);
    }

    if(createNewObjectData != null) {

      DestroyImmediate(createNewObjectData.taskObject);
    }
  }

  protected override OdinMenuTree BuildMenuTree() {

    OdinMenuTree tree = new OdinMenuTree();

    createNewTaskData = new CreateNewTaskData(tasksFolder);
    tree.Add("Neue Aufgabe", createNewTaskData);
    tree.AddAllAssetsAtPath("Aufgaben", tasksFolder, typeof(TaskDataSO), true, true)
      .SortMenuItemsByName()
    ;

    createNewObjectData = new CreateNewTaskObjectData(objectsFolder);
    tree.Add("Neues Objekt", createNewObjectData);
    tree.AddAllAssetsAtPath("Objekte", objectsFolder, typeof(TaskObjectSO), true, true)
      .SortMenuItemsByName()
    ;

    return tree;
  }

  private class CreateNewTaskData {

    private string tasksFolder;

    [LabelText("Dateiname")]
    public string fileName = "01-Aufgabe";

    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    public TaskDataSO taskData;

    public CreateNewTaskData(string tasksFolder) {

      this.tasksFolder = tasksFolder;
      taskData = ScriptableObject.CreateInstance<TaskDataSO>();
      taskData.name = "Neue Aufgabe";
    }

    [Button("Aufgabe speichern")]
    private void CreateNewData() {

      AssetDatabase.CreateAsset(taskData, tasksFolder + "/" + fileName + ".asset");
      AssetDatabase.SaveAssets();

      taskData = ScriptableObject.CreateInstance<TaskDataSO>();
      taskData.name = "Neue Aufgabe";
    }
  }

  private class CreateNewTaskObjectData {

    private string objectsFolder;

    [LabelText("Dateiname")]
    [LabelWidth(150)]
    public string fileName = "01-Objekt";

    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    public TaskObjectSO taskObject;

    public CreateNewTaskObjectData(string objectsFolder) {

      this.objectsFolder = objectsFolder;

      taskObject = ScriptableObject.CreateInstance<TaskObjectSO>();
      taskObject.name = "Neues Objekt";
    }

    [Button("Objekt Speichern")]
    private void CreateNewData() {

      AssetDatabase.CreateAsset(taskObject, objectsFolder + "/" + fileName + ".asset");
      AssetDatabase.SaveAssets();

      taskObject = ScriptableObject.CreateInstance<TaskObjectSO>();
      taskObject.name = "Neues Objekt";
    }
  }
}
