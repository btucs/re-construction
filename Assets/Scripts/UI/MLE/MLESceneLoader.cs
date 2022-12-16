#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;
using UnityEngine.SceneManagement;

public class MLESceneLoader : MonoBehaviour
{
  public MLEDataSO currentMLEData;
  public GameObject dataTransportPrefab;

  public TaskDataSO taskRef;
  public TaskObjectSO objectRef;


  public void LoadMLESceneFromTask() {
    GameObject dataTransporter = Instantiate(dataTransportPrefab);
    MLEDataTransporter dataTransporterScript = dataTransporter.GetComponent<MLEDataTransporter>();
    dataTransporterScript.mleTransportData = currentMLEData;
    dataTransporterScript.originSceneName = SceneManager.GetActiveScene().name;
    dataTransporterScript.typeOfAccess = OriginType.taskPreview; //defined in MLEDataTransporter
    dataTransporterScript.originSceneTask = taskRef;
    dataTransporterScript.originObject = objectRef;

    DontDestroyOnLoad(dataTransporter);
    SceneManager.LoadScene("MLE");
    //SceneManager.LoadScene("MLEIntro");
  }

  public void LoadMLESceneFromList() {
    GameObject dataTransporter = Instantiate(dataTransportPrefab);
    MLEDataTransporter dataTransporterScript = dataTransporter.GetComponent<MLEDataTransporter>();
    dataTransporterScript.mleTransportData = currentMLEData;
    dataTransporterScript.originSceneName = SceneManager.GetActiveScene().name;
    dataTransporterScript.typeOfAccess = OriginType.bibList; //defined in MLEDataTransporter
    dataTransporterScript.openPrevUI = false;

    DontDestroyOnLoad(dataTransporter);
    SceneManager.LoadScene("MLE");
  }

  public void LoadMLESceneWithOutUIRestore() {
    GameObject dataTransporter = Instantiate(dataTransportPrefab);
    MLEDataTransporter dataTransporterScript = dataTransporter.GetComponent<MLEDataTransporter>();
    dataTransporterScript.mleTransportData = currentMLEData;
    dataTransporterScript.originSceneName = SceneManager.GetActiveScene().name;
    dataTransporterScript.openPrevUI = false;

    DontDestroyOnLoad(dataTransporter);
    SceneManager.LoadScene("MLE");
  }

}
