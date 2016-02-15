using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MisGameWorld : MisSingleton<MisGameWorld> {

	private MisHero 		   _misHero;
	private MisCamera          _misCamera;
	private MisLevelGenerator  _misLevelGenerator;

	private GameObject   _level;
	private GameObject   _heroPrefab;
	private GameObject []_enemiesPrefab;

	private Vector2      _nextSpawningPoint;

	// Use this for initialization
	void Start () {

		DontDestroyOnLoad (gameObject);
	
		_misLevelGenerator = FindObjectOfType (typeof(MisLevelGenerator)) as MisLevelGenerator;
		if (!_misLevelGenerator) {

			GameObject temp = Instantiate(Resources.Load("Generators/MisLevelGenerator") as GameObject);
			_misLevelGenerator = temp.GetComponent<MisLevelGenerator>();
		}

		DontDestroyOnLoad (_misLevelGenerator.gameObject);
		
		// Loading hero prefab
		_heroPrefab = Resources.Load("Characters/MisPlayer") as GameObject;

		// Loading enemies prefabs
		Object[] objects = Resources.LoadAll("Characters/Enemies");

		_enemiesPrefab = new GameObject[objects.Length];
		for (int i = 0; i < objects.Length; i++)
			_enemiesPrefab[i] = (GameObject)objects[i];
	}

	void OnLevelWasLoaded(int level) {
	
		switch (level) {

		case 0:
			LoadTransictionScreen();
			break;

		case 1:
			break;

		case 2:
			break;
		
		default:
			LoadPlayableLevel();
			break;
		}
	}

	void LoadTransictionScreen() {

		if (_level)
			_level.SetActive (false);
	}

	void LoadPlayableLevel() {

		if (_level)
			_level.SetActive (true);
		
		_misCamera = FindObjectOfType (typeof(MisCamera)) as MisCamera;
		if (!_misCamera) {
			
			Debug.LogError ("You must define a camera!");
			return;
		}
		
		if (!_level) {
			
			_level = _misLevelGenerator.GenerateLevel ();
			_level.transform.parent = transform;

			DontDestroyOnLoad (_level);
		} else
			_level.SetActive (true);
		
		// Reset the hero
		if(_misHero)
			Destroy(_misHero.gameObject);
		
		SpawnHero (_nextSpawningPoint);
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
