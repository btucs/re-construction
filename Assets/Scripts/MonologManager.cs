#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

using Markdig;
using Markdig.Syntax;
using Markdig.Renderers;

[Serializable]
public class Monolog {

  public string trigger;
  public string[] messages;
}

public class MonologManager : MonoBehaviour {

  public static MonologManager instance;
  
  public TextAsset monolog;
  public GameObject messageBox;
  public TextMeshProUGUI boxContent;
  public bool clearQueueOnTrigger = false;

  private Queue<string> currentMonolog = new Queue<string>();

  private Dictionary<string, string[]> messages = new Dictionary<string, string[]>();

  private void Awake() {

    if (instance == null) {

      instance = this;
    }
  }

  private void Start() {

    if (monolog != null) {

      Monolog[] monologs = JsonUtilityArray.FromJsonArray<Monolog>(monolog.text);
      for (int i = 0; i < monologs.Length; i++) {

        string[] parsedMessages = RenderTextToTextMeshPro(monologs[i].messages);
        messages.Add(monologs[i].trigger, parsedMessages);
      }
    }
  }

  public void Trigger(string trigger) {

    if (clearQueueOnTrigger == true) {

      currentMonolog = new Queue<string>();
    }

    messages.TryGetValue(trigger, out string[] currentMessages);
    if (currentMessages == null) {

      return;
    }

    for (int j = 0; j < currentMessages.Length; j++) {

      currentMonolog.Enqueue(currentMessages[j]);
    }

    messageBox.SetActive(true);
    boxContent.text = currentMonolog.Dequeue();
  }

  public void ShowCustomMessage(string[] message) {

    if (message.Length == 0) {

      return;
    }

    currentMonolog = new Queue<string>();

    for (int j = 0; j < message.Length; j++) {

      string parsedMessage = RenderTextToTextMeshPro(message[j]);
      currentMonolog.Enqueue(parsedMessage);
    }

    messageBox.SetActive(true);
    boxContent.text = currentMonolog.Dequeue();
  }

  /**
   * @return bool indicate if there are more items in the queue
   */
  public bool NextMessage() {

    int currentCount = currentMonolog.Count;
    if (currentCount > 0) {

      boxContent.text = currentMonolog.Dequeue();
    } else {

      messageBox.SetActive(false);
    }

    return currentCount - 1 > 0;
  }

  public bool HasNextMessage() {

    return currentMonolog.Count > 1;
  }

  private string[] RenderTextToTextMeshPro(string[] texts) {

    return texts.Select((text) => RenderTextToTextMeshPro(text)).ToArray();
  }

  private string RenderTextToTextMeshPro(string text) {

    StringWriter writer = new StringWriter();
    TextMeshProRenderer textRenderer = new TextMeshProRenderer(writer);
    MarkdownDocument document = Markdown.Parse(text);

    textRenderer.Render(document);
    writer.Flush();

    return writer.ToString();
  }
}
