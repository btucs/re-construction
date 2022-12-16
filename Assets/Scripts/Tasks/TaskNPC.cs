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
using Sirenix.OdinInspector;

[Serializable]
public class TaskNPC {

  [LabelText("NPC")]
  public CharacterSO npc;
  [LabelText("Dialoge")]
  [MultiLineProperty(5)]
  [DetailedInfoBox(
    "Hinweise für Links, Listen, gegebene und gesuchte Werte",
    @"Links zum Glossar [Name im Text](Glossarüberschrift). Wenn Glossarüberschrift aus mehreren Wörtern besteht müssen diese mit _ getrennt werden anstatt mit Leerzeichen.

Gegebene und gesuchte Werte müssen vorher definiert werden\n
Gegebene Werte ~i:P1|optionale Beschreibung~ Beschreibung ist optional. Wird sie nicht gesetzt wird der Wert der Variablen angezeigt, z.B. 5 m
Gesuchte Werte ~o:F|Beschreibung~ Beschreibung ist pflicht, da der Wert ja gesucht ist.

Listen können den folgenden Buchstaben gestartet werden: -, *, 1., a, A
Wichtig für Einrückungen ist, dass der eingerückte Teil dort anfängt wo in der nicht eingerückten Zeile der Wert steht.
Zum Beispiel:
- hier gehts los
  - die nächste Zeile startet zwei Zeichen vom Anfang entfernt '- ' = 2 Zeichen

1. nummerierte Liste brauchen ein zeichen mehr
   1. ein neuer Unterpunkt fängt wieder bei 1 an und started 3 Zeichen vom Anfang entfernt '1. ' = 3 Zeichen
   2. zweiter Unterpunkt
2. oben gehts weiter"
  )]
  public string[] dialogs = new string[1];
}
