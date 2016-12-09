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

	private Canvas c;

	void Awake () {
		Instance = this;
		c = Preconditions.NotNull(DisplayText.transform.GetComponentInParent<Canvas>(), "Can not get canvas");
	}

	void Start() {
		DisplayText.enabled = false;
		Instance.init();
	}

	private void init() {
		Vector3 pos = DisplayText.GetComponent<RectTransform>().localPosition;
		endPos = new Vector3(pos.x, EndDispalyMessagePosition.GetComponent<RectTransform>().localPosition.y, pos.z);
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
		text.transform.position = DisplayText.GetComponent<RectTransform>().localPosition;

		text.text = message;
		text.color = color;

		AnimatedObject ao = text.GetComponent<AnimatedObject>();
		ao.AddMove(null, endPos, MoveSpeed, true).Build()
			.AddFadeUIText(null, 0f, FadeTime).Build()
			.DestroyOnStop(true).Run();
	}
}
