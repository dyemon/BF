using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationGroup : MonoBehaviour {

	private IList<AnimatedObject> objects = new List<AnimatedObject>();

	public void Add(AnimatedObject o) {
		objects.Add(o);
	}

	public void Clear() {
		objects.Clear();
	}

	public void Run() {
		foreach(AnimatedObject o in objects) {
			o.Run();
		}
	}
}
