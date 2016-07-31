using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Animation {

	AMove move;

	public void AddMove(Vector3 startPos, Vector3 movePos, float speed) {
		Predicates.Null(move, "Move animation already exist for this animation object");
		move = new AMove(startPos, movePos, speed);
	}

	public AMove GetMove() {
		return move;
	}

	public void Start() {
		if(move != null) {
			move.Start();
		}
	}

	public void setMoveTime(float time) {
		Predicates.NotNull(move, "Move animation is not added for this animation object");
		move.setTime(time);
	}
}

