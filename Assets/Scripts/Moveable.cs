using UnityEngine;
using System.Collections;

public class Moveable : MonoBehaviour {

	public float speed = 3;
	private Coroutine coroutine;

	private IEnumerator MoveCoroutine(Vector3 movePosition) {
		Vector3 startPosition = transform.position;
		float t = 0;
		float distance = Vector3.Distance (startPosition, movePosition);

		if (distance == 0) {
			MoveFinish ();
			yield break;
		}

		float cf = speed / distance;

		while(t < 1) {
			t += Time.deltaTime * cf;
			transform.position = Vector3.Lerp(startPosition, movePosition, t);
			yield return null;
		}

		MoveFinish ();
	}

	private void MoveFinish() {
		coroutine = null;
	}

	public void Move(Vector3 movePosition) {
		coroutine = StartCoroutine(MoveCoroutine(movePosition));
	}

	public void Stop() {
		if (coroutine != null) {
			StopCoroutine(coroutine);
		}
	}

	public bool IsMoving() {
		return (coroutine != null);
	}
}
