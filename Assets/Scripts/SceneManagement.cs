#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
  public SceneFader fadeController;

  public void LoadGameWorldScene() {
    SceneManager.LoadScene("OfficeStartScene");
  }

  public void LoadMentorSelectionScene() {
    SceneManager.LoadScene("MentorSelection");
  }

  public void LoadMLEScene() {
    SceneManager.LoadScene("MLE");
  }

  public void LoadInsectMainScene()
  {
    if(Application.isPlaying)
    {
      Debug.Log("Loading Insect Scene");
      SceneManager.LoadScene("InsectArea");
    } else {
      Debug.Log("You are in Unity-Editor (non playmode) - would now switch scene in play mode");
    }
  }

  public void LoadlastScene()
  {
    string lastScene = GameController.GetInstance().gameState.gameworldData.lastScene;

    if(lastScene != null && Application.CanStreamedLevelBeLoaded(lastScene))
    {
      LoadScene(lastScene);
    } else {
      Debug.LogError("Couldnt load scene " + lastScene);
      LoadScene("Game Opening");
    }
  }

  public void LoadPandemicMainScene()
  {
    if(Application.isPlaying)
    {
      Debug.Log("Loading Insect Scene");
      SceneManager.LoadScene("PandemicArea");
    } else {
      Debug.Log("You are in Unity-Editor (non playmode) - would now switch scene in play mode");
    }
  }

  public void LoadOnboardingSecondPart() {
    //Reset onboarding 
    Debug.Log("Resetting onboarding state in SceneManagement - loadingOnboardingScene");
    //ResetSaveGame();

    SceneManager.LoadScene("OnboardingSetName");
  }

  public void ResetSaveGame(bool isSoft = true) {
    GameController controller = GameController.GetInstance();
    if(isSoft == true) {

      controller.gameState.SoftReset();
    } else {

      controller.gameState.Reset();
    }
    controller.SaveGame();
  }

  public void DeleteDataAndEnterFirstScene() {
    ResetSaveGame(false);
    EnterIntroScene();
  }

  public void EnterIntroScene()
  {
    SceneManager.LoadScene("Game Opening");
  }

  public void LoadScene(string name) {

    if(name == null || name == "") {

      SceneManager.LoadScene("_Start");
    } else {

      SceneManager.LoadScene(name);
    }
  }

  public void LoadAndAddScene(string name) {

    if(name == null || name == "") {

      SceneManager.LoadScene("_Start");
    } else {

      SceneManager.LoadScene(name, LoadSceneMode.Additive);
      StartCoroutine(WaitUntilSceneIsActive(name));
      
    }
  }

  private IEnumerator WaitUntilSceneIsActive(string name) {

    yield return new WaitUntil(() => SceneManager.GetActiveScene().name == name);
  }

  public void LoadSceneWithFade(string name) {
    if(fadeController != null) {
      fadeController.LoadSceneWithAnimation(name);
    }
  }

  public void ReloadCurrentScene() {

    Scene currentScene = SceneManager.GetActiveScene();
    SceneManager.LoadScene(currentScene.name);
  }

  public void LoadCharacterCreation() {
    SceneManager.LoadScene("CharacterCreation");
  }
}
