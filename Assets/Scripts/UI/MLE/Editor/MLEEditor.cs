using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

public class MLEEditor : OdinMenuEditorWindow {

  private string mleFolder = "Assets/DataObjects/MLE";

  [MenuItem("Tools/MLE Editor")]
  public static void OpenWindow() {

    GetWindow<MLEEditor>().Show();
  }

  private CreateNewMLEData createNewMLEData;

  protected override void OnDestroy() {
    base.OnDestroy();

    if(createNewMLEData != null) {

      DestroyImmediate(createNewMLEData.mleData);
    }
  }

  protected override OdinMenuTree BuildMenuTree() {

    OdinMenuTree tree = new OdinMenuTree();

    createNewMLEData = new CreateNewMLEData(mleFolder);
    tree.Add("Neue MLE", createNewMLEData);
    tree.AddAllAssetsAtPath("MLEs", mleFolder, typeof(MLEDataSO))
      .SortMenuItemsByName()
    ;

    return tree;
  }

  private class CreateNewMLEData {

    private string mleFolder;

    [LabelText("Dateiname")]
    public string fileName = "01-MLE";

    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    public MLEDataSO mleData;

    public CreateNewMLEData(string mleFolder) {

      this.mleFolder = mleFolder;

      mleData = ScriptableObject.CreateInstance<MLEDataSO>();
      mleData.mleName = "Neue MLE";
    }

    [Button("MLE speichern")]
    private void CreateNewData() {

      AssetDatabase.CreateAsset(mleData, mleFolder + "/" + fileName + ".asset");
      AssetDatabase.SaveAssets();

      mleData = ScriptableObject.CreateInstance<MLEDataSO>();
      mleData.mleName = "Neue MLE";
    }
  }
}