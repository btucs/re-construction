using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public sealed class ReturnToSceneGUI
{
	private string previousScene;
	//public GameObject prefabInstance;
	

	//var returnButton = new ReturnToSceneGUI(currentScene);
	public ReturnToSceneGUI(string lastSceneName) {
		SceneView.duringSceneGui += RenderSceneGUI;
		previousScene = lastSceneName;
	}
	
	public void RenderSceneGUI(SceneView sceneview) {
		var style = new GUIStyle();
		style.margin = new RectOffset(10, 10, 10, 10);
		
		Handles.BeginGUI();
			GUILayout.BeginArea(new Rect(20, 20, 180, 300), style);
				var rect = EditorGUILayout.BeginVertical();
				GUI.Box(rect, GUIContent.none);
				
				if (GUILayout.Button("Speichern und zurükkehren", new GUILayoutOption[0])) {
					//PrefabUtility.ReplacePrefab(prefabInstance, PrefabUtility.GetPrefabParent(prefabInstance), ReplacePrefabOptions.ConnectToPrefab);
					SceneView.duringSceneGui -= RenderSceneGUI;
					EditorSceneManager.OpenScene(previousScene);
				}
				
				EditorGUILayout.EndVertical();
			GUILayout.EndArea();
		Handles.EndGUI();
	}
}