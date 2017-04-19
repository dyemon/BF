using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileItemData {
	public int X;
	public int Y;
	public TileItemType Type;
	public string TypeAsString;
	public int Level = 1;
	public int Health;
	public ChildTileItemData ChildTileItemData;
	public ChildTileItemData GeneratedTileItemData;
	public ChildTileItemData ParentTileItemData;

	public TileItemData(int x, int y, TileItemType type) {
		this.X = x;
		this.Y = y;
		this.Type = type;
	}

	public TileItemData(EnemySkillData eSkill) {
		Health = eSkill.Health;
		Type = (TileItemType)eSkill.Type;
		TypeAsString = eSkill.TypeAsString;
	}

	public bool HasChild() {
		return ChildTileItemData != null && ChildTileItemData.TypeAsString != null;
	}

	public bool HasGenerated() {
		return GeneratedTileItemData != null && GeneratedTileItemData.TypeAsString != null;
	}

	public bool HasParent() {
		return ParentTileItemData != null && ParentTileItemData.TypeAsString != null;
	}
}
