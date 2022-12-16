using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class SaveAssetsEditor {

  [MenuItem("Tools/Save Assets")]
  private static void SaveAssets() {

    AssetDatabase.SaveAssets();
  }
}
