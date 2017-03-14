using UnityEngine;
using System.Collections;
using Common.Net.Http;

public class FightProgressPanel : MonoBehaviour, IResizeListener {

	public void OnResize(float resizeRation, Vector2 size) {
		transform.localScale = new Vector3(1, 1, 1);
		Vector2 bounds = GetBounds.GetBounds(gameObject);
		float rRatio = size.x / bounds.x;
		transform.localScale = new Vector3(rRatio, rRatio, 1);
	}

}