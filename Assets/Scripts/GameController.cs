using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AnimationGroup))]
public class GameController : MonoBehaviour {
	private static float TILESYOFFSET = 0.25f;

	public delegate void OnCollectTileItem(TileItem tileItem);
	public event OnCollectTileItem onCollectTileItem;
	public delegate void OnMoveComplete();
	public event OnMoveComplete onMoveComplete;

	public static readonly int DEFAULT_TILEITEM_SORTING_ORDER = 10;
	public static readonly int BOMB_MARK_SORTING_ORDER = 20;
	public static readonly int BOMB_EXPLOSION_SORTING_ORDER = 25;

	private int numColumns;
	private int numRows;
	public LayerMask tilesItemLayer = -1;

	public GameObject[] tileItemsColor;
	public GameObject[] tileItemsColorBombH;
	public GameObject[] tileItemsColorBombV;
	public GameObject[] tileItemsColorEnvelop;
	public GameObject[] tileItemsColorBombHV;
	public GameObject[] tileItemsSpecial;
	public GameObject[] tileItemsStatic;
	public GameObject[] tileItemsSpecialStatic;
	public GameObject[] tileItemsBox;
	public GameObject[] barrierItems;
	public GameObject[] tileItemsColorIndependedBomb;

//	public GameObject tilesBg;

	public GameObject bombMark;

	private AnimationGroup animationGroup;


	private Tile[,] tiles;
	private LinkedList<Tile> selectedTiles = new LinkedList<Tile>();
	private LinkedList<Tile> specialSelectedTiles = new LinkedList<Tile>();
	private IList<Tile> bombMarkTiles = new List<Tile>();
	private IList<TileItem> bombSelectedTiles = new List<TileItem>();
	private IList<Tile> rotaitedBombs = new List<Tile>();
	private IList<Tile> slimeTiles = new List<Tile>();
	private IList<Generator> generatorTiles = new List<Generator>();
	private IList<TileItemData> dropRequire = new List<TileItemData>();

	private Tile[,] tmpTiles;

	private IDictionary<BarrierData, Barrier> barriers = new Dictionary<BarrierData, Barrier>();

	private IDictionary<string, Hero> heroes = new Dictionary<string, Hero>();

	private IDictionary<Vector2, List<TileItemData>> replacedItems = new Dictionary<Vector2, List<TileItemData>>();

	private int[] tileItemSpawnDelay;
	private bool[] tileColumnAvalibleForOffset;

	private LevelData levelData;
	private GameData gameData;

	private Rect tilesArea;
	private Tile bombTile = null;

	private TargetController targetController;
	private RestrictionsController restrictionsController;

	bool IsTileInputAvaliable { get; set;}

	private IDictionary<Tile, object> damagedTiles = new Dictionary<Tile, object>();
	private IDictionary<Barrier, object> damagedBarriers = new Dictionary<Barrier, object>();

	int autoDropOnCollectIndex;
	int collectedTileItemsCount;
	int dropedTileItemsCount;

	Slime slime;

	AutoDropTileItems autoDropTileItems;

	private bool suspendBomb = false;

	public ParticleSystem BombExplosionPS;
	public ParticleSystem BombExplosionBombPS;
	private float bombExplosionDelay = 0;
	private bool existBombAll;

	private Tile endTouchTile;

	private TileItemTypeGroup? selectedTileItemTypeGroup = null;

	void Start() {
		levelData = GameResources.Instance.LoadLevel(App.GetCurrentLevel());
		levelData.Init();
		gameData = new GameData();
		gameData.Init();

		numColumns = GameData.NumColumns;
		numRows = GameData.NumRows;

		autoDropTileItems = new AutoDropTileItems(levelData.AutoDropData);
		slime = new Slime(levelData.SlimeRatio);

		float areaOffset = 0.1f;
		tilesArea = new Rect(-numColumns / 2f - areaOffset, 0 - areaOffset, numColumns + 2*areaOffset, numRows + 2*areaOffset);

		animationGroup = GetComponent<AnimationGroup>();
		tiles = new Tile[numColumns, numRows];
		tileItemSpawnDelay = new int[numColumns];
		tileColumnAvalibleForOffset = new bool[numColumns];
	
	//	GameObject tileBgObj = Instantiate (tilesBg);
	//	tileBgObj.transform.position = IndexToPosition (3.0f, 3.0f);

		InitTiles();
		InitBarriers();
		InitHeroes();

		InitControllers();

		DetectUnavaliableTiles();

		UpdateTiles(true);
	}
	// Update is called once per frame
	void Update() {
		ProcessInput();
	
	}

	private void InitControllers() {
		GameObject go = Preconditions.NotNull(GameObject.Find("Target Panel"), "Can not find target panel");
		targetController = Preconditions.NotNull(go.GetComponent<TargetController>(), "Can not get target controller");
		targetController.LoadCurrentLevel();

		onCollectTileItem += targetController.OnCollectTileItem;
		onCollectTileItem += OnCheckAutoDropOnDestroyData;

		go = Preconditions.NotNull(GameObject.Find("Restrictions Panel"), "Can not find restrictions panel");
		restrictionsController = Preconditions.NotNull(go.GetComponent<RestrictionsController>(), "Can not get restrictions controller");
		restrictionsController.LoadCurrentLevel();
		onMoveComplete += restrictionsController.DecrementMoveScore;
	}

