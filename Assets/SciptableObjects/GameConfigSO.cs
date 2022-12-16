using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ApiConfig
{
  public string accessToken;
  public string apiBaseUrl;
}

[Serializable]
public class TuPConfig
{
  public string apiBaseURL;
  public string accessToken;
}

public class GameConfigSO : ScriptableObject
{
  public ApiConfig apiConfig = new ApiConfig();
  public TuPConfig tupConfig = new TuPConfig();
}
