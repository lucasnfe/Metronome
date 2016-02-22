using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MisGameWorld : MisSingleton<MisGameWorld> {

	private MisHero _misHero;
	public  MisHero GameHero { get { return _misHero; } }

	private MisCamera _misCamera;
	public  MisCamera GameCamera { get { return _misCamera; } }

	private MisLevelGenerator  _misLevelGenerator;
	public  MisLevelGenerator  GameGenerator { get { return _misLevelGenerator; } }

	private GameObject   _level;
	private GameObject   _heroPrefab;
	private GameObject []_enemiesPrefab;

	private Vector2      _nextSpawningPoint;

	void LoadTransictionScreen() {

		if (_level)
			_level.SetActive (false);
	}

	public void LoadPlayableLevel() {
				
		_misLevelGenerator = Instantiate (Resources.Load<MisLevelGenerator> ("Generators/MisLevelGenerator"));
		DontDestroyOnLoad (_misLevelGenerator);

		_level = _misLevelGenerator.GenerateLevel ();
		_level.transform.parent = transform;

		_level.SetActive (false);

		// Loading hero prefab
		_heroPrefab = Resources.Load("Characters/MisPlayer") as GameObject;

		// Loading enemies prefabs
		_enemiesPrefab = Resources.LoadAll<GameObject>("Characters/Enemies");
	}

	public void SetupLevel() {
		
		_misCamera = FindObjectOfType (typeof(MisCamera)) as MisCamera;
		if (_misCamera == null) {

			Debug.LogError ("You need to create a camera.");
			return;
		}

		BeatCounter beatCounter = FindObjectOfType (typeof(BeatCounter)) as BeatCounter;
		if (beatCounter == null) {

			Debug.LogError ("You need to create a beat counter.");
			return;
		}

		beatCounter.observers = new GameObject[1];
		beatCounter.observers[0] = _misLevelGenerator.gameObject;

		SpawnHero (_nextSpawningPoint);

		_level.SetActive (true);
	}

	public void ResetLevel(Vector2 heroSpawningPoint) {

		Destroy(_misHero.gameObject);

		_nextSpawningPoint = heroSpawningPoint;	
		MisSceneManager.Instance.ReloadScene ();
	}

	void SpawnHero(Vector2 spawningPoint) {

		_misHero = SpawnCharacter (_heroPrefab, spawningPoint).GetComponent<MisHero>();

		if (_misCamera) {
			
			_misCamera._player = _misHero;
			_misCamera.Move(_misHero.transform.position);
		}
	}

	public void SpawnEnemy(int enemyType, Vector2 spawningPoint) {

		GameObject enemyPrefab = _enemiesPrefab[enemyType];
		BoxCollider2D collider = enemyPrefab.GetComponent<BoxCollider2D> ();

		Vector2 pos = new Vector2 (spawningPoint.x, spawningPoint.y + collider.bounds.size.y);
		SpawnCharacter (enemyPrefab, pos);
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
