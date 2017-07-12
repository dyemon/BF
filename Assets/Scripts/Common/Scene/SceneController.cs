using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
//using UnityEditor;


public class SceneController : MonoBehaviour {
	public static SceneController Instance;

	public delegate void OnUnloadScene(string name, System.Object retVal);
	public event OnUnloadScene onUnloadScene;

//	public Image fadeImage;
	public float MinDuration = 0.05f;
	public float MaxAlpha = 0.5f;
	public float ChangeTimeSeconds = 0.05f;

	private float startAlpha = 0;
	private float endAlpha = 1;
	private Vector3 loadRotateAngles = new Vector3(0, 0, -10);

	float changeRate = 0;
	float timeSoFar = 0;
	bool fading = false;
	public CanvasGroup canvasGroup;

	private IList<string> loaddedScenes = new List<string>();
	private IDictionary<string, System.Object> parameters = new Dictionary<string, System.Object>();
	private IDictionary<string, System.Object> returnValues = new Dictionary<string, System.Object>();

	void Awake() {
		Instance = this;
	}

	void Start() {
		if(canvasGroup == null) {
			canvasGroup = GetComponent<CanvasGroup>();
		}

		if(canvasGroup == null) {
			Debug.Log("Must have canvas group attached!");
			this.enabled = false;
		} else {
			canvasGroup.alpha = 0f;
		}
	}

	public void LoadScene(string name) {
		Clear();
		SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
	}

	public void LoadSceneAdditive(string name, System.Object param = null, bool unloadOther = false) {
		if(unloadOther) {
			foreach(string scene in loaddedScenes) {
				if(name != scene) {
					StartCoroutine(UnloadSceneInternal(name, null));
				}
			}
		}

		if(!loaddedScenes.Contains(name)) {
			SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
			if(unloadOther) {
				Clear();
			}
			loaddedScenes.Add(name);
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
		} else if(unloadOther) {
			Clear();
			loaddedScenes.Add(name);
		} 

		parameters[name] = param;
	}

	public void UnloadScene(string name, System.Object retVal = null) {
		StartCoroutine(UnloadSceneInternal(name, retVal));

	}

	public bool IsSceneEist(string name) {
		return loaddedScenes.Contains(name);
	}

	public void UnloadCurrentScene(System.Object retVal = null) {
		Preconditions.Check(loaddedScenes.Count > 0, "Can not detect current scene"); 
		string name = GetCurrentScene();
		StartCoroutine(UnloadSceneInternal(name, retVal));
	}

	public string GetCurrentScene() {
		return loaddedScenes.Count > 0? loaddedScenes[loaddedScenes.Count - 1] : null;
	}

	private IEnumerator UnloadSceneInternal(string name, System.Object retVal) {
		yield return SceneManager.UnloadSceneAsync(name);
		loaddedScenes.Remove(name);
		onUnloadScene(name, retVal);
	}

	public void LoadSceneAsync(string name) {
		Clear();
		StartCoroutine(LoadSceneAsyncInternal(name));
	}

	void Update() {
	/*	if(Input.GetMouseButtonDown(0)) {
			LoadSceneAsync("Start");
		}
		*/

//		if(LoadImage != null && canvasGroup.alpha > 0) {
//			LoadImage.transform.Rotate(loadRotateAngles);
//		}
	}

	private IEnumerator LoadSceneAsyncInternal(string sceneName) {
		FadeIn();

		float endTime = Time.time + MinDuration;
		while(Time.time < endTime)
			yield return null;

		SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
	}

	void OnLevelWasLoaded() {
		if(canvasGroup.alpha > 0) {
			FadeOut();
		}
	}
		
	public void FadeIn() {
		startAlpha = canvasGroup.alpha;
		endAlpha = MaxAlpha;
		timeSoFar = 0;
		fading = true;
		StartCoroutine(FadeCoroutine());
	}

	public void FadeOut() {
		startAlpha = canvasGroup.alpha;
		endAlpha = 0;
		timeSoFar = 0;
		fading = true;
		StartCoroutine(FadeCoroutine());
	}

	IEnumerator FadeCoroutine() {
		changeRate = (endAlpha - startAlpha) / ChangeTimeSeconds;
		SetAlpha(startAlpha);
		while(fading) {
			timeSoFar += Time.deltaTime;

			if(timeSoFar > ChangeTimeSeconds) {
				fading = false;
				SetAlpha(endAlpha);
				yield break;
			} else {
				SetAlpha(canvasGroup.alpha + (changeRate * Time.deltaTime));
			}

			yield return null;
		}
	}
		
	public void SetAlpha(float alpha) {
		canvasGroup.alpha = Mathf.Clamp(alpha, 0, 1);
	}

	private void Clear() {
		loaddedScenes.Clear();
		parameters.Clear();
		returnValues.Clear();
	}

	public System.Object GetParameter(string name) {
		return parameters[name];
	}

	public void ShowUserAssetsScene(UserAssetType type, bool showMessage) {
		string sceneName;

		switch(type) {
		case UserAssetType.Money:
		case UserAssetType.Ring:
		case UserAssetType.Mobile:
			sceneName = LombardScene.SceneName;
			break;
		case UserAssetType.Energy:
			sceneName = EnergyScene.SceneName;
			break;
		default:
			throw new System.Exception("Invalid user asset type: " + type);
		}

		if(showMessage) {
			DisplayMessageController.DisplayNotEnoughMessage(type);
		}

		LoadSceneAdditive(sceneName, type, false);
	}

	public void LoadCurrentLevel() {
		LevelData levelData = GameResources.Instance.GetLevel(App.GetCurrentLevel());
		if(!GameResources.Instance.ChangeUserAsset(UserAssetType.Energy, -levelData.LevelPrice)) {
			ShowUserAssetsScene(UserAssetType.Energy, true);
			return;
		}
		LoadSceneAsync(GameController.SceneName);
	
	}
}
