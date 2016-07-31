using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

  private int screenWidth;
  private int screenHeight;
  Vector3 size;

  void Start() {
    SpriteRenderer render = GetComponent<SpriteRenderer>();
    size = render.bounds.size * render.sprite.pixelsPerUnit;

  }
  // Update is called once per frame
  void Update() {
  //  ResizeSpriteToScreen();
  //  return;
    if(Screen.width != screenWidth || Screen.height != screenHeight) {
  //  transform.localScale = new Vector3(1,1,1);
      screenWidth = Screen.width;
      screenHeight = Screen.height;

    SpriteRenderer render = GetComponent<SpriteRenderer>();
    Vector3 lSize = render.bounds.size;
		
      float wCf = screenWidth / lSize.x;
      float hCf = screenHeight / lSize.y;

      Debug.Log(wCf + " " + hCf);
      //Debug.Log(size);  
      if(wCf > hCf) {
  //    render.bounds.size = new Vector3(render.bounds.size.x * wCf, render.bounds.size.y * wCf, render.bounds.size.z);
      transform.localScale = new Vector3(wCf/hCf, wCf/hCf,1);
	//			transform.localScale = new Vector3(wCf, wCf,1);

      //  transform.localScale *= wCf;
      } else {
  //    render.bounds.size = new Vector3(render.bounds.size.x * wCf, render.bounds.size.y * wCf, render.bounds.size.z);
  //      transform.localScale *= hCf;
      transform.localScale = new Vector3(hCf/wCf, hCf/wCf,1);
	//			transform.localScale = new Vector3(hCf, hCf,1);

      }

   // Debug.Log(Camera.main.orthographicSize);
            
    float cf = Screen.width / size.x;
  //  Debug.Log(cf);
    float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
  //  Debug.Log(screenWidth + " " + screenHeight+ " "+ worldScreenHeight);

    }
  }

  void ResizeSpriteToScreen() {
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (sr == null) return;

    transform.localScale = new Vector3(1,1,1);

    float width = sr.sprite.bounds.size.x;
    float height = sr.sprite.bounds.size.y;

    float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
    float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

  //  transform.localScale.x = worldScreenWidth / width;
 //   transform.localScale.y = worldScreenHeight / height;
    transform.localScale = new Vector3(worldScreenWidth / width,worldScreenHeight / height,1);
  }
}
