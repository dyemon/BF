using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AnimationGroup))]
public class GameController : MonoBehaviour {
	public static readonly int TILEITEM_SORTING_ORDER = 10;

	private int numColumns;
	private int numRows;
	public LayerMask tilesItemLayer = -1;

	public GameObject[] tileItemsColor;
	public GameObject[] tileItemsColorBomb;
	public GameObject[] tileItemsColorEnvelop;
	public GameObject[] tileItemsSpecial;
	public GameObject[] tileItemsUnavaliable;
	public GameObject[] barrierItems;

	private AnimationGroup animationGroup;

	private Tile[,] tiles;
	private LinkedList<Tile> selectedTiles = new LinkedList<Tile>();
	private LinkedList<Tile> specialSelectedTiles = new LinkedList<Tile>();
	private Tile[,] tmpTiles;

	private IDictionary<BarrierData, Barrier> barriers = new Dictionary<BarrierData, Barrier>();

	private IDictionary<string, Hero> heroes = new Dictionary<string, Hero>();

	private IDictionary<Vector2, List<TileItemData>> replacedItems = new Dictionary<Vector2, List<TileItemData>>();

	private int[] tileItemSpawnDelay;
	private bool[] tileColumnAvalibleForOffset;

	private LevelData levelData;
	private UserData userData;
	private GameData gameData;

	private Rect tilesArea;

	bool IsTileInputAvaliable { get; set;}

	void Start() {
		levelData = new LevelData();
		levelData.Init();
		userData = new UserData();
		userData.Init();
		gameData = new GameData();
		gameData.Init();

		numColumns = LevelData.NumColumns;
		numRows = LevelData.NumRows;

		float areaOffset = 0.1f;
		tilesArea = new Rect(-numColumns / 2f - areaOffset, 0 - areaOffset, numColumns + 2*areaOffset, numRows + 2*areaOffset);

		animationGroup = GetComponent<AnimationGroup>();
		tiles = new Tile[numColumns, numRows];
		tileItemSpawnDelay = new int[numColumns];
		tileColumnAvalibleForOffset = new bool[numColumns];

		InitTiles();
		InitBarriers();
		InitHeroes();

		DetectUnavaliableTiles();
		UpdateTiles();
	}
	// Update is called once per frame
	void Update() {
		ProcessInput();
	
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
		return new Vector3(x - LevelData.NumColumns / 2f + 0.5f, y + 0.5f, 0);
	}
	public static Vector2 PositionToIndex(Vector3 pos) {
		return new Vector2(pos.x + LevelData.NumColumns / 2f - 0.5f, pos.y - 0.5f);
	}

	private TileItem InstantiateColorOrSpecialTileItem() {
		if(levelData.BrilliantDropRatio > 0 && Random.Range(0, levelData.BrilliantDropRatio) == 0) {
			return InstantiateTileItem(tileItemsSpecial, 0, TileItemType.Brilliant, 0, -10, true);
		}
		int index = Random.Range(0, tileItemsColor.Length);
		return InstantiateTileItem(tileItemsColor, index, (TileItemType)(index*20), 0, -10, true);
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
				return InstantiateTileItem(GetColorGameObjectsByTileItemType(type),  (int)group/20, type, x, y, convertIndexToPos);
			case TileItemTypeGroup.Unavaliable:
				return InstantiateTileItem(tileItemsUnavaliable, (int)(type) - (int)group, type, x, y, convertIndexToPos);
			case TileItemTypeGroup.Special:
				return InstantiateTileItem(tileItemsSpecial, (int)(type) - (int)group, type, x, y, convertIndexToPos);
		}

