using UnityEngine;
using System.Collections;

public class Barrier {
	public BarrierData Data { get; set;}
	public GameObject BarrierGO { get; set;}

	public Barrier(BarrierData data, GameObject go) {
		this.Data = data;
		this.BarrierGO = go;
	}

	virtual public int Damage(int damage) {
		return 1;
	}

	public static Barrier Instantiate(BarrierData data, GameObject go) {
		switch(data.Type) {
			case BarrierType.Wood:
				return new BreakableBarrier(data, go, 1);
		}

		return new Barrier(data, go);
	} 
		
}
