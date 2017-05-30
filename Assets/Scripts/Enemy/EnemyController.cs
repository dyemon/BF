using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

public class EnemyController : MonoBehaviour {
	public delegate void OnStrike();

	public static float STRIKE_DELAY = 1;

	public EnemyData enemyData;
	public Vector3 StartOffset = new Vector3();

	private HeroController heroController;

	public int Health { get; set; }
	public int CurrentTurns { 
		get { return Health > 0? currentTurns : 0; }
		set { currentTurns = value; }
	}

	private int currentSkillRatio;
	private int currentTurns;

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

	private string currentIdleAnimation;
		
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

	public bool IsStrike {
		get { return CurrentTurns >= TurnsSuccess; }
	}

	void Awake() {
		currentIdleAnimation = idleAnimationName;
		enemyData = GameResources.Instance.GetLevel(App.GetCurrentLevel()).EnemyData;
		if(enemyData == null) {
			return;
		}

		currentSkillRatio = enemyData.SkillRatio;
		Health = enemyData.Health;
		ResetTurns();
	}

	void Start () {
		if(enemyData == null) {
			return;
		}

		transform.position += StartOffset;

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.state;
		skeleton = skeletonAnimation.Skeleton;
	}

	public void IncreaseTurns(int turns) {
		CurrentTurns += turns;
	}
		
	public void ResetTurns() {
		CurrentTurns = 0;
	}

	public void Strike(OnStrike onStrike) {
		string animName = kickAnimationName;
		spineAnimationState.SetAnimation(0, animName, false); 
		spineAnimationState.AddAnimation(0, currentIdleAnimation, true, 0);
		heroController.Kick();
	//	DisplayMessageController.DisplayMessage("Удар врага", Color.red);
		StartCoroutine(StrikeInternal(onStrike));

	}

	public void Kick(HeroSkillData skill) {
		spineAnimationState.SetAnimation(0, currentIdleAnimation, false); 
		if(skill != null && skill.Type == HeroSkillType.Damage3) {
			TrackEntry tEntry = spineAnimationState.AddAnimation(0, damageAnimationName, false, 0.7f);
			tEntry.timeScale = 2f;
			tEntry = spineAnimationState.AddAnimation(0, damageAnimationName, false, 0);
			tEntry.timeScale = 2.2f;
			tEntry = spineAnimationState.AddAnimation(0, damageAnimationName, false, 0);
		} else {
			spineAnimationState.AddAnimation(0, damageAnimationName, false, 0.7f);
		}
		spineAnimationState.AddAnimation(0, currentIdleAnimation, true, 0);
	}

	public void Stunned(HeroSkillData skill) {
		if(skill != null) {
			spineAnimationState.SetAnimation(0, currentIdleAnimation, false);
			spineAnimationState.AddAnimation(0, damageAnimationName, false, 0.7f);
		}
		currentIdleAnimation = idl2eAnimationName;
		spineAnimationState.AddAnimation(0, currentIdleAnimation, true, 0);
	}

	public void Idle() {
		currentIdleAnimation = idleAnimationName;
		spineAnimationState.SetAnimation(0, currentIdleAnimation, true);
	}

	private IEnumerator StrikeInternal(OnStrike onStrike) {
		yield return new WaitForSeconds(STRIKE_DELAY);
		ResetTurns();
		onStrike();
	}

	public void DecreaseHealt(int damage) {
		Health -= damage;
		if(Health < 0) {
			Health = 0;
		}
			
	}

	public bool IsDeath(int evaluteValue) {
		return Health - evaluteValue <= 0;
	}

	public void SetHeroController(HeroController hc) {
		heroController = hc;
	}

	public void Death() {
		gameObject.SetActive(false);
	}

	public EnemySkillData GetSkill() {
		if(enemyData.SkillRatio == 0 || enemyData.SkillData == null || enemyData.SkillData.Length == 0) {
			return null;
		}

		currentSkillRatio--;
		if(currentSkillRatio == 0) {
			currentSkillRatio = enemyData.SkillRatio;
			return enemyData.SkillData[Random.Range(0, enemyData.SkillData.Length)];
		}
		return null;
	}


}
