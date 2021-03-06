﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Common.Animation {
public class AnimatedObject : MonoBehaviour {
	public delegate void OnStopAnimation(System.Object[] param);
	public delegate void OnStopAnimationSimple();
	public delegate void OnStepAnimation(AnimationType t, GameObject go);

	private bool isPlay = false;

	private IList<Animation> animations = new List<Animation>();
	private Animation currentAnimation;
	private Animation currentPlayAnimation;

	private int? sourceLayerSortingOrder = null;

	private bool destroyOnStop = false;
	private bool clearOnStop = true;
	private bool loop = false;

	private OnStopAnimation onStop;
	private OnStopAnimationSimple onStopSimple;
	private OnStepAnimation onStep;
	private System.Object[] onStopParam;

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
			
		bool isContinue = false;
		foreach(AnimationType type in Enum.GetValues(typeof(AnimationType))) {
			isContinue = PlayAnimation(type) || isContinue;
		}

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
			if(loop) {
				animations.Add((Animation)currentPlayAnimation.Clone());
			}
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

	public AnimatedObject ClearAnimations() {
		animations.Clear();
		currentAnimation = null;
		currentPlayAnimation = null;
		if(sourceLayerSortingOrder != null) {
			GetComponent<SpriteRenderer>().sortingOrder = sourceLayerSortingOrder.Value;
			sourceLayerSortingOrder = null;
		}
		onStop = null;
		onStopSimple = null;
		onStep = null;
		onStopParam = null;
		destroyOnStop = false;
		clearOnStop = true;

		return this;
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

		if(onStop != null) {
			onStop(onStopParam);
		}

		if(onStopSimple != null) {
			onStopSimple();
		}

		if(clearOnStop) {
			ClearAnimations();
		}
	}

	public bool IsAnimationExist() {
		return animations.Count > 0;
	}


	public AnimatedObject LayerSortingOrder(int order) {
		getCurrentAnimation().LayerSortingOrder = order;
		return this;
	}
		
	public AnimatedObject AddMove(Vector3? start, Vector3 end, float speed, ABase.BezierPoints points = null) {
		ABase a = new AMove(start, end, speed);
		a.SetBezierPoints(points);
		getCurrentAnimation().AddAnimation(AnimationType.Move, a);
		return this;
	}
		
	public AnimatedObject AddResize(Vector3? start, Vector3 end, float time, ABase.BezierPoints points = null) {
		ABase a = new AResize(start, end, time);
		a.SetBezierPoints(points);
		getCurrentAnimation().AddAnimation(AnimationType.Resize, a);
		return this;
	}

	public AnimatedObject AddRotate(Vector3? start, Vector3 end, float time, ABase.BezierPoints points = null) {
		ABase a = new ARotate(start, end, time);
		a.SetBezierPoints(points);
		getCurrentAnimation().AddAnimation(AnimationType.Rotate, a);
		return this;
	}

	public AnimatedObject AddMoveByTime(Vector3? start, Vector3 end, float time, ABase.BezierPoints points = null) {
		AMove a = new AMove(start, end);
		a.SetTime(time);
		a.SetBezierPoints(points);
		getCurrentAnimation().AddAnimation(AnimationType.Move, a);
		return this;
	}

	public AnimatedObject AddFade(float? startAlpha, float endAlpha, float time, ABase.BezierPoints points = null) {
		ABase a = new AFade(startAlpha, endAlpha, time);
		a.SetBezierPoints(points);
		getCurrentAnimation().AddAnimation(AnimationType.Fade, a);
		return this;
	}
		
	public AnimatedObject AddIdle(float time) {
		getCurrentAnimation().AddAnimation(AnimationType.Idle, new AIdle(time));
		return this;
	}

	public AnimatedObject AddFadeUIText(float? startAlpha, float endAlpha, float time, ABase.BezierPoints points = null) {
		ABase a = new AFadeUIText(startAlpha, endAlpha, time);
		a.SetBezierPoints(points);
		getCurrentAnimation().AddAnimation(AnimationType.Fade, a);
		return this;
	}

	public AnimatedObject AddFadeUI(float? startAlpha, float endAlpha, float time, ABase.BezierPoints points = null) {
		ABase a = new AFadeUI(startAlpha, endAlpha, time);
		a.SetBezierPoints(points);
		getCurrentAnimation().AddAnimation(AnimationType.Fade, a);
		return this;
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

	public AnimatedObject Loop(bool val) {
		this.loop = val;
		return this;
	}

	public AnimatedObject ClearOnStop(bool clearOnStop) {
		this.clearOnStop = clearOnStop;
		return this;
	}

	public AnimatedObject OnStop(OnStopAnimation onStop, System.Object[] param) {
		this.onStop = onStop;
		this.onStopParam = param;
		return this;
	}

	public AnimatedObject OnStop(OnStopAnimationSimple onStop) {
		this.onStopSimple = onStop;
		return this;
	}

	public AnimatedObject OnStep(OnStepAnimation onStep) {
		this.onStep = onStep;
		return this;
	}

	private bool PlayAnimation(AnimationType type) {
		Animation animation = getCurrentPlayAnimation();
		if(animation == null) {
			return false;
		}

		ABase a = animation.GetAnimation(type);
		if(a == null) {
			return false;
		}
			

		a.Animate(gameObject);

		if(onStep != null) {
			onStep(type, gameObject);
		}

		return true;
	}
}
}