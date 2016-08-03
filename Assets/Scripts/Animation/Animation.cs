using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Animation {

	private AMove move;
	private AIdle idle;

	public int? LayerSortingOrder { get; set;}

	public void AddMove(Vector3 startPos, Vector3 movePos, float speed) {
		Predicates.Null(move, "Move animation already exist for this animation object");
		move = new AMove(startPos, movePos, speed);
	}

	public void AddMoveByTime(Vector3 startPos, Vector3 movePos, float time) {
		Predicates.Null(move, "Move animation already exist for this animation object");
		move = new AMove(startPos, movePos);
		move.SetTime(time);
	}

	public void AddIdle(float time) {
		Predicates.Null(move, "Idle animation already exist for this animation object");
		idle = new AIdle(time);
	}

	public AMove GetMove() {
		return move;
	}

	public AIdle GetIdle() {
		return idle;
	}

	public void Run() {
		if(move != null) {
			move.Run();
		}
		if(idle != null) {
			idle.Run();
		}
	}
		

}

