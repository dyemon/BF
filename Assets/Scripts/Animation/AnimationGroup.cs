using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationGroup : MonoBehaviour {
	public delegate void CompleteAnimation();

	private IList<AnimatedObject> objects = new List<AnimatedObject>();

	public void Add(AnimatedObject o) {
		objects.Add(o);
	}

	public void Clear() {
		objects.Clear();
	}

	public void Run(CompleteAnimation complete) {
		StartCoroutine(RunAnimation(complete));
	}

	private IEnumerator RunAnimation(CompleteAnimation complete) {
		yield return null;
		foreach(AnimatedObject o in objects) {
			o.Run();
		}

		foreach(AnimatedObject o in objects) {
			if(!o.IsDone) {
				yield return null;
			}
		}

		if(complete != null) {
			complete();
		}
	}
}
