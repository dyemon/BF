using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
	public delegate void OnStrik();

	public static float STRIKE_DELAY = 1;

	public EnemyData enemyData;

	public int Health { get; set; }
	public int CurrentTurns { get; set; }

	#region Inspector
	// [SpineAnimation] attribute allows an Inspector dropdown of Spine animation names coming form SkeletonAnimation.
	[SpineAnimation]
	public string idleAnimationName;

	[SpineAnimation]
	public string idl2eAnimationName;

	[SpineAnimation]
	public string damageAnimationName;

	[SpineAnimation]
	public string kickAnimationName;


	#endregion

	SkeletonAnimation skeletonAnimation;

	// Spine.AnimationState and Spine.Skeleton are not Unity-serialized objects. You will not see them as fields in the inspector.
	public Spine.AnimationState spineAnimationState;
	public Spine.Skeleton skeleton;

	public int Damage {
		get { 
			return (enemyData == null)? 0 : enemyData.Damage; 
		}
	}
		
	public int TurnsSuccess {
		get { 
			return GameData.EnemyTurns; 
		}
	}

	public bool IsStrik {
		get { return CurrentTurns >= TurnsSuccess; }
	}

	void Start () {
		enemyData = GameResources.Instance.GetLevel(App.GetCurrentLevel()).EnemyData;
		if(enemyData == null) {
			return;
		}

		Health = enemyData.Health;
		ResetTurns();

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.state;
		skeleton = skeletonAnimation.Skeleton;
	}

	public void IncreesTurns(int turns) {
		CurrentTurns += turns;
	}
		
	public void ResetTurns() {
		CurrentTurns = 0;
	}

	public void Strike(OnStrik onStrik) {
		DisplayMessageController.DisplayMessage("Удар врага", Color.red);
		StartCoroutine(StrikInternal(onStrik));

	}

	public void GetKick() {
		spineAnimationState.SetAnimation(0, idleAnimationName, false); 
		spineAnimationState.AddAnimation(0, damageAnimationName, false, 0.7f);
		spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
	}

	private IEnumerator StrikInternal(OnStrik onStrik) {
		yield return new WaitForSeconds(STRIKE_DELAY);
		ResetTurns();
		onStrik();
	}

	public void DecreesHealt(int damage) {
		Health -= damage;
		if(Health < 0) {
			Health = 0;
		}
	}

	public bool IsDeath(int evaluteValue) {
		return Health - evaluteValue <= 0;
	}


}
