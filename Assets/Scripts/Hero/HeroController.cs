using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

public class HeroController : MonoBehaviour {
	public delegate void OnStrike(HeroSkillData skill);

	public static float STRIKE_DELAY = 1;

	UserData userData;
	public int Health { get; set; }
	public int Damage { get; set; }
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

	public HeroSkillController heroSkillController;

	private int startHealth;
	private int startDamage;

	public int StartHealth {
		get { return startHealth; }
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
		startHealth = Health = userData.Health;
		startDamage = Damage = userData.Damage;
		ResetPowerPoints();
	}

	void Start () {
		skeletonAnimation = GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.state;
		skeleton = skeletonAnimation.Skeleton;

	
	}
		
	public void IncreasePowerPoints(int points) {
		CurrentPowerPoints += points;
	}

	public void IncreaseHealth(int ratio, bool updateStart) {
		Health = CalcNewHealth(ratio);
		if(updateStart) {
			startHealth = Health;
		}
	}
	public void IncreaseDamage(int ratio, bool updateStart) {
		Damage = CalcNewDamage(ratio);
		if(updateStart) {
			startDamage = Damage;
		}
	}

	public void IncreaseHealthByValue(int value, bool updateStart) {
		Health += value;
		if(updateStart) {
			startHealth = Health;
		}
	}

	public int CalcNewHealth(int ratio) {
		return Health + (int)Mathf.Round(startHealth * ratio / 100f);
	}
	public int CalcNewDamage(int ratio) {
		return Damage + (int)Mathf.Round(startDamage * ratio / 100f);
	}

	public void ResetPowerPoints() {
		CurrentPowerPoints = CurrentPowerPoints - PowerPointSuccess;
		if(CurrentPowerPoints > PowerPointSuccess - 1) {
			CurrentPowerPoints = PowerPointSuccess - 1;
		}
		if(CurrentPowerPoints < 0) {
			CurrentPowerPoints = 0;
		}
	}

	public void Strike(HeroSkillData skill, OnStrike onStrike) {
		string animName = kick1AnimationName;
		float delay = STRIKE_DELAY;
		if(skill != null && skill.Type == HeroSkillType.Damage3) {
			animName = kick2AnimationName;
			delay = STRIKE_DELAY * 2;
		}

		spineAnimationState.SetAnimation(0, animName, false); 
		spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
		enemyController.Kick(skill);
	
		StartCoroutine(StrikeInternal(onStrike, skill, delay));
	}

	public void Stun(HeroSkillData skill) {
		string animName = kick1AnimationName;
		spineAnimationState.SetAnimation(0, animName, false); 
		spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
		enemyController.Stunned(skill);
	}

	public void Kick() {
		if(!heroSkillController.IsInvulnerability()) {
			spineAnimationState.SetAnimation(0, idleAnimationName, false); 
			spineAnimationState.AddAnimation(0, damageAnimationName, false, 0.7f);
			spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
			SoundController.Play(SoundController.Instance.Strike1, 1, 0.7f);
		} else {
			StartCoroutine(TemporarilyDeactivate(0.6f, 0.2f));
		}
	}

	private IEnumerator StrikeInternal(OnStrike onStrike, HeroSkillData skill, float delay) {
		yield return new WaitForSeconds(delay);
		if(skill == null) {
			ResetPowerPoints();
		}
		onStrike(skill);
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

	public void UseSkill() {
		spineAnimationState.SetAnimation(0, getCardAnimationName, false); 
		spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
		SoundController.Play(SoundController.Instance.HeroSkill, 1, 1.4f);
	}

	private IEnumerator TemporarilyDeactivate(float duration1, float duration2) {
		yield return new WaitForSeconds(duration1);
		gameObject.GetComponent<Renderer>().enabled = false;
		yield return new WaitForSeconds(duration2);
		gameObject.GetComponent<Renderer>().enabled = true;
	}

	public bool NeedSave() {
		return Health < startHealth * 0.3;
	}

	public void Hide() {
		gameObject.GetComponent<Renderer>().enabled = false;
	}
}
