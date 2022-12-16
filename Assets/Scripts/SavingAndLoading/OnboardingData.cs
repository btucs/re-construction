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

[Serializable]
public class OnboardingData
{
  public bool profileUnlocked = false;
  public bool bibUnlocked = false;
  public bool mapUnlocked = false;
  public bool officeWelcomeFinished = false;
  public bool outroEventFinished = false;
  public bool endCinematicPlayed = false;
  public List<string> finishedEvents = new List<string>();

  public KonstruktorOnboardingData konstruktorData = new KonstruktorOnboardingData();

  public bool OnboardingFinished()
  {
    return (finishedEvents.Count > 1);
  }

  public bool EventEntryExists(string finLookUp)
  {
    foreach (string eventName in finishedEvents)
    {
      if (eventName == finLookUp)
      {
        return true;
      }
    }
    return false;
  }
}

[Serializable]
public class KonstruktorOnboardingData
{
  public bool konstruktorIntroductionFinished = false;
  public bool givenSearchedOnboardingFinished = false;
  public bool modulesOnboardingFinished = false;
  public bool analyzeToolUnlocked = false;
}
