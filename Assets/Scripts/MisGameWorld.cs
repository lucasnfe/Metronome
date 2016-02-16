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

	void LoadTransictionScreen() {

		if (_level)
			_level.SetActive (false);
	}

	void OnLevelWasLoaded(int level) {

		if(level == 4) {

			_misCamera = FindObjectOfType (typeof(MisCamera)) as MisCamera;
			if (!_misCamera) {

				Debug.LogError("Camera is not set!");
				return;
			}

			SpawnHero (_nextSpawningPoint);
		}
	}

	public void LoadPlayableLevel() {
				
		_misLevelGenerator = FindObjectOfType (typeof(MisLevelGenerator)) as MisLevelGenerator;
		if (!_misLevelGenerator) {

			GameObject temp = Instantiate(Resources.Load("Generators/MisLevelGenerator") as GameObject);
			_misLevelGenerator = temp.GetComponent<MisLevelGenerator>();
		}

		_level = _misLevelGenerator.GenerateLevel ();
		_level.transform.parent = transform;

		// Loading hero prefab
		_heroPrefab = Resources.Load("Characters/MisPlayer") as GameObject;

		// Loading enemies prefabs
		_enemiesPrefab = Resources.LoadAll<GameObject>("Characters/Enemies");
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
