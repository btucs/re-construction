using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class NewtonSoftJSONTopicSOConverter : JsonConverter<TopicSO>
{
  public override TopicSO ReadJson(JsonReader reader, Type objectType, TopicSO existingValue, bool hasExistingValue, JsonSerializer serializer)
  {
    GameController controller = GameController.GetInstance();

    return controller.gameAssets.FindTopicByInternalName((string)reader.Value);
  }

  public override void WriteJson(JsonWriter writer, TopicSO value, JsonSerializer serializer)
  {
    writer.WriteValue(value.ToString());
  }
}
