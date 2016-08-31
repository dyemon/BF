using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AnimationGroup))]
public class GameController : MonoBehaviour {
	public static readonly int TILEITEM_SORTING_ORDER = 10;

	public static int numColumns = 7;
	public static int numRows = 8;
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

	private int[] tileItemSpawnDelay;
	private bool[] tileColumnAvalibleForOffset;

	private LevelData levelData;
	private UserData userData;

	void Start() {
		animationGroup = GetComponent<AnimationGroup>();
		tiles = new Tile[numColumns, numRows];
		tileItemSpawnDelay = new int[numColumns];
		tileColumnAvalibleForOffset = new bool[numColumns];

		levelData = new LevelData();
		levelData.Init(numRows);
		userData = new UserData();
		userData.Init();

		InitTiles();
		InitBarriers();
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
		return new Vector3(x - numColumns / 2f + 0.5f, y + 0.5f, 0);
	}
	public static Vector2 PositionToIndex(Vector3 pos) {
		return new Vector2(pos.x + numColumns / 2f - 0.5f, pos.y - 0.5f);
	}

	private TileItem InstantiateColorTileItem() {
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
						if(selectedTiles.Contains(tile)) {
							selectedTiles.Remove(tile);
						} else {
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

		animationGroup.Run(OnTileItemUpdateComplete);
	}

	private void OnTileItemUpdateComplete() {
		CheckTileItemSameColorCount();
		Debug.Log("dddddddddddddd");
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
			if(tiles[item.x, item.y].GetTileItem() != null) {
				throw new System.Exception("Invalid configuration for tile " + tiles[item.x, item.y] + ". Tile is configured twice");
			}
			tiles[item.x, item.y].SetTileItem(InstantiateTileItem(item.type, item.x, item.y, true));
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
			TileItem tileItem = InstantiateColorTileItem();
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

	private bool CheckTileItemSameColorCount() {
		IDictionary<TileItemTypeGroup, int> items = new Dictionary<TileItemTypeGroup, int>();
		TileItemTypeGroup? maxCountType = null;
		int maxCount = 0;

		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				Tile tile = tiles[x, y];
				if(!tile.IsColor) {
					continue;
				}

				TileItemTypeGroup tg = tile.GetTileItem().TypeGroup;
				if(!items.ContainsKey(tg)) {
					items.Add(tg, 0);
				}
				items[tg]++;
				if(items[tg] >= levelData.SuccessCount) {
					return true;
				}
				if(items[tg] > maxCount) {
					maxCount = items[tg];
					maxCountType = tg;
				}
			}
		}

		if(maxCountType == null) {
			maxCountType = TileItemTypeGroup.Red;
		}

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

				ClearTile(tile);
				tile.SetTileItem(InstantiateTileItem((TileItemType)maxCountType, tile.X, tile.Y, true));
				success--;
				if(success == 0) {
					return false;
				}
			}
		}

		throw new System.Exception("Can not instantiate " + levelData.SuccessCount + " items");
	}

	private void ClearTile(Tile tile) {
		if(tile.GetTileItemGO() != null) {
			Destroy(tile.GetTileItemGO());
		}
		tile.SetTileItem(null);
	}
}
