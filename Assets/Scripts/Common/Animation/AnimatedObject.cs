using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AnimatedObject : MonoBehaviour {

	private bool isPlay = false;

	private IList<Animation> animations = new List<Animation>();
	private Animation currentAnimation;
	private Animation currentPlayAnimation;

	private int? sourceLayerSortingOrder = null;

	private bool destroyOnStop = false;

	public bool IsDone {
		get {return !isPlay;}
	}


	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(!isPlay) {
			return;
		}
			
		bool isContinue = PlayMove();
		isContinue = PlayIdle() || isContinue;
		isContinue = PlayFade() || isContinue;

		if(!isContinue) {
			if(sourceLayerSortingOrder != null) {
				GetComponent<SpriteRenderer>().sortingOrder = sourceLayerSortingOrder.Value;
			}
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
			currentPlayAnimation.Run();
			if(currentPlayAnimation.LayerSortingOrder != null) {
				SpriteRenderer render = GetComponent<SpriteRenderer>();
				if(render != null) {
					sourceLayerSortingOrder = render.sortingOrder;
					render.sortingOrder = currentPlayAnimation.LayerSortingOrder.Value;
				}
			}
			animations.RemoveAt(0);
		}

		return currentPlayAnimation;
	}

	public void ClearAnimations() {
		animations.Clear();
		currentAnimation = null;
		currentPlayAnimation = null;
		if(sourceLayerSortingOrder != null) {
			GetComponent<SpriteRenderer>().sortingOrder = sourceLayerSortingOrder.Value;
		}
	}

	public void Run() {
		if(animations.Count == 0) {
			return;
		}

		isPlay = true;
	}

	public void Stop() {
		isPlay = false;
		if(currentPlayAnimation != null) {
			animations.Insert(0, currentPlayAnimation);
		}

		if(destroyOnStop) {
			Destroy(gameObject);
		}
	}

	public bool IsAnimationExist() {
		return animations.Count > 0;
	}

	public AnimatedObject AddMove(Vector3 end, float speed, bool ui = false) {
		getCurrentAnimation().AddMove(transform.position, end, speed, ui);
		return this;
	}

	public AnimatedObject LayerSortingOrder(int order) {
		getCurrentAnimation().LayerSortingOrder = order;
		return this;
	}

	public AnimatedObject AddMove(Vector3 start, Vector3 end, float speed, bool ui = false) {
		getCurrentAnimation().AddMove(start, end, speed, ui);
		return this;
	}

	public AnimatedObject AddMoveByTime(Vector3 end, float time, bool ui = false ) {
		getCurrentAnimation().AddMoveByTime(transform.position, end, time, ui);
		return this;
	}

	public AnimatedObject AddMoveByTime(Vector3 start, Vector3 end, float time, bool ui = false) {
		getCurrentAnimation().AddMoveByTime(start, end, time, ui);
		return this;
	}

	public AnimatedObject AddIdle(float time) {
		getCurrentAnimation().AddIdle(time);
		return this;
	}

	public AnimatedObject AddFade(float startAlpha, float endAlpha, float time) {
		getCurrentAnimation().AddFade(startAlpha, endAlpha, time);
		return this;
	}

	public AnimatedObject AddFade(float endAlpha, float time) {
		SpriteRenderer renderer = Preconditions.NotNull(gameObject.GetComponent<SpriteRenderer>(), "There is no 'SpriteRenderer' attached to the {0}", gameObject.name);
		return AddFade(renderer.material.color.a, endAlpha, time);
	}

	public AnimatedObject AddFadeUIText(float startAlpha, float endAlpha, float time) {
		getCurrentAnimation().AddFadeUIText(startAlpha, endAlpha, time);
		return this;
	}

	public AnimatedObject AddFadeUIText(float endAlpha, float time) {
		Text text = Preconditions.NotNull(gameObject.GetComponent<Text>(), "There is no 'Text' attached to the {0}", gameObject.name);
		return AddFadeUIText(text.color.a, endAlpha, time);
	}

	public AnimatedObject Build() {
		animations.Add(currentAnimation);
		currentAnimation = null;
		return this;
	}

	public AnimatedObject DestroyOnStop(bool destroyOnStop) {
		this.destroyOnStop = destroyOnStop;
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

	private bool PlayFade() {
		Animation animation = getCurrentPlayAnimation();
		if(animation == null) {
			return false;
		}

		AFade fade = animation.GetFade();
		if(fade == null) {
			return false;
		}
			
		return fade.Fade(gameObject);
	}
}
