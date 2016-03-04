using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MisLevelGenerator : MonoBehaviour {

	public GameObject[] _platforms;
	public GameObject[] _enemies;

	public int        _lenght = 1;
	public Vector2    _startPos;

	protected GameObject _level;
	protected Dictionary <Vector2, GameObject> _collidebleTiles;

	// Use this for initialization
	protected virtual void Awake () {

		_collidebleTiles = new Dictionary <Vector2, GameObject> ();
	}

	public GameObject GenerateLevel() {

		_level = new GameObject();
		_level.name = "Level";

		GenerateLevel (_startPos, _lenght);

		return _level;
	}

	protected virtual void GenerateLevel(Vector2 startPos, int levelLenght) {

	}

	public GameObject BuildTile(Vector2 position, Transform parent, GameObject tileObject, bool playSFX = false) {

		GameObject tile = null;

		float xKey = float.Parse(position.x.ToString("0.00"));
		float yKey = float.Parse(position.y.ToString("0.00"));
		Vector2 dictKey = new Vector2 (xKey, yKey);  

		if (!_collidebleTiles.ContainsKey (dictKey)) {

			tile = (GameObject) Instantiate (tileObject);
		} 
		else if(_collidebleTiles.ContainsKey (dictKey) && _collidebleTiles[dictKey].gameObject == null) {

			tile = (GameObject) Instantiate (tileObject);
		}
		else {
			
			tile = _collidebleTiles [dictKey];
		}

		tile.name = tileObject.name;
		tile.transform.parent = parent;

		Vector3 finalPos = tileObject.transform.position;
		finalPos.x = position.x;
		finalPos.y = position.y;
		tile.transform.position = finalPos;	

		_collidebleTiles [dictKey] = tile;

		return tile;
	}
		
	public bool DestroyTile(Vector2 position) {

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