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

	protected GameObject BuildTile(Vector2 position, Transform parent, Sprite sprite, Vector2 colliderOffset) {

		GameObject tile = null;

		float xKey = float.Parse(position.x.ToString("0.00"));
		float yKey = float.Parse(position.y.ToString("0.00"));
		Vector2 dictKey = new Vector2 (xKey, yKey);  

		if (!_collidebleTiles.ContainsKey (dictKey)) {

			tile = new GameObject ();
			tile.AddComponent<SpriteRenderer> ();
		} 
		else
			tile = _collidebleTiles [dictKey];

		tile.name = "Surface";
		tile.tag  = MisConstants.TAG_WALL;
		tile.isStatic = true;
		tile.transform.parent = parent;
		tile.transform.position = position;	

		tile.GetComponent<SpriteRenderer> ().sprite = sprite;

		_collidebleTiles [dictKey] = tile;

		return tile;
	}

	protected GameObject BuildCollidableTile(Vector2 position, Transform parent, Sprite sprite, Vector2 colliderOffset) {

		GameObject tile = BuildTile(position, parent, sprite, colliderOffset);

		BoxCollider2D bCollider = tile.GetComponent<BoxCollider2D> ();
		if(bCollider == null)
			bCollider = tile.AddComponent<BoxCollider2D> ();
	
		bCollider.offset = colliderOffset;

		return tile;
	}

	protected bool DestroyTile(Vector2 position, Transform parent, Sprite sprite, Vector2 colliderOffset) {

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