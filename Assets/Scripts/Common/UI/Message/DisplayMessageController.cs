using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayMessageController : MonoBehaviour {

	public GameObject DisplayText;
	public float StartY;
	public float EndY;

	private static DisplayMessageController displayMessageController;

	private Vector3 startPos;
	private Vector3 endPos;


	private void init() {
		startPos = new Vector3(0, StartY, displayMessageController.DisplayText.transform.position.z);
		endPos = new Vector3(0, EndY, displayMessageController.DisplayText.transform.position.z);
	}

	public static DisplayMessageController Instance () {
		if (!displayMessageController) {
			displayMessageController = FindObjectOfType(typeof (DisplayMessageController)) as DisplayMessageController;
			Preconditions.NotNull(displayMessageController, "There needs to be one active DisplayMessageManager script on a GameObject in your scene.");
			Preconditions.NotNull(displayMessageController.DisplayText, "Text control can not be null");
			displayMessageController.init();
		}

		return displayMessageController;
	}

	public static void DisplayMessage (string message) {
		DisplayMessage(message, Color.white);
	}

	public static void DisplayMessage (string message, Color color) {
		DisplayMessageController instance = Instance();

		GameObject textGO = (GameObject)Instantiate(instance.DisplayText, instance.startPos, Quaternion.identity);
		Text text = textGO.transform.Find("Text").gameObject.GetComponent<Text>();

		text.text = message;
		text.color = color;


	}
}
