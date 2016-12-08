using UnityEngine;
using System.Collections;

public class Barrier {
	public BarrierData Data { get; set;}
	public GameObject BarrierGO { get; set;}

	private TileItemController itemController;

	public Barrier(BarrierData data, GameObject go) {
		this.Data = data;
		this.BarrierGO = go;
		this.itemController = go.GetComponent<TileItemController>();	
	}

	public int Damage(int damage) {
		return (itemController == null)? 1 : itemController.Damage(damage);
	}

	public static Barrier Instantiate(BarrierData data, GameObject go) {
		return new Barrier(data, go);
	} 
		
}
