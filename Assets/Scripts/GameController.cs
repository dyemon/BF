using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AnimationGroup))]
public class GameController : MonoBehaviour {

	public int numColumns = 7;
	public int numRows = 7;
	public GameObject tileItem;

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

	void SpawnTileItems() {
		for(float i = -numColumns/2f + 0.5f; i < numColumns/2f; i++) {
			Instantiate(tileItem, new Vector3(i, 0.5f, 0), Quaternion.identity);
		}
	}

	private void ProcessInput() {
		InputController.Touch[] touches = InputController.getTouches();

		if(touches.Length > 0) {
			InputController.Touch touch = touches[0];
			if(touch.phase == TouchPhase.Began) {
				Ray ray = InputController.TouchToRay(touches[0]);
				Debug.Log(touches[0]);

			}
		}
	}

	private void UpdateTiles() {
		CreateTmpTiles();
		UpdateTilesColumns();
	}

	private void CreateTmpTiles() {
		for(int x = 0; x < numColumns; x++) {
			for(int y = 0; y < numRows; y++) {
				if(tiles[x, y] == null) {
					tiles[x, y] = new Tile();
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
		for(int y = 0; y < numRows;y++) {
			Tile tile = tmpTiles[x, y];
			if(!tile.IsEmpty()) {
				continue;
			}
		}
	}
}
