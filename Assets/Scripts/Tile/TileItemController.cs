using UnityEngine;
using System.Collections;

public class TileItemController : MonoBehaviour {

	public Sprite HighLightSprite;
	public ParticleSystem TransitionPS;

	private SpriteRenderer render;
	private Sprite sourceSprite;
	private Color sourceColor;
	private Material sourceMaterial;
	private Color darkColor = new Color(0.7f,0.7f,0.7f);
	private Material markMaterial;
	private TileItemRenderState currentRenderState = TileItemRenderState.Normal;
	private ParticleSystem transitionPS;
	private bool isMark = false;

	void Start () {
		render = GetComponent<SpriteRenderer>();
		sourceSprite = render.sprite;
		sourceColor = render.color;
		sourceMaterial = render.material;
		markMaterial = Resources.Load("Material/ShinyDefault", typeof(Material)) as Material;

		if(currentRenderState != TileItemRenderState.Normal) {
			SetRenderState(currentRenderState);
		}
		if(isMark) {
			Mark(true);	
		}

		if(TransitionPS != null) {
			transitionPS = Instantiate(TransitionPS);
			transitionPS.transform.parent = transform;
			transitionPS.transform.position = transform.position;
		}

		CheckLoadedResources();
	}

	private void CheckLoadedResources() {
		Preconditions.NotNull(markMaterial, "Can not load Mark material");
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void SetRenderState(TileItemRenderState state) {
		currentRenderState = state;

		if(render == null) {
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
			transitionPS.transform.LookAt(transitionPS.transform);
			transitionPS.Clear();
			transitionPS.Stop();
		}		
	}

	virtual protected void renderNormal() {
		render.color = sourceColor;
		render.sprite = sourceSprite;
	}

	virtual protected void renderDark() {
		render.color = darkColor;
	}

	virtual protected void renderHighLight() {
		if(HighLightSprite != null) {
			render.sprite = HighLightSprite;
		}
		render.color = sourceColor;
	}

	virtual public void Mark(bool isMark) {
		this.isMark = isMark;
			
		if(render != null) {
			render.material = (isMark) ? markMaterial : sourceMaterial;
		}
	}

	virtual public int Damage(int damage) {
		return 1;
	}
}
