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
	private IList<Tile> selectedTiles = new List<Tile>();
	private Tile[,] tmpTiles;

	private IDictionary<BarrierData, Barrier> barriers = new Dictionary<BarrierData, Barrier>();

	private IDictionary<string, Hero> heroes = new Dictionary<string, Hero>();

	private IDictionary<Vector2, List<TileItemData>> replacedItems = new Dictionary<Vector2, List<TileItemData>>();


	private int[] tileItemSpawnDelay;
	private bool[] tileColumnAvalibleForOffset;

	private LevelData levelData;
	private UserData userData;
	private GameData gameData;

	void Start() {
		levelData = new LevelData();
		levelData.Init();
		userData = new UserData();
		userData.Init();
		gameData = new GameData();
		gameData.Init();

		numColumns = LevelData.NumColumns;
		numRows = LevelData.NumRows;

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

		return new Barrier(data, go);
	}

	private TileItem InstantiateTileItem(TileItemType type, int x, int y, bool convertIndexToPos) {
		TileItemTypeGroup group = TileItem.TypeToGroupType(type);
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
		TileItem ti = new TileItem(type, go);
		go.GetComponent<SpriteRenderer>().sortingOrder = TILEITEM_SORTING_ORDER;

		return ti;
	}

	private void ProcessInput() {
		InputController.Touch[] touches = InputController.getTouches();

		if(touches.Length > 0) {
			InputController.Touch touch = touches[0];
			if(touch.phase == TouchPhase.Began) {
				Ray ray = InputController.TouchToRay(touches[0]);
				RaycastHit2D hit = Physics2D.Raycast( ray.origin, Vector2.zero, Mathf.Infinity, tilesItemLayer );
				if(hit.collider != null) {
					Tile tile = GetTile(hit.collider.gameObject);
					if(tile != null) {
						tile.GetTileItem().ToggleSelect();
						Vector2 index = new Vector2(tile.X, tile.Y);
						if(selectedTiles.Contains(tile)) {
							if(replacedItems.ContainsKey(index)) {
								ReplaceTileItems(replacedItems[index]);
								replacedItems.Remove(index);
							}
							selectedTiles.Remove(tile);
						} else {
							IList<TileItemData> replaceData = GetTileItemDataForEnvelopReplace(tile);
							Debug.Log(replaceData);
							if(replaceData != null) {
								replacedItems[index] = ReplaceTileItems(replaceData);
							}
							selectedTiles.Add(tile);
						}
					}

				}
			
			}
		}
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

				if(!tile.IsAvaliable) {
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
			//		InstantiateTileItem(TileItemType.Red, x, y);
					continue;
				}

				if((x == 0 || (left != null && !left.IsAvaliable) )
					&& (center != null && !center.IsAvaliable ) 
					&& ((x == numColumns - 1) || right != null && !right.IsAvaliable)) 
				{
					tile.Type = TileType.Unavaliable;
			//		InstantiateTileItem(TileItemType.Red, x, y);
				} else {
					tile.Type = TileType.Avaliable;
				}
			}
		}	
	}

	private bool CheckAvailabilityWithBarriers(Tile from, Tile to) {
		if(from == null ) {
			throw new System.ArgumentException("Tile from can not be null");
		}

		if(to == null) {
			return from.Y == numRows - 1;
		}

		if(!to.IsAvaliable || !from.IsAvaliable) {
			return false;
		}

		if(Mathf.Abs(from.X - to.X) > 1 || Mathf.Abs(from.Y - to.Y) > 1) {
			throw new System.ArgumentException("Can not check availability from " + from + " to " + to);
		}

		if(from.X == to.X || from.Y == to.Y) {
			return GetBarrier(from.X, from.Y, to.X, to.Y) == null;
		}

	
		bool b1 = GetBarrier(from.X, from.Y, to.X, from.Y) != null;
		bool b2 = GetBarrier(to.X, from.Y, to.X, to.Y) != null;
		bool b3 = GetBarrier(to.X, to.Y, from.X, to.Y) != null;
		bool b4 = GetBarrier(from.X, to.Y, from.X, from.Y) != null;

		if(b1 && b3 || b1 && b4 || b2 && b3 || b2 && b4) {
			return false;
		}

		return true;
	}

	public void DropTileItems() {
		foreach(Tile tile in selectedTiles) {
			ClearTile(tile);
		}
		selectedTiles.Clear();
		UpdateTiles();
	}

	private void RunAnimation() {
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

		animationGroup.Run(OnTileItemUpdateComplete, true);
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
		ResetTileItemSpawnDelay();

		while(UpdateTilesColumns()) {
			ResetTileColumnAvalibleForOffset();
			UpdateTilesWithOffset();
			ResetTileItemMoved();
		}
			
		RunAnimation();
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
				MoveTileItem(tile, tEmpty[0], false);
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
			MoveTileItem(spawnTile, tile, false);
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
				
			MoveTileItem(chosen, tile, true);
			tileColumnAvalibleForOffset[chosen.X] = false;

			if(chosen.Y == numRows -1) {
				tileItemSpawnDelay[chosen.X]++;
			}
		}
	}

	private Tile GetTile(int x, int y) {
		return (x < 0 || y < 0 || x >= numColumns || y >= numRows) ? null : tiles[x, y];
	}

	private void MoveTileItem(Tile from, Tile to, bool isOffset) {
		AnimatedObject ao = from.GetTileItemGO().GetComponent<AnimatedObject>();

		float speed = (isOffset) ? App.moveTileItemOffsetSpeed : App.moveTileItemSpeed;
		ao.AddMove(IndexToPosition(from.X, from.Y), IndexToPosition(to.X, to.Y), speed);
		to.SetTileItem(from.GetTileItem());

		if(!isOffset) {
			from.GetTileItem().IsMoved = true;
		} else {
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

		TileItem ti = InstantiateTileItem(itemData.Type, itemData.X, itemData.Y, false);
		Tile dest = avaliableTiles[Random.Range(0, avaliableTiles.Count)];

		AnimatedObject ao = ti.GetGameObject().GetComponent<AnimatedObject>();
		ao.AddMove(ti.GetGameObject().transform.position, dest.GetTileItemGO().transform.position, App.moveHeroItemSpeed);
		ao.Build();

		animationGroup.Add(ao);

		animationGroup.Run(OnTileItemUpdateComplete, new System.Object[] {ti, dest});
	}

	private IList<TileItemData> GetTileItemDataForEnvelopReplace(Tile tile) {
		if(!tile.GetTileItem().IsEnvelop) {
			return null;
		}

		bool reachable = false;
		IList<TileItemData> res = null;

		for(int x = tile.X -1; x <= tile.X + 1; x++) {
			for(int y = tile.Y - 1; y <= tile.Y + 1; y++) {
				Tile curTile = GetTile(x, y);
				if((x == tile.X && y == tile.Y) || curTile == null || !curTile.IsColor) {
					continue;
				}
				if(curTile.GetTileItem().TypeGroup == tile.GetTileItem().TypeGroup) {
					reachable = true;
				}
				if(curTile.GetTileItem().TypeGroup != tile.GetTileItem().TypeGroup && CheckAvailabilityWithBarriers(tile, curTile)) {
					if(res == null) {
						res = new List<TileItemData>();
					}
					TileItemData data = new TileItemData(curTile.X, curTile.Y, (TileItemType)tile.GetTileItem().TypeGroup);
					res.Add(data);
				}
			}
		}

		return (reachable)? res : null;
	}


	List<TileItemData> ReplaceTileItems(IList<TileItemData> replaceData) {
		List<TileItemData> oldItems = new List<TileItemData>();

		foreach(TileItemData itemData in replaceData) {
			Tile tile = GetTile(itemData.X, itemData.Y);
			if(tile == null) {
				throw new System.ArgumentException("Can not replace tile item for x=" + itemData.X + " y=" + itemData.Y);
			}
			TileItemData old = new TileItemData(tile.X, tile.Y, tile.GetTileItem().Type);
			oldItems.Add(old);
			ClearTile(tile);
			tile.SetTileItem(InstantiateTileItem(itemData.Type, itemData.X, itemData.Y, true));
		}

		return oldItems;
	}

	private void CheckConsistency() {
		bool validCount = CheckTileItemSameColorCount();
		TileItemData[,] data = GenerateTileItemDataFromCurrentTiles();
		bool validPosition = true;
		bool isFirst = true;

		if(!validCount) {
			data = MixTileItemData(data);
			isFirst = false;
		}

		int i = 0;
		while(!CheckTileItemsPosition(data)) {
			if(i++ > 100) {
				throw new System.Exception("Can not to position " + levelData.SuccessCount + " items");
			}

			validPosition = false;
			if(!isFirst) {
				data = GenerateTileItemDataFromCurrentTiles();
			} else {
				isFirst = false;
			}
			data = MixTileItemData(data);
		}

		if(!validCount || !validPosition) {
			RepositionTileItems(data);
		}
	}

	private bool CheckTileItemSameColorCount() {
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
				if(!tile.IsAvaliable) {
					continue;
				}

				if(!tile.IsEmpty && tile.GetTileItem().TypeGroup == maxCountType) {
					continue;
				}

				int tileItemIndex = TileItem.TypeToIndex(tile.GetTileItem().Type);
				ClearTile(tile);
				tile.SetTileItem(InstantiateTileItem((TileItemType)(maxCountType + tileItemIndex), tile.X, tile.Y, true));
				success--;
				if(success == 0) {
					return false;
				}
			}
		}

		throw new System.Exception("Can not instantiate " + levelData.SuccessCount + " items");
	}

	private TileItemData[,] GenerateTileItemDataFromCurrentTiles() {
		TileItemData[,] res = new TileItemData[numColumns, numRows];

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				Tile tile = tiles[x, y];
				TileItem tileItem = tile.GetTileItem();
				TileItemType type = (tile.Type == TileType.Avaliable && tileItem != null) ? tileItem.Type : TileItemType.Unavaliable_1;
				res[x, y] = new TileItemData(tile.X, tile.Y, type);
			}
		}

		return res;
	}

	bool CheckTileItemsPosition(TileItemData[,] data) {
		return true;
	}

	TileItemData[,] MixTileItemData(TileItemData[,] data) {
		return data;
	}

	void RepositionTileItems(TileItemData[,] data) {
	}
}








