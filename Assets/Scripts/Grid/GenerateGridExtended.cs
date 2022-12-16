#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Constants;

public struct CameraZoom {
  public float currentValue;
  public ZoomDirection direction;

  public override string ToString() {

    return "{currentValue: " + currentValue.ToString() + ", direction: " + direction.ToString() + "}";
  }
}

public enum ZoomDirection: int {
  In = -1,
  Out = 1
};

public class GenerateGridExtended: MonoBehaviour {
  
  //this is the line prefab
  public GameObject prefabLine, prefabLine2, canvasText;

  public Transform secondAxis, divAxis, canvasesY, canvasesX;
  //resolutions of divisiions
  public float resol = 1;
  public float divResol = 5;
  public int extralines = 10;
  public Camera mainCamera;

  // container for the axis
  Transform cam;

  int limitMinX, limitMaxX, limitMinY, limitMaxY;

  private readonly Dictionary<float, HashSet<GameObject>> gridObjects = new Dictionary<float, HashSet<GameObject>>();

  private IObservable<CameraZoom> cameraZoomObservable;
  private IObservable<Vector2> cameraPositionObservable;

  public float GetSmallestGridStep() {

    return resol / divResol;
  }

  void Start() {
  
    //camera parameters
    cam = mainCamera.transform;

    if (mainCamera.orthographic) {

      CreateOrthographicCameraZoomObservable();
    } else {

      CreateCameraZoomObservable();
    }
    CreateCameraPositionObservable();
    
    //limits for the grid
    RecalculateLimits();

    DrawGrid();

    //cameraZoomObservable.Subscribe((CameraZoom cz) => CheckZoom(cz)).AddTo(this);
    HandleNextResolution();
    HandlePreviousResolution();
    HandleCameraPosition();
  }

  private void CreateCameraZoomObservable() {

    IObservable<float> obs = Observable.EveryFixedUpdate()
      .Select((long _) => {

        float fov = mainCamera.fieldOfView;
        float x = Mathf.Tan(fov / 2 * Mathf.PI / 180) * -cam.position[2];

        return x;
      })
    ;

    cameraZoomObservable = CreateCameraZoomFromZoomObservable(obs);
  }

  private void CreateOrthographicCameraZoomObservable() {

     IObservable<float> obs = Observable.EveryFixedUpdate()
      .Select((long _) => {

        return mainCamera.orthographicSize;
      })
    ;

    cameraZoomObservable = CreateCameraZoomFromZoomObservable(obs);
  }

  private IObservable<CameraZoom> CreateCameraZoomFromZoomObservable(IObservable<float> obs) {

    return obs
      .DistinctUntilChanged()
      .Buffer(2, 1)
      .Select((IList<float> list) => {

        ZoomDirection direction = list[0] < list[1] ? ZoomDirection.Out : ZoomDirection.In;

        return new CameraZoom() {
          currentValue = list[1],
          direction = direction
        };
      })
    ;
  }

  private void CreateCameraPositionObservable() {

    cameraPositionObservable = Observable.EveryFixedUpdate()
      .Select((long _) => new Vector2 {
        x = cam.position.x,
        y = cam.position.y
      })
      .DistinctUntilChanged()
    ;
  }

  //destroys the grid by deleting all children
  void DestroyGrid() {

    DestroyGrid(float.NaN);
  }

  void DestroyGrid(float resolution) {

    if (float.IsNaN(resolution)) {

      DestroyGameObjectsAtResolution(gridObjects);
    } else {

      DestroyGameObjectsAtResolution(resolution, gridObjects);
    }
  }

  private void DestroyGameObjectsAtResolution(
    Dictionary<float, HashSet<GameObject>> dictionary
  ) {

    float[] keys = dictionary.Keys.ToArray<float>();
    foreach(float key in keys) {

      DestroyGameObjectsAtResolution(key, dictionary);
    }
  }

  private void DestroyGameObjectsAtResolution(
    float resolution,
    Dictionary<float, HashSet<GameObject>> dictionary
  ) {

    dictionary.TryGetValue(resolution, out HashSet<GameObject> gameObjects);
    if (gameObjects == null || gameObjects.Count == 0) {

      return;
    }

    foreach (GameObject obj in gameObjects) {

      Destroy(obj);
    }

    dictionary.Remove(resolution);
  }

  private void AddToDictioaryAtResolution(
    float resolution,
    Dictionary<float, HashSet<GameObject>> dictionary,
    GameObject obj
  ) {

    if (dictionary.ContainsKey(resolution) == false) {

      dictionary.Add(resolution, new HashSet<GameObject>());
    }

    dictionary[resolution].Add(obj);
  }

