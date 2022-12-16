using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UniRx;

public class GlossaryEditor : OdinMenuEditorWindow {

  private string glossaryEntriesFolder = "Assets/DataObjects/Glossary";

  private OdinMenuTree tree;
  private TextMeshProRendererFactory tmpFactory;

  private GlossaryEntrySO currentSelection;
  private GlossaryEntrySO previousSelection;

  [MenuItem("Tools/Glossar Editor")]
  public static void OpenWindow() {

    GetWindow<GlossaryEditor>().Show();
  }

  private CreateNewGlossaryEntry createGlossaryEntry;

  protected override void OnDestroy() {

    RenderTmpContent();

    base.OnDestroy();

    if(createGlossaryEntry != null) {

      DestroyImmediate(createGlossaryEntry.entry);
    }
  }

  protected override OdinMenuTree BuildMenuTree() {

    tree = new OdinMenuTree();
    tmpFactory = new TextMeshProRendererFactory();

    createGlossaryEntry = new CreateNewGlossaryEntry(glossaryEntriesFolder);
    tree.Add("Neuer Glossareintrag", createGlossaryEntry);
    tree.AddAllAssetsAtPath("Glossareinträge", glossaryEntriesFolder, typeof(GlossaryEntrySO))
      .SortMenuItemsByName()
    ;

    tree.Selection.SelectionChanged += OnSelectionChanged;

    return tree;
  }

  private void OnSelectionChanged(SelectionChangedType type) {

    if(type == SelectionChangedType.ItemAdded) {

      previousSelection = currentSelection;
      RenderTmpContent();

      currentSelection = tree.Selection.SelectedValue as GlossaryEntrySO;
    }
  }

  private void RenderTmpContent() {

    if(previousSelection != null) {

      previousSelection.scientific.tmpContent = tmpFactory.RenderMarkdownStringToTextMeshProString(previousSelection.scientific.content);
      previousSelection.easy.tmpContent = tmpFactory.RenderMarkdownStringToTextMeshProString(previousSelection.easy.content);
      previousSelection.medium.tmpContent = tmpFactory.RenderMarkdownStringToTextMeshProString(previousSelection.medium.content);
      EditorUtility.SetDirty(previousSelection);
    }
  }

  private class CreateNewGlossaryEntry {

    private string glossaryEntriesFolder;

    [LabelText("Dateiname")]
    [LabelWidth(100)]
    public string fileName = "Eintrag";

    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    public GlossaryEntrySO entry;

    public CreateNewGlossaryEntry(string glossaryEntriesFolder) {

      this.glossaryEntriesFolder = glossaryEntriesFolder;
      entry = ScriptableObject.CreateInstance<GlossaryEntrySO>();
      entry.name = "Neuer Eintrag";
    }

    [Button("Speichern")]
    private void CreateNewData() {

      AssetDatabase.CreateAsset(entry, glossaryEntriesFolder + "/" + fileName + ".asset");
      AssetDatabase.SaveAssets();

      entry = ScriptableObject.CreateInstance<GlossaryEntrySO>();
      entry.name = "Neuer Eintrag";
    }
  }
}
