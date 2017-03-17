using UnityEngine;
using System.Collections;

public class TileItemController : MonoBehaviour {

	public Sprite HighLightSprite;
	public Sprite RotateSprite;
	public Sprite RotateHighLightSprite;

	public ParticleSystem TransitionPS;

	protected SpriteRenderer render;
	private Sprite sourceSprite;
	private Color sourceColor;
	private Material sourceMaterial;
	private Color darkColor = new Color(0.7f,0.7f,0.7f);
	private Material markMaterial;
	private TileItemRenderState currentRenderState = TileItemRenderState.Normal;
	private ParticleSystem transitionPS;
	private bool isMark = false;
	private bool isRotate = false;
	private MaterialPropertyBlock _propBlock;

	protected virtual void Start () {
		render = GetComponent<SpriteRenderer>();
		_propBlock = new MaterialPropertyBlock();
		sourceSprite = render.sprite;
		sourceColor = render.color;
		sourceMaterial = render.material;
		markMaterial = Resources.Load("Material/ShinyDefault", typeof(Material)) as Material;

		if(currentRenderState != TileItemRenderState.Normal) {
			SetRenderState(currentRenderState);
		}
	/*	if(isMark) {
			Mark(true);	
		}*/

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

	private Sprite GetSourceSprite() {
		return (isRotate)? RotateSprite : sourceSprite;
	}
	private Sprite GetHighLightSprite() {
		return (isRotate)? RotateHighLightSprite : HighLightSprite;
	}

	public void SetRenderState(TileItemRenderState state) {
		currentRenderState = state;

		if(render == null) {
			return;
		}

		if(state == TileItemRenderState.Normal) {
			RenderNormal();
		} else if(state == TileItemRenderState.HighLight) {
			RenderHighLight();
		} else if(state == TileItemRenderState.Dark) {
			RenderDark();
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

	virtual protected void RenderNormal() {
		render.GetPropertyBlock(_propBlock);
		_propBlock.SetColor("_Color", sourceColor);
		_propBlock.SetTexture("_MainTex", GetSourceSprite().texture);
		render.SetPropertyBlock(_propBlock);
	//	render.color = sourceColor;
	//	render.sprite = GetSourceSprite();
	}

	virtual protected void RenderDark() {
		render.GetPropertyBlock(_propBlock);
		_propBlock.SetColor("_Color", darkColor);
		_propBlock.SetTexture("_MainTex", GetSourceSprite().texture);
		render.SetPropertyBlock(_propBlock);

		//render.color = darkColor;
	}

	virtual protected void RenderHighLight() {
		render.GetPropertyBlock(_propBlock);

		Sprite hl = GetHighLightSprite();
		if(hl != null) {
		//	render.sprite = hl;
			_propBlock.SetTexture("_MainTex", hl.texture);
		}
	//	render.color = sourceColor;
		_propBlock.SetColor("_Color", sourceColor);
		render.SetPropertyBlock(_propBlock);
	}
	/*
	virtual public void Mark(bool isMark) {
		this.isMark = isMark;
			
		if(render != null) {
			render.material = (isMark) ? markMaterial : sourceMaterial;
		}
	}
	*/
	virtual public int Damage(int damage) {
		return 1;
	}

	virtual public bool SetStartHealth(int health) {
		return true;
	}

	virtual public bool DestroyOnBreak() {
		return true;
	}
		
	public void Rotate() {
		if(RotateHighLightSprite == null || RotateSprite == null ) {
			return;
		}

		isRotate = !isRotate;
		SetRenderState(currentRenderState);
	}
}
