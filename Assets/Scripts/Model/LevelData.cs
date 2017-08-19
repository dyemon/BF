using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class LevelData {
	public string Id;
	public string Name;
	public string Description;
	public int LevelPrice;
	public int SuccessCount = 3;
	public int SlimeRatio;
	public int[] TileItemDropPercent;

	public FightLocationType FightLocationType;
	public string FightLocationTypeAsString;

	public TileItemData[] TileData = new TileItemData[0];
	public BarrierData[] BarrierData = new BarrierData[0];
	public TargetData[] TargetData = new TargetData[0];

	public Restrictions RestrictionData;

	public TileItemData[] AutoDropOnCollectData = new TileItemData[0];
	public AutoDropData[] AutoDropData = new AutoDropData[0];

	public EnemyData EnemyData = null;

	public LevelAwardData SuccessAwardData;
	public LevelAwardData FailureAwardData;

	public EducationData EducationData;

	public void Init() {
		FightLocationType = EnumUtill.Parse<FightLocationType>(FightLocationTypeAsString);

		if(TileData != null) {
			foreach(TileItemData item in TileData) {
				if(item.TypeAsString == "Random") {
					int rand = UnityEngine.Random.Range(0, (int)TileItemTypeGroup.Bomb);
					item.Type = (TileItemType)(rand - rand % TileItem.TILE_ITEM_GROUP_WEIGHT);
					item.TypeAsString = item.Type.ToString();
				} else {
					item.Type = (TileItemType)Enum.Parse(typeof(TileItemType), item.TypeAsString);
				}
				if(item.HasChild()) {
					item.ChildTileItemData.Type = (TileItemType)Enum.Parse(typeof(TileItemType), item.ChildTileItemData.TypeAsString);
				}
				if(item.HasGenerated()) {
					item.GeneratedTileItemData.Type = (TileItemType)Enum.Parse(typeof(TileItemType), item.GeneratedTileItemData.TypeAsString);
				}
				if(item.HasParent()) {
					item.ParentTileItemData.Type = (TileItemType)Enum.Parse(typeof(TileItemType), item.ParentTileItemData.TypeAsString);
				}
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

		if(TileItemDropPercent == null) {
			TileItemDropPercent = new int[5];
			for(int i = 0;i < TileItemDropPercent.Length;i++) {
				TileItemDropPercent[i] = 20;
			}
		}
		/*
		int sum = 0;
		for(int i = 0;i < TileItemDropPercent.Length;i++) {
			sum += TileItemDropPercent[i];
		}

		if(sum != 100) {
			throw new LevelConfigException("Sum of tile item drop percent must be 100. Now it is " + sum );
		}
		*/
		if(AutoDropOnCollectData != null) {
			foreach(TileItemData item in AutoDropOnCollectData) {
				item.Type = (TileItemType)Enum.Parse(typeof(TileItemType), item.TypeAsString);
			}
		}

		if(AutoDropData != null) {
			foreach(AutoDropData item in AutoDropData) {
				item.TileItem.Type = (TileItemType)Enum.Parse(typeof(TileItemType), item.TileItem.TypeAsString);
			}
		}

		if(HasEnemy()) {
			EnemyData.Init();
		}

		SuccessAwardData.Init();
		FailureAwardData.Init();

		if(EducationData != null) {
			EducationData.Init();
		}
	}

	public bool HasEnemy() {
		return EnemyData != null && EnemyData.Health > 0;
	}

	public bool HasEducation() {
		return EducationData != null && EducationData.HasEducation();
	}
}
