using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MovingGroup : MonoBehaviour {
	
	public delegate void MoveFinish (IList<Moveable> moveObjects);

	private IList<Vector3> movePositions = new List<Vector3>();
	private IList<Moveable> moveObjects = new List<Moveable>();

	public void Add(Moveable moveObject, Vector3 movePosition) {
		movePositions.Add(movePosition);
		moveObjects.Add(moveObject);
	}

	public void StartMove(MoveFinish onMoveFinish) {
		StartCoroutine(StartMoveCoroutine(onMoveFinish));
	}

	private IEnumerator StartMoveCoroutine(MoveFinish onMoveFinish) {
		for (int i = 0; i < moveObjects.Count; i++) {
			moveObjects [i].Move (movePositions [i]);
		}

		bool isMoving = true;
		while (isMoving) {
			isMoving = false;
			foreach (Moveable move in moveObjects) {
				if (move.IsMoving ()) {
					isMoving = true;
					break;
				}
			}

			if (isMoving) {
				yield return null;
			}
		}

		onMoveFinish (moveObjects);
	}

	public void Clear() {
		movePositions.Clear ();
		moveObjects.Clear ();
	}
}
