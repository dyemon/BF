using UnityEngine;
using System.Collections;

public interface IResizeListener {
	void OnResize(float resizeRation, Vector2 size);
}