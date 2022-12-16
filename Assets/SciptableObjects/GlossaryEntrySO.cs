#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public enum GlossaryType {
  Scientific,
  Easy,
  Medium
}

public class GlossaryEntrySO : ScriptableObject {

  [Serializable]
  public class GlossaryEntry  {
    [Multiline(13)]
    [LabelText("Inhalt")]
    [LabelWidth(100)]
    [VerticalGroup("entry/left")]
    [DetailedInfoBox(
      "Hinweise für Links und Listen",
      @"Links zu anderen Glossareinträgen [Name im Text](glossary:Glossarüberschrift). Wenn Glossarüberschrift aus mehreren Wörtern besteht müssen diese mit _ getrennt werden anstatt mit Leerzeichen.

Listen können den folgenden Buchstaben gestartet werden: -, *, 1., a, A
Wichtig für Einrückungen ist, dass der eingerückte Teil dort anfängt wo in der nicht eingerückten Zeile der Wert steht.
Zum Beispiel:
- hier gehts los
  - die nächste Zeile startet zwei Zeichen vom Anfang entfernt '- ' = 2 Zeichen

1. nummerierte Liste brauchen ein zeichen mehr
   1. ein neuer Unterpunkt fängt wieder bei 1 an und started 3 Zeichen vom Anfang entfernt '1. ' = 3 Zeichen
   2. zweiter Unterpunkt
2. oben gehts weiter
"
    )]
    public string content;

    [PreviewField(250, ObjectFieldAlignment.Right)]
    [HideLabel]
    [HorizontalGroup("entry", 250), VerticalGroup("entry/right")]
    public Sprite image;

    [HideInInspector]
    [SerializeField]
    public string tmpContent;
  }

  [LabelText("Überschrift")]
  [LabelWidth(100)]
  public string headline;

  [BoxGroup("einfach")]
  [HideLabel()]
  public GlossaryEntry easy = new GlossaryEntry();
  [BoxGroup("mittel")]
  [HideLabel()]
  public GlossaryEntry medium = new GlossaryEntry();
  [BoxGroup("schwer")]
  [HideLabel()]
  public GlossaryEntry scientific = new GlossaryEntry();

  public string tmpHeadline;

  public GlossaryEntry GetContent(GlossaryType type) {

    switch(type) {

      case GlossaryType.Scientific: return scientific;
      case GlossaryType.Easy: return easy;
      case GlossaryType.Medium: return medium;
    }

    return default;
  }
}
