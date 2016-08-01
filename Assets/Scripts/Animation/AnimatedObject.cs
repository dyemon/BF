using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatedObject : MonoBehaviour {

	private bool isPlay = false;

	private IList<Animation> animations = new List<Animation>();
	private Animation currentAnimation;
	private Animation currentPlayAnimation;

	public bool IsDone {
		get {return !isPlay;}
	} 


	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!isPlay) {
			return;
		}


		bool isContinue = PlayMove();
		isContinue = PlayIdle() || isContinue;

		if(!isContinue) {
			currentPlayAnimation = null;
			if(getCurrentPlayAnimation() == null) {
				Stop();
			}
		}
	}

	private Animation getCurrentAnimation() {
		if(currentAnimation == null) {
			currentAnimation = new Animation();
		}

		return currentAnimation;
	}

	private Animation getCurrentPlayAnimation() {
		if(currentPlayAnimation == null) {
			if(animations.Count == 0) {
				return null;
			}
			currentPlayAnimation = animations[0];
			currentPlayAnimation.Start();
			animations.RemoveAt(0);
		}

		return currentPlayAnimation;
	}

	public void ClearAnimations() {
		animations.Clear();
		currentAnimation = null;
		currentPlayAnimation = null;
	}

	public void Run() {
		isPlay = true;
	}

	public void Stop() {
		isPlay = false;
		if(currentPlayAnimation != null) {
			animations.Insert(0, currentPlayAnimation);
		}
	}

	public AnimatedObject AddMove(Vector3 end, float speed) {
		getCurrentAnimation().AddMove(transform.position, end, speed);
		return this;
	}

	public AnimatedObject AddMove(Vector3 start, Vector3 end, float speed) {
		getCurrentAnimation().AddMove(start, end, speed);
		return this;
	}

	public AnimatedObject AddMoveByTime(Vector3 end, float time) {
		getCurrentAnimation().AddMoveByTime(transform.position, end, time);
		return this;
	}

	public AnimatedObject AddMoveByTime(Vector3 start, Vector3 end, float time) {
		getCurrentAnimation().AddMoveByTime(start, end, time);
		return this;
	}

	public AnimatedObject AddIdle(float time) {
		getCurrentAnimation().AddIdle(time);
		return this;
	}

	public AnimatedObject Build() {
		animations.Add(currentAnimation);
		currentAnimation = null;
		return this;
	}

	private bool PlayMove() {
		Animation animation = getCurrentPlayAnimation();
		if(animation == null) {
			return false;
		}

		AMove move = animation.GetMove();
		if(move == null) {
			return false;
		}


		return move.Move(gameObject);
	}

	private bool PlayIdle() {
		Animation animation = getCurrentPlayAnimation();
		if(animation == null) {
			return false;
		}

		AIdle idle = animation.GetIdle();
		if(idle == null) {
			return false;
		}


		return idle.Idle();
	}
}
