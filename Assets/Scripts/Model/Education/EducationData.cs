using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EducationData {
	public EducationStep[] Steps;
	private int index;

	public void Init() {
		if(Steps != null) {
			foreach(EducationStep item in Steps) {
				item.Init();
			}
		}
	}

	public bool HasEducation() {
		return Steps != null && Steps.Length > 0;
	}

	public EducationStep GetStep() {
		return (Steps == null || index >= Steps.Length) ? null : Steps[index];
	}

	public void Next() {
		index++;
	}

	public void Reset() {
		index = 0;
	}
}
