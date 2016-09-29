using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class LevelData {
	public static int NumColumns = 7;
	public static int NumRows = 8;

	public string Id;
	public string Name;
	public string Description;
	public int SuccessCount = 3;
	public int BrilliantDropRatio;


	public TileItemData[] TileData;
	public BarrierData[] BarrierData;

	public void Init() {
		if(TileData != null) {
			foreach(TileItemData item in TileData) {
				item.Type = (TileItemType)Enum.Parse(typeof(TileItemType), item.TypeAsString);
			}
		}

		if(BarrierData != null) {
			foreach(BarrierData item in BarrierData) {
				item.Verify();
				item.Type = (BarrierType)Enum.Parse(typeof(BarrierType), item.TypeAsString);
			}
		}
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
 

	}
*/
}
