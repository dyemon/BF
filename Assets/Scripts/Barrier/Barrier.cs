using UnityEngine;
using System.Collections;

public class Barrier {
	BarrierData data;
	GameObject gameObject;

	public Barrier(BarrierData data, GameObject go) {
		this.data = data;
		this.gameObject = go;
	}
}
