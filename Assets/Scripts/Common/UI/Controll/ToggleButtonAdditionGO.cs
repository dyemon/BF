using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleButtonAdditionGO : MonoBehaviour {
	public GameObject OnGO;
	public GameObject OffGO;

	void Start() {
		OnToggle(GetComponent<Toggle>().isOn);
	}

	public void OnToggle(bool isOn) {
		OnGO.SetActive(isOn);
		OffGO.SetActive(!isOn);
	}
}
