using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MisGameWorld : MisSingleton<MisGameWorld> {

	private Vector2 _horizontalConstraints;	
	public  Vector2 WorldHorizontalConstraints { get { return _horizontalConstraints; } }

	private Vector2 _verticalConstraints;
	public  Vector2 WorldVerticalConstraints { get { return _verticalConstraints; } }

	private MisHero _misHero;
	public  MisHero WorldHero { get { return _misHero; } }

	private MisCamera _misCamera;
	public  MisCamera WorldCamera { get { return _misCamera; } }

	private MetronomeLevelGenerator  _misLevelGenerator;
	public  MetronomeLevelGenerator  GameGenerator { get { return _misLevelGenerator; } }

	private bool _isWorldSet;

	private Vector3 _nextSpawningPoint;

	private GameObject   _level;
	private GameObject   _heroPrefab;

	public bool IsPaused { get; set; }

	void Update() {

		if (_isWorldSet && !_misHero)
			ResetLevel ();
	}

	void LoadTransictionScreen() {

		if (_level)
			_level.SetActive (false);
	}

	public void LoadPlayableLevel() {
				
		_misLevelGenerator = Instantiate (Resources.Load<MetronomeLevelGenerator> ("Generators/MisLevelGenerator"));
		DontDestroyOnLoad (_misLevelGenerator);

		_level = _misLevelGenerator.GenerateLevel ();
		_level.transform.parent = transform;

		_horizontalConstraints.x = -0.5f * MisConstants.TILE_SIZE;
		_horizontalConstraints.y = Mathf.Infinity;

		_verticalConstraints.x = 0f;
		_verticalConstraints.y = (MisConstants.LEVEL_HEIGHT) * MisConstants.TILE_SIZE;

		_nextSpawningPoint = new Vector2(1f, MisConstants.LEVEL_GROUND_HEIGHT + 1f) * MisConstants.TILE_SIZE;

		// Loading hero prefab
		_heroPrefab = Resources.Load("Characters/MisPlayer") as GameObject;
	}

	public void SetupLevel() {
		
		_misCamera = FindObjectOfType (typeof(MisCamera)) as MisCamera;
		if (_misCamera == null) {

			Debug.LogError ("You need to create a camera.");
			return;
		}
			
		SpawnHero ();

		_isWorldSet = true;
	}

	public void ResetLevel() {

		_isWorldSet = false;

		if (_level)
			Destroy (_level);

		if (_misLevelGenerator)
			Destroy (_misLevelGenerator.gameObject);
		
		if(_misHero)
			Destroy(_misHero.gameObject);
		
		MisSceneManager.Instance.ReloadScene (true, LoadPlayableLevel, SetupLevel);
	}

	public void SpawnHero() {

		_misHero = SpawnCharacter (_heroPrefab, _nextSpawningPoint).GetComponent<MisHero>();
	}

	GameObject SpawnCharacter(GameObject characterPrefab, Vector2 spawningPoint) {
		
		float zPos = characterPrefab.transform.position.z;
		Vector3 spawningPos = new Vector3(spawningPoint.x, spawningPoint.y, zPos);
		
		GameObject charObj = Instantiate (characterPrefab, spawningPos, Quaternion.identity) as GameObject;
		charObj.name = characterPrefab.name;
		charObj.transform.parent = transform;

		return charObj;
	}
}
