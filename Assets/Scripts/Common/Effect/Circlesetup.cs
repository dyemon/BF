using UnityEngine;
using System.Collections;

public class Circlesetup : MonoBehaviour {

	public float borderWidth = 0.2f;

	Material renderer_material;

	// Use this for initialization
	void Start () {
		renderer_material = GetComponent<SpriteRenderer>().material;
	}

	// Update is called once per frame
	void Update () {

		Vector4 sz = new Vector4(transform.lossyScale.x, transform.lossyScale.y);

		float rad = sz.x * 0.5f;
		float inner_rad_u = Mathf.Max((rad - borderWidth) / sz.x,0);
		renderer_material.SetFloat("_InnerRadiusU", inner_rad_u);
		renderer_material.SetFloat("_HalfMinusInnerRadiusU", 0.5f-inner_rad_u);
	}
}
