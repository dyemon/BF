using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelData {

	public IList<TileItemData> TileData = new List<TileItemData>();
	public IList<BarrierData> BarrierData = new List<BarrierData>();

	public int SuccessCount = 20;

	public int BrilliantDropRatio = 10;

	public void Init(int numRows) {
		//	tileData.Add(new TileItemData(0, numRows - 1, TileItemType.Unavaliable_2));
		TileData.Add(new TileItemData(1, numRows - 2, TileItemType.Unavaliable_1));
		TileData.Add(new TileItemData(2, 1, TileItemType.Unavaliable_1));
		//	tileData.Add(new TileItemData(5, 2, TileItemType.Unavaliable_1));
		TileData.Add(new TileItemData(5, 3, TileItemType.Unavaliable_1));
		TileData.Add(new TileItemData(2, numRows - 1, TileItemType.Unavaliable_2));
		//	tileData.Add(new TileItemData(3, numRows - 1, TileItemType.Unavaliable_1));
		TileData.Add(new TileItemData(4, numRows - 1, TileItemType.Unavaliable_2));
		//	tileData.Add(new TileItemData(5, numRows - 1, TileItemType.Unavaliable_1));
		TileData.Add(new TileItemData(6, numRows - 1, TileItemType.Unavaliable_2));

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
}