	private void ResetTileColumnAvalibleForOffset() {
		for(int i = 0;i < numColumns; ++i) {
			tileColumnAvalibleForOffset[i] = true;
		}	
	}
	private void ResetTileItemSpawnDelay() {
		for(int i = 0;i < numColumns; ++i) {
			tileItemSpawnDelay[i] = 0;
		}	
	}
	private void ResetTileItemMoved() {
		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				TileItem ti = tiles[x, y].GetTileItem();
				if(ti != null) {
					ti.IsMoved = false;
				}
			}
		}
	}

	public static Vector3 IndexToPosition(float x, float y) {
		return new Vector3(x - GameData.NumColumns / 2f + 0.5f, y + 0.5f + TILESYOFFSET, 0);
	}
	public static Vector2 PositionToIndex(Vector3 pos) {
		return new Vector2(pos.x + GameData.NumColumns / 2f - 0.5f, pos.y - 0.5f - TILESYOFFSET);
	}

	private TileItem InstantiateColorOrSpecialTileItem(int column) {
		dropedTileItemsCount++;
		TileItemData itemData = null;

		if(dropRequire.Count > 0 && autoDropOnCollectIndex == dropedTileItemsCount) {
			itemData = dropRequire[0];
			dropRequire.Remove(itemData);
			return InstantiateTileItem(itemData.Type, 0, -10, true);
		}

		itemData = autoDropTileItems.GetDropeItem();
		if(itemData != null) {
			return InstantiateTileItem(itemData.Type, 0, -10, true);
		}

		int rand = Random.Range(1, 101);
		int sum = 0;
		int index = 0;
		for(int i = 0;i < levelData.TileItemDropPercent.Length;i++) {
			sum += levelData.TileItemDropPercent[i];
			if(rand <= sum) {
				index = i;
				break;
			}
		}
	//	Debug.Log(rand + " " + index);
		return InstantiateTileItem(tileItemsColor, index, (TileItemType)(index*TileItem.TILE_ITEM_GROUP_WEIGHT), 0, -10, true);
	}

	private Barrier InstantiateBarrier(BarrierData data) {
		GameObject go = (GameObject)Instantiate(barrierItems[(int)data.Type], 
			IndexToPosition(0.5f*(data.X1 + data.X2), 0.5f*(data.Y1 + data.Y2)), Quaternion.identity);

		if(data.Isvertical) {
			go.transform.Rotate(new Vector3(0, 0, 90));
		}

		return Barrier.Instantiate(data, go);
	}

	private TileItem InstantiateTileItem(TileItemType type, int x, int y, bool convertIndexToPos) {
		TileItemTypeGroup group = TileItem.TypeToTypeGroup(type);
		switch(group) {
			case TileItemTypeGroup.Red:
			case TileItemTypeGroup.Green:
			case TileItemTypeGroup.Blue:
			case TileItemTypeGroup.Yellow:
			case TileItemTypeGroup.Purple:	
				return InstantiateTileItem(GetColorGameObjectsByTileItemType(type),  (int)group/100, type, x, y, convertIndexToPos);
			case TileItemTypeGroup.Bomb:
				return InstantiateTileItem(tileItemsColorIndependedBomb, (int)(type) - (int)group, type, x, y, convertIndexToPos);
			case TileItemTypeGroup.Static:
				return InstantiateTileItem(tileItemsStatic, (int)(type) - (int)group, type, x, y, convertIndexToPos);
			case TileItemTypeGroup.Special:
				return InstantiateTileItem(tileItemsSpecial, (int)(type) - (int)group, type, x, y, convertIndexToPos);
			case TileItemTypeGroup.Box:
				return InstantiateTileItem(tileItemsBox, (int)(type) - (int)group, type, x, y, convertIndexToPos);
			case TileItemTypeGroup.SpecialStatic:
				return InstantiateTileItem(tileItemsSpecialStatic, (int)(type) - (int)group, type, x, y, convertIndexToPos);
		}

		throw new System.NotImplementedException("Can not instantient tile item with type " + type.ToString());
	}

	private GameObject[] GetColorGameObjectsByTileItemType(TileItemType type) {
		int index = TileItem.TypeToIndex(type);
		switch(index) {
			case 0:
				return tileItemsColor;
			case TileItem.BOMBH_OFFSET:
				return tileItemsColorBombH;
			case TileItem.BOMBV_OFFSET:
				return tileItemsColorBombV;
			case TileItem.ENVELOP_OFFSET:
				return tileItemsColorEnvelop;
			case TileItem.BOMBHV_OFFSET:
				return tileItemsColorBombHV;
		}

		throw new System.NotImplementedException("Can not get tile item game objects for type " + type.ToString());

	}

	private TileItem InstantiateTileItem(GameObject[] items, int index, TileItemType type, int x, int y, bool convertIndexToPos) {
		GameObject go = (GameObject)Instantiate(items[index], (!convertIndexToPos)? new Vector3(x, y, 0) : IndexToPosition(x, y), Quaternion.identity);
		TileItem ti = TileItem.Instantiate(type, go);
		go.GetComponent<SpriteRenderer>().sortingOrder = DEFAULT_TILEITEM_SORTING_ORDER;

		return ti;
	}

	private void ProcessInput() {
		InputController.Touch[] touches = InputController.getTouches();

		if(touches.Length > 0) {
			InputController.Touch touch = touches[0];
			if(!IsTileInputAvaliable) {
				return;
			}

			Tile tile = null;
			Ray ray = InputController.TouchToRay(touches[0]);

			RaycastHit2D hit = Physics2D.Raycast( ray.origin, Vector2.zero, Mathf.Infinity, tilesItemLayer );
			if(hit.collider != null) {		
				tile = GetTile(hit.collider.gameObject);
			}
	
			if(touch.phase == TouchPhase.Began) {
				BeganTouch(tile);
			} else if(touch.phase == TouchPhase.Moved) {
				MoveTouch(tile);
			} else if(touch.phase == TouchPhase.Ended) {
				EndTouch(tile);
			} else if(touch.phase == TouchPhase.Canceled) {
				ResetSelected();
			}
		}
		
	}

	private void BeganTouch(Tile tile) {
		bombTile = null;
		suspendBomb = false;
		endTouchTile = null;
		selectedTileItemTypeGroup = null;
		bombSelectedTiles.Clear();
		
		if(tile == null || tile.GetTileItem() == null || !tile.GetTileItem().MayBeFirst ) {
			return;
		}
		TileItem tileItem = tile.GetTileItem();
		Preconditions.Check(replacedItems.Count == 0, "replacedItems count must be 0 {0}", replacedItems.Count);
		Preconditions.Check(selectedTiles.Count == 0, "selectedTiles count must be 0 {0}", selectedTiles.Count);

		if(!tileItem.IsColorIndepended) {
			SetTileItemsRenderState(TileItemRenderState.Dark, tileItem.TypeGroup);
			selectedTileItemTypeGroup = tileItem.TypeGroup;
		}		
		SelectTileItem(tile, true);

		if(tileItem.IsBomb) {
			bombSelectedTiles.Add(tileItem);
			MarkBombTiles();
			if(tileItem.IsMovableBomb) {
				bombTile = tile;		
			}
		}

	}

	private void MoveTouch(Tile tile) {
		if(tile == null || tile.GetTileItem() == null || selectedTiles.Count == 0) {
			return;
		}

		Tile lastTile = selectedTiles.Last.Value;
		TileItem tileItem = tile.GetTileItem();
		TileItemTypeGroup curTypeGroup = tileItem.TypeGroup;
		if(lastTile == tile || (selectedTileItemTypeGroup != null &&  curTypeGroup != selectedTileItemTypeGroup.Value && !tileItem.IsColorIndepended)) {
			return;
		}

		Debug.Log(tileItem.TypeGroup + " " + lastTile.GetTileItem().TypeGroup + " " +selectedTileItemTypeGroup);
		bool updateMarkBombTiles = false;

		if(selectedTileItemTypeGroup == null && !tileItem.IsColorIndepended) {
			SetTileItemsRenderState(TileItemRenderState.Dark, tileItem.TypeGroup);
			selectedTileItemTypeGroup = tileItem.TypeGroup;
			updateMarkBombTiles = true;
		}		
			
		bool stop = false;
		Tile predLastTile = null;
		if(selectedTiles.Count > 1) {
			predLastTile = selectedTiles.Last.Previous.Value;
			if(predLastTile == tile) {
				Vector2 index = new Vector2(lastTile.X, lastTile.Y);
				if(replacedItems.ContainsKey(index)) {
					ReplaceTileItems(replacedItems[index], TileItemRenderState.Dark, false);
					replacedItems.Remove(index);
				}
				
				SelectTileItem(lastTile, false);
			
				if(bombTile != null && lastTile == bombTile) {
					bombTile = null;
					updateMarkBombTiles = true;
				} else if(bombTile != null && !suspendBomb) {
					ResetBombMark();
					updateMarkBombTiles = true;
					ExchangeTileItem(lastTile, tile);
					tile.GetTileItem().Select(null);
					lastTile.GetTileItem().UnSelect(TileItemRenderState.Normal);

					if(selectedTiles.Count > 1) {
						tile.GetTileItem().SetTransitionTileItem(selectedTiles.Last.Previous.Value.GetTileItem());
					}				
				}

				CheckSelectedColor();

				if(bombSelectedTiles.Contains(lastTile.GetTileItem())) {
					bombSelectedTiles.Remove(lastTile.GetTileItem());
					updateMarkBombTiles = true;
				}
				if(bombSelectedTiles.Contains(tile.GetTileItem())) {
					bombSelectedTiles.Remove(lastTile.GetTileItem());
					updateMarkBombTiles = true;
				}

				if(suspendBomb && tile.GetTileItem().IsBomb) {
					suspendBomb = false;
				}

				if(updateMarkBombTiles) {
					ResetBombMark();
					MarkBombTiles();
				}

				return;
			}
		}
		if(selectedTiles.Contains(tile)) {

		} else if(CheckAvailabilityTransition(lastTile.X, lastTile.Y, tile.X, tile.Y)){
			IList<TileItemData> replaceData = GetTileItemDataForEnvelopReplace(tile);
			if(replaceData != null) {
				Vector2 index = new Vector2(tile.X, tile.Y);
				replacedItems[index] = ReplaceTileItems(replaceData, TileItemRenderState.Normal, true);
			}

			SelectTileItem(tile, true, TileItemRenderState.HighLight, lastTile.GetTileItem());
			if(tile.GetTileItem().IsBomb && !bombSelectedTiles.Contains(tile.GetTileItem())) {
				bombSelectedTiles.Add(tile.GetTileItem());
				updateMarkBombTiles = true;
			}

			if(bombTile != null && !tile.GetTileItem().IsNotStatic) {
				suspendBomb = true;
			} else if(bombTile != null && bombTile != tile && !suspendBomb) {
				updateMarkBombTiles = true;
				ResetBombMark();
				ExchangeTileItem(lastTile, tile);
				lastTile.GetTileItem().SetTransitionTileItem(null);
				tile.GetTileItem().SetTransitionTileItem(lastTile.GetTileItem());
				if(predLastTile != null) {
					lastTile.GetTileItem().SetTransitionTileItem(predLastTile.GetTileItem());
				} 

			} else if(bombTile == null && tile.GetTileItem().IsMovableBomb) {
				bombTile = tile;
				updateMarkBombTiles = true;
			}
			


		}
		
		if(updateMarkBombTiles) {
			ResetBombMark();
			MarkBombTiles();
		}

	}

	private void EndTouch(Tile tile) {
		endTouchTile = tile;
		if(selectedTiles.Count >= levelData.SuccessCount) {
			CollectTileItems();
		} else {
			ResetSelected();
		}
	}

	private void CheckSelectedColor() {
		foreach(Tile tile in selectedTiles) {
			TileItem ti = tile.GetTileItem();
			if(!ti.IsColorIndepended) {
				return;
			}
		}

		SetTileItemsRenderState(TileItemRenderState.Normal, null);
		selectedTileItemTypeGroup = null;
	}

	private void ExchangeTileItem(Tile t1, Tile t2) {
		TileItem ti1 = t1.GetTileItem();
		TileItem ti2 = t2.GetTileItem();
		Vector3 pos1 = new Vector3(ti1.GetGameObject().transform.position.x, ti1.GetGameObject().transform.position.y, ti1.GetGameObject().transform.position.z);
		Vector3 pos2 = new Vector3(ti2.GetGameObject().transform.position.x, ti2.GetGameObject().transform.position.y, ti2.GetGameObject().transform.position.z);
		t1.SetTileItem(ti2);
		t2.SetTileItem(ti1);

		ti1.GetGameObject().transform.position = pos2;
		ti2.GetGameObject().transform.position = pos1;

	}

	private void ResetSelected() {
		SetTileItemsRenderState(TileItemRenderState.Normal, null);
		foreach(Tile tile in specialSelectedTiles) {
			tile.GetTileItem().UnSelect(TileItemRenderState.Normal);
		}

		ResetBombMark();

		for( LinkedListNode<Tile> node = selectedTiles.Last;node != null; node = node.Previous ) {
			Tile curTile = node.Value;
			curTile.GetTileItem().UnSelect(TileItemRenderState.Normal);

			if(bombTile != null && curTile != bombTile && curTile.GetTileItem().IsBomb) {
				Tile prevTile = node.Previous.Value;
				if(prevTile != null) {
					prevTile.GetTileItem().UnSelect(TileItemRenderState.Normal);
					prevTile.MarkBomb(false);
					ExchangeTileItem(curTile, prevTile);
				}
			} else if(bombTile != null && curTile == bombTile ) {
				bombTile = null;
			}
		}
		foreach(Vector2 index in replacedItems.Keys) {
			ReplaceTileItems(replacedItems[index], TileItemRenderState.Normal, false);
		}


	//	RotateBombs();

		rotaitedBombs.Clear();
		specialSelectedTiles.Clear();
		selectedTiles.Clear();
		replacedItems.Clear();
		bombTile = null;
	}

	public void CollectTileItems() {
		collectedTileItemsCount = 0;
		damagedBarriers.Clear();
		damagedTiles.Clear();

		bool isDetectAvaliable = false;
		SetTileItemsRenderState(TileItemRenderState.Normal, null);
		/*
		if(bombMarkTiles.Count > 1) {
			if(BreakBarriersByBomb(5)) {
				isDetectAvaliable = true;
			}
		}*/
		existBombAll = false;
		foreach(Tile tile in bombMarkTiles) {
			if(!tile.IsEmpty && tile.GetTileItem().IsBombAll) {
				existBombAll = true;
			}
		}

		foreach(Tile tile in bombMarkTiles) {
			InitBombExplosion(tile);

			if(tile.GetTileItem() != null && tile.GetTileItem().IsBomb) {
				int a = 5;
			}
			tile.MarkBomb(false);
		//	bool isNotStatic = tile.IsEmpty || tile.GetTileItem().IsNotStatic || tile.GetTileItem().IsSlime;
			if(tile.IsEmpty && BreakBarriers(tile.X, tile.Y, 5)) {
				isDetectAvaliable = true;
			}

			if(tile.IsEmpty) {
				continue;
			}

			if((tile.GetTileItem().IsColor || tile.GetTileItem().IsColorIndepended || tile.GetTileItem().IsSpecialCollect || tile.GetTileItem().IsBreakableOnlyByBomb )) {
				CollectTileItem(tile);
				if(BreakBarriers(tile.X, tile.Y, 5)) {
					isDetectAvaliable = true;
				}
			}
			if(BreakTileItems(tile.X, tile.Y, 5, false)) {
				isDetectAvaliable = true;
				BreakBarriers(tile.X, tile.Y, 5);
			}
		}

		foreach(Tile tile in selectedTiles) {
			if(tile.GetTileItem() == null) {
				continue;
			}
			bool isNotStatic = tile.GetTileItem().IsNotStatic;
			CollectTileItem(tile);
			if(BreakTileItems(tile.X, tile.Y, 1, isNotStatic)) {
				isDetectAvaliable = true;
			}

			if(isNotStatic && BreakBarriers(tile.X, tile.Y, 1)) {
				isDetectAvaliable = true;
			}
		}

		foreach(Tile tile in specialSelectedTiles) {
			if(tile.GetTileItem() == null) {
				continue;
			}
			CollectTileItem(tile);
		}

		rotaitedBombs.Clear();
		specialSelectedTiles.Clear();
		selectedTiles.Clear();
		replacedItems.Clear();
		bombMarkTiles.Clear();
		bombTile = null;

		if(isDetectAvaliable) {
			DetectUnavaliableTiles();
		}

		if(onMoveComplete != null) {
			onMoveComplete();
		}

		if(targetController.CheckSuccess()) {
			LevelSuccess();
			return;
		} else if (!restrictionsController.CheckRestrictions()){
			LevelFailure();
			return;
		}


		StartCoroutine(UpdateTilesWitDelay(true, bombExplosionDelay));
	}

	private bool BreakBarriersByBomb(int damage) {
		bool res = false;
		IList<Barrier> breakedBarriers = new List<Barrier>();

		foreach(Barrier b in barriers.Values) {
			Tile t1 = GetTile(b.Data.X1, b.Data.Y1);
			Tile t2 = GetTile(b.Data.X2, b.Data.Y2);

			if(t1 == null || t2 == null || !bombMarkTiles.Contains(t1) || !bombMarkTiles.Contains(t2)) {
				continue;
			}

			if(b.Damage(damage) <= 0) {
				res = true;
				breakedBarriers.Add(b);
			}
		}

		foreach(Barrier b in breakedBarriers) {
			BreakBarrier(b);
		} 

		return res;
	}

	private void CollectTileItem(Tile tile) {
		if(selectedTiles.Contains(tile)) {
			tile.GetTileItem().UnSelect(TileItemRenderState.Normal);
		}

		TileItem parentTi = tile.GetTileItem().GetParentTileItem();
		if(parentTi != null) {
			return;
		}

		if(onCollectTileItem != null) {
			onCollectTileItem(tile.GetTileItem());
		}
			
		if(tile.GetTileItem().DestroyOnBreak()) {
			TileItem ti = tile.GetTileItem();
			TileItem child = ti.GetChildTileItem();
			ClearTile(tile);

			if(child != null) {
				tile.SetTileItem(child);
			} else {
				collectedTileItemsCount++;
			}

			if(slimeTiles.Contains(tile)) {
				slimeTiles.Remove(tile);
				slime.Collect();
			}
		}
	}

	private void SelectTileItem(Tile tile, bool isSelect, TileItemRenderState unSelectedState = TileItemRenderState.Normal, TileItem transitionTileItem = null) {
		Preconditions.NotNull(tile, "Tile {0} {1} can not be null", tile.X, tile.Y);
		TileItem tileItem = Preconditions.NotNull(tile.GetTileItem(), "Tile Item {0} {1} can not be null", tile.X, tile.Y);

		if(isSelect) {
			/*if(bombMarkTiles.Contains(tile)) {
				tileItem.Mark(false);
				bombMarkTiles.Remove(tile);
			}*/
			tileItem.Select(transitionTileItem);
			if(!tile.GetTileItem().IsSpecialCollect) {
				selectedTiles.AddLast(tile);
				AddSpecialTileItems(tile.X, tile.Y);
			} else {
				specialSelectedTiles.AddLast(tile);
			}
		} else {
			tileItem.UnSelect(unSelectedState);
			if(!tile.GetTileItem().IsSpecialCollect) {
				selectedTiles.Remove(tile);
				RemoveSpecialTileItems(tile.X, tile.Y);
			} else {
				specialSelectedTiles.Remove(tile);
			}
		}
	}
		
	private void AddSpecialTileItems(int x, int y) {
		Tile[] nearTiles = new Tile[4];
		nearTiles[0] = GetTile(x - 1, y);
		nearTiles[1] = GetTile(x + 1, y);
		nearTiles[2] = GetTile(x, y - 1);
		nearTiles[3] = GetTile(x, y + 1);

		foreach(Tile tile in nearTiles) {
			if(tile != null && tile.GetTileItem() != null && tile.GetTileItem().IsSpecialCollect && !specialSelectedTiles.Contains(tile)
				&& CheckAvailabilityWithBarriers(x, y, tile.X, tile.Y)) {
				SelectTileItem(tile, true);
			}
		}
	}

	private void RemoveSpecialTileItems(int x, int y) {
		Tile[] nearTiles = new Tile[4];
		IList<Tile> removed = new List<Tile>();

		foreach(Tile tile in specialSelectedTiles) {
			nearTiles[0] = GetTile(tile.X - 1, tile.Y);
			nearTiles[1] = GetTile(tile.X + 1, tile.Y);
			nearTiles[2] = GetTile(tile.X, tile.Y - 1);
			nearTiles[3] = GetTile(tile.X, tile.Y + 1);
			Tile findTile = null;

			foreach(Tile checkTile in nearTiles) {
				if(checkTile != null && selectedTiles.Contains(checkTile)) {
					findTile = checkTile;
					break;
				}
			}

			if(findTile == null) {
				removed.Add(tile);
			}
		}

		foreach(Tile tile in removed) {
			SelectTileItem(tile, false, TileItemRenderState.Dark);
		}
	}
		
	private bool BreakTileItems(int x, int y, int damage, bool breakAround) {
		bool res = false;
		Tile[] nearTiles;
		if(breakAround) {
			nearTiles = new Tile[5];
			nearTiles[0] = GetTile(x - 1, y);
			nearTiles[1] = GetTile(x + 1, y);
			nearTiles[2] = GetTile(x, y - 1);
			nearTiles[3] = GetTile(x, y + 1);
			nearTiles[4] = GetTile(x, y);
		} else {
			nearTiles = new Tile[1];
			nearTiles[0] = GetTile(x, y);
		}


		foreach(Tile tile in nearTiles) {
			if(tile == null || tile.GetTileItem() == null) {
				continue;
			}
			if(breakAround && !CheckAvailabilityWithBarriers(x, y, tile.X, tile.Y)) {
				continue;
			}

			if(damagedTiles.ContainsKey(tile)) {
						continue;
			}

			TileItem parentTi = tile.GetTileItem().GetParentTileItem();
			if(parentTi == null) {
				damagedTiles.Add(tile, null);
			}
			if(parentTi == null && tile.GetTileItem().Damage(damage) <= 0) {
				res = true;
				BreakTileItem(tile);
			} else if(parentTi != null && tile.X == x && tile.Y == y && parentTi.Damage(damage) <= 0) {
				Destroy(parentTi.GetGameObject());
				tile.GetTileItem().SetParentTileItem(null);
			}

		}

		return res;
	}

	private void BreakTileItem(Tile tile) {
		CollectTileItem(tile);
	}

	private bool BreakBarriers(int x, int y, int damage) {
		bool res = false;
		Barrier[] nearBarriers = new Barrier[4];
		nearBarriers[0] = GetBarrier(x, y, x - 1, y);
		nearBarriers[1] = GetBarrier(x, y, x + 1, y);
		nearBarriers[2] = GetBarrier(x, y, x, y - 1);
		nearBarriers[3] = GetBarrier(x, y, x, y + 1);

		foreach(Barrier barrier in nearBarriers) {
			if(barrier == null) {
				continue;
			}

			if(damagedBarriers.ContainsKey(barrier)) {
				continue;
			}

			damagedBarriers.Add(barrier, null);
			if(barrier.Damage(damage) <= 0) {
				res = true;
				BreakBarrier(barrier);
			}
		}

		return res;
	}

	private void BreakBarrier(Barrier barrier) {
		barriers.Remove(barrier.Data);
		Destroy(barrier.BarrierGO);
	}

	private void SetTileItemsRenderState(TileItemRenderState state, TileItemTypeGroup? excludeTypeGroup ) {
		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				Tile tile = tiles[x, y];
				if(tile.GetTileItem() == null || selectedTiles.Contains(tile)) {
					continue;
				}
				TileItem tileItem = tile.GetTileItem().GetChildTileItem() != null? tile.GetTileItem().GetChildTileItem() : tile.GetTileItem();
				if(!tileItem.IsNotStatic && tileItem.GetParentTileItem() == null) {
					continue;
				}

				if(excludeTypeGroup == null || tileItem.TypeGroup != excludeTypeGroup.Value && !tileItem.IsColorIndepended) {
					tileItem.SetRenderState(state);
				}
			}
		}
	}
		
	private bool IsExitFromTilesArea(Vector3 pos) {
		return !tilesArea.Contains(new Vector2(pos.x, pos.y));
	}

	private Tile GetTile(GameObject go) {
		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				if(tiles[x, y].IsEmpty ) {
					continue;
				}
				if(tiles[x, y].GetTileItemGO() == go) {
					return tiles[x, y];
				}
			}
		}

		return null;
	}

	private Barrier GetBarrier(int x1, int y1, int x2, int y2) {
		Barrier val;
		barriers.TryGetValue(new BarrierData(x1, y1, x2, y2, BarrierType.Barrier_2), out val);
		return val;
	}

	private void DetectUnavaliableTiles() {
		for(int y = numRows - 1; y >= 0; y--) {
			for(int x = 0; x < numColumns; x++) {
				Tile tile = tiles[x, y];
				tile.Type = TileType.Avaliable;

				if(tile.GetTileItem() != null && !tile.GetTileItem().IsNotStatic) {
					tile.Type = TileType.UnavaliableForDrop;
					continue;
				}

				Tile left = GetTile(x - 1, y + 1);
				Tile center = GetTile(x, y + 1);
				Tile right = GetTile(x + 1, y + 1);

				bool av1 = CheckAvailabilityWithBarriers(tile, left);
				bool av2 = CheckAvailabilityWithBarriers(tile, center);
				bool av3 = CheckAvailabilityWithBarriers(tile, right);

				if(!av1 && !av2 && !av3) {
					tile.Type = TileType.UnavaliableForDrop;
	//				InstantiateTileItem(TileItemType.Brilliant, x, y, true);
					continue;
				}

				if((x == 0 || (left != null && !left.IsAvaliableForDrop) || !av1)
					&& ((center != null && !center.IsAvaliableForDrop ) || !av2)
					&& ((x == numColumns - 1) || (right != null && !right.IsAvaliableForDrop) || !av3)) 
				{
					tile.Type = TileType.UnavaliableForDrop;
	//				InstantiateTileItem(TileItemType.Brilliant, x, y, true);
				} 
			}
		}	
	}

	private bool CheckAvailabilityWithBarriers(Tile from, Tile to) {
		Preconditions.NotNull(from, "Tile from can not be null");

		if(to == null) {
			return from.Y == numRows - 1;
		}
		/*
		if(!to.IsNotStatic || !from.IsNotStatic) {
			return false;
		}*/

		return CheckAvailabilityWithBarriers(from.X, from.Y, to.X, to.Y);
	}

	private bool CheckAvailabilityWithBarriers(int fromX, int fromY, int toX, int toY) {
		if(Mathf.Abs(fromX - toX) > 1 || Mathf.Abs(fromY - toY) > 1) {
			return false;
		}

		if(fromX == toX && fromY == toY) {
			return false;
		}
		if(fromX == toX || fromY == toY) {
			return GetBarrier(fromX, fromY, toX, toY) == null;
		}
				
		bool b1 = GetBarrier(fromX, fromY, toX, fromY) != null;
		bool b2 = GetBarrier(toX, fromY, toX, toY) != null;
		bool b3 = GetBarrier(toX, toY, fromX, toY) != null;
		bool b4 = GetBarrier(fromX, toY, fromX, fromY) != null;

		if(b1 && b3 || b1 && b4 || b2 && b3 || b2 && b4) {
			return false;
		}

		return true;
	}

	private bool CheckAvailabilityTransition(int fromX, int fromY, int toX, int toY) {
		if(Mathf.Abs(fromX - toX) > 1 || Mathf.Abs(fromY - toY) > 1) {
			return false;
		}

		return CheckAvailabilityWithBarriers(fromX, fromY, toX, toY);
	}

	private void RunTileItemsAnimation<T>(AnimationGroup.CompleteAnimation<T> complete, T param) {
		animationGroup.Clear();

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				if(tiles[x, y].IsEmpty || !tiles[x, y].IsNotStatic){
					continue;
				}

				AnimatedObject ao = tiles[x, y].GetAnimatedObject();
				if(ao.IsAnimationExist()) {
					animationGroup.Add(ao);
				}
			}
		}

		animationGroup.Run(complete, param);
	}

	private void OnTileItemUpdateComplete(bool first) {
		if(first) {
			animationGroup.Clear();

			UpdateSlime();
			UpdateTileItemsByGenerators();
			TileItemData data = GetHeroItemData();
			if(data != null) {
				InitHeroItemAnimation(data);
			}

			if(animationGroup.AnimationExist()) {
				animationGroup.Run(OnTileItemUpdateComplete, false);
				return;
			}
		}

		CheckConsistency();
	}

	private void OnTileItemUpdateComplete(System.Object[] param) {
		TileItem ti = (TileItem)param[0];
		Tile tile = (Tile)param[1];

		ClearTile(tile);
		tile.SetTileItem(ti);

		OnTileItemUpdateComplete(false);
	}

	private void UpdateTiles(bool first) {
		IsTileInputAvaliable = false;
		ResetTileItemSpawnDelay();

		if(first) {
			bombExplosionDelay = 0;
			dropedTileItemsCount = 0;
			autoDropTileItems.ReseteDroped();

			if(dropRequire.Count > 0 && collectedTileItemsCount > 1) {
				autoDropOnCollectIndex = Random.Range(0, collectedTileItemsCount - 1);
			}
		}

		while(UpdateTilesColumns()) {
			ResetTileColumnAvalibleForOffset();
			UpdateTilesWithOffset(true);
			ResetTileItemMoved();
		}
		UpdateTilesWithOffset(false);

		RunTileItemsAnimation(OnTileItemUpdateComplete, first);
	}

	private void InitTiles() {
		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				if(tiles[x, y] == null) {
					tiles[x, y] = new Tile(x, y);
					GameObject go = (GameObject)Instantiate(bombMark, IndexToPosition(x, y), Quaternion.identity);
					go.GetComponent<SpriteRenderer>().sortingOrder = BOMB_MARK_SORTING_ORDER;
					go.active = false;
					tiles[x, y].SetBombMarkGO(go);
				}
			}
		}

		foreach(TileItemData item in levelData.TileData) {
			if(tiles[item.X, item.Y].GetTileItem() != null) {
				throw new System.Exception("Invalid configuration for tile " + tiles[item.X, item.Y] + ". Tile is configured twice");
			}
			TileItem ti = InstantiateTileItem(item.Type, item.X, item.Y, true);
		//	if(ti.IsBomb || ti.IsEnvelop) {
		//		Preconditions.Check(item.Level > 0, "Level of TileItem {0}, {1} must be greater than zero", item.X, item.Y);
		//	}
			tiles[item.X, item.Y].SetTileItem(ti);
			InitTileItem(item, ti);
			if(item.HasChild()) {
				InitChildTileItem(item.ChildTileItemData, tiles[item.X, item.Y]);
				SpriteRenderer render = ti.GetGameObject().GetComponent<SpriteRenderer>();
				render.sortingOrder = DEFAULT_TILEITEM_SORTING_ORDER + 1;
			}
			if(item.HasParent()) {
				InitParentTileItem(item.ParentTileItemData, tiles[item.X, item.Y]);
			}
			if(item.HasGenerated()) {
				generatorTiles.Add(new Generator(item.X, item.Y, item.GeneratedTileItemData));
			}
			if(ti.IsSlime) {
				slimeTiles.Add(tiles[item.X, item.Y]);
			}
		}
	}

	private void InitTileItem(TileItemData tileItemData, TileItem tileItem) {
		tileItem.Level = tileItemData.Level;
		tileItem.SetStartHealth(tileItemData.Health);
	}
	private void InitTileItem(ChildTileItemData tileItemData, TileItem tileItem) {
		tileItem.Level = tileItemData.Level;
		tileItem.SetStartHealth(tileItemData.Health);
	}

	private void InitChildTileItem(ChildTileItemData tileItemData, Tile tile) {
		if(tileItemData == null) {
			return;
		}

		TileItem tileItem = Preconditions.NotNull(tile.GetTileItem(), "Can not init child TileItem for tile {0},{1} curentTileItem is null", tile.X, tile.Y);
		TileItem ti = InstantiateTileItem(tileItemData.Type, tile.X, tile.Y, true);
		InitTileItem(tileItemData, ti);
		tileItem.SetChildTileItem(ti);
	}

	private void InitParentTileItem(ChildTileItemData tileItemData, Tile tile) {
		if(tileItemData == null) {
			return;
		}

		TileItem tileItem = Preconditions.NotNull(tile.GetTileItem(), "Can not init parent TileItem for tile {0},{1} curentTileItem is null", tile.X, tile.Y);
		TileItem ti = InstantiateTileItem(tileItemData.Type, tile.X, tile.Y, true);
		InitTileItem(tileItemData, ti);
		tileItem.SetParentTileItem(ti);
		SpriteRenderer render = ti.GetGameObject().GetComponent<SpriteRenderer>();
		render.sortingOrder = DEFAULT_TILEITEM_SORTING_ORDER + 1;
	}

	private void InitBarriers() {
		if(levelData.BarrierData == null) {
			return;
		}
		foreach(BarrierData data in levelData.BarrierData) {
			if(!barriers.ContainsKey(data)) {
				barriers.Add(data, InstantiateBarrier(data));
			} else {
				throw new System.Exception("Invalid configuration for Barrier " + data + ". Barrier is configured twice");
			}
		}
	}

	private void InitHeroes() {
		foreach(UserHeroData data in GameResources.Instance.GetUserData().HeroesData) {
			gameData.HeroData[data.Id].Level = data.Level;
			heroes[data.Id] = new Hero(gameData.HeroData[data.Id]);
		}
	}

	private bool UpdateTilesColumns() {
		bool res = false;
		for(int x = 0;x < numColumns; x++) {
			res = UpdateTilesColumn(x) || res;
		}

		return res;
	}

	private bool UpdateTilesColumn(int x) {
		IList<Tile> tEmpty = new List<Tile>();
		Tile spawnTile = tiles[x, numRows - 1];
		int delay = spawnTile.IsEmpty ? 0 : 1;
		bool res = false;

		for(int y = 0; y < numRows;y++) {
			Tile tile = tiles[x, y];

			if(!tile.IsNotStatic) {
				tEmpty.Clear();
				continue;
			}

			if(tile.IsEmpty) {
				tEmpty.Add(tile);
				if(tile.IsAvaliableForDrop) {
					res = true;
				}
			}

			if(tEmpty.Count > 0 && !tile.IsEmpty) {
				MoveTileItem(tile, tEmpty[0], TileItemMoveType.DOWN, 0);
				tEmpty.RemoveAt(0);
				tEmpty.Add(tile);
			}

			if(y < numRows - 1 && !CheckAvailabilityWithBarriers(tile, tiles[x, y + 1] )) {
				tEmpty.Clear();
			}
		}
			
		if(tEmpty.Count > 0) {
			tileItemSpawnDelay[x] += delay;
		}

		foreach(Tile tile in tEmpty) {
			TileItem tileItem = InstantiateColorOrSpecialTileItem(x);
			tileItem.GetGameObject().GetComponent<AnimatedObject>().AddIdle((App.MoveTileItemTimeUnit + App.moveTileItemDelay) * tileItemSpawnDelay[x]).Build();
			spawnTile.SetTileItem(tileItem);
			MoveTileItem(spawnTile, tile, TileItemMoveType.DOWN, 0);
			tileItemSpawnDelay[x]++;
		}

		return res;
	}

	private void UpdateTilesWithOffset(bool oneOffset) {
		bool res = true;
		while(res) {
			res = false;
			for(int y = 0; y < numRows - 1; y++) {
				res = UpdateTilesWithOffsetRow(y) || res;
			}
			if (oneOffset) {
				break;
			}
		}
	}

	private bool UpdateTilesWithOffsetRow(int y) {
		int direction = (Random.Range(0, 2) == 0)? 1 : -1;
		IList<Tile> suitables = new List<Tile>();
		bool res = false;

		for(int x = (direction > 0)? 0 : numColumns - 1;x < numColumns && x >= 0; x += direction) {
			Tile tile = tiles[x, y];
			Tile top = GetTile(x, y + 1);
			bool colAvaliable = tileColumnAvalibleForOffset[x];

			if(top != null && !top.IsNotStatic) {
				tileColumnAvalibleForOffset[top.X] = true;
			}

			if(!colAvaliable) {
				continue;
			}
			if(!tile.IsNotStatic || !tile.IsEmpty) {
				continue;
			}

			Tile left = GetTile(x + 1, y + 1);
			Tile right = GetTile(x - 1, y + 1);
			Tile chosen = null;
			suitables.Clear();

			if(left != null && left.IsNotStatic && !left.IsEmpty && tileColumnAvalibleForOffset[left.X]
				&& CheckAvailabilityWithBarriers(tile, left)) 
			{
				suitables.Add(left);
			}
			if(right != null && right.IsNotStatic && !right.IsEmpty && tileColumnAvalibleForOffset[right.X]
				&& CheckAvailabilityWithBarriers(tile, right)) 
			{
				suitables.Add(right);
			}

			if(suitables.Count == 0) {
				continue;
			} else if(suitables.Count == 1) {
				chosen = suitables[0];
			} else {
				if(suitables[0].GetTileItem().IsMoved && !suitables[1].GetTileItem().IsMoved) {
					chosen = suitables[1];
				} else if(suitables[1].GetTileItem().IsMoved && !suitables[0].GetTileItem().IsMoved) {
					chosen = suitables[0];
				} else {
					chosen = suitables[Random.Range(0, 2)];
				}
			}
				
			float rotate = chosen.X < tile.X ? -90 : 90;
			if(!chosen.GetTileItem().IsRotateOnDrop) {
				rotate = 0;
			}
			MoveTileItem(chosen, tile, TileItemMoveType.OFFSET, rotate);
			res = true;
			tileColumnAvalibleForOffset[chosen.X] = false;

			if(chosen.Y == numRows -1) {
				tileItemSpawnDelay[chosen.X]++;
			}
		}

		return res;
	}

	private Tile GetTile(int x, int y) {
		return (x < 0 || y < 0 || x >= numColumns || y >= numRows) ? null : tiles[x, y];
	}
	private Tile GetTile(Vector2 index) {
		return GetTile((int)index.x, (int)index.y);
	}

	private TileItemData GetTileItemData(int x, int y, TileItemData[,] data) {
		return (x < 0 || y < 0 || x >= numColumns || y >= numRows) ? null : data[x, y];
	}

	private void MoveTileItem(Tile from, Tile to, TileItemMoveType moveType, float rotate) {
		AnimatedObject ao = from.GetTileItemGO().GetComponent<AnimatedObject>();

		float speed = App.GetTileItemSpeed(moveType);
		ao.AddMove(IndexToPosition(from.X, from.Y), IndexToPosition(to.X, to.Y), speed);
		to.SetTileItem(from.GetTileItem());

		if(moveType == TileItemMoveType.DOWN) {
			from.GetTileItem().IsMoved = true;
		} else if(moveType == TileItemMoveType.OFFSET) {
			ao.LayerSortingOrder(DEFAULT_TILEITEM_SORTING_ORDER - 3);
		}
			
		if(rotate != 0) {
			ao.AddRotate(null, new Vector3(0, 0, rotate), 
				AMove.CalcTime(IndexToPosition(from.X, from.Y), IndexToPosition(to.X, to.Y), speed));
	
		}

		ao.Build();

		if(from != to) {
			from.SetTileItem(null);
		}
	}

	private void ClearTile(Tile tile, bool destroy = true) {
		if(destroy && tile.GetTileItemGO() != null) {
			Destroy(tile.GetTileItemGO());
		}
		tile.SetTileItem(null);
	}

	private TileItemData GetHeroItemData() {
		foreach(Hero hero in heroes.Values) {
			TileItemData data = hero.GetHeroItemData();
			if(data != null) {
				return data;
			}
		}

		return null;
	}

	private void InitHeroItemAnimation(TileItemData itemData) {
		IList<Tile> avaliableTiles = new List<Tile>();

		for(int x = 0;x < numColumns;x++) {
			Tile tile = tiles[x, numRows - 1];
			if(tile.IsSimple && tile.GetTileItem().IsNotStatic) {
				avaliableTiles.Add(tile);
			}
		}

		if(avaliableTiles.Count == 0) {
			return;
		}
			
		TileItem ti = InstantiateTileItem(itemData.Type, itemData.X, itemData.Y, false);
		ti.Level = itemData.Level;
		Tile dest = avaliableTiles[Random.Range(0, avaliableTiles.Count)];

		AnimatedObject ao = ti.GetGameObject().GetComponent<AnimatedObject>();
		ao.AddMove(ti.GetGameObject().transform.position, dest.GetTileItemGO().transform.position, App.GetTileItemSpeed(TileItemMoveType.HERO_DROP))
			.LayerSortingOrder(DEFAULT_TILEITEM_SORTING_ORDER + 1)
			.OnStop(DestroyGameObjects, new System.Object[] {dest.GetTileItemGO()})
			.Build();

		animationGroup.Add(ao);
		ClearTile(dest, false);
		dest.SetTileItem(ti);
	}


	private IList<TileItemData> GetTileItemDataForEnvelopReplace(Tile tile) {
		return GetTileItemDataForEnvelopReplace(tile.X, tile.Y, tile.TileItemType, null);
	}
		
	private IList<TileItemData> GetTileItemDataForEnvelopReplace(int tileX, int tileY, TileItemType type, TileItemData[,] tileData) {
		if(!TileItem.IsEnvelopItem(type)) {
			return null;
		}

		TileItemTypeGroup typeGroup = TileItem.TypeToTypeGroup(type);
		bool reachable = false;
		IList<TileItemData> res = null;
		TileItemType curTileType;
		int dataX, dataY;
		IList<Vector2> positions = new List<Vector2>();
		int level = (tileData == null)? tiles[tileX, tileY].GetTileItem().Level : tileData[tileX, tileY].Level;
			
		for(int x = tileX - 1; x <= tileX + 1; x++) {
			for(int y = tileY - 1; y <= tileY + 1; y++) {
				if(x == tileX && y == tileY) {
					continue;
				}
				positions.Add(new Vector2(x, y));
			}
		}

		if(level > 1) {
			positions.Add(new Vector2(tileX, tileY - 2));
			positions.Add(new Vector2(tileX, tileY + 2));
			positions.Add(new Vector2(tileX - 2, tileY));
			positions.Add(new Vector2(tileX + 2, tileY));
		}
		if(level > 2) {
			positions.Add(new Vector2(tileX - 1, tileY - 2));
			positions.Add(new Vector2(tileX - 1, tileY + 2));
			positions.Add(new Vector2(tileX + 1, tileY - 2));
			positions.Add(new Vector2(tileX + 1, tileY + 2));
		}
		if(level > 3) {
			positions.Add(new Vector2(tileX - 2, tileY - 1));
			positions.Add(new Vector2(tileX - 2, tileY + 1));
			positions.Add(new Vector2(tileX + 2, tileY - 1));
			positions.Add(new Vector2(tileX + 2, tileY + 1));
		}
		if(level > 4) {
			positions.Add(new Vector2(tileX - 2, tileY - 2));
			positions.Add(new Vector2(tileX - 2, tileY + 2));
			positions.Add(new Vector2(tileX + 2, tileY - 2));
			positions.Add(new Vector2(tileX + 2, tileY + 2));
		}

		foreach(Vector2 pos in positions) {
			bool hasParent;
			if(tileData == null) {
				Tile curTile = GetTile(pos);
				if(curTile == null) {
					continue;
				}
				curTileType = curTile.TileItemType;
				dataX = (int)pos.x;
				dataY = (int)pos.y;
				hasParent = curTile.GetTileItem() != null && curTile.GetTileItem().GetParentTileItem() != null;
			} else {
		//		Preconditions.Check(!isSelected, "Tile {0} {1} can not be selected (check tiledata)", x, y);
				TileItemData curTile = GetTileItemData((int)pos.x, (int)pos.y, tileData);
				if(curTile == null) {
					continue;
				}
				curTileType = curTile.Type;
				dataX = curTile.X;
				dataY = curTile.Y;
				hasParent = curTile.HasParent();
			}
				
			TileItemTypeGroup curTypeGroup = TileItem.TypeToTypeGroup(curTileType);

			if((curTypeGroup == typeGroup || TileItem.IsColorIndependedItem(curTileType)) && TileItem.MayBeFirstItem(curTileType) && CheckAvailabilityTransition(tileX, tileY, (int)pos.x, (int)pos.y)) {
				reachable = true;
			}
			if(hasParent) {
				continue;
			}
			if(curTypeGroup != typeGroup && TileItem.IsSimpleItem(curTileType)) {
				if(res == null) {
					res = new List<TileItemData>();
				}
				TileItemData data = new TileItemData(dataX, dataY, (TileItemType)typeGroup);
				res.Add(data);
			}
		}


		if(!reachable && selectedTiles.Count > 1) {
			Tile lastTile = selectedTiles.Last.Value;
			if(CheckAvailabilityTransition(lastTile.X, lastTile.Y, tileX, tileY)) {
				reachable = true;
			}
		}
		return (reachable)? res : null;
	}


	List<TileItemData> ReplaceTileItems(IList<TileItemData> replaceData, TileItemRenderState state, bool saveOld) {
		List<TileItemData> oldItems = new List<TileItemData>();
		Vector2 index = new Vector2(0, 0);

		foreach(TileItemData itemData in replaceData) {
			Tile tile = GetTile(itemData.X, itemData.Y);
			Preconditions.NotNull(tile, "Can not replace tile item for x={0} y={1}", itemData.X, itemData.Y);
			TileItem tileItem = tile.GetTileItem();
			
			if(saveOld) {
				index.x = itemData.X; index.y = itemData.Y;
				TileItemData old = new TileItemData(tile.X, tile.Y, tileItem.Type);
				oldItems.Add(old);
				if(selectedTiles.Contains(tile)) {
					SelectTileItem(tile, false, state);
				}
			}
		
			TileItem ti = InstantiateTileItem(itemData.Type, itemData.X, itemData.Y, true);
			ti.Level = itemData.Level;
			ti.GetGameObject().transform.rotation = tileItem.GetGameObject().transform.rotation;
			ClearTile(tile);
			tile.SetTileItem(ti);
			ti.SetRenderState(state);
		}

		return oldItems;
	}

	List<TileItemData> ReplaceTileItemsData(IList<TileItemData> replaceData, TileItemData[,] data) {
		List<TileItemData> oldItems = new List<TileItemData>();
		IDictionary<Vector2, TileItemData> dataMap = new Dictionary<Vector2, TileItemData>();
		Vector2 pos = new Vector2(0, 0);

		foreach(TileItemData itemData in replaceData) {
			dataMap.Add(new Vector2(itemData.X, itemData.Y), itemData);
		}

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				pos.x = data[x, y].X;
				pos.y = data[x, y].Y;
				if(dataMap.ContainsKey(pos)) {
					oldItems.Add(data[x, y]);
					data[x, y] = dataMap[pos];
				}
			}
		}
			
		return oldItems;
	}

	private void CheckConsistency() {
		TileItemTypeGroup? group = CheckTileItemSameColorCount(false);
		if(group == null) {
			LevelFailureByColorCount();
			return;
		}

		TileItemData[,] data = GenerateTileItemDataFromCurrentTiles();
		bool validPosition = true;

		int i = 0;
		while(!CheckTileItemsPosition(data) ) {
			validPosition = false;
			if(i++ > 100) {
				data = RepositionTileItemsBySuccessPath();
				if(data == null) {
					LevelFailureByColorCount();
					return;
				}
				break;
			}
			data = MixTileItemData(data);
		}

		Debug.Log("i=" + i);

		if(!validPosition) {
			RepositionTileItems(data);
		}

		IsTileInputAvaliable = true;
	}

	private TileItemTypeGroup? CheckTileItemSameColorCount(bool strict) {
		IDictionary<TileItemTypeGroup, TileItemSameColorCount> items = new Dictionary<TileItemTypeGroup, TileItemSameColorCount>();
		TileItemTypeGroup maxCountType = TileItemTypeGroup.Red;
		int maxCount = 0;

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				Tile tile = tiles[x, y];
				TileItem ti = tile.GetTileItem();

				if(ti == null || !tile.IsColor && !ti.IsColorIndepended) {
					continue;
				}
				if(strict && !tile.GetTileItem().IsReposition) {
					continue;
				}

				int count = 0;
				TileItemTypeGroup tg;
				
				if(ti.IsColorIndepended) {
					foreach(TileItemTypeGroup tgt in TileItem.GetAllColorTileItemGroup()) {
						if(!items.ContainsKey(tgt)) {
							items.Add(tgt, new TileItemSameColorCount(tgt));
						}
						items[tgt].Increment();
					}
					count = maxCount + 1;
					tg = maxCountType;
				} else {
					tg = tile.GetTileItem().TypeGroup;
					if(!items.ContainsKey(tg)) {
						items.Add(tg, new TileItemSameColorCount(tg));
					}
					count = items[tg].Increment();
				}
				
				if(!strict && tile.GetTileItem().IsEnvelop) {
					count += tile.GetTileItem().GetEnvelopReplaceItemCount() ;
				}
					
				if(count > maxCount) {
					maxCount = count;
					maxCountType = tg;
				}
			}
		}

		if(maxCount >= levelData.SuccessCount) {
			return maxCountType;
		}
			
		return null;
	}

	private TileItemData[,] GenerateTileItemDataFromCurrentTiles() {
		TileItemData[,] res = new TileItemData[numColumns, numRows];

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				Tile tile = tiles[x, y];
				res[x, y] = new TileItemData(tile.X, tile.Y, tile.TileItemType);
				if(tile.GetTileItem() != null) {
					res[x, y].Level = tile.GetTileItem().Level;
					if(tile.GetTileItem().GetParentTileItem() != null) { 
						res[x, y].ParentTileItemData = new ChildTileItemData();
						res[x, y].ParentTileItemData.Type = tile.GetTileItem().GetParentTileItem().Type;
						res[x, y].ParentTileItemData.TypeAsString = res[x, y].ParentTileItemData.Type.ToString();
					}
				}
			}
		}

		return res;
	}

	TileItemData[,] MixTileItemData(TileItemData[,] data) {
		IList<TileItemData> avaliableItems = new List<TileItemData>();
		IList<Vector2> positions = new List<Vector2>();

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				if(TileItem.IsRepositionItem(data[x, y])) {
					avaliableItems.Add(data[x, y]);
					positions.Add(new Vector2(x, y));
				}
			}
		}

		int i = 0;
		while(avaliableItems.Count > 0) {
			int index = Random.Range(0, avaliableItems.Count);
			TileItemData itemData = avaliableItems[index];
			Vector2 pos = positions[i++];
			data[(int)pos.x, (int)pos.y] = itemData;
			avaliableItems.RemoveAt(index);
		}

		return data;
	}

	void RepositionTileItems(TileItemData[,] data) {
		Tile[,] tilesSave = new Tile[numColumns, numRows];

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				TileItemData itemData = data[x, y];
				if(!TileItem.IsRepositionItem(itemData) || (x == itemData.X && y == itemData.Y)) {
					continue;
				}

				Tile from = tiles[itemData.X, itemData.Y];
				if(tilesSave[itemData.X, itemData.Y] != null) {
					from = tilesSave[itemData.X, itemData.Y];
				}
				if(tilesSave[x, y] != null) {
					throw new System.Exception("Can not reposition tile item " + itemData.X + " " + itemData.Y);
				}

				tilesSave[x, y] = new Tile(x, y);
				tilesSave[x, y].SetTileItem(tiles[x, y].GetTileItem());

				try {
					MoveTileItem(from, tiles[x, y], TileItemMoveType.MIX, 0);
				} catch(System.Exception e) {
					Debug.Log(from.X + " " + from.Y);
					throw e;
				}
			}
		}

		RunTileItemsAnimation(null, 0);
	}
		
	private bool CheckTileItemsPosition(TileItemData[,] data) {
		IDictionary<Vector2, Object> chain = new Dictionary<Vector2, Object>();

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {	
				TileItemData itemData = data[x, y];
				if(!TileItem.MayBeFirstItem(itemData.Type)) {
					continue;
				}

				Vector2 pos = new Vector2(x, y);

				chain.Clear();
				chain.Add(pos, null);

				if(CheckTileItemsPositionChain(pos, chain, data)) {

					DebugUtill.Log(bestChain);
					Debug.Log("Check position success " +pos);
					return true;
				}
					
			}
		}

		return false;
	}

	IDictionary<Vector2, Object> bestChain;

	private bool CheckTileItemsPositionChain(Vector2 pos, IDictionary<Vector2, Object> chain, TileItemData[,] data) {
		TileItemType type = data[(int)pos.x, (int)pos.y].Type;
		TileItemTypeGroup typeGroup = TileItem.TypeToTypeGroup(type);

		for(int x = (int)pos.x - 1; x <= pos.x + 1; x++) {
			for(int y = (int)pos.y - 1; y <= pos.y + 1; y++) {
				TileItemData itemData = GetTileItemData(x, y, data);
				if(itemData == null || (!TileItem.IsColorItem(itemData.Type) && !TileItem.IsColorIndependedItem(itemData.Type)) {
					continue;
				}
				Vector2 curPos = new Vector2(x, y);
				if(chain.ContainsKey(curPos) || (TileItem.TypeToTypeGroup(itemData.Type) != typeGroup && !TileItem.IsColorIndependedItem(itemData.Type)) {
					continue;
				}

				if(!CheckAvailabilityWithBarriers((int)pos.x, (int)pos.y, x, y)) {
					continue;
				}

				if(chain.Count + 1 >= levelData.SuccessCount) {
					bestChain = chain;
					bestChain.Add(curPos, null);
					return true;
				}

				IDictionary<Vector2, Object> chainNew = new Dictionary<Vector2, Object>(chain);
				chainNew.Add(curPos, null);

				if(CheckTileItemsPositionChain(curPos, chainNew, data)) {
					return true;
				}
					
				if(TileItem.IsEnvelopItem(itemData.Type)) {
					IList<TileItemData> replace = GetTileItemDataForEnvelopReplace(x, y, itemData.Type, data);
					if(replace != null && replace.Count > 0) {
						IList<TileItemData> old = ReplaceTileItemsData(replace, data);
						chainNew = new Dictionary<Vector2, Object>(chain);
						chainNew.Add(curPos, null);
						if(CheckTileItemsPositionChain(curPos, chainNew, data)) {
							ReplaceTileItemsData(old, data);
							return true;
						}
						ReplaceTileItemsData(old, data);

					}
				}
			}
		}
			
		return false;
	}

	TileItemData[,] RepositionTileItemsBySuccessPath() {
		IDictionary<Vector2, Object> path = FindSuccessPath(false);
		if(path == null) {
			path = FindSuccessPath(true);
			if(path == null) {
				return null;
			}
		}

		TileItemTypeGroup? maxTypeGroup = CheckTileItemSameColorCount(true);
		if(maxTypeGroup == null) {
			return null;
		}
			
		int i = 0;
		LinkedList<Vector2> movePositions = new LinkedList<Vector2>();
		TileItemData[,] data = GenerateTileItemDataFromCurrentTiles();
		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {	
				TileItemData tid = data[x, y];
				if(tid == null || (!TileItem.IsColorItem(tid.Type) && !TileItem.IsColorIndependedItem(tid.Type)) || !TileItem.IsRepositionItem(tid)) {
					continue;
				}

				TileItemTypeGroup tg = TileItem.TypeToTypeGroup(tid.Type);
				if(tg == maxTypeGroup || TileItem.IsColorIndependedItem(tid.Type)) {
					Vector2 pos = new Vector2(x, y);
					i++;
					if(!path.ContainsKey(pos)) {
						movePositions.AddLast(pos);
					}
				}

				if(i >= levelData.SuccessCount) {
					break;
				}
			}
		}

		DebugUtill.Log(movePositions, "movePositions");

		foreach(Vector2 pos in path.Keys) {
			TileItemData tid = data[(int)pos.x, (int)pos.y];
			if(TileItem.TypeToTypeGroup(tid.Type) == maxTypeGroup || TileItem.IsColorIndependedItem(tid.Type)) {
				continue;
			}
			Vector2 movePos = movePositions.First.Value;
			data[(int)pos.x, (int)pos.y] = data[(int)movePos.x, (int)movePos.y];
			data[(int)movePos.x, (int)movePos.y] = tid;
			movePositions.RemoveFirst();
		}

		return data;
	}

	IDictionary<Vector2, Object> FindSuccessPath(bool checkEmpty) {
		IDictionary<Vector2, Object> chain = new Dictionary<Vector2, Object>();

		int hDirection = (Random.Range(0, 2) == 0)? 1 : -1;
		int vDirection = (Random.Range(0, 2) == 0)? 1 : -1;

		for(int x = (hDirection > 0)? 0 : numColumns - 1; x < numColumns && x >= 0; x += hDirection) {
			for(int y = (vDirection > 0)? 0 : numRows - 1; y < numRows && y >= 0; y += vDirection) {
				Tile tile = tiles[x, y];
				TileItem item = tile.GetTileItem();

				bool avaliable = (checkEmpty)? item == null || item.IsReposition : item != null && item.IsReposition;
				if(!avaliable) {
					continue;
				}

				Vector2 pos = new Vector2(x, y);

				chain.Clear();
				chain.Add(pos, null);

				if(FindSuccessPathChain(pos, chain, checkEmpty)) {
					DebugUtill.Log(bestChain);
					Debug.Log("Success path " +pos);
					return bestChain;
				}
			}
		}

		return null;
	}

	bool FindSuccessPathChain(Vector2 pos, IDictionary<Vector2, Object> chain, bool checkEmpty) {
		for(int x = (int)pos.x - 1; x <= pos.x + 1; x++) {
			for(int y = (int)pos.y - 1; y <= pos.y + 1; y++) {
				Tile tile = GetTile(x, y);
				if(tile == null) {
					continue;
				}
				TileItem item = tile.GetTileItem();

				bool avaliable = (checkEmpty)? item == null || item.IsReposition : item != null && item.IsReposition;
				if(!avaliable) {
					continue;
				}

				Vector2 curPos = new Vector2(x, y);
				if(chain.ContainsKey(curPos) || !CheckAvailabilityWithBarriers((int)pos.x, (int)pos.y, x, y)) {
					continue;
				}

				if(chain.Count + 1 >= levelData.SuccessCount) {
					bestChain = chain;
					bestChain.Add(curPos, null);
					return true;
				}

				IDictionary<Vector2, Object> chainNew = new Dictionary<Vector2, Object>(chain);
				chainNew.Add(curPos, null);

				if(FindSuccessPathChain(curPos, chainNew, checkEmpty)) {
					return true;
				}
			}
		}

		return false;
	}

	private void ResetBombMark() {
		foreach(Tile tile in bombMarkTiles) {
			TileItem ti = tile.GetTileItem();
			if(rotaitedBombs.Contains(tile)) {
				RotateBomb(tile);
			}
			tile.MarkBomb(false);
		}

		rotaitedBombs.Clear();
		bombMarkTiles.Clear();
	}

	private void MarkBombTiles() {
		if(bombSelectedTiles.Count > 1) {
			int a = 7;
		}
		Debug.Log(bombSelectedTiles.Count);
		foreach(TileItem ti in bombSelectedTiles) {
			Vector3 pos = new Vector3(ti.GetGameObject().transform.position.x ,
				ti.GetGameObject().transform.position.y, 0);
			Tile tile = GetTile(PositionToIndex(pos));
	//		Debug.Log(tile);
			if(!bombMarkTiles.Contains(tile)) {
				MarkBombTile(tile);
			}
		}
	}
	
	private void MarkBombTile(Tile tile) {
		TileItem tileItem = Preconditions.NotNull(tile.GetTileItem());
		Preconditions.Check(tileItem.IsBomb || tileItem.IsBombAll, "Tile item must be bomb");

		IList<Vector2> positions = new List<Vector2>();

		if(tileItem.IsBombH || tileItem.IsBombV) {
			int ratio = gameData.GetBombRatio(tileItem.Level);
			int start = tileItem.IsBombH ? tile.X : tile.Y;

			for(int i = Mathf.Max(0, start - ratio); i <= Mathf.Min(Mathf.Max(numColumns, numRows), start + ratio); i++) {
			//	if(i == start) {
			//		continue;
			//	}

				int x = tileItem.IsBombH ? i : tile.X;
				int y = tileItem.IsBombV ? i : tile.Y;
				positions.Add(new Vector2(x, y));
			}
		} else if(tileItem.IsBombHV) {
			positions.Add(new Vector2(tile.X - 1, tile.Y));
			positions.Add(new Vector2(tile.X + 1, tile.Y));
			positions.Add(new Vector2(tile.X, tile.Y - 1));
			positions.Add(new Vector2(tile.X, tile.Y + 1));
			positions.Add(new Vector2(tile.X, tile.Y));

			if(tileItem.Level > 1) {
				positions.Add(new Vector2(tile.X - 1, tile.Y + 1));
				positions.Add(new Vector2(tile.X + 1, tile.Y - 1));
			}
			if(tileItem.Level > 2) {
				positions.Add(new Vector2(tile.X + 1, tile.Y + 1));
				positions.Add(new Vector2(tile.X - 1, tile.Y - 1));
			}
			if(tileItem.Level > 3) {
				positions.Add(new Vector2(tile.X, tile.Y + 2));
				positions.Add(new Vector2(tile.X, tile.Y - 2));
			}
			if(tileItem.Level > 4) {
				positions.Add(new Vector2(tile.X - 2, tile.Y));
				positions.Add(new Vector2(tile.X + 2, tile.Y));
			}
		} else if(tileItem.IsBombAll || (tileItem.IsBombC && selectedTileItemTypeGroup != null)) {
			for(int x = 0; x < numColumns; x++) {
				for(int y = 0; y < numRows; y++) {
					Tile curTile = tiles[x, y];
					if(tileItem.IsBombAll) {
						positions.Add(new Vector2(x, y));
					} else if(curTile.GetTileItem() != null && curTile.GetTileItem().TypeGroup == selectedTileItemTypeGroup.Value && curTile.IsSimple) {
						positions.Add(new Vector2(x, y));
					}
				}
			}
		} else if(tileItem.IsBombP) {
			for(int x = tile.X - 1; x <= tile.X + 1; x++) {
				for(int y = tile.Y - 1; y <= tile.Y + 1; y++) {
					positions.Add(new Vector2(x, y));
				}
			}
		}

		foreach(Vector2 pos in positions) {
			Tile curTile = GetTile(pos);

			if(curTile == null) {
				continue;
			}
				
			if(!bombMarkTiles.Contains(curTile) /*&& !specialSelectedTiles.Contains(curTile)*/) {
				bombMarkTiles.Add(curTile);
				curTile.MarkBomb(true);

				if(curTile.GetTileItem() == null) {
					continue;
				}

				if((tile != curTile) && (curTile.GetTileItem().IsBomb || curTile.GetTileItem().IsBombAll) && !tileItem.IsBombAll) {
					if(((curTile.GetTileItem().IsBombH && tileItem.IsBombH) || (curTile.GetTileItem().IsBombV && tileItem.IsBombV))) {
						RotateBomb(curTile);

						rotaitedBombs.Add(curTile);
					}
					MarkBombTile(curTile);
				}
			}
		}
	}

	public void RotateBomb(Tile tile) {
		TileItem tileItem = tile.GetTileItem();
		TileItemTypeGroup group = tileItem.TypeGroup;
		TileItemType type;
		Debug.Log(tile);
		if(tileItem.IsColorIndependedBomb) {
			type = tileItem.IsBombH? TileItemType.BombV : TileItemType.BombH;
		} else {
			type = (TileItemType)((int)group + ((tileItem.IsBombH) ? TileItem.BOMBV_OFFSET : TileItem.BOMBH_OFFSET));
		}
		
		tileItem.Rotate();
		tileItem.SetType(type);
	}

	public void RotateBombs() {
		foreach(Tile tile in rotaitedBombs) {
			RotateBomb(tile);
		}
	}

	private void LevelSuccess() {
		SceneController.Instance.LoadSceneAsync("LevelSuccess");
	}

	private void LevelFailure() {
		SceneController.Instance.LoadSceneAsync("LevelFailure");
	}

	private void LevelFailureByColorCount() {
		DisplayMessageController.DisplayMessage("Невозможно собрать цепочку", Color.red);
	//	Invoke("LevelFailure", 3);
	}

	private void OnCheckAutoDropOnDestroyData(TileItem tileItem) {
		if(levelData.AutoDropOnCollectData == null) {
			return;
		}

		foreach(TileItemData itemData in levelData.AutoDropOnCollectData) {
			if(tileItem.Type == itemData.Type) {
				dropRequire.Add(itemData);
			}
		}
	}

	public void UpdateSlime() {
		if(slimeTiles.Count == 0) {
			return;
		}

		TileItemType? type = slime.DropSlime();
		if(type == null) {
			return;
		}

		Tile[] aroundTiles = new Tile[4];
		IList<Tile> avaliableTiles = new List<Tile>();
		foreach(Tile tile in slimeTiles) {
			aroundTiles[0] = GetTile(tile.X + 1, tile.Y);
			aroundTiles[1] = GetTile(tile.X - 1, tile.Y);
			aroundTiles[2] = GetTile(tile.X, tile.Y + 1);
			aroundTiles[3] = GetTile(tile.X, tile.Y - 1);
			foreach(Tile t in aroundTiles) {
				if(t == null || t.IsEmpty ) {
					continue;
				}
				if(t.GetTileItem().IsAbsorbable && !avaliableTiles.Contains(t)
					&& CheckAvailabilityWithBarriers(tile, t)) {
					avaliableTiles.Add(t);
				}
			}
		}

		if(avaliableTiles.Count == 0) {
			return;
		}

		Tile choiceTile = avaliableTiles[Random.Range(0, avaliableTiles.Count)];
		TileItem ti = InstantiateTileItem(type.Value, choiceTile.X, choiceTile.Y, true);
		ClearTile(choiceTile);
		choiceTile.SetTileItem(ti);
		if(!slimeTiles.Contains(choiceTile)) {
			slimeTiles.Add(choiceTile);
		}

		AnimatedObject ao = ti.GetGameObject().GetComponent<AnimatedObject>();
		ao.AddResize(new Vector3(0, 0, 1), new Vector3(1, 1, 1), App.SlimeAnimationTime).Build();
		animationGroup.Add(ao);
	}

	private void UpdateTileItemsByGenerators() {
		foreach(Generator gen in generatorTiles) {
			Tile tile = GetTile(gen.X, gen.Y - 1);
			if(tile == null || !CheckAvailabilityWithBarriers(gen.X, gen.Y, tile.X, tile.Y)) {
				continue;
			}
				
			if(tile.IsEmpty || tile.GetTileItem().IsReplacedByGenerator) {
				TileItemData genItemData = gen.Generate();
				if(genItemData == null) {
					continue;
				}

				if(tile.IsEmpty || tile.GetTileItem().Type != genItemData.Type) {
					TileItem ti = InstantiateTileItem(genItemData.Type, gen.X, gen.Y, true);
					InitTileItem(genItemData, ti);
					if(!tile.IsEmpty) {
						ClearTile(tile);
					}
					AnimatedObject ao = ti.GetGameObject().GetComponent<AnimatedObject>();
					float speed = App.GetTileItemSpeed(TileItemMoveType.GENERATED_TILEITEM_DROP);
					float time = AMove.CalcTime(IndexToPosition(gen.X, gen.Y), IndexToPosition(tile.X, tile.Y), speed);
					ao.AddMove(null, IndexToPosition(tile.X, tile.Y), speed).LayerSortingOrder(DEFAULT_TILEITEM_SORTING_ORDER + 1)
						.AddResize(null, new Vector3(1.3f, 1.3f, 1), time * 0.2f)
						.AddResize(null, new Vector3(1f, 1f, 1), time * 0.7f)
						.Build();
					animationGroup.Add(ao);
					tile.SetTileItem(ti);
				}
			}
		}
	}

	private void OnTileItemReplace(System.Object[] param) {
		TileItem ti = (TileItem)param[0];
		Tile tile = (Tile)param[1];

		if(!tile.IsEmpty) {
			ClearTile(tile);
		}
		tile.SetTileItem(ti);
	}

	private void DestroyGameObjects(System.Object[] objects) {
		foreach(System.Object obj in objects) {
			Destroy((GameObject)obj);
		}
	}

	IEnumerator UpdateTilesWitDelay(bool first, float delay) {
		yield return new WaitForSeconds(delay);
		UpdateTiles(first);
	}

	private void InitBombExplosion(Tile tile) {
		ParticleSystem explosion;
		if(!tile.IsEmpty && ((tile.GetTileItem().IsBomb && !existBombAll) || tile.GetTileItem().IsBombAll)) {
			explosion = Instantiate(BombExplosionBombPS, IndexToPosition(tile.X, tile.Y), Quaternion.identity);
		} else {
			explosion = Instantiate(BombExplosionPS, IndexToPosition(tile.X, tile.Y), Quaternion.identity);
		}
		Destroy(explosion.gameObject, explosion.main.duration);
		if(explosion.main.duration > bombExplosionDelay) {
			bombExplosionDelay = explosion.main.duration;
		}
		explosion.GetComponent<Renderer>().sortingOrder = BOMB_EXPLOSION_SORTING_ORDER;
	/*	ParticleSystem[] children = explosion.GetComponentsInChildren<ParticleSystem>();
		foreach(ParticleSystem ps in children) {
			ps.GetComponent<Renderer>().sortingOrder = BOMB_EXPLOSION_SORTING_ORDER;
		}*/
	}
}








