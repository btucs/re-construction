#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;

public class CameraCapture : MonoBehaviour
{
  public Camera captureCamera;
  public string fileName;
  public bool isPlayer = false;

  private RenderTexture renderTexture;
  private string imageFolder;

  public void GrabImage() {

    captureCamera.targetTexture = RenderTexture.GetTemporary(512, 512, 16);
    imageFolder = Application.persistentDataPath + "/images/";
    RenderPipelineManager.endCameraRendering += EndCameraRendering;
  }

  private void EndCameraRendering(ScriptableRenderContext src, Camera camera) {

    RenderPipelineManager.endCameraRendering -= EndCameraRendering;
    Capture();
  }

  public void Capture() {
  
    RenderTexture renderTexture = captureCamera.targetTexture;

    Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
    Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
    image.ReadPixels(rect, 0, 0, true);

    byte[] bytes = image.EncodeToPNG();

    if(isPlayer == true) {

      GameController controller = GameController.GetInstance();
      Texture2D playerThumbnailTexture = new Texture2D(1, 1);
      playerThumbnailTexture.LoadImage(bytes);
      Sprite playerThumbnailSprite = Sprite.Create(playerThumbnailTexture, new Rect(0, 0, playerThumbnailTexture.width, playerThumbnailTexture.height), new Vector2(0.5f, 0.5f));
      controller.gameState.characterData.player.thumbnail = playerThumbnailSprite;
    }

    if(Directory.Exists(imageFolder) == false) {

      Directory.CreateDirectory(imageFolder);
    }

    File.WriteAllBytes(imageFolder + fileName + ".png", bytes);

    RenderTexture.ReleaseTemporary(renderTexture);
    captureCamera.targetTexture = null;
  }

#if UNITY_EDITOR
  [Button(Name = "Save Face image to Assets")]
  public void SaveImage() {

    captureCamera.targetTexture = RenderTexture.GetTemporary(512, 512, 16);
    imageFolder = Application.dataPath + "/Sprites/Characters/";
    RenderPipelineManager.endCameraRendering += EndCameraRendering;
  }
#endif
}
