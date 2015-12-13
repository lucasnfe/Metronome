using UnityEngine;
using System.Collections;

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

	// Use this for initialization
	protected virtual void Start () {

		if (_surface.Length == 0) {

			Debug.Log("Surface array must have at least one element.");
			return;
		}
	}

	public GameObject GenerateLevel() {

		_level = new GameObject();
		_level.name = "Level";

		GenerateLevel (_startPosition.x, _endPosition.x, _startPosition.y, MisConstants.TILE_SIZE/MisConstants.PIXEL_UNIT);

		return _level;
	}

	protected virtual void GenerateLevel (float startPosX, float endPosX, float startPosY, float tileSize) {

	}

	protected GameObject BuildCollidableTile(Vector2 position, Transform parent, Sprite sprite, Vector2 colliderOffset) {

		GameObject tile = new GameObject();
		tile.name = "Surface";
		tile.tag  = MisConstants.TAG_WALL;
		tile.isStatic = true;
		tile.transform.parent = parent;
		tile.transform.position = position;	
		
		// Add surface sprite to the new surface object
		SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
		renderer.sprite = sprite;
		
		// Add box colliders to the new surface objects
		BoxCollider2D bCollider = tile.AddComponent<BoxCollider2D>();
		bCollider.offset = colliderOffset;

		return tile;
	}
}