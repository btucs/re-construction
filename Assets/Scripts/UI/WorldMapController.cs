#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class WorldMapController : MonoBehaviour
{	
  public WorldMapUI uiController;

  public GameObject worldAreaButtonPrefab;
  public GameObject worldMapTilePrefab;
  public GameObject linePrefab;
  public RectTransform worldTilesContainer; //could be replaced by scollview.content (?)
  public WorldMapData mapData = new WorldMapData();
  public int worldTileSize = 10;
  public RectTransform scrollRectObj;
  public bool interactable = true;
  public ScrollRect scrollview;

  public UnityEvent onIncreaseWorldState = new UnityEvent();
  public UnityEvent onDecreaseWorldState = new UnityEvent();

  private Dictionary<Vector2, WorldMapTileController> tileDictionary = new Dictionary<Vector2, WorldMapTileController>();
  private GameController saveDataController;
  private float scrollbarXTarget, scrollbarYTarget, animTimestamp;
  private bool animated = false;
  private float borderSizeX, borderSizeY;
  private bool firstFrame;
  private Vector2Int playerPosition = Vector2Int.zero;
  private List<AreaChange> changes = new List<AreaChange>();


  void Awake() {
  	CheckForSaveController();
    TryGetMapData();    
    firstFrame = true;
  }

  void Update()
  {
  	if(firstFrame)
  	{
    	firstFrame = false;
  		CheckForSaveController();
  		Canvas.ForceUpdateCanvases();
  		CalcWorldMapSize();
  		CreateWorldMapTiles();
  		//DrawLineBetweenAreas();
  		ConnectSubAreas();
    	CreateAreaButtons();
    	ScrollToPlayerPosition();

		  UpdateScrollable();
    	UpdateWorldState();
  	}

  	if(animated)
  	{
  		scrollview.horizontalScrollbar.value = Mathf.Lerp(scrollview.horizontalScrollbar.value, scrollbarXTarget, animTimestamp);
  		scrollview.verticalScrollbar.value = Mathf.Lerp(scrollview.verticalScrollbar.value, scrollbarYTarget, animTimestamp);
  		animTimestamp += Time.deltaTime;
  		if(scrollview.horizontalScrollbar.value == scrollbarXTarget && scrollview.verticalScrollbar.value == scrollbarYTarget)
  			animated = false;
  	} 
  }

  public void Initialize()
  {
  	firstFrame = true;
  }

  public int GetAreaState(string sceneName)
  {
    int areaStateInt = 0;

    TryGetMapData();
    foreach(AreaData _area in mapData.areaList)
    {
      if(_area.IsAreaOfScene(sceneName))
      {
        areaStateInt = _area.GetAreaState();
      }
    }
    return areaStateInt;
  }

  private void CheckForSaveController()
  {
  	if(saveDataController == null)
  	{
  		saveDataController = GameController.GetInstance();
  	}
  }

  private void UpdateScrollable()
  {
  	scrollview.enabled = interactable;
  }

  private void TryGetMapData()
  {
  	//if(saveDataController.gameState.gameworldData.mapData==null) saveDataController.gameState.gameworldData.mapData = new WorldMapData();
  	WorldMapData newData = saveDataController.gameState.gameworldData.mapData;
  	if(newData == null || newData.areaList.Count < 1)
  	{
  		Debug.Log("No map data. Saving default state");

  		foreach(AreaData levelData in mapData.areaList) {
	      AddTilesToAreaRandom(levelData, tileState.bad, 45);
	      AddTilesToAreaRandom(levelData, tileState.average, 20);
	    }

  		SaveNewMapData(this.mapData);
  	} else {
  		Debug.Log("Map Data found and loaded");
  		mapData = newData;
  	}
  }

  private void SaveNewMapData(WorldMapData dataToSave)
  {
  	saveDataController.gameState.gameworldData.mapData = dataToSave;
  	saveDataController.SaveGame();

  }

  public void OpenAreaDescription() {
	uiController.OpenAreaDescription();
  }

  public void CloseAreaDescription() {
    uiController.CloseAreaDescription();
  }
  
  private void ScrollToPlayerPosition()
  {
  	if(playerPosition == Vector2Int.zero)
  	{
  		foreach(AreaData levelData in mapData.areaList) {

  			if(saveDataController.gameState.konstruktorSceneData.returnSceneName == levelData.areaInfo.sceneName)
  			{
  				playerPosition = levelData.areaInfo.mapPositionOrOffset;
  				break;
  			}
	    	foreach(SubAreaData subArea in levelData.subAreas)
	    	{
	    		if(saveDataController.gameState.konstruktorSceneData.returnSceneName == subArea.areaInfo.sceneName)
	  			{
	  				playerPosition = levelData.areaInfo.mapPositionOrOffset + subArea.areaInfo.mapPositionOrOffset;
	  				break;
	  			}
	    	}
    	}
  	}
  	ScrollToMapPositionInstant(playerPosition, Vector2.zero);
  }

	private AreaData GetCurrentMainArea()
	{
		CheckForSaveController();
	  	foreach(AreaData levelData in mapData.areaList) {

	  		if(saveDataController.gameState.konstruktorSceneData.returnSceneName == levelData.areaInfo.sceneName)
	  		{
	  			return levelData;
	  		}
		    foreach(SubAreaData subArea in levelData.subAreas)
		    {
		    	if(saveDataController.gameState.konstruktorSceneData.returnSceneName == subArea.areaInfo.sceneName)
		  		{
		  			return levelData;
		  		}
		    }
	    }

	    return null;
	}

  private void ScrollToMapPositionInstant(Vector2Int mapPos, Vector2 offset)
  {

  	Debug.Log("scrolling to " + mapPos);
  	Vector2 mapSize = worldTilesContainer.rect.size;
  	mapSize.x -= borderSizeX * 2;
  	mapSize.y -= borderSizeY * 2;
  	Vector2 realMapPosition = new Vector2((float)(mapPos.x * worldTileSize), (float)(mapPos.y * worldTileSize));
  	float xScrollValue = (realMapPosition.x / mapSize.x) + (offset.x / mapSize.x);
  	float yScrollValue = (realMapPosition.y / mapSize.y) + (offset.y / mapSize.y);

  	scrollview.horizontalScrollbar.value = xScrollValue; 
  	scrollview.verticalScrollbar.value = yScrollValue;
  }

  public void ScrollToMapPosition(Vector2Int mapPos, Vector2 offset)
  {
  	Vector2 mapSize = worldTilesContainer.rect.size;
  	mapSize.x -= borderSizeX * 2;
  	mapSize.y -= borderSizeY * 2;
  	Vector2 realMapPosition = new Vector2((float)(mapPos.x * worldTileSize), (float)(mapPos.y * worldTileSize));
  	float xScrollValue = (realMapPosition.x / mapSize.x) + (offset.x / mapSize.x);
  	float yScrollValue = (realMapPosition.y / mapSize.y) + (offset.y / mapSize.y);

  	scrollbarXTarget = xScrollValue; 
  	scrollbarYTarget = yScrollValue;
  	animTimestamp = 0f;
  	animated = true;
  }

  private WorldMapTile GetTileAt(Vector2Int lookUpPos)
  {
  	foreach(AreaData levelData in mapData.areaList)
  	{
  		foreach(WorldMapTile lookUpTile in levelData.impactTiles)
  		{
  			if(lookUpTile.mapPosition == lookUpPos)
  			{
  				return lookUpTile;
  			}
  		}
  	}
  	return null;
  }


  private void AddTilesToAreaRandom(AreaData spawnArea, tileState _tileState, int addAmount)
  {
  	//list of tiles should already be sorted by distance
  	//otherwise add logic to sort list of free Tiles so we can prefer closer  
  	List<Vector2Int> freeTiles = GetFreeTilesAroundMapPos(spawnArea.areaInfo.mapPositionOrOffset, addAmount*3);
  	
  	for(int i = 0; i < addAmount; i++)
  	{
  		int listIndexPicker = Random.Range(0, freeTiles.Count);
  		int distanceWeight = Random.Range(1, 5);
  		listIndexPicker = listIndexPicker/distanceWeight;

  		WorldMapTile newTile = new WorldMapTile(freeTiles[listIndexPicker], _tileState);
  		spawnArea.impactTiles.Add(newTile);
  		freeTiles.RemoveAt(listIndexPicker);
  	}
  }

  private List<Vector2Int> GetFreeTilesAroundMapPos(Vector2Int lookUpPos, int amount)
  {
  	List<Vector2Int> tilePositions = new List<Vector2Int>();

  	int offset = 1;
  	while(tilePositions.Count < amount && offset < 10)
  	{
  		for(int x = offset*-1; x <= offset; x++)
	  	{
	  		for(int y = offset*-1; y<= offset; y++)
	  		{
	  			if(Mathf.Abs(x) == offset || Mathf.Abs(y) == offset)
	  			{
	  				Vector2Int currentPos = new Vector2Int(lookUpPos.x + x, lookUpPos.y + y);
		  			if(GetTileAt(currentPos) == null)
		  			{
		  				tilePositions.Add(currentPos);
		  				if(tilePositions.Count >= amount)
		  					return tilePositions;
		  			}	
	  			}
	  		}
	  	}
	  	offset++;
  	}
  	Debug.Log("Warning - You have added a lot of map tiles. Precautiously exitting"); //only displayed when offset is above 10
  	return tilePositions;
  }

  private void CalcWorldMapSize() {
  	LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRectObj);
  	scrollRectObj.ForceUpdateRectTransforms();

  	borderSizeX = scrollRectObj.rect.width / 2;
    borderSizeY = scrollRectObj.rect.height / 2;

    Vector2 currentWorldMapSize = Vector2.zero;

    foreach(AreaData levelData in mapData.areaList) {
       if((float)levelData.areaInfo.mapPositionOrOffset.x * worldTileSize > currentWorldMapSize.x) {
         currentWorldMapSize.x = (float)levelData.areaInfo.mapPositionOrOffset.x * worldTileSize;
       }

       if((float)levelData.areaInfo.mapPositionOrOffset.y * worldTileSize > currentWorldMapSize.y) {
         currentWorldMapSize.y = (float)levelData.areaInfo.mapPositionOrOffset.y * worldTileSize;
       }

       foreach(SubAreaData subArea in levelData.subAreas)
    	{
    		if((float)(levelData.areaInfo.mapPositionOrOffset.x + subArea.areaInfo.mapPositionOrOffset.x) * worldTileSize > currentWorldMapSize.x) {
        		currentWorldMapSize.x = (float)(levelData.areaInfo.mapPositionOrOffset.x + subArea.areaInfo.mapPositionOrOffset.x) * worldTileSize;
       		}

       		if((float)(levelData.areaInfo.mapPositionOrOffset.y + subArea.areaInfo.mapPositionOrOffset.y) * worldTileSize > currentWorldMapSize.y) {
        		currentWorldMapSize.y = (float)(levelData.areaInfo.mapPositionOrOffset.y + subArea.areaInfo.mapPositionOrOffset.y) * worldTileSize;
       		}
    	} 
     }

    currentWorldMapSize.x += borderSizeX * 2;
    currentWorldMapSize.y += borderSizeY * 2;

    worldTilesContainer.sizeDelta = currentWorldMapSize;
  }

  public void CreateWorldMapTiles() 
  {
  	foreach(Transform childObj in worldTilesContainer)
  	{
  		Destroy(childObj.gameObject);
	}
	
	tileDictionary.Clear();

  	foreach(AreaData levelData in mapData.areaList)
  	{
  		//Debug.Log("creating tiles for " + levelData.areaInfo.areaName);
  		foreach(WorldMapTile lookUpTile in levelData.impactTiles)
  		{
  			//Debug.Log("creating tile at " + lookUpTile.mapPosition);
  			GameObject TileInstance = Instantiate(worldMapTilePrefab, worldTilesContainer);
		    RectTransform tileTransform = TileInstance.transform as RectTransform;
		    tileTransform.anchoredPosition = new Vector2((float)lookUpTile.mapPosition.x * worldTileSize + borderSizeX, (float)lookUpTile.mapPosition.y * worldTileSize  + borderSizeY);
		    tileTransform.localScale = new Vector3(1, 1, 1);

		    WorldMapTileController tileScriptSave = TileInstance.GetComponent<WorldMapTileController>();
		    tileScriptSave.tileData = lookUpTile;
		    tileScriptSave.UpdateColorInstant();
		    tileDictionary.Add(lookUpTile.mapPosition, tileScriptSave);
  		}
  	}
  }

  public void CreateAreaButtons() {
    foreach(AreaData levelData in mapData.areaList) {
    	GameObject buttonObj = InstantiateAreaButton(levelData.areaInfo, levelData.areaInfo.mapPositionOrOffset);
    	RectTransform areaButtonTransform = buttonObj.transform as RectTransform;
    	Vector2 mainAreaPos = new Vector2((float)levelData.areaInfo.mapPositionOrOffset.x * worldTileSize + borderSizeX, (float)levelData.areaInfo.mapPositionOrOffset.y * worldTileSize + borderSizeY);
    	areaButtonTransform.anchoredPosition = mainAreaPos;
    	areaButtonTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

    	foreach(SubAreaData subArea in levelData.subAreas)
    	{
        if(subArea.unlockedOnMap == true)
        {
      		Vector2Int subAreaMapPos = levelData.areaInfo.mapPositionOrOffset + subArea.areaInfo.mapPositionOrOffset;
      		GameObject subAreaObj = InstantiateAreaButton(subArea.areaInfo, subAreaMapPos);
      		RectTransform subButtonTransform = subAreaObj.transform as RectTransform;
      		Vector2 subAreaPos = new Vector2 (mainAreaPos.x + (subArea.areaInfo.mapPositionOrOffset.x * worldTileSize), mainAreaPos.y + (subArea.areaInfo.mapPositionOrOffset.y * worldTileSize));
      		subButtonTransform.anchoredPosition = subAreaPos;
      		subButtonTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
    	}
    }
  }

	private GameObject InstantiateAreaButton(AreaDataSO displayData, Vector2Int buttonPosition)
	{
		GameObject areaButtonInstance = Instantiate(worldAreaButtonPrefab, worldTilesContainer);

		AreaSelection areaSelection = areaButtonInstance.GetComponent<AreaSelection>();
    	bool isActiveArea = displayData.sceneName == SceneManager.GetActiveScene().name;
    	areaSelection.Setup(displayData, this, buttonPosition, isActiveArea);
    	if(isActiveArea)
    		playerPosition = buttonPosition;
    	uiController.areaSelectionButtons.Add(areaSelection);
    	return areaButtonInstance;
	}

  public void PositiveAreaEffect(AreaData effectArea, int numberOfAffectedTiles)
  {
    List<WorldMapTileController> changeTiles = new List<WorldMapTileController>();
    int changedAmount = 0;
    foreach(WorldMapTile singleTile in effectArea.impactTiles)
    {
    	if(singleTile.tileStatus != tileState.good)
    	{
    		WorldMapTileController currentTileController;
    		if(tileDictionary.TryGetValue(singleTile.mapPosition, out currentTileController))
  			{
  				singleTile.ImproveTileCondition();
  				changeTiles.Add(currentTileController);
  				changedAmount++;

  				if(changedAmount >= numberOfAffectedTiles)
  				{
  					break;
  				}
  			} else {
  				Debug.Log("Did not find tile Controller of area tile position");
  			}
    	}
    }

    if(onIncreaseWorldState != null)
          onIncreaseWorldState.Invoke();

    for(int i = 0; i < changeTiles.Count; i++) {
      StartCoroutine(UpdateTileWithDelay(changeTiles[i], 0.1f * (float)i));
    }
  }

  private void NegativeAreaEffect(AreaData effectArea, int numberOfAffectedTiles) {
    List<WorldMapTileController> changeTiles = new List<WorldMapTileController>();
    int changedAmount = 0;
    foreach(WorldMapTile singleTile in effectArea.impactTiles)
    {
    	if(singleTile.tileStatus != tileState.bad)
    	{
    		WorldMapTileController currentTileController;
    		if(tileDictionary.TryGetValue(singleTile.mapPosition, out currentTileController))
  			{
  				singleTile.AggrevateTileCondition();
  				changeTiles.Add(currentTileController);
  				changedAmount++;

  				if(changedAmount >= numberOfAffectedTiles)
  				{
  					break;
  				}
  			} else {
  				Debug.Log("Did not find tile Controller of area tile position");
  			}
    	}
    }

    if(onDecreaseWorldState != null)
          onDecreaseWorldState.Invoke();

    for(int i = 0; i < changeTiles.Count; i++) {
      StartCoroutine(UpdateTileWithDelay(changeTiles[i], 0.1f * (float)i));
    }
  }


  private IEnumerator UpdateTileWithDelay(WorldMapTileController tileToChange, float delay) {
    yield return new WaitForSeconds(delay);
    tileToChange.UpdateTileDisplay();
  }

  public void UpdateWorldState()
  {
  	foreach(AreaChange change in changes)
  	{
  		if(change.improve)
  		{
  			PositiveAreaEffect(change.changeArea, change.changeAmount);
  		} else 
  		{
  			NegativeAreaEffect(change.changeArea, change.changeAmount);
  		}
  	}
  	changes.Clear();

  	SaveNewMapData(this.mapData);
  }

	public void QueueActiveAreaChange(int amount, bool improve) {
		AreaData activeArea = GetCurrentMainArea();

		QueueAreaChange(activeArea, amount, improve);
	}

  public void QueueAreaChange(AreaData targetArea, int amount, bool improve) {
    AreaChange change = new AreaChange(targetArea, amount, improve);
    changes.Add(change);
  }

  private void DrawLineBetweenAreas() {

    for(int i = 0; i < mapData.areaList.Count; i++) {
      if(i > 0) {
        Vector2Int startPos = mapData.areaList[i].areaInfo.mapPositionOrOffset;
        Vector2Int endPos = mapData.areaList[i - 1].areaInfo.mapPositionOrOffset;
        DrawLineFromTo(startPos, endPos, 1f);
      }
    }
  }

  private void ConnectSubAreas()
  {
  	foreach(AreaData targetArea in mapData.areaList) 
  	{
    	foreach(SubAreaData subArea in targetArea.subAreas)
    	{
        if(subArea.unlockedOnMap == true)
        {
      		Vector2Int subAreaMapPos = targetArea.areaInfo.mapPositionOrOffset + subArea.areaInfo.mapPositionOrOffset;
      		DrawLineFromTo(targetArea.areaInfo.mapPositionOrOffset, subAreaMapPos, 2f);          
        }
    	}
    }
  }

  private void DrawLineFromTo(Vector2Int handle1Pos, Vector2Int handle2Pos, float lineWeight) {

    Debug.Log("Drawing a Line");

    float centerX = ((float)handle1Pos.x + (float)handle2Pos.x) * worldTileSize / 2 + borderSizeX;
    float centerY = ((float)handle1Pos.y + (float)handle2Pos.y) * worldTileSize / 2 + borderSizeY;

    //Vector3 center = new Vector3(handle1Pos.position.x + handle2Pos.position.x, handle1Pos.position.y + handle2Pos.position.y) / 2;
    float direction = Mathf.Atan2(handle1Pos.y - handle2Pos.y, handle1Pos.x - handle2Pos.x) * 180 / Mathf.PI;
    float sideLengthA = Mathf.Abs(handle2Pos.x - handle1Pos.x);
    float sideLengthB = Mathf.Abs(handle2Pos.y - handle1Pos.y);
    float scaleX = Mathf.Sqrt(sideLengthA * sideLengthA + sideLengthB * sideLengthB);

    //transform.position, transform.rotation

    GameObject lineObject = Instantiate(linePrefab, transform.position, transform.rotation);
    lineObject.transform.SetParent(worldTilesContainer);
    RectTransform lineTransform = lineObject.transform as RectTransform;
    lineTransform.localScale = new Vector3(1, 1, 1);

    lineTransform.anchoredPosition = new Vector2(centerX, centerY);
    lineTransform.sizeDelta = new Vector2(scaleX * worldTileSize, lineWeight);
    lineTransform.eulerAngles = new Vector3(0f, 0f, direction);
  }


}

public class AreaChange
{
	public AreaData changeArea;
	public int changeAmount;
	public bool improve;

	public AreaChange(AreaData area, int amount, bool _improve)
	{
		changeArea = area;
		changeAmount = amount;
		improve = _improve;
	}
}