using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayMessageController : MonoBehaviour {

	public Text DisplayText;
	public float MoveSpeed;
	public float FadeTime;
	public GameObject EndDispalyMessagePosition;

	public static DisplayMessageController Instance;

	private Vector3 endPos;

	public Canvas c;

	void Awake () {
		Instance = this;
	//	c = Preconditions.NotNull(DisplayText.transform.GetComponentInParent<Canvas>(), "Can not get canvas");
	}

	void Start() {
		DisplayText.enabled = false;
		Instance.init();
	}

	private void init() {
		Vector3 pos = DisplayText.transform.position;
		endPos = new Vector3(pos.x, EndDispalyMessagePosition.transform.position.y, pos.z);
	}

	public static void DisplayMessage(string message, Color color) {
		Instance.displayMessageInternal(message, color);
	}

	public static void DisplayMessage (string message) {
		Instance.displayMessageInternal(message, Color.white);
	}

	private void displayMessageInternal (string message, Color color) {
		Text text = (Text)Instantiate(DisplayText);
		text.enabled = true;

		text.transform.SetParent(c.transform);
		text.transform.localScale = new Vector3(1, 1, 1);
		text.transform.position = DisplayText.transform.position;

		text.text = message;
		text.color = color;

		AnimatedObject ao = text.GetComponent<AnimatedObject>();
		ao.AddMove(null, endPos, MoveSpeed).Build()
			.AddFadeUIText(null, 0f, FadeTime).Build()
			.DestroyOnStop(true).Run();
	}

	public static void DisplayNotEnoughMessage(UserAssetType type) {
		string t;

		switch(type) {
		case UserAssetType.Money:
			t = "бабла";
			break;
		case UserAssetType.Mobile:
			t = "мобилок";
			break;
		case UserAssetType.Energy:
			t = "семак";
			break;
		case UserAssetType.Ring:
			t = "печатак";
			break;
		case UserAssetType.Star:
			t = "звёзд";
			break;
		default:
			throw new System.Exception("Invalid user asset type: " + type);
		}

		DisplayMessage(string.Format("Не хватает {0}", t));
	}
}
