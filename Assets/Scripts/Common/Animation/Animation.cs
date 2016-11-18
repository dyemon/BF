using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Animation {

	private AMove move;
	private AIdle idle;
	private AFade fade;

	public int? LayerSortingOrder { get; set;}

	public void AddMove(Vector3 startPos, Vector3 movePos, float speed, bool ui) {
		Preconditions.IsNull(move, "Move animation already exist for this animation object");
		move = new AMove(startPos, movePos, speed, ui);
	}

	public void AddMoveByTime(Vector3 startPos, Vector3 movePos, float time, bool ui) {
		Preconditions.IsNull(move, "Move animation already exist for this animation object");
		move = new AMove(startPos, movePos, ui);
		move.SetTime(time);
	}

	public void AddIdle(float time) {
		Preconditions.IsNull(idle, "Idle animation already exist for this animation object");
		idle = new AIdle(time);
	}

	public void AddFade(float startAlpha, float endAlpha, float time) {
		Preconditions.IsNull(fade, "Fade animation already exist for this animation object");
		fade = new AFade(startAlpha, endAlpha, time);
	}

	public void AddFadeUIText(float startAlpha, float endAlpha, float time) {
		Preconditions.IsNull(fade, "Fade animation already exist for this animation object");
		fade = new AFadeUIText(startAlpha, endAlpha, time);
	}

	public AMove GetMove() {
		return move;
	}

	public AIdle GetIdle() {
		return idle;
	}

	public AFade GetFade() {
		return fade;
	}

	public void Run() {
		if(move != null) {
			move.Run();
		}
		if(idle != null) {
			idle.Run();
		}
		if(fade != null) {
			fade.Run();
		}
	}
		

}

