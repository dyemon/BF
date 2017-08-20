using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class EducationStep {
	public EducationType Type;
	public string TypeAsString;
	public bool Strict = false;
	public string Description;
	public bool ShowCloseButton = false;
	public Vector2 DescriptionOffset;
	public Vector2 ArrowOffset;
	public string ArrowGameObjectName;
	public float ArrowAngle;
	public bool ArrowGameObjectIsUI = true;
	public bool StartNextStepOnNext = false;
	public bool NextOnClickTargetButton = false;

	public Vector2[] Positions1;
	public Vector2[] Positions2;

	private int index = 1;

	public void Init() {
		Type = EnumUtill.Parse<EducationType>(TypeAsString);
	}

	public Vector2[] GetPositions() {
		return (index == 1)? Positions1 : (Vector2[])Positions1.Concat(Positions2).ToArray();
	}

	public Vector2[] GetHandPositions() {
		return (index == 1)? Positions1 : Positions2;
	}

	public bool NextPositionIndex() {
		if(Positions2 != null && index == 1) {
			index = 2;
			return true;
		}
		return false;
	}

	public bool ResetPositionIndex() {
		if(Positions2 != null && index == 2) {
			index = 1;
			return true;
		}
		return false;
	}

	public bool IsShowHand() {
		return Positions1 != null && Positions1.Length > 0;
	}
	public bool IsShowArrow() {
		return !string.IsNullOrEmpty(ArrowGameObjectName);
	}

	public Vector2 GetStartHandPosition() {
		return IsShowHand()? GetHandPositions()[0] : Vector2.zero;
	}
}