  private void DrawGrid() {

    DrawGrid(resol);
  }

  private void DrawGrid(float resolution) {
    ///////////////////
    //drawing the grid
    ///////////////////
    GameObject go;

    //left part
    for (int i = limitMinX; i <= limitMinX + extralines; i++) {

      for (int j = 1; j <= divResol - 1; j++) {

        go = GameObject.Instantiate(prefabLine2, new Vector3(i * resolution + j * resolution / divResol, 0, 0.001f), Quaternion.Euler(0, 0, 0));
        go.transform.SetParent(divAxis);
        go.layer = Layers.coordinateSystem;
        AddToDictioaryAtResolution(resolution, gridObjects, go);
      }

      //lines
      go = GameObject.Instantiate(prefabLine, new Vector3(i * resolution, 0, 0), Quaternion.Euler(0, 0, 0));
      go.transform.SetParent(secondAxis);
      go.layer = Layers.coordinateSystem;
      AddToDictioaryAtResolution(resolution, gridObjects, go);

      //text
      if (i != 0) {

        go = GameObject.Instantiate(canvasText, new Vector3(i * resolution, 0, -0.001f), Quaternion.Euler(0, 0, 0));
        go.transform.SetParent(canvasesX);
        go.transform.GetComponentInChildren<Text>().text = "" + i * resolution;
        go.layer = Layers.coordinateSystem;
        AddToDictioaryAtResolution(resolution, gridObjects, go);
      }
    }

    //right part
    for (int i = limitMaxX - extralines; i <= limitMaxX; i++) {

      for (int j = 1; j <= divResol - 1; j++) {

        go = GameObject.Instantiate(prefabLine2, new Vector3((i - 1) * resolution + j * resolution / divResol, 0, 0.001f), Quaternion.Euler(0, 0, 0));
        go.transform.SetParent(divAxis);
        go.layer = Layers.coordinateSystem;
        AddToDictioaryAtResolution(resolution, gridObjects, go);
      }

      //lines
      go = GameObject.Instantiate(prefabLine, new Vector3(i * resolution, 0, 0), Quaternion.Euler(0, 0, 0));
      go.transform.SetParent(secondAxis);
      go.layer = Layers.coordinateSystem;
      AddToDictioaryAtResolution(resolution, gridObjects, go);

      //text
      if (i != 0) {

        go = GameObject.Instantiate(canvasText, new Vector3(i * resolution, 0, -0.001f), Quaternion.Euler(0, 0, 0));
        go.transform.SetParent(canvasesX);
        go.layer = Layers.coordinateSystem;
        go.transform.GetComponentInChildren<Text>().text = "" + i * resolution;
        AddToDictioaryAtResolution(resolution, gridObjects, go);
      }
    }

    //bottom part
    for (int i = limitMinY; i <= limitMinY + extralines; i++) {

      for (int j = 1; j <= divResol - 1; j++) {

        go = GameObject.Instantiate(prefabLine2, new Vector3(0, i * resolution + j * resolution / divResol, 0.001f), Quaternion.Euler(0, 0, 90));
        go.transform.SetParent(divAxis);
        go.layer = Layers.coordinateSystem;
        AddToDictioaryAtResolution(resolution, gridObjects, go);
      }

      //lines
      go = GameObject.Instantiate(prefabLine, new Vector3(0, i * resolution, 0), Quaternion.Euler(0, 0, 90));
      go.transform.SetParent(secondAxis);
      go.layer = Layers.coordinateSystem;
      AddToDictioaryAtResolution(resolution, gridObjects, go);

      //text
      if (i != 0) {

        go = GameObject.Instantiate(canvasText, new Vector3(0, i * resolution, -0.001f), Quaternion.Euler(0, 0, 0));
        go.transform.SetParent(canvasesY);
        go.layer = Layers.coordinateSystem;
        go.transform.GetComponentInChildren<Text>().text = "" + i * resolution;
        AddToDictioaryAtResolution(resolution, gridObjects, go);
      }
    }

    //top part
    for (int i = limitMaxY - extralines; i <= limitMaxY; i++) {

      for (int j = 1; j <= divResol - 1; j++) {

        go = GameObject.Instantiate(prefabLine2, new Vector3(0, (i - 1) * resolution + j * resolution / divResol, 0.001f), Quaternion.Euler(0, 0, 90));
        go.transform.SetParent(divAxis);
        go.layer = Layers.coordinateSystem;
        AddToDictioaryAtResolution(resolution, gridObjects, go);
      }

      //lines
      go = GameObject.Instantiate(prefabLine, new Vector3(0, i * resolution, 0), Quaternion.Euler(0, 0, 90));
      go.transform.SetParent(secondAxis);
      go.layer = Layers.coordinateSystem;
      AddToDictioaryAtResolution(resolution, gridObjects, go);

      //text
      if (i != 0) {

        go = GameObject.Instantiate(canvasText, new Vector3(0, i * resolution, -0.001f), Quaternion.Euler(0, 0, 0));
        go.transform.SetParent(canvasesY);
        go.layer = Layers.coordinateSystem;
        go.transform.GetComponentInChildren<Text>().text = "" + i * resolution;
        AddToDictioaryAtResolution(resolution, gridObjects, go);
      }
    }
  }

