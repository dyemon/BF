using UnityEngine;
using System.Collections;

public class TileItemController : MonoBehaviour {

	public Sprite HighLightSprite;
	public ParticleSystem TransitionPS;

	private SpriteRenderer renderer;
	private Sprite sourceSprite;
	private Color sourceColor;
	private Color darkColor = new Color(0.7f,0.7f,0.7f);
	private TileItemRenderState currentRenderState = TileItemRenderState.Normal;
	private ParticleSystem transitionPS;

	void Start () {
		renderer = GetComponent<SpriteRenderer>();
		sourceSprite = renderer.sprite;
		sourceColor = renderer.color;

		if(currentRenderState != TileItemRenderState.Normal) {
			SetRenderState(currentRenderState);
		}

		if(TransitionPS != null) {
			transitionPS = Instantiate(TransitionPS);
			transitionPS.transform.parent = transform;
			transitionPS.transform.position = transform.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetRenderState(TileItemRenderState state) {
		currentRenderState = state;

		if(renderer == null) {
			return;
		}

		if(state == TileItemRenderState.Normal) {
			renderNormal();
		} else if(state == TileItemRenderState.HighLight) {
			renderHighLight();
		} else if(state == TileItemRenderState.Dark) {
			renderDark();
		}
	}

	public void SetTransition(GameObject go) {
		if(transitionPS != null) {
			transitionPS.transform.LookAt(go.transform);
			transitionPS.Play();
		}
	}
	public void UnsertTransition() {
		if(transitionPS != null) {
			transitionPS.Stop();
		}		
	}

	virtual protected void renderNormal() {
		renderer.color = sourceColor;
		renderer.sprite = sourceSprite;
	}

	virtual protected void renderDark() {
		renderer.color = darkColor;
	}

	virtual protected void renderHighLight() {
		if(HighLightSprite != null) {
			renderer.sprite = HighLightSprite;
		}
		renderer.color = sourceColor;
	}
}