		throw new System.NotImplementedException("Can not instantient tile item with type " + type.ToString());
	}

	private GameObject[] GetColorGameObjectsByTileItemType(TileItemType type) {
		int index = TileItem.TypeToIndex(type);
		switch(index) {
			case 0:
				return tileItemsColor;
			case TileItem.BOMB_OFFSET:
				return tileItemsColorBomb;
			case TileItem.ENVELOP_OFFSET:
				return tileItemsColorEnvelop;
		}

		throw new System.NotImplementedException("Can not get tile item game objects for type " + type.ToString());

	}

	private TileItem InstantiateTileItem(GameObject[] items, int index, TileItemType type, int x, int y, bool convertIndexToPos) {
		GameObject go = (GameObject)Instantiate(items[index], (!convertIndexToPos)? new Vector3(x, y, 0) : IndexToPosition(x, y), Quaternion.identity);
		TileItem ti = TileItem.Instantiate(type, go);
		go.GetComponent<SpriteRenderer>().sortingOrder = TILEITEM_SORTING_ORDER;

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
		if(tile == null || tile.GetTileItem() == null || !tile.GetTileItem().MayBeFirst) {
			return;
		}
		Preconditions.Check(replacedItems.Count == 0, "replacedItems count must be 0 {0}", replacedItems.Count);
		Preconditions.Check(selectedTiles.Count == 0, "selectedTiles count must be 0 {0}", selectedTiles.Count);

		SetTileItemsState(TileItemState.Dark, tile.GetTileItem().TypeGroup);
		SelectTileItem(tile, true);
	}

	private void MoveTouch(Tile tile) {
		if(tile == null || tile.GetTileItem() == null || selectedTiles.Count == 0) {
			return;
		}

		Tile lastTile = selectedTiles.Last.Value;
		if(lastTile == tile || tile.GetTileItem().TypeGroup != lastTile.GetTileItem().TypeGroup) {
			return;
		}

		Tile predLastTile = null;
		if(selectedTiles.Count > 1) {
			predLastTile = selectedTiles.Last.Previous.Value;
			if(predLastTile == tile) {
				Vector2 index = new Vector2(lastTile.X, lastTile.Y);
				if(replacedItems.ContainsKey(index)) {
					ReplaceTileItems(replacedItems[index], TileItemState.Dark, false);
					replacedItems.Remove(index);
				}
				SelectTileItem(lastTile, false);
				return;
			}
		}
		if(selectedTiles.Contains(tile)) {

		} else if(CheckAvailabilityTransition(lastTile.X, lastTile.Y, tile.X, tile.Y)){
			IList<TileItemData> replaceData = GetTileItemDataForEnvelopReplace(tile);
			if(replaceData != null) {
				Vector2 index = new Vector2(tile.X, tile.Y);
				replacedItems[index] = ReplaceTileItems(replaceData, TileItemState.Normal, true);
			}
			SelectTileItem(tile, true);
		}

	}

	private void EndTouch(Tile tile) {
		if(selectedTiles.Count >= levelData.SuccessCount) {
			CollectTileItems();
		} else {
			ResetSelected();
		}
	}

	private void ResetSelected() {
		SetTileItemsState(TileItemState.Normal, null);
		foreach(Vector2 index in replacedItems.Keys) {
			ReplaceTileItems(replacedItems[index], TileItemState.Normal, false);
		}

		specialSelectedTiles.Clear();
		selectedTiles.Clear();
		replacedItems.Clear();
	}

	public void CollectTileItems() {
		bool isDetectAvaliable = false;
		SetTileItemsState(TileItemState.Normal, null);

		foreach(Tile tile in selectedTiles) {
			CollectTileItem(tile);
			if(BreakTileItems(tile.X, tile.Y)) {
				isDetectAvaliable = true;
			}

			if(BreakBarriers(tile.X, tile.Y)) {
				isDetectAvaliable = true;
			}
		}
		foreach(Tile tile in specialSelectedTiles) {
			CollectTileItem(tile);
		}

		specialSelectedTiles.Clear();
		selectedTiles.Clear();
		replacedItems.Clear();

		if(isDetectAvaliable) {
			DetectUnavaliableTiles();
		}

		UpdateTiles();
	}

	private void CollectTileItem(Tile tile) {
		ClearTile(tile);
	}

	private void SelectTileItem(Tile tile, bool isSelect, TileItemState unSelectedState = TileItemState.Normal) {
		Preconditions.NotNull(tile, "Tile {0} {1} can not be null", tile.X, tile.Y);
		TileItem tileItem = Preconditions.NotNull(tile.GetTileItem(), "Tile Item {0} {1} can not be null", tile.X, tile.Y);

		if(isSelect) {
			tileItem.Select();
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
			if(tile != null && tile.GetTileItem() != null && tile.GetTileItem().IsSpecialCollect && !specialSelectedTiles.Contains(tile)) {
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
			SelectTileItem(tile, false, TileItemState.Dark);
		}
	}
		
	private bool BreakTileItems(int x, int y) {
		bool res = false;
		Tile[] nearTiles = new Tile[4];
		nearTiles[0] = GetTile(x - 1, y);
		nearTiles[1] = GetTile(x + 1, y);
		nearTiles[2] = GetTile(x, y - 1);
		nearTiles[3] = GetTile(x, y + 1);

		foreach(Tile tile in nearTiles) {
			if(tile == null || tile.GetTileItem() == null) {
				continue;
			}

			if(tile.GetTileItem().Damage(1) <= 0) {
				res = true;
				BreakTileItem(tile);
			}
		}

		return res;
	}

	private void BreakTileItem(Tile tile) {
		ClearTile(tile);
	}

	private bool BreakBarriers(int x, int y) {
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

			if(barrier.Damage(1) <= 0) {
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

	private void SetTileItemsState(TileItemState state, TileItemTypeGroup? excludeTypeGroup ) {
		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				Tile tile = tiles[x, y];
				if(!tile.IsAvaliable || tile.GetTileItem() == null) {
					continue;
				}

				if(excludeTypeGroup == null || tile.GetTileItem().TypeGroup != excludeTypeGroup.Value) {
					tile.GetTileItem().SetState(state);
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
				if(!tiles[x, y].IsAvaliable || tiles[x, y].IsEmpty ) {
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
		barriers.TryGetValue(new BarrierData(x1, y1, x2, y2, BarrierType.Wood), out val);
		return val;
	}

	private void DetectUnavaliableTiles() {
		for(int y = numRows - 1; y >= 0; y--) {
			for(int x = 0; x < numColumns; x++) {
				Tile tile = tiles[x, y];
				tile.Type = TileType.Avaliable;

				if(x == 3 && y == 2) {
					tile = tiles[x, y];
				}
				if(tile.GetTileItem() != null && !tile.GetTileItem().IsAvaliable()) {
					continue;
				}

				Tile left = GetTile(x - 1, y + 1);
				Tile center = GetTile(x, y + 1);
				Tile right = GetTile(x + 1, y + 1);

				bool av1 = CheckAvailabilityWithBarriers(tile, left);
				bool av2 = CheckAvailabilityWithBarriers(tile, center);
				bool av3 = CheckAvailabilityWithBarriers(tile, right);

				if(!av1 && !av2 && !av3) {
					tile.Type = TileType.Unavaliable;
			//		InstantiateTileItem(TileItemType.Brilliant, x, y, true);
					continue;
				}

				if((x == 0 || (left != null && !left.IsAvaliable) )
					&& (center != null && !center.IsAvaliable ) 
					&& ((x == numColumns - 1) || right != null && !right.IsAvaliable)) 
				{
					tile.Type = TileType.Unavaliable;
			//		InstantiateTileItem(TileItemType.Brilliant, x, y, true);
				} else {
			//		tile.Type = TileType.Avaliable;
			//		if(tile.GetTileItem() != null && tile.GetTileItem().Type == TileItemType.Brilliant) {
			//			ClearTile(tile);
			//		}
				}
			}
		}	
	}

	private bool CheckAvailabilityWithBarriers(Tile from, Tile to) {
		Preconditions.NotNull(from, "Tile from can not be null");

		if(to == null) {
			return from.Y == numRows - 1;
		}

		if(!to.IsAvaliable || !from.IsAvaliable) {
			return false;
		}

		return CheckAvailabilityWithBarriers(from.X, from.Y, to.X, to.Y);
	}

	private bool CheckAvailabilityWithBarriers(int fromX, int fromY, int toX, int toY) {
		if(Mathf.Abs(fromX - toX) > 1 || Mathf.Abs(fromY - toY) > 1) {
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
		bool res = false;

		if(Mathf.Abs(fromX - toX) > 1 || Mathf.Abs(fromY - toY) > 1) {
			return false;
		}

		return CheckAvailabilityWithBarriers(fromX, fromY, toX, toY);
	}

	private void RunTileItemsAnimation<T>(AnimationGroup.CompleteAnimation<T> complete, T param) {
		animationGroup.Clear();

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				if(!tiles[x, y].IsAvaliable || tiles[x, y].IsEmpty){
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

	private void OnTileItemUpdateComplete(bool getHeroItem) {
		if(getHeroItem) {
			TileItemData data = GetHeroItemData();
			if(data != null) {
				RunHeroItemAnimation(data);
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

	private void UpdateTiles() {
		IsTileInputAvaliable = false;
		ResetTileItemSpawnDelay();

		while(UpdateTilesColumns()) {
			ResetTileColumnAvalibleForOffset();
			UpdateTilesWithOffset();
			ResetTileItemMoved();
		}
			
		RunTileItemsAnimation(OnTileItemUpdateComplete, true);
	}

	private void InitTiles() {
		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				if(tiles[x, y] == null) {
					tiles[x, y] = new Tile(x, y);
				}
			}
		}

		foreach(TileItemData item in levelData.TileData) {
			if(tiles[item.X, item.Y].GetTileItem() != null) {
				throw new System.Exception("Invalid configuration for tile " + tiles[item.X, item.Y] + ". Tile is configured twice");
			}
			tiles[item.X, item.Y].SetTileItem(InstantiateTileItem(item.Type, item.X, item.Y, true));
		}
	}

	private void InitBarriers() {
		foreach(BarrierData data in levelData.BarrierData) {
			if(!barriers.ContainsKey(data)) {
				barriers.Add(data, InstantiateBarrier(data));
			} else {
				throw new System.Exception("Invalid configuration for Barrier " + data + ". Barrier is configured twice");
			}
		}
	}

	private void InitHeroes() {
		foreach(string id in userData.HeroeIds) {
			heroes[id] = new Hero(gameData.HeroData[id]);
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

			if(!tile.IsAvaliable) {
				tEmpty.Clear();
				continue;
			}

			if(tile.IsEmpty) {
				tEmpty.Add(tile);
				res = true;
			}

			if(tEmpty.Count > 0 && !tile.IsEmpty) {
				MoveTileItem(tile, tEmpty[0], TileItemMoveType.DOWN);
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
			TileItem tileItem = InstantiateColorOrSpecialTileItem();
			tileItem.GetGameObject().GetComponent<AnimatedObject>().AddIdle((App.MoveTileItemTimeUnit + App.moveTileItemDelay) * tileItemSpawnDelay[x]).Build();
			spawnTile.SetTileItem(tileItem);
			MoveTileItem(spawnTile, tile, TileItemMoveType.DOWN);
			tileItemSpawnDelay[x]++;
		}

		return res;
	}

	private void UpdateTilesWithOffset() {
		for(int y = 0;y < numRows - 1;y++) {
			UpdateTilesWithOffsetRow(y);
		}
	}

	private void UpdateTilesWithOffsetRow(int y) {
		int direction = (Random.Range(0, 2) == 0)? 1 : -1;
		IList<Tile> suitables = new List<Tile>();

		for(int x = (direction > 0)? 0 : numColumns - 1;x < numColumns && x >= 0; x += direction) {
			Tile tile = tiles[x, y];
			Tile top = GetTile(x, y + 1);
			bool colAvaliable = tileColumnAvalibleForOffset[x];

			if(top != null && !top.IsAvaliable) {
				tileColumnAvalibleForOffset[top.X] = true;
			}

			if(!colAvaliable) {
				continue;
			}
			if(!tile.IsAvaliable || !tile.IsEmpty) {
				continue;
			}

			Tile left = GetTile(x + 1, y + 1);
			Tile right = GetTile(x - 1, y + 1);
			Tile chosen = null;
			suitables.Clear();

			if(left != null && left.IsAvaliable && !left.IsEmpty && tileColumnAvalibleForOffset[left.X]
				&& CheckAvailabilityWithBarriers(tile, left)) 
			{
				suitables.Add(left);
			}
			if(right != null && right.IsAvaliable && !right.IsEmpty && tileColumnAvalibleForOffset[right.X]
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
				
			MoveTileItem(chosen, tile, TileItemMoveType.OFFSET);
			tileColumnAvalibleForOffset[chosen.X] = false;

			if(chosen.Y == numRows -1) {
				tileItemSpawnDelay[chosen.X]++;
			}
		}
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

	private void MoveTileItem(Tile from, Tile to, TileItemMoveType moveType) {
		AnimatedObject ao = from.GetTileItemGO().GetComponent<AnimatedObject>();

		float speed = App.GetTileItemSpeed(moveType);
		ao.AddMove(IndexToPosition(from.X, from.Y), IndexToPosition(to.X, to.Y), speed);
		to.SetTileItem(from.GetTileItem());

		if(moveType == TileItemMoveType.DOWN) {
			from.GetTileItem().IsMoved = true;
		} else if(moveType == TileItemMoveType.OFFSET) {
			ao.LayerSortingOrder(TILEITEM_SORTING_ORDER - 3);
		}

		ao.Build();

		if(from != to) {
			from.SetTileItem(null);
		}
	}

	private void ClearTile(Tile tile) {
		if(tile.GetTileItemGO() != null) {
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

	private void RunHeroItemAnimation(TileItemData itemData) {
		IList<Tile> avaliableTiles = new List<Tile>();

		for(int x = 0;x < numColumns;x++) {
			Tile tile = tiles[x, numRows - 1];
			if(tile.IsSimple) {
				avaliableTiles.Add(tile);
			}
		}

		if(avaliableTiles.Count == 0) {
			CheckConsistency();
			return;
		}

		animationGroup.Clear();

		TileItem ti = InstantiateTileItem(itemData.Type, itemData.X, itemData.Y, false);
		Tile dest = avaliableTiles[Random.Range(0, avaliableTiles.Count)];

		AnimatedObject ao = ti.GetGameObject().GetComponent<AnimatedObject>();
		ao.AddMove(ti.GetGameObject().transform.position, dest.GetTileItemGO().transform.position, App.GetTileItemSpeed(TileItemMoveType.HERO_DROP));
		ao.Build();

		animationGroup.Add(ao);

		animationGroup.Run(OnTileItemUpdateComplete, new System.Object[] {ti, dest});
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
		Vector2 pos = new Vector2(0, 0);

		for(int x = tileX -1; x <= tileX + 1; x++) {
			for(int y = tileY - 1; y <= tileY + 1; y++) {
				if(x == tileX && y == tileY) {
					continue;
				}
				pos.x = x;
				pos.y = y;
			//	bool isSelected = selectedTiles.ContainsKey(pos);

				if(tileData == null) {
					Tile curTile = GetTile(x, y);
					if(curTile == null) {
						continue;
					}
					curTileType = curTile.TileItemType;
					dataX = x;
					dataY = y;
				} else {
			//		Preconditions.Check(!isSelected, "Tile {0} {1} can not be selected (check tiledata)", x, y);
					TileItemData curTile = GetTileItemData(x, y, tileData);
					if(curTile == null) {
						continue;
					}
					curTileType = curTile.Type;
					dataX = curTile.X;
					dataY = curTile.Y;
				}
				
				if(!TileItem.IsColorItem(curTileType)) {
						continue;
				}

				TileItemTypeGroup curTypeGroup = TileItem.TypeToTypeGroup(curTileType);

				if(curTypeGroup == typeGroup && TileItem.IsSimpleItem(curTileType) && CheckAvailabilityWithBarriers(tileX, tileY, x, y)) {
					reachable = true;
				}
				if(curTypeGroup != typeGroup && CheckAvailabilityWithBarriers(tileX, tileY, x, y)) {
					if(res == null) {
						res = new List<TileItemData>();
					}
					TileItemData data = new TileItemData(dataX, dataY, (TileItemType)typeGroup);
					res.Add(data);
				}
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


	List<TileItemData> ReplaceTileItems(IList<TileItemData> replaceData, TileItemState state, bool saveOld) {
		List<TileItemData> oldItems = new List<TileItemData>();
		Vector2 index = new Vector2(0, 0);

		foreach(TileItemData itemData in replaceData) {
			Tile tile = GetTile(itemData.X, itemData.Y);
			Preconditions.NotNull(tile, "Can not replace tile item for x={0} y={1}", itemData.X, itemData.Y);

			if(saveOld) {
				index.x = itemData.X; index.y = itemData.Y;
				TileItemData old = new TileItemData(tile.X, tile.Y, tile.GetTileItem().Type);
				oldItems.Add(old);
				if(selectedTiles.Contains(tile)) {
					SelectTileItem(tile, false, state);
				}
			}

			ClearTile(tile);
			TileItem ti = InstantiateTileItem(itemData.Type, itemData.X, itemData.Y, true);
			tile.SetTileItem(ti);
			ti.SetState(state);
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
		bool? validCount = CheckTileItemSameColorCount(true);
		if(validCount == null) {
			validCount = CheckTileItemSameColorCount(false);
		}

		TileItemData[,] data = GenerateTileItemDataFromCurrentTiles();
		bool validPosition = true;

		if(!validCount.Value) {
			data = MixTileItemData(data);
		}

		int i = 0;
		while(!CheckTileItemsPosition(data)) {
			if(i++ > 50) {
				RecolorTileItemsBySuccessPath();
				IsTileInputAvaliable = true;
				return;
			}
		
			validPosition = false;
			data = MixTileItemData(data);
		}

		Debug.Log("i=" + i);

		if(!validCount.Value || !validPosition) {
			RepositionTileItems(data);
		}

		IsTileInputAvaliable = true;
	}

	private bool? CheckTileItemSameColorCount(bool colorOnly) {
		IDictionary<TileItemTypeGroup, TileItemSameColorCount> items = new Dictionary<TileItemTypeGroup, TileItemSameColorCount>();
		TileItemTypeGroup maxCountType = TileItemTypeGroup.Red;
		int maxCount = 0;

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				Tile tile = tiles[x, y];
				if(!tile.IsColor) {
					continue;
				}

				TileItemTypeGroup tg = tile.GetTileItem().TypeGroup;
				if(!items.ContainsKey(tg)) {
					items.Add(tg, new TileItemSameColorCount(tg));
				}
				int count = items[tg].Increment();
				if(tile.GetTileItem().IsEnvelop) {
					count = items[tg].AddPositions(GetTileItemDataForEnvelopReplace(tile));
				}

				if(count >= levelData.SuccessCount) {
					return true;
				}

				if(count > maxCount) {
					maxCount = count;
					maxCountType = tg;
				}
			}
		}

		Debug.Log("CheckTileItemSameColorCount " + maxCount + " " + maxCountType);

		maxCount = items.ContainsKey(maxCountType) ? items[maxCountType].GetItemCount() : 0;
		int success = levelData.SuccessCount - maxCount;
		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				Tile tile = tiles[x, y];
				bool avaliable = (colorOnly) ? tile.IsColor : tile.IsAvaliable;
				if(!avaliable) {
					continue;
				}

				if(!tile.IsEmpty && tile.GetTileItem().TypeGroup == maxCountType) {
					continue;
				}

				RecolorTileItem(tile, maxCountType);
				success--;
				if(success == 0) {
					return false;
				}
			}
		}

		if(colorOnly) {
			return null;
		}

		throw new LevelConfigException("Can not instantiate " + levelData.SuccessCount + " items same color");
	}

	private void RecolorTileItem(Tile tile, TileItemTypeGroup type) {
		int tileItemIndex = TileItem.TypeToIndex(tile.GetTileItem().Type);
		ClearTile(tile);
		tile.SetTileItem(InstantiateTileItem((TileItemType)(type + tileItemIndex), tile.X, tile.Y, true));
	}

	private TileItemData[,] GenerateTileItemDataFromCurrentTiles() {
		TileItemData[,] res = new TileItemData[numColumns, numRows];

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				Tile tile = tiles[x, y];
				res[x, y] = new TileItemData(tile.X, tile.Y, tile.TileItemType);
			}
		}

		return res;
	}

	TileItemData[,] MixTileItemData(TileItemData[,] data) {
		IList<TileItemData> avaliableItems = new List<TileItemData>();
		IList<Vector2> positions = new List<Vector2>();

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				if(TileItem.IsAvaliableItem(data[x, y].Type)) {
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
				if(!TileItem.IsAvaliableItem(itemData.Type) || (x == itemData.X && y == itemData.Y)) {
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
					MoveTileItem(from, tiles[x, y], TileItemMoveType.MIX);
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
				if(itemData == null || !TileItem.IsColorItem(itemData.Type)) {
					continue;
				}
				Vector2 curPos = new Vector2(x, y);
				if(chain.ContainsKey(curPos) || TileItem.TypeToTypeGroup(itemData.Type) != typeGroup) {
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

	void RecolorTileItemsBySuccessPath() {
		IDictionary<Vector2, Object> path = FindSuccessPath(true);
		if(path == null) {
			path = FindSuccessPath(false);
		}

		IDictionary<TileItemTypeGroup, int> colorCount = new Dictionary<TileItemTypeGroup, int>();
		int maxCount = 0;
		TileItemTypeGroup maxTypeGroup = TileItemTypeGroup.Red;

		foreach(Vector2 pos in path.Keys) {
			Tile tile = tiles[(int)pos.x, (int)pos.y];
			if(!tile.IsColor) {
				continue;
			}

			TileItemTypeGroup tg = tile.GetTileItem().TypeGroup;
			if(!colorCount.ContainsKey(tg)) {
				colorCount.Add(tg, 1);
			}

			if(colorCount[tg] > maxCount) {
				maxCount = colorCount[tg];
				maxTypeGroup = tg;
			}
		}

		foreach(Vector2 pos in path.Keys) {
			Tile tile = tiles[(int)pos.x, (int)pos.y];
			RecolorTileItem(tile, maxTypeGroup);
		}
	}

	IDictionary<Vector2, Object> FindSuccessPath(bool colorOnly) {
		IDictionary<Vector2, Object> chain = new Dictionary<Vector2, Object>();

		int hDirection = (Random.Range(0, 2) == 0)? 1 : -1;
		int vDirection = (Random.Range(0, 2) == 0)? 1 : -1;

		for(int x = (hDirection > 0)? 0 : numColumns - 1; x < numColumns && x >= 0; x += hDirection) {
			for(int y = (vDirection > 0)? 0 : numRows - 1; y < numRows && y >= 0; y += vDirection) {
				Tile tile = tiles[x, y];

				bool avaliable = (colorOnly) ? tile.IsColor : tile.IsAvaliable;
				if(!avaliable) {
					continue;
				}

				Vector2 pos = new Vector2(x, y);

				chain.Clear();
				chain.Add(pos, null);

				if(FindSuccessPathChain(pos, chain, colorOnly)) {
					DebugUtill.Log(bestChain);
					Debug.Log("Success path " +pos);
					return bestChain;
				}
			}
		}

		if(colorOnly) {
			return null;
		}

		throw new LevelConfigException("Can not detect sucess path");
	}

	bool FindSuccessPathChain(Vector2 pos, IDictionary<Vector2, Object> chain, bool colorOnly) {
		for(int x = (int)pos.x - 1; x <= pos.x + 1; x++) {
			for(int y = (int)pos.y - 1; y <= pos.y + 1; y++) {
				Tile tile = GetTile(x, y);
				if(tile == null) {
					continue;
				}
				bool avaliable = (colorOnly) ? tile.IsColor : tile.IsAvaliable;
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

				if(FindSuccessPathChain(curPos, chainNew, colorOnly)) {
					return true;
				}
			}
		}

		return false;	
	}


}








