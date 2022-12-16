using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

public class HandoutEditor : OdinMenuEditorWindow
{
  private string handoutFolder = "Assets/DataObjects/Handouts";

  [MenuItem("Tools/Handout Editor")]
  public static void OpenWindow()
  {
    GetWindow<HandoutEditor>().Show();
  }

  private CreateNewHandoutData createNewHandoutData;

  protected override void OnDestroy()
  {
    base.OnDestroy();

    if (createNewHandoutData != null)
    {
      DestroyImmediate(createNewHandoutData.handoutData);
    }
  }

  protected override OdinMenuTree BuildMenuTree()
  {
    OdinMenuTree tree = new OdinMenuTree();

    createNewHandoutData = new CreateNewHandoutData(handoutFolder);
    tree.Add("Neues Handout", createNewHandoutData);
    tree.AddAllAssetsAtPath("Handouts", handoutFolder, typeof(MLEHandoutsSO))
      .SortMenuItemsByName()
    ;
    return tree;
  }

  private class CreateNewHandoutData
  {
    private string handoutFolder;

    [LabelText("Dateiname")]
    public string fileName = "01-Handout";

    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    public MLEHandoutsSO handoutData;

    public CreateNewHandoutData(string handoutFolder)
    {
      this.handoutFolder = handoutFolder;

      handoutData = ScriptableObject.CreateInstance<MLEHandoutsSO>();
      handoutData.mleName = "Wählen Sie ein MLE aus";
    }

    [Button("Handout speichern")]

    private void CreateNewData()
    {
      AssetDatabase.CreateAsset(handoutData, handoutFolder + "/" + fileName + ".asset");
      AssetDatabase.SaveAssets();

      handoutData = ScriptableObject.CreateInstance<MLEHandoutsSO>();
      handoutData.mleName = "Wählen Sie ein MLE aus";
    }
  }
}