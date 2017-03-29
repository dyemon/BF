using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityUtill {
	
	public static Vector2 GetBounds(GameObject go) {
		float minX = float.PositiveInfinity;
		float maxX = float.NegativeInfinity; 
		float minY = float.PositiveInfinity;
		float maxY = float.NegativeInfinity;

		SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
		if(sr != null) {
			return new Vector2(sr.bounds.max.x - sr.bounds.min.x, sr.bounds.max.y - sr.bounds.min.y);
		}

		foreach(SpriteRenderer r in go.GetComponentsInChildren<SpriteRenderer>()) {
			if(r == null) {
				continue;
			}

			Bounds bounds = r.bounds;
			if(bounds.max.x > maxX) {
				maxX = bounds.max.x;
			}
			if(bounds.max.y > maxY) {
				maxY = bounds.max.y;
			}
			if(bounds.min.x < minX) {
				minX = bounds.min.x;
			}
			if(bounds.min.y < minY) {
				minY = bounds.min.y;
			}
		}

		return new Vector2(maxX - minX, maxY - minY);
	}		
}