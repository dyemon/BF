using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Animation;
using UnityEngine.UI;
using UnityEngine.Events;

public class EducationController : MonoBehaviour {
	public delegate void OnEducationComplete();
	public event OnEducationComplete onEducationComplete;

	private EducationData educationData;
	private EducationStep currentEducationStep;

	public GameObject Hand;
	public GameObject Arrow;
	public GameObject Description;
	public GameObject CloseButton;

	UnityAction OnTargetClick;

	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
		LevelData levelData = GameResources.Instance.GetLevel(App.CurrentLevel);
		if(!levelData.HasEducation()) {
			return;
		}

		UserData uData = GameResources.Instance.GetUserData();
		if(uData.Level > App.CurrentLevel) {
			return;
		}

		educationData = levelData.EducationData; 
		educationData.Reset();

		AnimatedObject ao = Arrow.GetComponent<AnimatedObject>();
		ao.AddResize(null, new Vector3(1.1f, 1.1f, 1), 0.3f)
			.AddResize(null, new Vector3(1f, 1f, 1), 0.3f)
			.ClearOnStop(false)
			.Loop(true)
			.Build();
	}
	


	public void StartStep() {
		if(educationData == null) {
			return;
		}

		currentEducationStep = educationData.GetStep();
		if(currentEducationStep == null) {
			educationData = null;
			gameObject.SetActive(false);
			if(onEducationComplete != null) {
				onEducationComplete();
			}

			return;
		}

		gameObject.SetActive(true);

		if(currentEducationStep.IsShowHand()) {
			Hand.SetActive(true);
			StartHandAnimation();
		} else {
			Hand.SetActive(false);
		}
			
		if(currentEducationStep.IsShowArrow()) {
			Arrow.SetActive(true);
			ShowArrow();
		} else {
			Arrow.SetActive(false);
		}

		Description.transform.Find("Text").GetComponent<Text>().text = currentEducationStep.Description;
		Description.transform.position = GetDescriptionPosition();

		CloseButton.SetActive(currentEducationStep.ShowCloseButton);

		Show(true);
	}

	public bool HasCurrentEducationStep() {
		return currentEducationStep != null;
	}

	public bool IsCurrentEducationType(EducationType type) {
		return HasCurrentEducationStep() && currentEducationStep.Type == type;
	}

	public Vector2[] GetPositions() {
		return currentEducationStep == null ? null : currentEducationStep.GetPositions();
	}
	public Vector2[] GetPositions1() {
		return currentEducationStep == null ? null : currentEducationStep.Positions1;
	}

	public Vector2[] GetHandPositions() {
		return currentEducationStep == null ? null : currentEducationStep.GetHandPositions();
	}

	public void NextPositionIndex() {
		if(currentEducationStep != null) {
			if(currentEducationStep.NextPositionIndex()) {
				StartHandAnimation();
			}
		}
	}

	public void ResetPositionIndex() {
		if(currentEducationStep != null) {
			if(currentEducationStep.ResetPositionIndex()) {
				StartHandAnimation();
			}
		}
	}

	public bool IsStrict() {
		return currentEducationStep == null ? false : currentEducationStep.Strict;
	}

	public void Next() {
		bool goNext = currentEducationStep.StartNextStepOnNext;
		educationData.Next();
		currentEducationStep = null;
		AnimatedObject ao = Hand.GetComponent<AnimatedObject>();
		ao.Stop();
		ao.ClearAnimations();
		Arrow.GetComponent<AnimatedObject>().Stop();
		Show(false);
		if(goNext) {
			Invoke("StartStep", 1);
		}
	}

	void StartHandAnimation() {
		Vector2 pos = currentEducationStep.GetStartHandPosition();
		Vector3 start = GameController.IndexToPosition(pos.x, pos.y);
		start = Camera.main.WorldToScreenPoint(start);
		Hand.transform.position = start;

		AnimatedObject ao = Hand.GetComponent<AnimatedObject>();
		ao.ClearAnimations();
		ao.AddIdle(0.5f).Build();
		int i = 0;
		foreach(Vector2 aPos in currentEducationStep.GetHandPositions()) {
			if(i++ == 0) {
				continue;
			}

			Vector3 pos3 = GameController.IndexToPosition(aPos.x, aPos.y);
			pos3 = Camera.main.WorldToScreenPoint(pos3);

			ao.AddMoveByTime(null, pos3, 0.8f);
		}

		ao.AddMoveByTime(null, start, 0.3f).Build();
		ao.Loop(true).Run();
	}

	void Show(bool show) {
		GetComponent<AnimatedObject>().AddFadeUI(null, show ? 1 : 0, 0.3f)
			.Build().Run();
	}

	Vector3 GetDescriptionPosition() {
		Vector3 res = Vector3.zero;
		Vector2 offset = currentEducationStep.DescriptionOffset;

		if(currentEducationStep.IsShowHand()) {
			Vector2 pos = currentEducationStep.GetStartHandPosition();
			Vector3 start = GameController.IndexToPosition(pos.x, pos.y);
			res = Camera.main.WorldToScreenPoint(start + new Vector3(offset.x, offset.y, 0));
		} else if(currentEducationStep.IsShowArrow()) {
			GameObject target = GameObject.Find(currentEducationStep.ArrowGameObjectName);
			Preconditions.NotNull(target, "Can not find gameobject " + currentEducationStep.ArrowGameObjectName);
			Vector3 pos = target.transform.position;
			if(currentEducationStep.ArrowGameObjectIsUI) {
				pos = Camera.main.ScreenToWorldPoint(pos);
			}
			res = Camera.main.WorldToScreenPoint(pos + new Vector3(offset.x, offset.y, 0));
		}


		return res ;
	}

	void ShowArrow() {
		GameObject target = GameObject.Find(currentEducationStep.ArrowGameObjectName);
		Preconditions.NotNull(target, "Can not find gameobject " + currentEducationStep.ArrowGameObjectName);

		Vector3 pos = target.transform.position;
		if(currentEducationStep.ArrowGameObjectIsUI) {
			pos = Camera.main.ScreenToWorldPoint(pos);
		}
		pos += new Vector3(currentEducationStep.ArrowOffset.x, currentEducationStep.ArrowOffset.y, 0);
		Arrow.transform.position = Camera.main.WorldToScreenPoint(pos);
		Arrow.transform.rotation = Quaternion.Euler(0, 0, currentEducationStep.ArrowAngle);

		Arrow.GetComponent<AnimatedObject>().Run();

		Button b = target.GetComponent<Button>();
		if(b != null) {
			b.interactable = true;
		}

		if(b != null && currentEducationStep.NextOnClickTargetButton) {
			b.interactable = true;
			OnTargetClick = () => {
				Next();
				b.onClick.RemoveListener(OnTargetClick);
			};
			b.onClick.AddListener(OnTargetClick);
		}
	}

	public IList<HeroSkillData> GetHeroSkills() {
		HeroSkillData[] aSkills = GameResources.Instance.GetGameData().HeroSkillData; 
		IList<HeroSkillData> res = new List<HeroSkillData>();
		res.Add(aSkills[8]);
		res.Add(aSkills[0]);
		res.Add(aSkills[1]);

		return res;
	}
}
