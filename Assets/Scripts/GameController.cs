using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AnimationGroup))]
public class GameController : MonoBehaviour {

	public static int numColumns = 7;
	public static int numRows = 7;

	public GameObject[] tileItemsNornal;
	public GameObject[] tileItemsNotAvaliable;

	private AnimationGroup animationGroup;

	private Tile[,] tiles;
	private Tile[,] tmpTiles;

	void Start() {
		animationGroup = GetComponent<AnimationGroup>();
		tiles = new Tile[numColumns, numRows];
		tmpTiles = new Tile[numColumns, numRows];

		UpdateTiles();
	}
	// Update is called once per frame
	void Update() {
		ProcessInput();
	}

	public static Vector3 IndexToPosition(int x, int y) {
		return new Vector3(x - numColumns / 2 + 0.5f, y + 0.5f, 0);
	}
	public static Vector2 PositionToIndex(Vector3 pos) {
		return new Vector2(pos.x + numColumns / 2 - 0.5f, pos.y - 0.5f);
	}

	private TileItem InstantiateTileItem(TileItemTypeGroup group, int x, int y) {
		int index;
		GameObject go;

		if(group == TileItemTypeGroup.Normal) {
			index = Random.Range(0, tileItemsNornal.Length);
			go = (GameObject)Instantiate(tileItemsNornal[index], IndexToPosition(x, y), Quaternion.identity);
		} else {
			throw new UnityException("Can not instantiate tile item for group" + group);
		}

		TileItem ti = new TileItem(group, index, go);

		return ti;
	}

	void SpawnTileItems() {

	}

	private void ProcessInput() {
		InputController.Touch[] touches = InputController.getTouches();

		if(touches.Length > 0) {
			InputController.Touch touch = touches[0];
			if(touch.phase == TouchPhase.Began) {
		//		Ray ray = InputController.TouchToRay(touches[0]);
				Debug.Log(touches[0]);

			}
		}
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

		animationGroup.Run();
	}

	private void UpdateTiles() {
		CreateTmpTiles();
		UpdateTilesColumns();

		RunAnimation();
	}

	private void CreateTmpTiles() {
		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				if(tiles[x, y] == null) {
					tiles[x, y] = new Tile(x, y);
				}

				tmpTiles[x, y] = (Tile)tiles[x, y].Clone();
			}
		}
	}

	private void UpdateTilesColumns() {
		for(int x = 0;x < numColumns; x++) {
			UpdateTilesColumn(x);
		}
	}

	private void UpdateTilesColumn(int x) {
		IList<Tile> tEmpty = new List<Tile>();
		Tile spawnTile = tmpTiles[x, numRows - 1];
		int delay = spawnTile.IsEmpty()? 0 : 1;

		for(int y = 0; y < numRows;y++) {
			Tile tile = tmpTiles[x, y];

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
			TileItem tileItem = InstantiateTileItem(TileItemTypeGroup.Normal, x, numRows - 1);
			tileItem.GetGameObject().GetComponent<AnimatedObject>().AddIdle(App.MoveTileItemTimeUnit * delay).Build();
			spawnTile.SetTileItem(tileItem);
			if(tile != spawnTile) {
				MoveTmpTileItem(spawnTile, tile);
			}
			delay++;
		}
	}

	private void MoveTmpTileItem(Tile from, Tile to) {
		AnimatedObject ao = from.GetTileItemGO().GetComponent<AnimatedObject>();

		ao.AddMove(IndexToPosition(from.X, from.Y), IndexToPosition(to.X, to.Y), App.moveTileItemSpeed).Build();
		to.SetTileItem(from.GetTileItem());
		from.SetTileItem(null);
	}
}
