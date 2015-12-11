using UnityEngine;
using System.Collections;

public class MisMapGenerator : MonoBehaviour {

	private SpriteRenderer _renderer;

	// Use this for initialization
	void Start () {

		if (GetComponent<SpriteRenderer> () == null)
			gameObject.AddComponent<SpriteRenderer> ();

		_renderer = gameObject.GetComponent<SpriteRenderer> ();

		// Generate black background
		GenerateMapBackground ();

		// Generate cities in random positions
		GenerateCities (5);
	}

	void GenerateMapBackground() {

		// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
		Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
		
		// set the pixel values
		texture.SetPixel(0, 0, Color.black);
		texture.SetPixel(1, 0, Color.black);
		texture.SetPixel(0, 1, Color.black);
		texture.SetPixel(1, 1, Color.black);
		
		// Apply all SetPixel calls
		texture.Apply();
		
		// connect texture to material of GameObject this script is attached to
		_renderer.sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(2f, 2f)), Vector2.zero);

		transform.localScale = new Vector3(400f, 400f, 1f);
		transform.position  -= new Vector3(2f * transform.localScale.x/200f,
		                                   2f * transform.localScale.y/200f, 0f);
	}

	void GenerateCities(int amount) {

		for (int i = 0; i < amount; i++) {

			GameObject city = new GameObject();
			SpriteRenderer renderer = city.AddComponent<SpriteRenderer>();
		
			// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
			Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
			
			// set the pixel values
			texture.SetPixel(0, 0, Color.white);
			texture.SetPixel(1, 0, Color.white);
			texture.SetPixel(0, 1, Color.white);
			texture.SetPixel(1, 1, Color.white);

			renderer.sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(2f, 2f)), Vector2.zero);
			city.transform.localScale = new Vector3(20f,20f, 1f);

			city.AddComponent<BoxCollider2D>();
			city.AddComponent<MisWorldMap>();

			Vector2 offsetFromCenter = Random.insideUnitCircle * 3f;
			city.transform.position += new Vector3(offsetFromCenter.x, offsetFromCenter.y, -2f);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
