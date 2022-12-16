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
using CodeMonkey.Utils;

/**
 * @see based on https://unitycodemonkey.com/video.php?v=8jrAWtI8RXg
 * Gridsystem with Generics
 **/
public class GeneralGrid<TGridItem> {

  private int width;
  private int height;
  private float cellHeight;
  private float cellWidth;
  private Vector3 originPosition;
  private TGridItem[][] gridArray;

  public GeneralGrid(
    int width,
    int height,
    float cellHeight,
    float cellWidth,
    Vector3 originPosition,
    Func<GeneralGrid<TGridItem>, int, int, TGridItem> createGridObject
  ) {

    this.width = width;
    this.height = height;
    this.cellHeight = cellHeight;
    this.cellWidth = cellWidth;
    this.originPosition = originPosition;

    gridArray = Enumerable
      .Range(0, height)
      .Select(y => Enumerable
        .Range(0, width)
        .Select(x => createGridObject(this, x, y))
        .ToArray()
      )
      .ToArray()
    ;

    bool showDebug = false;
    if(showDebug) {
      TextMesh[,] debugTextArray = new TextMesh[width, height];

      for(int y = 0; y < gridArray.Count(); y++) {
        for(int x = 0; x < gridArray[y].Count(); x++) {
          debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[y][x]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellWidth, cellHeight) * .5f, 30, Color.white, TextAnchor.MiddleCenter);
          Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
          Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
        }
      }
      Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
      Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }
  }

  public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
  public class OnGridValueChangedEventArgs : EventArgs {

    public int x;
    public int y;
  }

  public Vector3 GetWorldPosition(int x, int y) {

    return new Vector3(x * cellWidth, y * cellHeight) + originPosition;
  }

  private Vector2Int GetXY(Vector3 worldPosition) {

    Vector3 relativePosition = worldPosition - originPosition;

    return new Vector2Int() {
      x = Mathf.FloorToInt(relativePosition.x / cellWidth),
      y = Mathf.FloorToInt(relativePosition.y / cellHeight),
    };
  }

  public void SetGridItem(int x, int y, TGridItem value) {

    if(x >= 0 && y >= 0 && x < width && y < height) {

      gridArray[y][x] = value;
      OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs {
        x = x,
        y = y
      });
    }
  }

  public TGridItem[] GetColumn(int x) {

    return gridArray.Select((TGridItem[] row) => row[x]).ToArray();
  }

  public void SetGridItem(Vector3 worldPosition, TGridItem value) {

    Vector2Int xy = GetXY(worldPosition);
    SetGridItem(xy.x, xy.y, value);
  }

  public TGridItem GetGridItem(int x, int y) {

    if(x >= 0 && y >= 0 && x < width && y < height) {

      return gridArray[y][x];
    } else {

      return default(TGridItem);
    }
  }

  public TGridItem GetGridItem(Vector3 worldPosition) {

    Vector2Int xy = GetXY(worldPosition);

    return GetGridItem(xy.x, xy.y);
  }

  public void TriggerGridItemChanged(int x, int y) {

    OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs {
      x = x,
      y = y
    });
  }
}
