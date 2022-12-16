#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent (typeof (characterMovement))]
public class playerScript : MonoBehaviour
{
  [SerializeField]
  private bool interactionEnabled = true;
  public bool menuOpen = false;

  public UnityEvent onDragCamera = new UnityEvent();

  private GameController controller;

  private WorldSceneCameraController camController;
  private characterMovement movementController;
  private Vector3 mouseTarget;
  private bool removeLock = false;
  private MenuUIController menuController;
  private Camera cameraObject;

  private static playerScript instance;
  public static playerScript Instance
  {
    get { return instance; }
  }

  private void Awake ()
  {
    if (instance == null) { instance = this; }
    else if (instance != this) { Destroy(this.gameObject); }
  }

  private Vector3 mouseStartPos, mouseCurrentPos, mousePositionChange, worldStartPos;
  private Vector3 camTargetPosition;

  private float mouseClickDistance;

  private bool mouseIsDown = false;
  private bool hasDragged = false;
  private bool mouseDownOverUI = false;

  public characterMovement GetPlayerMovementScript()
  {
    return movementController;
  }

  // Start is called before the first frame update
  private void Start()
  {
    controller = GameController.GetInstance();
    controller.playerScript = this;

    movementController = GetComponent<characterMovement>();

    if(cameraObject == null) {

      cameraObject = Camera.main;
    }

    camController = cameraObject.transform.GetComponent<WorldSceneCameraController>();
    camTargetPosition = cameraObject.transform.position;
    menuController = MenuUIController.Instance;
  }

  // Update is called once per frame
  private void Update()
  {
    if(interactionEnabled && menuController.IsClosed())
    {

      if (Input.GetMouseButton(0))
      {
        if(!mouseIsDown)
        {
          mouseIsDown = true;
          mouseStartPos = Input.mousePosition;
          worldStartPos = cameraObject.ScreenToWorldPoint(mouseStartPos);
          mouseDownOverUI = IsPointerOverUI();
        } 
        else 
        {
          mouseCurrentPos = Input.mousePosition;
          mouseClickDistance = Vector3.Distance(mouseStartPos, mouseCurrentPos);
          if(mouseClickDistance > 50f)
          {
            mousePositionChange = worldStartPos - cameraObject.ScreenToWorldPoint(mouseCurrentPos);
            camTargetPosition = cameraObject.transform.position + mousePositionChange;
            hasDragged = true;
            camController.SetGoalPosition(camTargetPosition);
            if(onDragCamera != null)
              onDragCamera.Invoke();
          }
        }  
        

      }

      if (Input.GetMouseButtonUp(0))
      {
        if(!hasDragged)
        {
          if(!IsPointerOverUI() && mouseDownOverUI == false)
          {
            //RaycastObjectDetection();

            mouseTarget = cameraObject.ScreenToWorldPoint(Input.mousePosition);
            mouseTarget = new Vector3 (mouseTarget.x, transform.position.y, transform.position.z);
            movementController.SetGoalPosition(mouseTarget);
          }                
        } 
        else
        {
          hasDragged = false;
        }
        mouseIsDown = false;
        mouseDownOverUI = false;
      }
    }

    if(!interactionEnabled && removeLock)
    {
      interactionEnabled = true;
      removeLock = false;
    }

    if(Input.GetMouseButton(0)==false && mouseIsDown)
    {
      mouseIsDown = false;
    }
  }

  public void EnableInteraction()
  {
    if(!interactionEnabled)
      removeLock = true;
  }

  public void DisableInteraction()
  {
    interactionEnabled = false;
    removeLock = false;
    mouseIsDown = false;
  }

  public characterGraphicsUpdater GetPlayerGraphics()
  {
    return transform.GetComponentInChildren<characterGraphicsUpdater>();
  }

  private bool IsPointerOverUI()
  {
    if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
      return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
    else 
      return EventSystem.current.IsPointerOverGameObject();
  }

  private void RaycastObjectDetection()
  {
    RaycastHit hit;
    Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
  
    if (Physics.Raycast(ray, out hit)) {
      GameObject objectHit = hit.transform.gameObject;
      GameWorldObject gwObject = objectHit.GetComponent<GameWorldObject>();
      if(gwObject != null) {

        gwObject.OpenObjectMenu();

        KonstruktorSceneData konstructorData = controller.gameState.konstruktorSceneData;
        konstructorData.interactablesPrefabs = new KonstruktorSceneData.InteractableData[] {
          new KonstruktorSceneData.InteractableData() {
            taskObject = gwObject.objectData,
            position = objectHit.transform.position,
          }
        };

        konstructorData.cameraPosition = objectHit.transform.position + new Vector3(0, 0, camTargetPosition.z);
      }
    }
  }

}
