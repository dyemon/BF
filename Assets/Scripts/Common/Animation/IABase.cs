using UnityEngine;
using System.Collections;

public interface IABase {
	bool Animate(GameObject gameObject);
	void Run();
	bool IsCompleteAnimation();
}
