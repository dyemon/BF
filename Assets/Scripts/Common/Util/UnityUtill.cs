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
		} else {
			Renderer rend = go.GetComponent<Renderer>();
			if(rend != null) {
				return new Vector2(rend.bounds.max.x - rend.bounds.min.x, rend.bounds.max.y - rend.bounds.min.y);
			}
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

		if(minX == float.PositiveInfinity) {
			foreach(Renderer r in go.GetComponentsInChildren<Renderer>()) {
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
		}

		return new Vector2(maxX - minX, maxY - minY);
	}

	public static void SetSortingOrder(GameObject go, int order) {
		go.GetComponent<Renderer>().sortingOrder = order;
		foreach(Renderer r in go.GetComponentsInChildren<Renderer>()) {
			r.sortingOrder = order;
		}
	}

	public static Transform FindByName(Transform root, string name) {
		Transform res = root.Find(name);
		if(res != null) {
			return res;
		}

		foreach(Transform tr in root.transform) {
			res = FindByName(tr, name);
			if(res != null) {
				return res;
			}
		}

		return null;
	}

	public static IList<Transform> FindByTag(Transform root, string tag) {
		IList<Transform> res = new List<Transform>();

		foreach(Transform tr in root.transform) {
			if(tr.tag == tag) {
				res.Add(tr);
			}
		}

		return res;
	}

	public static void DestroyByTag(Transform root, string tag) {
		foreach(Transform tr in FindByTag(root, tag)) {
			GameObject.Destroy(tr.gameObject);
		}
	}
		
}