#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using UnityEngine;

public class CameraBounds2D : MonoBehaviour
{
    public Vector2 scaleBound = new Vector2(1, 1);
    public Vector2 offset;

    [HideInInspector] public Vector2 maxXlimit;
    [HideInInspector] public Vector2 maxYlimit;

    Camera _camera;

    public void Initialize(Camera temp_camera)
    {
        _camera = temp_camera;
        CalculateBounds();
    }

    public void CalculateBounds()
    {
        float cameraHalfWidth = _camera.aspect * _camera.orthographicSize;
        maxXlimit = new Vector2((transform.position.x + offset.x - (scaleBound.x / 2)) + cameraHalfWidth, (transform.position.x + offset.x + (scaleBound.x / 2)) - cameraHalfWidth);
        maxYlimit = new Vector2((transform.position.y + offset.y - (scaleBound.y / 2)) + _camera.orthographicSize, (transform.position.y + offset.y + (scaleBound.y / 2)) - _camera.orthographicSize);
    }
}
