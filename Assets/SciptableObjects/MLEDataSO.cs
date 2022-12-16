#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "MLE")]
public class MLEDataSO : ScriptableObject, UIDSearchableInterface, IEquatable<MLEDataSO>
{
  [SerializeField, ReadOnly]
  private string uid;
  [LabelText("MLE Name")]
  public string mleName;
  [LabelText("MLE Handout")]
  public MLEHandoutsSO mleHandout;
  [MultiLineProperty(5)]
  [LabelText("Introtext")]
  public string mleIntroText;
  [LabelText("Videoschnipsel")]
  public MLEVideoData[] videoData;
  [LabelText("Vorraussetzungen für diesen Inhalt")]
  public List<MLEDataSO> requiredProgress = new List<MLEDataSO>();

  public int GetMaxPoints() {

    return videoData.Sum((MLEVideoData videoDataEntry) => 2 * videoDataEntry.questions.Length);
  }

  public string UID {
    get {
      return uid;
    }
  }

  public bool IsUnlocked()
  {
    GameController controller = GameController.GetInstance();
    foreach(MLEDataSO requirement in requiredProgress)
    {
      if(requirement != null && controller.gameState.taskHistoryData.GetTopicProgressIndex(requirement) < 2f)
        return false;
    }
    return true;
  }

  public bool Equals(MLEDataSO other)
  {
    return (this.uid != null && other != null && this.uid == other.UID);
  }

  private void OnValidate() {
#if UNITY_EDITOR
    if(uid == "" || uid == null) {
      uid = Guid.NewGuid().ToString();
      UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
  }
}

[Serializable]
public class MLEVideoData
{
  [LabelText("Videolink")]
  public string videoUrl;
  [LabelText("Titel des Abschnitts")]
  public string navTitle = "Video"; 
  [LabelText("Fragen")]
  public MLEQuiz[] questions;
}

[Serializable]
public class MLEQuiz
{
  [LabelText("Frage")]
  public string question;
  [LabelText("Auswahlmöglichkeiten")]
  public MLEQuizChoice[] choices;
}

[Serializable]
public class MLEQuizChoice
{
  [LabelText("Antworttext")]
  public string name;
  [LabelText("ist Antwort?")]
  public bool isAnswer;
  [LabelText("Feedbacknachricht")]
  [MultiLineProperty(3)]
  public string feedbackMessage;

  public MLEQuizChoice (string questiontext, string feedbacktext, bool correct = false)
  {
    name = questiontext;
    feedbackMessage = feedbacktext;
    isAnswer = correct;
  }
}