  private void HandleNextResolution() {

    cameraZoomObservable.Where((CameraZoom cz) =>
      cz.currentValue > resol * 9 &&
      cz.direction == ZoomDirection.Out
    ).Subscribe((CameraZoom cz) => {

      DestroyGrid(resol);

      resol = resol * 10;

      //limits for the grid
      RecalculateLimits();
      DrawGrid(resol);
    }).AddTo(this);
  }

  private void HandlePreviousResolution() {

    cameraZoomObservable.Where((CameraZoom cz) =>
      cz.currentValue < 1 * resol &&
      cz.direction == ZoomDirection.In
    ).Subscribe((CameraZoom cz) => {

      DestroyGrid(resol);

      resol = resol / 10;

      //limits for the grid
      RecalculateLimits();
      DrawGrid(resol);
    }).AddTo(this);
  }
 
  void CheckNumberPosition() {

    //check number position
    float fov = cam.GetComponent<Camera>().fieldOfView;
    float y = (Mathf.Tan(fov / 2 * Mathf.PI / 180) * -cam.position[2]);
    float x = (Mathf.Tan(fov / 2 * Mathf.PI / 180) * -cam.position[2]) * Screen.width / Screen.height;

    //out of limits when going right
    if (cam.position[0] - x > 0) {

      for (int i = 0; i < canvasesY.childCount; i++) {

        GameObject go = (canvasesY.GetChild(i).gameObject);

        go.transform.GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
        go.transform.GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
        go.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

        go.transform.position = new Vector3(cam.position[0] - x, go.transform.position[1], go.transform.position[2]);
      }
    }

    //out of limits when going left
    if (cam.position[0] + x < 0) {

      for (int i = 0; i < canvasesY.childCount; i++) {

        GameObject go = (canvasesY.GetChild(i).gameObject);

        go.transform.GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
        go.transform.GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
        go.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleRight;

        go.transform.position = new Vector3(cam.position[0] + x, go.transform.position[1], go.transform.position[2]);
      }
    }

    //out of limits when going up
    if (cam.position[1] - y > 0) {

      for (int i = 0; i < canvasesX.childCount; i++) {

        GameObject go = (canvasesX.GetChild(i).gameObject);

        go.transform.GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1.2f);
        go.transform.GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1.2f);
        go.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.LowerCenter;

        go.transform.position = new Vector3(go.transform.position[0], cam.position[1] - y, go.transform.position[2]);
      }
    }

    //out of limits when going down
    if (cam.position[1] + y < 0) {

      for (int i = 0; i < canvasesX.childCount; i++) {

        GameObject go = (canvasesX.GetChild(i).gameObject);

        go.transform.GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0f);
        go.transform.GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0f);
        go.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.UpperCenter;

        go.transform.position = new Vector3(go.transform.position[0], cam.position[1] + y, go.transform.position[2]);
      }
    }
  }

  private void HandleCameraPosition() {

    cameraPositionObservable
      .Do((Vector2 _) =>
        //check the position of the numbers
        CheckNumberPosition()
      )
      .Where((Vector2 position) => {
        //check axis values
        return position.x / resol > limitMaxX / 1.5f ||
            position.x / resol < limitMinX / 1.5f ||
            position.y / resol > limitMaxY / 1.5f ||
            position.y / resol < limitMinY / 1.5f;
      })
      .Subscribe((Vector2 _) => {

        DestroyGrid();

        //limits for the grid
        RecalculateLimits();
        DrawGrid();
      }).AddTo(this)
    ;
  }

  private void RecalculateLimits() {

    limitMinX = (int)Mathf.Round(cam.position[0] / resol) - extralines;
    limitMinY = (int)Mathf.Round(cam.position[1] / resol) - extralines;
    limitMaxX = (int)Mathf.Round(cam.position[0] / resol) + extralines;
    limitMaxY = (int)Mathf.Round(cam.position[1] / resol) + extralines;
  }
}
