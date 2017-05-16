using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

public class HeroController : MonoBehaviour {
	public delegate void OnStrike(HeroSkillData skill);

	public static float STRIKE_DELAY = 1;

	UserData userData;
	public int Health { get; set; }
	public int CurrentPowerPoints { get; set; }

	#region Inspector
	// [SpineAnimation] attribute allows an Inspector dropdown of Spine animation names coming form SkeletonAnimation.
	[SpineAnimation]
	public string idleAnimationName;

	[SpineAnimation]
	public string baseAnimationName;

	[SpineAnimation]
	public string damageAnimationName;

	[SpineAnimation]
	public string getCardAnimationName;

	[SpineAnimation]
	public string kick1AnimationName;

	[SpineAnimation]
	public string kick2AnimationName;
	#endregion

	SkeletonAnimation skeletonAnimation;

	// Spine.AnimationState and Spine.Skeleton are not Unity-serialized objects. You will not see them as fields in the inspector.
	public Spine.AnimationState spineAnimationState;
	public Spine.Skeleton skeleton;

	private EnemyController enemyController;

	public int Damage {
		get { 
			return userData.Damage; 
		}
	}

	public int PowerPointSuccess {
		get { 
			return GameData.PowerPointSuccess; 
		}
	}

	public bool IsStrike {
		get { return CurrentPowerPoints >= PowerPointSuccess; }
	}

	public void SetEnemyController(EnemyController enemyController) {
		this.enemyController = enemyController;
	}

	void Awake() {
		userData = GameResources.Instance.GetUserData();
		Health = userData.Health;
		ResetPowerPoints();
	}

	void Start () {
		skeletonAnimation = GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.state;
		skeleton = skeletonAnimation.Skeleton;

	
	}
		
	public void IncreesPowerPoints(int points) {
		CurrentPowerPoints += points;
	}



	public void ResetPowerPoints() {
		CurrentPowerPoints = 0;
	}

	public void Strike(HeroSkillData skill, OnStrike onStrike) {
		string animName = kick1AnimationName;//(Random.Range(0, 2) >= 1)? kick1AnimationName : kick2AnimationName;
		spineAnimationState.SetAnimation(0, animName, false); 
		spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
		enemyController.GetKick();
	//	DisplayMessageController.DisplayMessage("Удар героя", Color.green);
		StartCoroutine(StrikeInternal(onStrike, skill));
	}

	public void GetKick() {
		spineAnimationState.SetAnimation(0, idleAnimationName, false); 
		spineAnimationState.AddAnimation(0, damageAnimationName, false, 0.7f);
		spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
	}

	private IEnumerator StrikeInternal(OnStrike onStrike, HeroSkillData skill) {
		yield return new WaitForSeconds(STRIKE_DELAY);
		ResetPowerPoints();
		onStrike(skill);
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

	public void UseSkill() {
		spineAnimationState.SetAnimation(0, getCardAnimationName, false); 
		spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
	}
}
