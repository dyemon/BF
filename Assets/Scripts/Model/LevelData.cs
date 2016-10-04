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
	public TargetData[] TargetData;

	public Restrictions RestrictionData;

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

		if(TargetData != null) {
			foreach(TargetData item in TargetData) {
				item.Type = (TargetType)Enum.Parse(typeof(TargetType), item.TypeAsString);
			}
		}

		if(RestrictionData == null) {
			RestrictionData = new Restrictions();
		}
	}

}
