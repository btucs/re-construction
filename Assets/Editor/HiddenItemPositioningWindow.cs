using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class HiddenItemPositioningWindow : EditorWindow
{
	public static void ShowWindow()
	{
		GetWindow<HiddenItemPositioningWindow>("HiddenItemPositioner");
	}

    void OnGUI()
    {
        
    }
}
