#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class GameAssetsSO : ScriptableObject
{

#pragma warning disable CS0649
  [AssetList(Path = "/Prefabs/PlacesPrefabs", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private GameObject[] places;

  [AssetList(Path = "/DataObjects/TaskObjects", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private TaskObjectSO[] objects;

  [AssetList(Path = "/DataObjects/Tasks", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private TaskDataSO[] tasks;


  [AssetList(Path = "/DataObjects/MapData/Areas", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private AreaDataSO[] mapAreas;

  [AssetList(Path = "/DataObjects/MapData/Tweets", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private TweetData[] tweets;

  [AssetList(Path = "/DataObjects/Characters", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private CharacterSO[] characters;

  [AssetList(Path = "/DataObjects/Items", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private PermanentItem[] items;

  [AssetList(Path = "/DataObjects/MLE", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private MLEDataSO[] mles;

  [AssetList(Path = "/DataObjects/Quests", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private MainQuestSO[] quests;

  [AssetList(Path = "/DataObjects/Handouts", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private MLEHandoutsSO[] handouts;

  [AssetList(Path = "/DataObjects/KonstruktorModules", AutoPopulate = false)]
  [ShowInInspector]
  [SerializeField]
  private KonstructorModuleSO[] konstructorModules;

  [AssetList(Path = "/DataObjects/Quests/Events", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private ScriptedEventDataSO[] eventData;

  [AssetList(Path = "/DataObjects/Topics", AutoPopulate = true)]
  [ShowInInspector]
  [SerializeField]
  private TopicSO[] topics;

#pragma warning restore CS0649

  public VariableInfoSO variableInfo;
  public PlayerRankSO playerRanks;
  public AssessmentFeedbackSO feedbackInfo;
  public GameConfigSO gameConfig;

  public GameObject FindPlace(string name) {

    return places.FirstOrDefault((GameObject item) => item.name == name);
  }

  public TaskObjectSO FindTaskObject(string name) {

    if(IsGUID(name)) {

      return Find<TaskObjectSO>(objects, name);
    }

    return objects.FirstOrDefault((TaskObjectSO item) => item.objectName == name);
  }

  public TaskDataSO FindTask(string name) {

    if(IsGUID(name)) {

      return Find<TaskDataSO>(tasks, name);
    }

    return tasks.FirstOrDefault((TaskDataSO item) => item.taskName == name);
  }

  public List<TaskDataSO> GetTasksOfTopic(TopicSO taskType)
  {
    List<TaskDataSO> returnList = new List<TaskDataSO>();
    for(int i=0; i<tasks.Length; i++)
    {
      if(tasks[i].topic != null && tasks[i].topic.name == taskType.name)
        returnList.Add(tasks[i]);
    }
    Debug.Log("Anzahl der gefundenen Aufgaben zum Thema: " + taskType.name + " ist: " + returnList.Count);
    return returnList;
  }

  public List<TopicSO> GetAllTopics()
  {
    List<TopicSO> returnList = new List<TopicSO>();
    for(int i=0; i < tasks.Length; i++)
    {
      if(tasks[i].topic != null && returnList.Contains(tasks[i].topic) == false)
      {
        returnList.Add(tasks[i].topic);
      }
    }
    return returnList;
  }

  public TweetData[] GetTweets() {

    return tweets;
  }

  public PermanentItem FindItem(string itemID) {

    if(IsGUID(itemID)) {

      return Find<PermanentItem>(items, itemID);
    }

    return null;
  }

  public PermanentItem[] GetItems() {
    //PermanentItem[] returnItems = new PermanentItem[items.Length];
    //items.CopyTo(returnItems, 0);
    return items;
  }

  public List<CosmeticItem> GetCosmeticsOfCategory(CosmeticCategory _category, bool onlyDefault = false) {

    List<CosmeticItem> returnItems = new List<CosmeticItem>();

    for(int i = 0; i < items.Length; i++) {

      if(items[i] is CosmeticItem) {

        CosmeticItem cosmeticItem = items[i] as CosmeticItem;
        if(cosmeticItem.category == _category) {

          if(onlyDefault == false || cosmeticItem.availableOnGameStart == true) {

            returnItems.Add(cosmeticItem);
          }
        }
      }
    }
    return returnItems;
  }

  public AreaDataSO FindAreaData(string areaID) {

    if(IsGUID(areaID)) {

      return Find<AreaDataSO>(mapAreas, areaID);
    }

    return null;
  }

  public TweetData FindTweet(string tweetID) {

    if(IsGUID(tweetID)) {

      return Find<TweetData>(tweets, tweetID);
    }

    return null;
  }

  public CharacterSO FindCharacter(string name) {

    if(IsGUID(name)) {

      return Find<CharacterSO>(characters, name);
    }

    return characters.FirstOrDefault((CharacterSO item) => item.characterName == name);
  }

  public MLEDataSO FindMLE(string name) {

    if(IsGUID(name)) {

      return Find<MLEDataSO>(mles, name);
    }

    return null;
  }

  public MainQuestSO FindQuestByID(string name) {

    if(name == null || name.Length == 0)
    {
      return null;
    }

    if(IsGUID(name)) {

      return Find<MainQuestSO>(quests, name);
    }

    MainQuestSO shortIDResult = FindQuestByShortID(name);

    if(shortIDResult != null) {

      return shortIDResult;
    } else {

      return quests.FirstOrDefault((MainQuestSO quest) => quest.title == name);
    }
  }

  public MainQuestSO FindQuestByShortID(string searchname) {

    return quests.FirstOrDefault((MainQuestSO quest) => quest.UID.StartsWith(searchname));
  }

  public MainQuestSO FindQuest(string name) {

    foreach(MainQuestSO quest in quests) {

      if(quest.title == name) {

        return quest;
      }
    }

    return FindQuestByID(name);
  }

  public KonstructorModuleSO FindKonstructorModule(string id) {

    return Find<KonstructorModuleSO>(konstructorModules, id);
  }

  public KonstructorModuleSO FindKonstructorModule(KonstruktorModuleType type) {

    return konstructorModules.FirstOrDefault((KonstructorModuleSO module) => module.moduleType == type);
  }

  private bool IsGUID(string guidOrName) {

    bool isValid = Guid.TryParse(guidOrName, out Guid guidOutput);

    return isValid;
  }

  private T Find<T>(T[] list, string uid) where T : UIDSearchableInterface {

    return list.FirstOrDefault((T item) => item.UID == uid);
  }

  public MLEHandoutsSO[] GetHandouts() {

    return handouts;
  }

  public MLEDataSO[] GetMLEs() {

    return mles;
  }

  public ScriptedEventDataSO[] GetScriptedEvents() {

    return eventData;
  }

  public TopicSO FindTopicByMLE(MLEDataSO mle)
  {
    return topics.FirstOrDefault((TopicSO topic) => topic.mles.Contains(mle));
  }

  public TopicSO FindTopicByGlossary(GlossaryEntrySO glossary)
  {
    return topics.FirstOrDefault((TopicSO topic) => topic.glossaryLinks.Contains(glossary));
  }

  public TopicSO FindTopicByInternalName(string topicName)
  {
    return topics.FirstOrDefault((TopicSO topic) => topic.internalName == topicName);
  }

#if UNITY_EDITOR
  [Button(Name = "Save")]
  private void SaveAsset() {

    EditorUtility.SetDirty(this);
  }

  [Button(Name = "MarkDirtyAndSave")]
  private void MarkDirty() {

    Action<ScriptableObject> SetDirtyFkt = (ScriptableObject item) => EditorUtility.SetDirty(item);

    objects.ForEach(SetDirtyFkt);
    tasks.ForEach(SetDirtyFkt);
    mles.ForEach(SetDirtyFkt);
    characters.ForEach(SetDirtyFkt);

    AssetDatabase.SaveAssets();
  }
#endif
}
