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
	public GameObject[] tileItemsUnavaliable;

	private AnimationGroup animationGroup;

	private Tile[,] tiles;
	private IList<Tile> selectedTiles = new List<Tile>();
	private Tile[,] tmpTiles;

	private int[] tileItemSpawnDelay;
	private bool[] tileColumnAvalibleForOffset;

	private IList<TileItemData> tileData = new List<TileItemData>();

	void Start() {
		animationGroup = GetComponent<AnimationGroup>();
		tiles = new Tile[numColumns, numRows];
		tileItemSpawnDelay = new int[numColumns];
		tileColumnAvalibleForOffset = new bool[numColumns];

		tileData.Add(new TileItemData(0, numRows - 1, TileItemType.Unavaliable_2));
		tileData.Add(new TileItemData(1, numRows - 2, TileItemType.Unavaliable_1));
		tileData.Add(new TileItemData(2, 1, TileItemType.Unavaliable_1));
		tileData.Add(new TileItemData(5, 4, TileItemType.Unavaliable_1));
		tileData.Add(new TileItemData(2, numRows - 1, TileItemType.Unavaliable_2));
	//	tileData.Add(new TileItemData(3, numRows - 1, TileItemType.Unavaliable_1));
		tileData.Add(new TileItemData(4, numRows - 1, TileItemType.Unavaliable_2));
	//	tileData.Add(new TileItemData(5, numRows - 1, TileItemType.Unavaliable_1));
		tileData.Add(new TileItemData(6, numRows - 1, TileItemType.Unavaliable_2));
		/*
		tileData.Add(new TileItemData(0, 4, TileItemType.Red));
		tileData.Add(new TileItemData(1, 3, TileItemType.Green));
		tileData.Add(new TileItemData(2, 5, TileItemType.Blue));
		tileData.Add(new TileItemData(5, 3, TileItemType.Yellow));
		tileData.Add(new TileItemData(5, 6, TileItemType.Purple));
		*/
		InitTiles();
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
					ti.SetMoved(false);
				}
			}
		}
	}

	public static Vector3 IndexToPosition(int x, int y) {
		return new Vector3(x - numColumns / 2f + 0.5f, y + 0.5f, 0);
	}
	public static Vector2 PositionToIndex(Vector3 pos) {
		return new Vector2(pos.x + numColumns / 2f - 0.5f, pos.y - 0.5f);
	}

	private TileItem InstantiateColorTileItem() {
		int index = Random.Range(0, tileItemsColor.Length);
		return InstantiateTileItem(tileItemsColor, index, (TileItemType)(index*20), 0, -10);
	}

	private TileItem InstantiateTileItem(TileItemType type, int x, int y) {
		TileItemTypeGroup group = TileItem.TypeToGroupType(type);
		switch(group) {
			case TileItemTypeGroup.Red:
			case TileItemTypeGroup.Green:
			case TileItemTypeGroup.Blue:
			case TileItemTypeGroup.Yellow:
			case TileItemTypeGroup.Purple:	
				return InstantiateTileItem(tileItemsColor,  (int)group/20, type, x, y);
			case TileItemTypeGroup.Unavaliable:
				return InstantiateTileItem(tileItemsUnavaliable, (int)(type) - (int)group, type, x, y);
		}

		throw new System.NotImplementedException("Can not instantient tile item with type " + type.ToString());
	}

	private TileItem InstantiateTileItem(GameObject[] items, int index, TileItemType type, int x, int y) {
		GameObject go = (GameObject)Instantiate(items[index], IndexToPosition(x, y), Quaternion.identity);
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

	public void DropTileItems() {
		foreach(Tile tile in selectedTiles) {
			Destroy(tile.GetTileItemGO());
			tile.SetTileItem(null);
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

		foreach(TileItemData item in tileData) {
			tiles[item.x, item.y].SetTileItem(InstantiateTileItem(item.type, item.x, item.y));
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
				continue;
			}

			if(tEmpty.Count > 0) {
				MoveTileItem(tile, tEmpty[0], false);
				tEmpty.RemoveAt(0);
				tEmpty.Add(tile);
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

			if(left != null && left.IsAvaliable && !left.IsEmpty && tileColumnAvalibleForOffset[left.X]) {
				suitables.Add(left);
			}
			if(right != null && right.IsAvaliable && !right.IsEmpty && tileColumnAvalibleForOffset[right.X]) {
				suitables.Add(right);
			}

			if(suitables.Count == 0) {
				continue;
			} else if(suitables.Count == 1) {
				chosen = suitables[0];
			} else {
				if(suitables[0].GetTileItem().IsMoved() && !suitables[1].GetTileItem().IsMoved()) {
					chosen = suitables[1];
				} else if(suitables[1].GetTileItem().IsMoved() && !suitables[0].GetTileItem().IsMoved()) {
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
			from.GetTileItem().SetMoved(true);
		} else {
			ao.LayerSortingOrder(TILEITEM_SORTING_ORDER - 3);
		}

		ao.Build();

		if(from != to) {
			from.SetTileItem(null);
		}
	}
}
