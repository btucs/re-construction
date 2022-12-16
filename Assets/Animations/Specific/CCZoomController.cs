#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCZoomController : MonoBehaviour
{
  public Animator CharacterPreviewAnim;
  bool faceZoom = false;

  public void SetFullBodyZoom()
  {
    if(CharacterPreviewAnim && faceZoom == true)
    {
    	faceZoom = false;
      //CharacterPreviewAnim.SetTrigger("BodyZoom");	
      CharacterPreviewAnim.SetBool("zoomFace", false);
      CharacterPreviewAnim.SetFloat("speedMultiplier", -1.0f);
    }
  }

  public void SetFaceZoom()
  {
    if(CharacterPreviewAnim && faceZoom == false)
    {
    	faceZoom = true;
      //CharacterPreviewAnim.SetTrigger("FaceZoom");	
      CharacterPreviewAnim.SetBool("zoomFace", true);
      CharacterPreviewAnim.SetFloat("speedMultiplier", 1.0f);
    }
  }
}
