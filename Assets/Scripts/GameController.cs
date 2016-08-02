using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AnimationGroup))]
public class GameController : MonoBehaviour {

	public static int numColumns = 7;
	public static int numRows = 8;
	public LayerMask tilesItemLayer = -1;

	public GameObject[] tileItemsColor;
	public GameObject[] tileItemsNotAvaliable;

	private AnimationGroup animationGroup;

	private Tile[,] tiles;
	private IList<Tile> selectedTiles = new List<Tile>();
	//private Tile[,] tmpTiles;

	private IList<TileItemData> data = new List<TileItemData>();

	void Start() {
		animationGroup = GetComponent<AnimationGroup>();
		tiles = new Tile[numColumns, numRows];
	
		data.Add(new TileItemData(0, numRows - 1, TileItemType.NotAvaliable_1));

		InitTiles();
		UpdateTiles();
	}
	// Update is called once per frame
	void Update() {
		ProcessInput();
	
	}

	public static Vector3 IndexToPosition(int x, int y) {
		return new Vector3(x - numColumns / 2f + 0.5f, y + 0.5f, 0);
	}
	public static Vector2 PositionToIndex(Vector3 pos) {
		return new Vector2(pos.x + numColumns / 2f - 0.5f, pos.y - 0.5f);
	}

	private TileItem InstantiateColorTileItem() {
		int index = Random.Range(0, tileItemsColor.Length);
		GameObject go = (GameObject)Instantiate(tileItemsColor[index], new Vector3(0f, -10f, 0f), Quaternion.identity);
		TileItem ti = new TileItem((TileItemTypeGroup)(index * 10), 0, go);

		return ti;
	}
	private TileItem InstantiateNotAvaliableTileItem(TileItemType type, int x, int y) {
		GameObject go = (GameObject)Instantiate(tileItemsNotAvaliable[type - TileItemTypeGroup.NotAvaliable], IndexToPosition(x, y), Quaternion.identity);
		TileItem ti = new TileItem(type, go);

		return ti;
	}
	/*
	private InstantiateTileItem(TileItemType type, int x, int y) {
		TileItemTypeGroup group = TileItem.TypeToGroupType(type);
		switch(group) {
			case(TileItemTypeGroup.)
		}
	}*/
	private TileItem InstantiateTileItem(GameObject[] items, int index, TileItemType type, int x, int y) {
		GameObject go = (GameObject)Instantiate(items[index], IndexToPosition(x, y), Quaternion.identity);
		TileItem ti = new TileItem(type, go);

		return ti;
	}

	void SpawnTileItems() {

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
				if(tiles[x, y].GetTileItemGO() == go) {
					return tiles[x, y];
				}
			}
		}

		return null;
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
				if(!tiles[x, y].IsAvaliable() || tiles[x, y].IsEmpty()){
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
		UpdateTilesColumns();

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

		foreach(TileItemData item in data) {
			tiles[item.x, item.y].SetTileItem(InstantiateTileItem(item.type, item.x, item.y));
		}
	}

	private void UpdateTilesColumns() {
		for(int x = 0;x < numColumns; x++) {
			UpdateTilesColumn(x);
		}
	}

	private void UpdateTilesColumn(int x) {
		IList<Tile> tEmpty = new List<Tile>();
		Tile spawnTile = tiles[x, numRows - 1];
		int delay = spawnTile.IsEmpty()? 0 : 1;

		for(int y = 0; y < numRows;y++) {
			Tile tile = tiles[x, y];

			if(!tile.IsAvaliable()) {
				tEmpty.Clear();
				continue;
			}
			if(tile.IsEmpty()) {
				tEmpty.Add(tile);
				continue;
			}

			if(tEmpty.Count > 0) {
				MoveTmpTileItem(tile, tEmpty[0]);
				tEmpty.RemoveAt(0);
				tEmpty.Add(tile);
			}
		}
			
		foreach(Tile tile in tEmpty) {
			TileItem tileItem = InstantiateColorTileItem();
			tileItem.GetGameObject().GetComponent<AnimatedObject>().AddIdle((App.MoveTileItemTimeUnit + App.moveTileItemDelay) * delay).Build();
			spawnTile.SetTileItem(tileItem);
		//	if(tile != spawnTile) {
				MoveTmpTileItem(spawnTile, tile);
		//	}
			delay++;
		}
	}

	private void MoveTmpTileItem(Tile from, Tile to) {
		AnimatedObject ao = from.GetTileItemGO().GetComponent<AnimatedObject>();

		ao.AddMove(IndexToPosition(from.X, from.Y), IndexToPosition(to.X, to.Y), App.moveTileItemSpeed).Build();
		to.SetTileItem(from.GetTileItem());
		if(from != to) {
			from.SetTileItem(null);
		}
	}
}
