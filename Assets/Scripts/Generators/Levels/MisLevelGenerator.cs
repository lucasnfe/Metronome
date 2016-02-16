using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MisLevelGenerator : MonoBehaviour {

	public Sprite  []_surface;
	public Sprite  []_intertal;
	public Sprite  []_external;
	public Sprite  []_platforms;
	public Sprite  []_enemies;

	public Vector2 _startPosition;
	public Vector2 _endPosition;
	public Vector2 _colliderOffset;

	protected GameObject _level;

	protected Dictionary <Vector2, GameObject> _collidebleTiles;

	// Use this for initialization
	protected virtual void Awake () {

		_collidebleTiles = new Dictionary <Vector2, GameObject> ();
	}

	public GameObject GenerateLevel() {

		_level = new GameObject();
		_level.name = "Level";

		GenerateLevel (_startPosition.x, _endPosition.x, _startPosition.y, (float)MisConstants.TILE_SIZE/MisConstants.PIXEL_UNIT);

		return _level;
	}

	protected virtual void GenerateLevel (float startPosX, float endPosX, float startPosY, float tileSize) {

	}

	protected GameObject BuildCollidableTile(Vector2 position, Transform parent, Sprite sprite, Vector2 colliderOffset) {

		GameObject tile = null;
		SpriteRenderer renderer = null;
		BoxCollider2D collider = null;

		float xKey = float.Parse(position.x.ToString("0.00"));
		float yKey = float.Parse(position.y.ToString("0.00"));
		Vector2 dictKey = new Vector2 (xKey, yKey);  

		if (!_collidebleTiles.ContainsKey (dictKey))

			tile = new GameObject ();
		else 
			tile = _collidebleTiles [dictKey];

		tile.name = "Surface";
		tile.tag  = MisConstants.TAG_WALL;
		tile.isStatic = true;
		tile.transform.parent = parent;
		tile.transform.position = position;	

		if (!_collidebleTiles.ContainsKey (dictKey))

			renderer = tile.AddComponent<SpriteRenderer> ();
		else
			renderer = tile.GetComponent<SpriteRenderer> ();

		renderer.sprite = sprite;

		if (!_collidebleTiles.ContainsKey (dictKey))

			collider = tile.AddComponent<BoxCollider2D> ();
		else
			collider = tile.GetComponent<BoxCollider2D> ();

		collider.offset = colliderOffset;

		_collidebleTiles [dictKey] = tile;

		return tile;
	}

	protected bool DestroyCollidableTile(Vector2 position, Transform parent, Sprite sprite, Vector2 colliderOffset) {

		float xKey = float.Parse(position.x.ToString("0.00"));
		float yKey = float.Parse(position.y.ToString("0.00"));
		Vector2 dictKey = new Vector2 (xKey, yKey);  

		if (_collidebleTiles.ContainsKey (dictKey)) {

			Destroy (_collidebleTiles [dictKey].gameObject);
			return true;
		}

		return false;
	}
}