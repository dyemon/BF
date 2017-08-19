using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Animation;
using UnityEngine.UI;

public class EducationController : MonoBehaviour {

	private EducationData educationData;
	private EducationStep currentEducationStep;

	public GameObject Hand;
	public GameObject Arrow;
	public GameObject Description;
	public GameObject CloseButton;

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
	}
	


	public void StartStep() {
		if(educationData == null) {
			return;
		}

		currentEducationStep = educationData.GetStep();
		if(currentEducationStep == null) {
			educationData = null;
			gameObject.SetActive(false);
			return;
		}

		gameObject.SetActive(true);

		if(currentEducationStep.IsShowHand()) {
			Hand.SetActive(true);
			StartHandAnimation();
		
		} else {
			Hand.SetActive(false);
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
		educationData.Next();
		currentEducationStep = null;
		AnimatedObject ao = Hand.GetComponent<AnimatedObject>();
		ao.Stop();
		ao.ClearAnimations();
		Show(false);
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
		}


		return res ;
	}
}
