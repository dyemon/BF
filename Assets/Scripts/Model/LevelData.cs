using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelData {

	public IList<TileItemData> TileData = new List<TileItemData>();
	public IList<BarrierData> BarrierData = new List<BarrierData>();

	public int SuccessCount = 10;

	public int BrilliantDropRatio = 20;

	public static int NumColumns = 7;
	public static int NumRows = 8;

	public void Init() {
//		Level1();
		Level2();
//		Level3();
	}

	private void Level1() {

	}

	private void Level2() {
		//	tileData.Add(new TileItemData(0, numRows - 1, TileItemType.Unavaliable_2));
		TileData.Add(new TileItemData(1, NumRows - 2, TileItemType.Unavaliable_1));
		TileData.Add(new TileItemData(2, 1, TileItemType.Unavaliable_1));
		//	tileData.Add(new TileItemData(5, 2, TileItemType.Unavaliable_1));
		TileData.Add(new TileItemData(5, 3, TileItemType.Unavaliable_1));
		TileData.Add(new TileItemData(2, NumRows - 1, TileItemType.Unavaliable_2));
		//	tileData.Add(new TileItemData(3, numRows - 1, TileItemType.Unavaliable_1));
		TileData.Add(new TileItemData(4, NumRows - 1, TileItemType.Unavaliable_2));
		//	tileData.Add(new TileItemData(5, numRows - 1, TileItemType.Unavaliable_1));
		TileData.Add(new TileItemData(6, NumRows - 1, TileItemType.Unavaliable_2));

		TileData.Add(new TileItemData(0, 4, TileItemType.Red));
		TileData.Add(new TileItemData(1, 3, TileItemType.Green));
		TileData.Add(new TileItemData(2, 5, TileItemType.Blue));
		TileData.Add(new TileItemData(5, 6, TileItemType.Purple));


		BarrierData.Add(new BarrierData(0, 0, 1, 0, BarrierType.Wood));
		BarrierData.Add(new BarrierData(0, 0, 0, 1, BarrierType.Iron));

		BarrierData.Add(new BarrierData(1, 2, 1, 3, BarrierType.Wood));
		BarrierData.Add(new BarrierData(2, 2, 3, 2, BarrierType.Wood));
		BarrierData.Add(new BarrierData(2, 2, 2, 3, BarrierType.Iron));
		BarrierData.Add(new BarrierData(3, 2, 3, 3, BarrierType.Wood));
		BarrierData.Add(new BarrierData(2, 3, 3, 3, BarrierType.Iron));
		BarrierData.Add(new BarrierData(4, 2, 4, 3, BarrierType.Iron));
		BarrierData.Add(new BarrierData(5, 2, 5, 3, BarrierType.Iron));

		BarrierData.Add(new BarrierData(3, 4, 3, 5, BarrierType.Wood));
	}

	private void Level3() {
		TileData.Add(new TileItemData(0, NumRows - 1, TileItemType.Unavaliable_2));
		TileData.Add(new TileItemData(NumColumns - 1, NumRows - 1, TileItemType.Unavaliable_2));

		BarrierData.Add(new BarrierData(0, NumRows - 1, 1, NumRows - 1, BarrierType.Wood));
		BarrierData.Add(new BarrierData(0, NumRows - 2, 1, NumRows - 2, BarrierType.Wood));
		BarrierData.Add(new BarrierData(0, NumRows - 3, 1, NumRows - 3, BarrierType.Wood));

		BarrierData.Add(new BarrierData(NumColumns - 1, NumRows - 1, NumColumns - 2, NumRows - 1, BarrierType.Wood));
		BarrierData.Add(new BarrierData(NumColumns - 1, NumRows - 2, NumColumns - 2, NumRows - 2, BarrierType.Wood));
		BarrierData.Add(new BarrierData(NumColumns - 1, NumRows - 3, NumColumns - 2, NumRows - 3, BarrierType.Wood));

		BarrierData.Add(new BarrierData(1, NumRows - 2, 1, NumRows - 3, BarrierType.Wood));
		BarrierData.Add(new BarrierData(2, NumRows - 2, 2, NumRows - 3, BarrierType.Wood));
		BarrierData.Add(new BarrierData(3, NumRows - 2, 3, NumRows - 3, BarrierType.Wood));
		BarrierData.Add(new BarrierData(4, NumRows - 2, 4, NumRows - 3, BarrierType.Wood));
		BarrierData.Add(new BarrierData(5, NumRows - 2, 5, NumRows - 3, BarrierType.Wood));
		/*
		TileData.Add(new TileItemData(1, 6, TileItemType.Red));
		TileData.Add(new TileItemData(2, 6, TileItemType.Green));
		TileData.Add(new TileItemData(3, 6, TileItemType.Blue));
		TileData.Add(new TileItemData(4, 6, TileItemType.Purple));
		TileData.Add(new TileItemData(5, 6, TileItemType.Yellow));
		TileData.Add(new TileItemData(1, 7, TileItemType.Red));
		TileData.Add(new TileItemData(2, 7, TileItemType.Green));
		TileData.Add(new TileItemData(3, 7, TileItemType.Blue));
		TileData.Add(new TileItemData(4, 7, TileItemType.PurpleEnvelops));
		TileData.Add(new TileItemData(5, 7, TileItemType.Yellow));
		*/

	}

}
