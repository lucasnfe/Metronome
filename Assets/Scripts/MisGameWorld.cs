using UnityEngine;
using System.Collections;

public class MisGameWorld : MisSingleton<MisGameWorld> {

	public MisCamera _misCamera;
	public MisGroundGenerator _levelGenerator;
	
	public GameObject   _heroPrefab;
	public GameObject []_enemiesPrefab;

	private MisHero _misHero;

	private Vector2 _nextSpawningPoint;
	
	// Use this for initialization
	void Awake () {
	
		if (!_levelGenerator) {

			Debug.LogError("You must define a level generator!");
			return;
		}

		_levelGenerator.GenerateLevel ();

		SpawnHero (_nextSpawningPoint);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ResetLevel(Vector2 heroSpawningPoint) {

		_nextSpawningPoint = heroSpawningPoint;
		Application.LoadLevel (Application.loadedLevel);
	}

	void SpawnHero(Vector2 spawningPoint) {

		float zPos = _heroPrefab.transform.position.z;
		Vector3 spawningPos = new Vector3(spawningPoint.x, spawningPoint.y, zPos);

		GameObject heroObj = Instantiate (_heroPrefab, spawningPos, Quaternion.identity) as GameObject;
		heroObj.name = MisConstants.HERO_NAME;
		
		_misHero = heroObj.GetComponent<MisHero>();
		
		if(_misCamera)
			_misCamera._player = _misHero.gameObject;
	}
}
